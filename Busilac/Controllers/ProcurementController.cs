using Busilac.Models;
using Busilac.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Busilac.Controllers
{
    [Authorize(Roles = "Procurement, Admin, Warehouse")]
    public class ProcurementController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Sales
        [Route("Warehouse/PurchaseOrders")]
        public ActionResult Index()
        {
            var msolvm = new List<MaterialSalesOrdersListViewModel>();

            foreach (var item in db.MaterialsSalesOrders.ToList())
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

        public ActionResult Details(int id)
        {
            var model = new MaterialSalesOrderDetailsViewModel()
            {
                MaterialSalesOrders = db.MaterialsSalesOrders.Where(m => m.MaterialSalesOrdersId == id).First(),
                MaterialSalesOrderDetails = db.MaterialsSalesOrdersDetails.Where(m => m.MaterialSalesOrdersId == id).ToList()
            };

            return View(model);
        }

        public ActionResult Print(int id)
        {
            var model = new MaterialSalesOrderDetailsViewModel()
            {
                MaterialSalesOrders = db.MaterialsSalesOrders.Where(m => m.MaterialSalesOrdersId == id).First(),
                MaterialSalesOrderDetails = db.MaterialsSalesOrdersDetails.Where(m => m.MaterialSalesOrdersId == id).ToList()
            };

            return View(model);
        }

        [HttpGet]
        public ActionResult CreateSalesOrderMaterials()
        {
            CreateSalesOrderViewModel createSalesOrderViewModel = new CreateSalesOrderViewModel();
            createSalesOrderViewModel.MaterialsList = db.Materials.Where(m => m.isVoid == 0).ToList();
            createSalesOrderViewModel.SupplierList = db.Users.Where(m => m.Roles.Any(x => x.RoleId == "2" && x.RoleId != "1"))
                                        .Select(m => new SupplierListViewModel()
                                        {
                                            Name = m.UserName,
                                            Id = m.Id
                                        }).ToList();

            return View(createSalesOrderViewModel);
        }

        [HttpPost]
        public ActionResult CreateSalesOrderMaterials(CreateSalesOrderViewModel csovm)
        {
            MaterialsSalesOrders mso = new MaterialsSalesOrders()
            {
                OrderDate = DateTime.Now,
                StatusId = 1, // Status = Pending Approval
                SupplierId = csovm.MaterialsSalesOrders.SupplierId
            };
            db.MaterialsSalesOrders.Add(mso);

            foreach (var item in csovm.MaterialsSalesOrdersDetails)
            {
                if(item.Weight > 0)
                {
                    var msodetails = new MaterialsSalesOrdersDetails()
                    {
                        MaterialSalesOrders = mso,
                        MaterialId = item.MaterialId,
                        Weight = item.Weight,
                        Price = item.Price
                    };
                    db.MaterialsSalesOrdersDetails.Add(msodetails);
                }
            }

            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}