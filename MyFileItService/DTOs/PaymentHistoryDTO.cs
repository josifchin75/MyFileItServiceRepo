using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using MyFileItDataLayer.Models;
namespace MyFileItService.DTOs
{
    public class PaymentHistoryDTO
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public int PRIMARYAPPUSERID { get; set; }
        [DataMember]
        public Nullable<System.DateTime> DATEPURCHASED { get; set; }
        [DataMember]
        public Nullable<int> QUANTITY { get; set; }
        [DataMember]
        public string ITEM { get; set; }
        [DataMember]
        public Nullable<decimal> AMOUNT { get; set; }
        [DataMember]
        public string LAST4CC { get; set; }
        [DataMember]
        public string PROMOCODE { get; set; }
        [DataMember]
        public string AUTHORIZATIONNUMBER { get; set; }
        [DataMember]
        public string NAME { get; set; }

        public PaymentHistoryDTO() { }

        public PaymentHistoryDTO(PAYMENTHISTORY phEF)
        {
            ID = phEF.ID;
            NAME = phEF.NAME;
            PRIMARYAPPUSERID = phEF.PRIMARYAPPUSERID;
            DATEPURCHASED = phEF.DATEPURCHASED;
            QUANTITY = phEF.QUANTITY;
            ITEM = phEF.ITEM;
            AMOUNT = phEF.AMOUNT;
            LAST4CC = phEF.LAST4CC;
            PROMOCODE = phEF.PROMOCODE;
            AUTHORIZATIONNUMBER = phEF.AUTHORIZATIONNUMBER;
        }
        // User-defined conversion from dto to ef 
        public static implicit operator PAYMENTHISTORY(PaymentHistoryDTO dto)
        {
            return new PAYMENTHISTORY()
            {
                ID = dto.ID,
                NAME = dto.NAME ?? "",
                PRIMARYAPPUSERID = dto.PRIMARYAPPUSERID,
                DATEPURCHASED = dto.DATEPURCHASED,
                QUANTITY = dto.QUANTITY,
                ITEM = dto.ITEM,
                AMOUNT = dto.AMOUNT,
                LAST4CC = dto.LAST4CC,
                PROMOCODE = dto.PROMOCODE,
                AUTHORIZATIONNUMBER = dto.AUTHORIZATIONNUMBER
            };
        }
    }
}
