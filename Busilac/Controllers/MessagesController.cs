using Busilac.Models;
using Busilac.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Busilac.Controllers
{
    public class MessagesController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: Messages
        public ActionResult Index()
        {
            return View();
        }

        //public JsonResult SendMessage(string senderId, string recipientId)
        //{

        //}

        public JsonResult GetMessages(string userId)
        {
            IEnumerable<MessageDisplayViewModel> MessageList =
                        db.Messages.Where(m => m.isVoid == 0 && m.RecipientId == userId).ToList()
                          .Select(m => new MessageDisplayViewModel {
                              Message = m,
                              TimeAgo = TimeAgo(m.Timestamp)
                          }).Take(5).ToList();

            return Json(MessageList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult MessagesRead(string userId)
        {
            var userMessages = db.Messages.Where(m => m.Recipient.Id == userId).ToList();

            foreach (var item in userMessages)
            {
                Messages message = db.Messages.First(m => m.MessageId == item.MessageId);
                message.isRead = 1;

                db.SaveChanges();
            }

            return Json(new { success = true });
        }

        public JsonResult SendMessage(string senderId, string recipientId, string message)
        {
            try
            {
                var msg = new Messages();
                msg.SenderId = senderId;
                msg.RecipientId = recipientId;
                msg.Timestamp = DateTime.Now;
                msg.Message = message;

                db.Messages.Add(msg);
                db.SaveChanges();

                return Json(new { success = true });
            } catch(Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        public JsonResult GetRecipients()
        {
            IEnumerable<MessageRecipientsViewModel> usersList = db.Users
                .Select(m => new MessageRecipientsViewModel
                {
                    UserId = m.Id,
                    UserName = m.UserName
                })
                .ToList();

            return Json(usersList, JsonRequestBehavior.AllowGet);
        }

        // Helpers

        public static string TimeAgo(DateTime dt)
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