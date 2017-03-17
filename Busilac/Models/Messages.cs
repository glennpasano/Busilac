using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Busilac.Models
{
    public class Messages
    {
        [Key]
        public int MessageId { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; }
        public int isVoid { get; set; }
        public int isRead { get; set; }
        public DateTime Timestamp { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}