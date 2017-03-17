using Busilac.Models;
using Busilac.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Busilac.Controllers
{
    [Authorize(Roles = "Client")]
    public class ClientController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Client
        public ActionResult Index()
        {
            var covm = new List<ClientOrdersViewModels>();

            foreach (var item in db.ProductSalesOrders.Where(m => m.StatusId == 1).ToList())
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

            return View(covm);
        }

        [HttpGet]
        public ActionResult Create()
        {
            var cpovm = new CreateProductOrdersViewModels();
            
            cpovm.ProductList = db.Products.Where(m => m.isVoid == 0).ToList();

            return View(cpovm);
        }

        [HttpPost]
        public ActionResult Create(CreateProductOrdersViewModels cpovm)
        {
            var productSalesOrder = new ProductSalesOrders();
            productSalesOrder.ClientName = cpovm.ProductSalesOrder.ClientName;
            productSalesOrder.OrderDate = DateTime.Now;
            productSalesOrder.StatusId = 1;

            db.ProductSalesOrders.Add(productSalesOrder);

            foreach (var item in cpovm.ProductSalesOrderDetails.Where(m => m.Quantity > 0))
            {
                var psoDetails = new ProductSalesOrderDetails();
                psoDetails.ProductId = item.ProductId;
                psoDetails.Quantity = item.Quantity;
                psoDetails.ProductSalesOrders = productSalesOrder;

                db.ProductSalesOrderDetails.Add(psoDetails);
            }

            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}