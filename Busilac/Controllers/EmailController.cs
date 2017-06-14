using Busilac.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Busilac.Controllers
{
    public class EmailController : Controller
    {
        // GET: Email
        public ActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> SendEmailAsync(string[] sentTo, string messageText, string subject)
        {
            var message = new MailMessage();
            foreach (var item in sentTo)
            {
                message.To.Add(new MailAddress(item));
            }
            message.Subject = subject;
            message.Body = messageText;
            message.IsBodyHtml = true;

            using (var smtp = new SmtpClient())
            {
                try { 
                    await smtp.SendMailAsync(message);
                    return Json(new { success = true });
                } catch
                {
                    return Json(new { success = true });
                }
            }
        }
    }
}