﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class NannyStateEntities : DbContext
    {
        public NannyStateEntities()
            : base("name=NannyStateEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            
        }
    
        public DbSet<contact> contacts { get; set; }
        public DbSet<event_log> event_logs { get; set; }
        public DbSet<event_history> event_history { get; set; }
    }
}
