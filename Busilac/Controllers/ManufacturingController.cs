using Busilac.Models;
using Busilac.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Busilac.Controllers
{
    public class ManufacturingController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Manufacturing
        public ActionResult Index()
        {
            var mvm = new List<ManufacturingProductionOrdersViewModel>();

            var prodOrders = db.ProductManufactureOrders.ToList();

            foreach (var item in prodOrders)
            {
                var mpovm = new ManufacturingProductionOrdersViewModel();
                mpovm.ProductManufactureOrders = item;

                var prodOrderDetails = db.ProductManufactureOrderDetails
                                                    .Where(m => m.ProductManufactureOrderId == item.ProductManufactureOrderId)
                                                    .GroupBy(m => m.Products)
                                                    .Select(group => new OrderedProductsViewModel
                                                    {
                                                        Product = group.Key,
                                                        ProductOrderedTotalCount = group.Sum(x => x.Quantity)
                                                    }).ToList();

                foreach (var details in prodOrderDetails)
                {
                    mpovm.OrderDetailsString += string.Format("{0}({1}), ", details.Product.Name, details.ProductOrderedTotalCount);
                }

                mpovm.OrderDetailsString = mpovm.OrderDetailsString.TrimEnd(',', ' ');

                mvm.Add(mpovm);
            }
            mvm = mvm.OrderBy(m => m.ProductManufactureOrders.StatusId).ToList();
            return View(mvm);
        }

        [HttpGet]
        public ActionResult Approve(int id)
        {
            var opavm = OrderedProductsApprovalModel(id);

            return View(opavm);
        }

        public ActionResult Approve(OrderedProductsApprovalViewModel opavm)
        {
            var po = db.ProductManufactureOrders.First(m => m.ProductManufactureOrderId == opavm.ProductManufactureOrders.ProductManufactureOrderId);
            var poDetails = db.ProductManufactureOrderDetails.Where(m => m.ProductManufactureOrderId == po.ProductManufactureOrderId).ToList();

            foreach (var item in poDetails)
            {
                // Check if Material inventory is enough to create product
                foreach (var mats in db.ProductBuildMaterials.Where(m => m.ProductId == item.ProductId).ToList())
                {
                    var materialInventory = db.MaterialsInventory.Where(m => m.MaterialId == mats.MaterialId).ToList();
                    var subtractToMatsInventory = new MaterialsInventory();
                    decimal materialInventoryCount = materialInventory.Any() ? materialInventory.Sum(m => m.Weight) : 0;
                    decimal requiredMaterialsCount = item.Quantity * mats.Quantity;

                    if (requiredMaterialsCount > materialInventoryCount)
                    {
                        ModelState.AddModelError("Error", "Saving Failed! Not enough Materials");
                        // build the product and materials list again.
                        var orderProductsVm = OrderedProductsApprovalModel(opavm.ProductManufactureOrders.ProductManufactureOrderId);

                        return View(orderProductsVm);
                    } else
                    {
                        subtractToMatsInventory.MaterialId = mats.MaterialId;
                        subtractToMatsInventory.Weight = requiredMaterialsCount * -1;

                        db.MaterialsInventory.Add(subtractToMatsInventory);
                    }
                }

                // Add products to inventory.
                var prodInventory = new ProductInventory();
                prodInventory.Product = item.Products;
                prodInventory.Quantity = item.Quantity;

                // Subtract from Materials Inventory

                db.ProductInventory.Add(prodInventory);
            }

            // Change ProductMAnnufactureOrder to Complete
            po.StatusId = 3;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // Helpers
        public OrderedProductsApprovalViewModel OrderedProductsApprovalModel(int id)
        {
            var opavm = new OrderedProductsApprovalViewModel();

            opavm.AvailableMaterialsViewModel = new List<AvailableMaterialsViewModel>();
            opavm.ProductMaterials = new List<ProductOrderApproveViewModels>();

            var po = db.ProductManufactureOrders.Where(m => m.ProductManufactureOrderId == id).First();
            opavm.ProductManufactureOrders = po;

            var poDetails = db.ProductManufactureOrderDetails.Where(m => m.ProductManufactureOrderId == po.ProductManufactureOrderId).ToList();

            foreach (var item in poDetails)
            {
                var pm = new ProductOrderApproveViewModels();
                pm.Product = item.Products;
                pm.QuantityOrdered = item.Quantity;

                var pbmDetails = db.ProductBuildMaterials.Where(m => m.ProductId == item.ProductId).ToList();

                foreach (var details in pbmDetails)
                {
                    pm.ProductMaterialsString += string.Format("{0}({1}), ", details.Materials.Name, details.Quantity);
                    pm.ProductMaterialsStringTotals += string.Format("{0}({1}), ", details.Materials.Name, details.Quantity * item.Quantity);
                }
                pm.ProductMaterialsStringTotals = pm.ProductMaterialsStringTotals.TrimEnd(',', ' ');
                pm.ProductMaterialsString = pm.ProductMaterialsString.TrimEnd(',', ' ');

                opavm.ProductMaterials.Add(pm);
            }

            foreach (var item in db.Materials.Where(m => m.isVoid == 0).ToList())
            {
                var availableMaterials = new AvailableMaterialsViewModel();
                var totalInventory = db.MaterialsInventory.Where(m => m.MaterialId == item.MaterialId).ToList();

                availableMaterials.Materials = item;
                availableMaterials.TotalInventoryCount = totalInventory.Any() ? totalInventory.Sum(m => m.Weight) : 0;

                opavm.AvailableMaterialsViewModel.Add(availableMaterials);
            }

            return opavm;
        }
    }
}