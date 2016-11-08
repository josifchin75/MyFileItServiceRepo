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
    
    public partial class USER
    {
        public USER()
        {
            this.GROUPS = new HashSet<GROUP>();
            this.GROUPS_USERS = new HashSet<GROUPS_USERS>();
            this.ROLES = new HashSet<ROLE>();
        }
    
        public string USERNAME { get; set; }
        public string FULLNAME { get; set; }
        public string PASS { get; set; }
        public string ACCOUNT { get; set; }
        public string EMAIL { get; set; }
        public string DESCRIPTION { get; set; }
        public string PROFILE { get; set; }
        public Nullable<int> ID { get; set; }
        public Nullable<System.DateTime> CREATED { get; set; }
        public string ACTIVEUSER { get; set; }
        public Nullable<int> PRINTWIDTH { get; set; }
    
        public virtual ICollection<GROUP> GROUPS { get; set; }
        public virtual ICollection<GROUPS_USERS> GROUPS_USERS { get; set; }
        public virtual ICollection<ROLE> ROLES { get; set; }
    }
}