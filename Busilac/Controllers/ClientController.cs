using Busilac.Models;
using Busilac.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Busilac.Controllers
{
    [Authorize(Roles = "ClientSales, Client, Admin")]
    [RoutePrefix("Sales")]
    public class ClientController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Client
        [Route("OrderProducts")]
        public ActionResult Index()
        {
            var covm = new ClientOrdersViewModels();
            covm.ProductSalesOrderDetails = new List<ProductSalesOrderDetailsViewModels>();
            covm.MaterialsInventory = new List<MaterialsInventoryViewModel>();

            foreach (var item in db.ProductSalesOrders.Where(m => m.StatusId == 1).ToList())
            {
                var co = new ProductSalesOrderDetailsViewModels();
                co.ProductSalesOrders = item;

                foreach (var details in db.ProductSalesOrderDetails.Where(m => m.ProductSalesOrdersId == item.ProductSalesOrdersId).ToList())
                {
                    co.ProductListString += string.Format("{0}({1}), ", details.Products.Name, details.Quantity);
                }
                
                co.ProductListString = co.ProductListString == null ? "" : co.ProductListString.TrimEnd(',', ' ');
                covm.ProductSalesOrderDetails.Add(co);
            }

            // query current materials inventory
            foreach (var item in db.Materials.Where(m => m.isVoid == 0).ToList())
            {
                var mi = new MaterialsInventoryViewModel()
                {
                    Material = item,
                    TotalMaterialWeight = db.MaterialsInventory
                                            .Where(m => m.MaterialId == item.MaterialId)
                                            .Sum(m => m.Weight)
                };
                
                covm.MaterialsInventory.Add(mi);
            }
            

            return View(covm);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var cpovm = new CreateProductOrdersViewModels()
            {
                ProductList = db.Products.Where(m => m.isVoid == 0).ToList()
            };

            return View(cpovm);
        }

        [HttpPost]
        public ActionResult Create(CreateProductOrdersViewModels cpovm)
        {
            var productSalesOrder = new ProductSalesOrders()
            {
                ClientName = cpovm.ProductSalesOrder.ClientName,
                OrderDate = DateTime.Now,
                StatusId = 1
            };

            db.ProductSalesOrders.Add(productSalesOrder);

            foreach (var item in cpovm.ProductSalesOrderDetails.Where(m => m.Quantity > 0))
            {
                var psoDetails = new ProductSalesOrderDetails()
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    ProductSalesOrders = productSalesOrder,
                    Price = item.Price
                };

                db.ProductSalesOrderDetails.Add(psoDetails);
            }

            if (!ModelState.IsValid || !cpovm.ProductSalesOrderDetails.Any(m => m.Quantity > 0))
            {
                cpovm.ProductList = db.Products.Where(m => m.isVoid == 0).ToList();

                return View(cpovm);
            }

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // APIs

        [HttpGet]
        public JsonResult GetInventory()
        {
            var inventory = new List<MaterialsInventoryViewModel>();

            foreach (var item in db.Materials.Where(m => m.isVoid == 0).ToList())
            {
                var mi = new MaterialsInventoryViewModel()
                {
                    Material = item,
                    TotalMaterialWeight = db.MaterialsInventory
                                            .Where(m => m.MaterialId == item.MaterialId)
                                            .Sum(m => m.Weight)
                };

                inventory.Add(mi);
            }

            return Json(inventory, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetSalesOrder(int salesOrderId)
        {
            var model = new ListProductOrdersViewModel() {
                ProductSalesOrder = db.ProductSalesOrders.First(m => m.ProductSalesOrdersId == salesOrderId),
                ProductSalesOrderDetails = db.ProductSalesOrderDetails.Where(m => m.ProductSalesOrdersId == salesOrderId).ToList()
            };

            model.OrderDateString = model.ProductSalesOrder.OrderDate.ToShortDateString();

            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}