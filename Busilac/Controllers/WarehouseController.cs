using Busilac.Models;
using Busilac.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Busilac.Controllers
{
    [Authorize(Roles = "Warehouse")]
    public class WarehouseController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var homeViewModel = new WarehouseHomeViewModel();

            homeViewModel.MaterialsInventory = db.MaterialsInventory
                                                    .GroupBy(m => m.Materials)
                                                    .Select(group => new MaterialsInventoryViewModel
                                                    {
                                                        Material = group.Key,
                                                        TotalMaterialWeight = group.Sum(x => x.Weight)
                                                    }).ToList();

            homeViewModel.ProductInventory = db.ProductInventory
                                                    .GroupBy(m => m.Product)
                                                    .Select(group => new ProductsInventoryViewModel
                                                    {
                                                        Products = group.Key,
                                                        TotalProductCount = group.Sum(m => m.Quantity)
                                                    }).ToList();

            return View(homeViewModel);
        }

        public ActionResult MaterialSalesOrders()
        {
            var msolvm = new List<MaterialSalesOrdersListViewModel>();

            // Get All Sales Orders that are Delivered (StatusId = 3) and Delivering (StatusId = 1002)
            foreach (var item in db.MaterialsSalesOrders.Where(m => m.StatusId == 1003 || m.StatusId == 1002 || m.StatusId == 4).OrderBy(m => m.StatusId).ToList())
            {
                var solvm = new MaterialSalesOrdersListViewModel();
                solvm.MaterialSalesOrders = item;

                foreach (var detail in db.MaterialsSalesOrdersDetails.Where(m => m.MaterialSalesOrdersId == item.MaterialSalesOrdersId).ToList())
                {
                    solvm.MaterialsList += string.Format("{0}({1}), ", detail.Materials.Name, detail.Weight);
                }
                solvm.MaterialsList = solvm.MaterialsList.TrimEnd(',', ' ');

                msolvm.Add(solvm);
            }

            return View(msolvm);
        }

        [HttpGet]
        public ActionResult ApproveDelivery(int id)
        {
            CreateSalesOrderViewModel csovm = new CreateSalesOrderViewModel();

            csovm.MaterialsSalesOrders = db.MaterialsSalesOrders.First(m => m.MaterialSalesOrdersId == id);
            csovm.MaterialsSalesOrdersDetails = db.MaterialsSalesOrdersDetails.Where(m => m.MaterialSalesOrdersId == id).ToList();

            return View(csovm);
        }

        [HttpPost]
        public ActionResult ApproveDelivery(CreateSalesOrderViewModel csovm)
        {
            var mso = db.MaterialsSalesOrders.FirstOrDefault(m => m.MaterialSalesOrdersId == csovm.MaterialsSalesOrders.MaterialSalesOrdersId);

            if (mso != null)
            {
                CheckDiscrepancy(csovm.MaterialsSalesOrders, csovm.MaterialsSalesOrdersDetails);
                UpdateMaterialsInventory(csovm.MaterialsSalesOrdersDetails);

                mso.StatusId = 1003; // On Approval; Change to "Delivered" status
                db.SaveChanges();
            }

            return RedirectToAction("MaterialSalesOrders");
        }

        public ActionResult ManageProductionOrders()
        {
            var productOrdersList = new List<ProductOrdersListViewModel>();

            foreach (var item in db.ProductManufactureOrders.ToList())
            {
                var pmo = new ProductOrdersListViewModel();
                pmo.ProductManufactureOrders = item;

                var prodDetails = db.ProductManufactureOrderDetails
                                    .Where(m => m.ProductManufactureOrderId == item.ProductManufactureOrderId)
                                    .ToList();

                foreach (var details in prodDetails)
                {
                    pmo.ProductsList += string.Format("{0}({1}), ", details.Products.Name, details.Quantity);
                }
                pmo.ProductsList = pmo.ProductsList.TrimEnd(',', ' ');

                productOrdersList.Add(pmo);
            }

            return View(productOrdersList);
        }

        [HttpGet]
        public ActionResult CreateProductionOrder()
        {
            var createProductsVM = new CreateProductOrdersViewModel();
            createProductsVM = CreateProductOrderViewModelData(createProductsVM);

            return View(createProductsVM);
        }

        [HttpPost]
        public ActionResult CreateProductionOrder(CreateProductOrdersViewModel cpovm)
        {

            var pmo = new ProductManufactureOrders();
            pmo.RequestDate = DateTime.Now;
            pmo.StatusId = 1;

            db.ProductManufactureOrders.Add(pmo);

            foreach (var item in cpovm.ProductManufactureOrderDetailsList.Where(m => m.Quantity > 0))
            {
                var pmod = new ProductManufactureOrderDetails();

                // Get Product Materials
                var productMaterials = db.ProductBuildMaterials.Where(m => m.ProductId == item.ProductId).ToList();
                // Check if we have enough materials for the product before proceeding with Order
                foreach (var mats in productMaterials)
                {
                    var materialInventory = db.MaterialsInventory.Where(m => m.MaterialId == mats.MaterialId).ToList();
                    decimal materialInventoryCount;

                    materialInventoryCount = materialInventory.Any() ? materialInventory.Sum(m => m.Weight) : 0;

                    if ((item.Quantity * mats.Quantity) > materialInventoryCount)
                    {
                        ModelState.AddModelError("Error", "Saving Failed! Not enough Materials");
                        // build the product and materials list again.
                        var createProductsVM = new CreateProductOrdersViewModel();
                        createProductsVM = CreateProductOrderViewModelData(createProductsVM);

                        return View(createProductsVM);
                    }

                }

                pmod.ProductManufactureOrders = pmo;
                pmod = item;

                db.ProductManufactureOrderDetails.Add(pmod);
            }

            db.SaveChanges();

            return RedirectToAction("ManageProductionOrders");
        }

        // Helpers

        private CreateProductOrdersViewModel CreateProductOrderViewModelData(CreateProductOrdersViewModel createProductsVM)
        {
            createProductsVM.ProductsAndMaterialsList = new List<ProductMaterialBuildsViewModel>();
            createProductsVM.AvailableMaterialsViewModel = new List<AvailableMaterialsViewModel>();

            foreach (var item in db.Products.Where(m => m.isVoid == 0).ToList())
            {
                var productsBuild = new ProductMaterialBuildsViewModel();
                productsBuild.ProductBuildMaterials = new List<ProductBuildMaterials>();
                productsBuild.Product = item;

                foreach (var mats in db.ProductBuildMaterials.Where(m => m.ProductId == item.ProductId).ToList())
                {
                    productsBuild.ProductBuildMaterials.Add(mats);
                    productsBuild.ProductBuildMaterialsString += string.Format("{0}({1}), ", mats.Materials.Name, mats.Quantity);
                }
                productsBuild.ProductBuildMaterialsString = productsBuild.ProductBuildMaterialsString.TrimEnd(',', ' ');
                createProductsVM.ProductsAndMaterialsList.Add(productsBuild);
            }

            foreach (var item in db.Materials.Where(m => m.isVoid == 0).ToList())
            {
                var availableMaterials = new AvailableMaterialsViewModel();
                var totalInventory = db.MaterialsInventory.Where(m => m.MaterialId == item.MaterialId).ToList();

                availableMaterials.Materials = item;
                availableMaterials.TotalInventoryCount = totalInventory.Any() ? totalInventory.Sum(m => m.Weight) : 0;

                createProductsVM.AvailableMaterialsViewModel.Add(availableMaterials);
            }

            return createProductsVM;
        }

        private void UpdateMaterialsInventory(List<MaterialsSalesOrdersDetails> actualDetails)
        {
            if(actualDetails.Any())
            {
                foreach (var item in actualDetails)
                {
                    var matInventory = new MaterialsInventory();

                    matInventory.MaterialId = item.MaterialId;
                    matInventory.Weight = item.Weight;

                    db.MaterialsInventory.Add(matInventory);
                }

                db.SaveChanges();
            }
        }

        private void CheckDiscrepancy(MaterialsSalesOrders mso, List<MaterialsSalesOrdersDetails> actualDetails)
        {
            // Get current Material Sales Order
            MaterialsSalesOrders salesOrder = db.MaterialsSalesOrders
                                                .Where(m => m.MaterialSalesOrdersId == mso.MaterialSalesOrdersId)
                                                .FirstOrDefault();

            List<MaterialsSalesOrdersDetails> salesOrderDetails = db.MaterialsSalesOrdersDetails
                                                                    .Where(m => m.MaterialSalesOrdersId == mso.MaterialSalesOrdersId)
                                                                    .ToList();

            // If something changed, create new SalesOrder with follow up status
            var newSalesOrder = new MaterialsSalesOrders();
            var newSalesOrdersDetailsList = new List<MaterialsSalesOrdersDetails>();
            
            foreach (var item in salesOrderDetails)
            {
                foreach (var actlItem in actualDetails)
                {
                    if(item.Id == actlItem.Id)
                    {
                        // Check if delivered items were complete
                        if(actlItem.Weight < item.Weight)
                        {
                            var owedValue = item.Weight - actlItem.Weight;
                            var newOrderDetail = new MaterialsSalesOrdersDetails();

                            newOrderDetail.MaterialId = item.MaterialId;
                            newOrderDetail.Weight = owedValue;

                            newSalesOrdersDetailsList.Add(newOrderDetail);
                        }
                    }
                }
            }

            if(newSalesOrdersDetailsList.Any())
            {
                newSalesOrder.OrderDate = DateTime.Now;
                newSalesOrder.StatusId = 4; // Status = Follow up.

                db.MaterialsSalesOrders.Add(newSalesOrder);
                foreach (var item in newSalesOrdersDetailsList)
                {
                    item.MaterialSalesOrders = newSalesOrder;
                    db.MaterialsSalesOrdersDetails.Add(item);
                }

                db.SaveChanges();
            }
        }

    }
}