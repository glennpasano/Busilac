using Busilac.Models;
using Busilac.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Busilac.Controllers
{
    public class APIController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        public JsonResult GetMaterial(int materialId)
        {
            return Json(new
            {
                Material = db.Materials.First(m => m.MaterialId == materialId),
                TotalInventory = db.MaterialsInventory.Where(m => m.MaterialId == materialId).Sum(m => m.Weight)
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetProduct(int productId)
        {
            return Json(new
            {
                Name = db.Products.First(m => m.ProductId == productId).Name,
                TotalInventory = db.ProductInventory.Where(m => m.ProductId == productId).Sum(m => m.Quantity),
                NormalQuantity = db.Products.First(m => m.ProductId == productId).NormalLevelQuantity
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSuppliers()
        {
            return Json(new
            {
                Suppliers = db.Users.Where(m => m.Roles.Any(x => x.RoleId == "2" && x.RoleId != "1"))
                                .Select(m => new {
                                    Name = m.UserName,
                                    Id = m.Id
                                })
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetPendingSalesOrders()
        {
            var materialsSalesOrders = new List<MaterialSalesOrdersListViewModel>();

            foreach (var so in db.MaterialsSalesOrders.Where(m => m.StatusId == 1).ToList())
            {
                var msovm = new MaterialSalesOrdersListViewModel();
                var materialsList = "";

                msovm.MaterialSalesOrders = so;

                foreach (var details in db.MaterialsSalesOrdersDetails.Where(m => m.MaterialSalesOrdersId == so.MaterialSalesOrdersId).ToList())
                {
                    materialsList += string.Format("{0} ({1}kg), ", details.Materials.Name, details.Weight);
                }

                msovm.MaterialsList = materialsList.TrimEnd(',', ' ');
                msovm.OrderDateString = so.OrderDate.ToShortDateString();

                materialsSalesOrders.Add(msovm);
            }

            var manufactureOrders = new List<ManufacturingProductionOrdersViewModel>();

            foreach (var mo in db.ProductManufactureOrders.Where(m => m.StatusId == 1).ToList())
            {
                var pso = new ManufacturingProductionOrdersViewModel();
                var productList = "";

                pso.ProductManufactureOrders = mo;

                foreach (var mod in db.ProductManufactureOrderDetails.Where(m => m.ProductManufactureOrderId == mo.ProductManufactureOrderId).ToList())
                {
                    productList += string.Format("{0}({1}), ", mod.Products.Name, mod.Quantity);
                }

                pso.OrderDetailsString = productList.TrimEnd(',', ' ');
                pso.OrderDateString = mo.RequestDate.ToShortDateString();

                manufactureOrders.Add(pso);
            }

            return Json(new
            {
                Materials = materialsSalesOrders,
                Products = manufactureOrders
            }, JsonRequestBehavior.AllowGet);
        }
    }
}