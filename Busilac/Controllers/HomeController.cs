using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Busilac.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if(User.IsInRole("Supplier")) {
                return RedirectToAction("Index", "Supplier");
            }

            return View();
        }

    }
}