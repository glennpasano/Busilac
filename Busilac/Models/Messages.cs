using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Busilac.Models
{
    public class Messages
    {
        [Key]
        public int MessageId { get; set; }
        public string SenderId { get; set; }
        public string RecipientId { get; set; }
        public string Message { get; set; }
        public int isVoid { get; set; }
        public int isRead { get; set; }
        public DateTime Timestamp { get; set; }

        [ForeignKey("SenderId")]
        public virtual ApplicationUser Sender { get; set; }
        [ForeignKey("RecipientId")]
        public virtual ApplicationUser Recipient { get; set; }
    }
}