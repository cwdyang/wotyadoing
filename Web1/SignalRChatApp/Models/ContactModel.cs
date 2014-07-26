using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRChatApp.Models
{
    public class ContactModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Relationship { get; set; }
        public string PreferredContact { get; set; }
        public string Address { get; set; }
        public string Cellphone { get; set; }
        public string Homephone {get; set;}
        public string Email { get;set;}
    }
}