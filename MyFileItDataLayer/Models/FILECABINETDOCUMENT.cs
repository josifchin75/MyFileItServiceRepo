//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MyFileItDataLayer.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class FILECABINETDOCUMENT
    {
        public FILECABINETDOCUMENT()
        {
            this.SHAREDOCUMENTs = new HashSet<SHAREDOCUMENT>();
        }
    
        public int ID { get; set; }
        public string DOCUMENTID { get; set; }
        public Nullable<int> APPUSERID { get; set; }
        public Nullable<System.DateTime> SCANDATE { get; set; }
        public string FIRSTNAME { get; set; }
        public string LASTNAME { get; set; }
        public string COMMENT { get; set; }
        public Nullable<System.DateTime> DOCUMENTDATE { get; set; }
        public string DOCUMENTLOCATION { get; set; }
        public int DOCUMENTTYPEID { get; set; }
        public int DOCUMENTSTATUSID { get; set; }
        public Nullable<System.DateTime> DATECREATED { get; set; }
        public string CABINETID { get; set; }
    
        public virtual APPUSER APPUSER { get; set; }
        public virtual DOCUMENTSTATU DOCUMENTSTATU { get; set; }
        public virtual DOCUMENTTYPE DOCUMENTTYPE { get; set; }
        public virtual ICollection<SHAREDOCUMENT> SHAREDOCUMENTs { get; set; }
    }
}
