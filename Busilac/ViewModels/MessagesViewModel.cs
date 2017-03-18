using Busilac.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Busilac.ViewModels
{
    public class MessagesViewModel
    {
    }

    public class MessageRecipientsViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
    }

    public class MessageDisplayViewModel
    {
        public Messages Message { get; set; }
        public string TimeAgo { get; set; }
    }
}