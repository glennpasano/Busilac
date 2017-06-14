using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Busilac.Models;
using System.Collections.Generic;

namespace Busilac.Controllers
{
    public class AdminPageController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationUserManager _userManager;

        public AdminPageController()
        {

        }

        public AdminPageController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
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


        public ActionResult ManageUsers()
        {
            var model = new List<ManageUsersViewModel>();

            foreach (var item in db.Users.ToList())
            {
                var mvm = new ManageUsersViewModel()
                {
                    UserName = item.UserName,
                    Roles = _userManager.GetRoles(item.Id).First()
                };

                model.Add(mvm);
            }

            return View(model);
        }
    }
}
