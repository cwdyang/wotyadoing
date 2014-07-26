using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRChatApp.Models
{
    public partial class event_log
    {
        public IList<string> Severities 
        {
            get
            {
                return new List<string> { "critical", "high", "med", "low" };
            }
        }
        public IList<string> Statuses 
        {
            get
            {
                return new List<string> { "Unassigned", "WIP", "ER sent", "Resolved" };
            }
        }
        public IList<string> AlertTypes
        {
            get 
            {
                return new List<string> { "Cancel", "Gas", "Fall", "Panic" };
            }
        }
        public IEnumerable<contact> Contacts { get; set; }
        public IEnumerable<contact> CareContacts
        {
            get
            {
                return Contacts.Where(c => c.relationship != "self").ToList();
            }
        }

        public contact PrimaryContact
        {
            get
            {
                return Contacts.Where(c => c.primary_contact == 1).FirstOrDefault();
            }
        }

        public contact CareReceiverContact
        {
            get
            {
                return Contacts.Where(c => c.relationship == "self").FirstOrDefault();
            }
        }

        public bool AtHome
        {
            get
            {
                return CareReceiverContact != null && CareReceiverContact.address == altitude;
            }
        }

        public IEnumerable<event_history> History { get; set; }
        
    }
}