﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FileItDataLayer.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class SystemFileitEntities : DbContext
    {
        public SystemFileitEntities()
            : base("name=SystemFileitEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ACCOUNT> ACCOUNTS { get; set; }
        public virtual DbSet<CABINET> CABINETS { get; set; }
        public virtual DbSet<CABINETS_ACCESS> CABINETS_ACCESS { get; set; }
        public virtual DbSet<CABINETS_LOOKUPFILE> CABINETS_LOOKUPFILE { get; set; }
        public virtual DbSet<CUSTOMERCABINET> CUSTOMERCABINETS { get; set; }
        public virtual DbSet<CUSTOMEREMAIL> CUSTOMEREMAILS { get; set; }
        public virtual DbSet<CUSTOMERRATE> CUSTOMERRATES { get; set; }
        public virtual DbSet<CUSTOMER> CUSTOMERS { get; set; }
        public virtual DbSet<EMAIL_IN> EMAIL_IN { get; set; }
        public virtual DbSet<EMAIL_OUT> EMAIL_OUT { get; set; }
        public virtual DbSet<GROUP> GROUPS { get; set; }
        public virtual DbSet<GROUPS_USERS> GROUPS_USERS { get; set; }
        public virtual DbSet<LOG> LOGs { get; set; }
        public virtual DbSet<LOG_EMAIL> LOG_EMAIL { get; set; }
        public virtual DbSet<MULTCABINETSEARCH> MULTCABINETSEARCHes { get; set; }
        public virtual DbSet<ORDERITEM> ORDERITEMS { get; set; }
        public virtual DbSet<ORDER> ORDERS { get; set; }
        public virtual DbSet<QUEUE_DEFINITIONS> QUEUE_DEFINITIONS { get; set; }
        public virtual DbSet<QUEUE_HISTORY> QUEUE_HISTORY { get; set; }
        public virtual DbSet<QUEUE_NOTES> QUEUE_NOTES { get; set; }
        public virtual DbSet<QUEUE_SCANNER_ACCESS> QUEUE_SCANNER_ACCESS { get; set; }
        public virtual DbSet<QUEUE_USER_ACCESS> QUEUE_USER_ACCESS { get; set; }
        public virtual DbSet<QUEUE_WORKING> QUEUE_WORKING { get; set; }
        public virtual DbSet<ROLE> ROLES { get; set; }
        public virtual DbSet<SCANNER> SCANNERS { get; set; }
        public virtual DbSet<STDTEMPLATE_DEFINITION> STDTEMPLATE_DEFINITION { get; set; }
        public virtual DbSet<STDTEMPLATE> STDTEMPLATES { get; set; }
        public virtual DbSet<STORAGE_MAP> STORAGE_MAP { get; set; }
        public virtual DbSet<SYSTEM_LOG> SYSTEM_LOG { get; set; }
        public virtual DbSet<TEMPLATE_DEFINITION> TEMPLATE_DEFINITION { get; set; }
        public virtual DbSet<TEMPLATE> TEMPLATES { get; set; }
        public virtual DbSet<USER> USERS { get; set; }
        public virtual DbSet<VERSIONTEMP> VERSIONTEMPs { get; set; }
        public virtual DbSet<WORKFLOW> WORKFLOWS { get; set; }
        public virtual DbSet<LISTBOX_TEMPLATES> LISTBOX_TEMPLATES { get; set; }
        public virtual DbSet<SCANNER_CABINETS> SCANNER_CABINETS { get; set; }
    }
}