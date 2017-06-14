using Busilac.Models;
using Busilac.ViewModels;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Busilac.Controllers
{
    [Authorize(Roles = "Supplier, Admin")]
    public class SupplierController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        
        private ApplicationUserManager _userManager;
        private ApplicationSignInManager _signInManager;

        public SupplierController() { }

        public SupplierController(ApplicationUserManager UserManager, ApplicationSignInManager SigninManager)
        {
            _userManager = UserManager;
            _signInManager = SigninManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Supplier
        public async Task<ActionResult> Index()
        {
            var UserManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

            var user = await UserManager.FindByNameAsync(User.Identity.Name);

            var msolvm = new List<MaterialSalesOrdersListViewModel>();

            // Get All Sales Orders that are Sent to Supplier (StatusId = 3) and Delivery (StatusId = 1002)
            foreach (var item in db.MaterialsSalesOrders.Where(m => (m.StatusId == 3 || m.StatusId == 1002 || 
                                                                     m.StatusId == 4 || m.StatusId == 1003) && m.SupplierId == user.Id)
                                                        .OrderByDescending(m => m.StatusId).ToList())
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