using Busilac.Models;
using Busilac.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Busilac.Controllers
{
    [Authorize(Roles = "Supplier, Admin")]
    public class SupplierController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Supplier
        public ActionResult Index()
        {
            var msolvm = new List<MaterialSalesOrdersListViewModel>();

            // Get All Sales Orders that are Sent to Supplier (StatusId = 3) and Delivery (StatusId = 1002)
            foreach (var item in db.MaterialsSalesOrders.Where(m => m.StatusId == 3 || m.StatusId == 1002 || m.StatusId == 4).OrderByDescending(m => m.StatusId).ToList())
            {
                var solvm = new MaterialSalesOrdersListViewModel();
                solvm.MaterialSalesOrders = item;

                foreach (var detail in db.MaterialsSalesOrdersDetails.Where(m => m.MaterialSalesOrdersId == item.MaterialSalesOrdersId).ToList())
                {
                    solvm.MaterialsList += string.Format("{0}({1}), ", detail.Materials.Name, detail.Weight);
                }
                solvm.MaterialsList.TrimEnd(',', ' ');

                msolvm.Add(solvm);
            }

            return View(msolvm);
        }

        [HttpGet]
        public ActionResult Approve(int id)
        {
            CreateSalesOrderViewModel csovm = new CreateSalesOrderViewModel();

            csovm.MaterialsSalesOrders = db.MaterialsSalesOrders.First(m => m.MaterialSalesOrdersId == id);
            csovm.MaterialsSalesOrdersDetails = db.MaterialsSalesOrdersDetails.Where(m => m.MaterialSalesOrdersId == id).ToList();

            return View(csovm);
        }

        [HttpPost]
        public ActionResult Approve(CreateSalesOrderViewModel csovm)
        {
            var mso = db.MaterialsSalesOrders.FirstOrDefault(m => m.MaterialSalesOrdersId == csovm.MaterialsSalesOrders.MaterialSalesOrdersId);

            if (mso != null)
            {
                mso.StatusId = 1002; // On Approval; Change to "Delivering" status
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}