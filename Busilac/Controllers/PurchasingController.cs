using Busilac.Models;
using Busilac.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Busilac.Controllers
{
    [Authorize(Roles = "Purchaser, Admin")]
    public class PurchasingController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        NotificationsController notification = new NotificationsController();
        // GET: Purchasing
        public ActionResult Index()
        {
            var msolvm = new List<MaterialSalesOrdersListViewModel>();

            // Get All Sales Orders that are for Approval (StatusId = 1)
            foreach (var item in db.MaterialsSalesOrders.Where(m => m.StatusId == 1 || m.StatusId == 3).OrderBy(m => m.StatusId).ToList())
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
        public ActionResult Approve(int Id)
        {
            CreateSalesOrderViewModel csovm = new CreateSalesOrderViewModel();

            csovm.MaterialsSalesOrders = db.MaterialsSalesOrders.First(m => m.MaterialSalesOrdersId == Id);
            csovm.MaterialsSalesOrdersDetails = db.MaterialsSalesOrdersDetails.Where(m => m.MaterialSalesOrdersId == Id).ToList();

            return View(csovm);
        }

        [HttpPost]
        public ActionResult Approve(CreateSalesOrderViewModel csovm)
        {
            // csovm.MaterialsSalesOrders.MaterialSalesOrdersId;

            var mso = db.MaterialsSalesOrders.FirstOrDefault(m => m.MaterialSalesOrdersId == csovm.MaterialsSalesOrders.MaterialSalesOrdersId);

            if(mso != null)
            {
                mso.StatusId = 3; // On Approval; Changed to "Sent to supplier" status

                // Notify and Email Supplier
                // Find all Suppliers
                var suppliers = db.Roles.Where(m => m.Name == "Supplier").ToList();

                foreach (var item in suppliers)
                {
                    foreach (var user in item.Users)
                    {
                        notification.NotifyUser(new Notifications
                        {
                            NotificationMessage = "New Purchase Order has been assigned to you.",
                            UserId = user.UserId
                        });
                    }
                }

                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}