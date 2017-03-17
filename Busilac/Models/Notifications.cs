using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Busilac.Models
{
    public class Notifications
    {
        [Key]
        public int NotificationId { get; set; }
        public int UserId { get; set; }
        public string NotificationMessage { get; set; }
        public int isRead { get; set; }
        public int isVoid { get; set; }
        public DateTime Timestamp { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}