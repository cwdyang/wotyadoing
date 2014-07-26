using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalRChatApp.Models
{
    public class AlertModel
    {
        public string Severity { get; set; }
        public string Status { get; set; }
        public IList<string> Severities { get; set; }
        public IList<string> Statuses { get; set; }
        public ContactModel PrimaryContact { get; set; }
        public IList<ContactModel> Contacts { get; set; }

        public string CareReceiver { get; set; }
        public string AlertType { get; set; }
        public string IncidentAddress { get; set; }
        public bool AtHome { get; set; }
        public string HomeAddress { get; set; }
        public string SeverityComment { get; set; }
        public string StatusComment { get; set; }
    }
}