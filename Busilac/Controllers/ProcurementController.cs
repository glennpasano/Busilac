using Busilac.Models;
using Busilac.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Busilac.Controllers
{
    [Authorize(Roles = "Procurement, Admin")]
    public class ProcurementController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Sales
        [Route("[controller]/Materials/SalesOrders")]
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

        [HttpGet]
        [Route("[controller]/Materials/SalesOrders/Create")]
        public ActionResult CreateSalesOrderMaterials()
        {
            CreateSalesOrderViewModel createSalesOrderViewModel = new CreateSalesOrderViewModel();
            createSalesOrderViewModel.MaterialsList = db.Materials.Where(m => m.isVoid == 0).ToList();

            return View(createSalesOrderViewModel);
        }

        [HttpPost]
        [Route("[controller]/Materials/SalesOrders/Create")]
        public ActionResult CreateSalesOrderMaterials(CreateSalesOrderViewModel csovm)
        {
            MaterialsSalesOrders mso = new MaterialsSalesOrders();
            mso.OrderDate = DateTime.Now;
            mso.StatusId = 1; // Status = Pending Approval

            db.MaterialsSalesOrders.Add(mso);

            foreach (var item in csovm.MaterialsSalesOrdersDetails)
            {
                if(item.Weight > 0)
                {
                    var msodetails = new MaterialsSalesOrdersDetails();

                    msodetails.MaterialSalesOrders = mso;
                    msodetails.MaterialId = item.MaterialId;
                    msodetails.Weight = item.Weight;

                    db.MaterialsSalesOrdersDetails.Add(msodetails);
                }
            }

            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}