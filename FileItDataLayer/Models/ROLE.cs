//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class ROLE
    {
        public ROLE()
        {
            this.USERS = new HashSet<USER>();
        }
    
        public string ROLENAME { get; set; }
        public string DESCRIPTION { get; set; }
        public string VIEW_SCANS { get; set; }
        public string EDIT_SCANINDEX { get; set; }
        public string ADD_USERS { get; set; }
        public string EDIT_USERS { get; set; }
        public string DELETE_USERS { get; set; }
        public string MANAGE_PROFILES { get; set; }
        public string ADD_GROUP { get; set; }
        public string EDIT_GROUP { get; set; }
        public string DELETE_GROUP { get; set; }
        public string ADD_GROUP_USERS { get; set; }
        public string ADD_CABINET { get; set; }
        public string DELETE_CABINET { get; set; }
        public string ADD_CABINET_ACCESS { get; set; }
        public string ADD_TEMPLATE { get; set; }
        public string DELETE_TEMPLATE { get; set; }
        public string ADD_SCANNER { get; set; }
        public string EDIT_SCANNER { get; set; }
        public string DELETE_SCANNER { get; set; }
        public string ADD_STORAGE { get; set; }
        public string EDIT_STORAGE { get; set; }
        public string DELETE_STORAGE { get; set; }
        public string PRINT_SCAN { get; set; }
        public string EMAIL_SCAN { get; set; }
        public string DOWNLOAD_SCAN { get; set; }
        public string EDIT_PUBLIC { get; set; }
        public string VIEW_NON_PUBLIC { get; set; }
        public string EDIT_CABINET_FILTERS { get; set; }
        public string BILLING_ACCESS { get; set; }
        public string VERSION_ACCESS { get; set; }
        public string WEBUPLOAD { get; set; }
        public string CHANGE_PASS { get; set; }
        public string CREATE_CABINET { get; set; }
        public string SHOWADMINLINK { get; set; }
        public string SHOW_QUEUES { get; set; }
        public string DELETEIMAGE { get; set; }
        public string RESTOREIMAGE { get; set; }
        public string MOVEIMAGE { get; set; }
        public string FULLINDEXUPDATE { get; set; }
        public string VIEWDOCUMENTHISTORY { get; set; }
        public string MANDATORY_REVIEW_ADMIN { get; set; }
    
        public virtual ICollection<USER> USERS { get; set; }
    }
}
