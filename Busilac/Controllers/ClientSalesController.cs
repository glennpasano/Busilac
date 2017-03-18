using Busilac.Models;
using Busilac.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Busilac.Controllers
{
    [Authorize(Roles = "ClientSales, Admin")]
    public class ClientSalesController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: ClientSales
        public ActionResult Index()
        {
            var covm = new List<ClientOrdersViewModels>();

            foreach (var item in db.ProductSalesOrders.Where(m => m.StatusId == 1 || m.StatusId == 3).ToList())
            {
                var co = new ClientOrdersViewModels();
                co.ProductSalesOrders = item;

                foreach (var details in db.ProductSalesOrderDetails.Where(m => m.ProductSalesOrdersId == item.ProductSalesOrdersId).ToList())
                {
                    co.ProductListString += string.Format("{0}({1}), ", details.Products.Name, details.Quantity);
                }

                co.ProductListString = co.ProductListString.TrimEnd(',', ' ');
                covm.Add(co);
            }

            return View(covm.OrderBy(m => m.ProductSalesOrders.StatusId));
        }

        [HttpGet]
        public ActionResult Approve(int id)
        {
            var apovm = ApproveProducOrdersVMData(id);

            return View(apovm);
        }

        [HttpPost]
        public ActionResult Approve(ApproveProductOrdersViewModels apovm)
        {
            var prodOrder = db.ProductSalesOrders.First(m => m.ProductSalesOrdersId == apovm.ProductSalesOrder.ProductSalesOrdersId);
            var prodOrderDetails = db.ProductSalesOrderDetails.Where(m => m.ProductSalesOrdersId == prodOrder.ProductSalesOrdersId).ToList();

            foreach (var item in prodOrderDetails)
            {
                var productInventory = db.ProductInventory.Where(m => m.ProductId == item.ProductId).ToList(); ;

                // Check if we have enough Inventory to complete transaction
                if (item.Quantity > productInventory.Sum(m => m.Quantity))
                {
                    ModelState.AddModelError("Error", "Saving Failed! Not enough Products");
                    var lastViewModel = ApproveProducOrdersVMData(apovm.ProductSalesOrder.ProductSalesOrdersId);

                    return View(lastViewModel);
                }

                // Subtract orders from Inventory
                var subtProdInventory = new ProductInventory();
                subtProdInventory.ProductId = item.ProductId;
                subtProdInventory.Quantity = item.Quantity*-1;

                db.ProductInventory.Add(subtProdInventory);
            }

            // Change status to delivered after everything
            prodOrder.StatusId = 3;

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // Helpers
        public ApproveProductOrdersViewModels ApproveProducOrdersVMData(int id)
        {
            var apovm = new ApproveProductOrdersViewModels();
            apovm.ProductSalesOrderDetails = new List<ProductSalesOrderDetails>();
            apovm.ProductSalesOrder = db.ProductSalesOrders.First(m => m.ProductSalesOrdersId == id);

            foreach (var item in db.ProductSalesOrderDetails.Where(m => m.ProductSalesOrdersId == id).ToList())
            {
                apovm.ProductSalesOrderDetails.Add(item);
            }

            apovm.ProductsInventory = db.ProductInventory
                                            .GroupBy(m => m.Product)
                                            .Select(group => new ProductsInventoryViewModel
                                            {
                                                Products = group.Key,
                                                TotalProductCount = group.Sum(m => m.Quantity)
                                            }).ToList();

            return apovm;
        }
    }
}