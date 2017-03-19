using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Busilac.Models;
using Busilac.ViewModels;

namespace Busilac.Controllers
{
    public class NotificationsController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Notifications
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetNotifications(string userId)
        {
            IEnumerable<NotificationsViewModel> NotificationsList =
                        db.Notifications.Where(m => m.isVoid == 0 && m.UserId == userId).ToList()
                          .OrderByDescending(m => m.Timestamp)
                          .Select(m => new NotificationsViewModel
                          {
                              Notification = m,
                              TimeAgo = TimeAgo(m.Timestamp)
                          }).Take(5).Reverse().ToList();

            return Json(NotificationsList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult NotificationsRead(string userId)
        {
            var userNotifications = db.Notifications.Where(m => m.UserId == userId).ToList();

            foreach (var item in userNotifications)
            {
                Notifications notification = db.Notifications.First(m => m.NotificationId == item.NotificationId);
                notification.isRead = 1;

                db.SaveChanges();
            }

            return Json(new { success = true });
        }

        public void NotifyUser(Notifications notification)
        {
            notification.Timestamp = DateTime.Now;
            db.Notifications.Add(notification);

            db.SaveChanges();
        }

        // Helpers

        private static string TimeAgo(DateTime dt)
        {
            TimeSpan span = DateTime.Now - dt;
            if (span.Days > 365)
            {
                int years = (span.Days / 365);
                if (span.Days % 365 != 0)
                    years += 1;
                return String.Format("about {0} {1} ago",
                years, years == 1 ? "year" : "years");
            }
            if (span.Days > 30)
            {
                int months = (span.Days / 30);
                if (span.Days % 31 != 0)
                    months += 1;
                return String.Format("about {0} {1} ago",
                months, months == 1 ? "month" : "months");
            }
            if (span.Days > 0)
                return String.Format("about {0} {1} ago",
                span.Days, span.Days == 1 ? "day" : "days");
            if (span.Hours > 0)
                return String.Format("about {0} {1} ago",
                span.Hours, span.Hours == 1 ? "hour" : "hours");
            if (span.Minutes > 0)
                return String.Format("about {0} {1} ago",
                span.Minutes, span.Minutes == 1 ? "minute" : "minutes");
            if (span.Seconds > 5)
                return String.Format("about {0} seconds ago", span.Seconds);
            if (span.Seconds <= 5)
                return "just now";

            return string.Empty;
        }
    }
}