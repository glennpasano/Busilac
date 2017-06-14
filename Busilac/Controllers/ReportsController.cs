using Busilac.Models;
using Busilac.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Busilac.Controllers
{
    public class ReportsController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Reports
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProductOverview(DateTime? startdate, DateTime? enddate, int? productId)
        {
            var model = new SoldProductsReportViewModel()
            {
                SoldProductsList = db.ProductSalesOrderDetails
                .Join(db.ProductSalesOrderDetails, pso => pso.ProductSalesOrdersId,
                            psod => psod.ProductSalesOrdersId, (pso, psod) => new { pso, psod })
                .Where(m => m.pso.ProductSalesOrdersId == m.psod.ProductSalesOrdersId)
                .Where(m => startdate == null || m.pso.ProductSalesOrders.OrderDate >= startdate)
                .Where(m => enddate == null || m.pso.ProductSalesOrders.OrderDate <= enddate)
                .Where(m => productId == null || m.pso.ProductId == productId).ToList().Select(m => new SoldProductsViewModel
                {
                    ProductName = m.pso.Products.Name,
                    Quantity = m.pso.Quantity.ToString(),
                    ClientName = m.pso.ProductSalesOrders.ClientName,
                    DateOrdered = m.pso.ProductSalesOrders.OrderDate.ToShortDateString()
                }).ToList(),
                ProductList = db.Products.Select(m => new ReportDropdownViewModel
                {
                    Name = m.Name,
                    Id = m.ProductId
                }).ToList()
            };

            return View(model);
        }

        public ActionResult MaterialsOverview(DateTime? startdate, DateTime? enddate, int? materialId)
        {
            var model = new OrderedMaterialsReportViewModel() {
                OrderedMaterials = db.MaterialsSalesOrders
                .Join(db.MaterialsSalesOrdersDetails, mo => mo.MaterialSalesOrdersId,
                            mod => mod.MaterialSalesOrdersId, (mo, mod) => new { mo, mod })
                .Where(m => m.mo.MaterialSalesOrdersId == m.mod.MaterialSalesOrdersId)
                .Where(m => materialId == null || m.mod.MaterialId == materialId)
                .Where(m => startdate == null || m.mo.OrderDate >= startdate)
                .Where(m => enddate == null || m.mo.OrderDate <= enddate).ToList()
                .Select(m => new OrderedMaterialsViewModel
                {
                    OrderedDate = m.mo.OrderDate.ToShortDateString(),
                    MaterialName = m.mod.Materials.Name,
                    MaterialType = m.mod.Materials.Type.TypeName,
                    Weight = m.mod.Weight
                }).ToList(),
                MaterialsList = db.Materials.ToList().Select(m => new ReportDropdownViewModel {
                    Id = m.MaterialId,
                    Name = m.Name
                }).ToList()
            };

            return View(model);
        }


        public JsonResult OrderedMaterialsChartData()
        {
            var chartData = db.MaterialsSalesOrders
                .Join(db.MaterialsSalesOrdersDetails, mo => mo.MaterialSalesOrdersId,
                            mod => mod.MaterialSalesOrdersId, (mo, mod) => new { mo, mod })
                .Where(m => m.mo.MaterialSalesOrdersId == m.mod.MaterialSalesOrdersId).ToList()
                .GroupBy(m => m.mod.Materials)
                .Select(group => new OrderedMaterialsViewModel
                {
                    MaterialName = group.Key.Name,
                    MaterialType = group.Key.Type.TypeName,
                    Weight = group.Sum(m => m.mod.Weight)
                }).ToList();

            return Json(chartData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult OrderedProductsChartData()
        {
            var chartData = db.ProductSalesOrderDetails
                .Join(db.ProductSalesOrderDetails, pso => pso.ProductSalesOrdersId,
                            psod => psod.ProductSalesOrdersId, (pso, psod) => new { pso, psod })
                .Where(m => m.pso.ProductSalesOrdersId == m.psod.ProductSalesOrdersId)
                .GroupBy(m => m.psod.Products)
                .Select(group => new SoldProductsViewModel
                {
                    ProductName = group.Key.Name,
                    Quantity = group.Sum(m => m.psod.Quantity).ToString()
                }).ToList();

            return Json(chartData, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TotalProductBuiltRatioChartData()
        {
            var chartData = db.ProductManufactureOrders
                .Join(db.ProductManufactureOrderDetails, pmo => pmo.ProductManufactureOrderId,
                            pmod => pmod.ProductManufactureOrderId, (pmo, pmod) => new { pmo, pmod })
                .Where(m => m.pmo.ProductManufactureOrderId == m.pmod.ProductManufactureOrderId)
                .GroupBy(m => m.pmod)
                .Select(group => new DonutChartViewModel
                {
                    label = group.Key.Products.Name,
                    value = group.Sum(m => m.pmod.Quantity)
                }).ToList()
                .GroupBy(m => m.label)
                .Select(group => new DonutChartViewModel
                {
                    label = group.Key,
                    value = group.Sum(m => m.value)
                }).ToList();

            return Json(chartData, JsonRequestBehavior.AllowGet);
        }
    }
}