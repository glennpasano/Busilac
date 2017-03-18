using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Busilac.Models;

namespace Busilac.Controllers
{
    public class NotificationsController : Controller
    {
        // GET: Notifications
        public ActionResult Index()
        {
            return View();
        }
    }
}