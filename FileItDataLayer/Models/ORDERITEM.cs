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
    
    public partial class ORDERITEM
    {
        public int ID { get; set; }
        public int ORDERNO { get; set; }
        public Nullable<System.DateTime> BILLSTART { get; set; }
        public Nullable<System.DateTime> BILLSTOP { get; set; }
        public Nullable<int> IMAGECOUNT { get; set; }
        public string LINEDESC { get; set; }
        public string LINETYPE { get; set; }
        public Nullable<decimal> AMOUNT { get; set; }
        public Nullable<decimal> AMT_PAID { get; set; }
        public Nullable<int> LINEORDER { get; set; }
    }
}