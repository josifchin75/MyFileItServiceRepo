using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFileItDataLayer.Models
{
    public partial class PAYMENTHISTORY
    {
        public void SetNewID()
        {
            using (var db = new MyFileItEntities())
            {
                var ev = db.PAYMENTHISTORies.OrderByDescending(o => o.ID).FirstOrDefault();
                this.ID = ev == null ? 1 : ev.ID + 1;
            }
        }


        public bool UpdateObject(PAYMENTHISTORY updated)
        {
            var result = false;
            PRIMARYAPPUSERID = updated.PRIMARYAPPUSERID;
            DATEPURCHASED = updated.DATEPURCHASED;
            QUANTITY = updated.QUANTITY;
            ITEM = updated.ITEM;
            AMOUNT = updated.AMOUNT;
            LAST4CC = updated.LAST4CC;
            PROMOCODE = updated.PROMOCODE;
            AUTHORIZATIONNUMBER = updated.AUTHORIZATIONNUMBER;

            result = true;
            return result;
        }
    }
}
