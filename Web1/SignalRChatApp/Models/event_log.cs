//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SignalRChatApp.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class event_log
    {
        public string id { get; set; }
        public System.DateTimeOffset C__createdAt { get; set; }
        public Nullable<System.DateTimeOffset> C__updatedAt { get; set; }
        public byte[] C__version { get; set; }
        public string care_receiver { get; set; }
        public string status { get; set; }
        public string event_type { get; set; }
        public string severity { get; set; }
        public string operator_NAME { get; set; }
        public string device_id { get; set; }
        public string comments { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string altitude { get; set; }
        public string event_reason { get; set; }
    }
}