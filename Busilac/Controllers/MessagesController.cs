using Busilac.Models;
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
            IEnumerable<Messages> MessageList = 
                        db.Messages.Where(m => m.isVoid == 0 && m.RecipientId == userId).Take(5).ToList();

            return Json(MessageList, JsonRequestBehavior.AllowGet);
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

                return Json(new { success = true });
            } catch(Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}