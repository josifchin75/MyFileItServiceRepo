using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFileItDataLayer.Models
{
    public partial class SHAREKEY
    {
        public SHAREKEY() { }

        public SHAREKEY(int primaryAppUserId, DateTime purchaseDate, string promoCode, string last4Digits, decimal amount, int salesRepId)
        {
            PRIMARYAPPUSERID = primaryAppUserId;
            SHAREKEYCODE = CreateShareKeyCode();
            PURCHASEDATE = purchaseDate;
            PROMOCODE = promoCode;
            LAST4CC = last4Digits;
            AMOUNT = amount;
           // SALESREPID = salesRepId;
            DATECREATED = DateTime.Now;
            SetNewID();
        }

        public void SetNewID()
        {
            using (var db = new MyFileItEntities())
            {
                var sk = db.SHAREKEYs.OrderByDescending(o => o.ID).FirstOrDefault();
                this.ID = sk == null ? 1 : sk.ID + 1;
            }
        }

        private string CreateShareKeyCode() {
            //create the share key code
            //use date time and a count
            var result = DateTime.Now.ToString("yyyyMMdd-");
            result += PRIMARYAPPUSERID.ToString().PadLeft(6,'0');

            var curCnt = 1; 
            var testResult = result + curCnt.ToString().PadLeft(6, '0');
            using (var db = new MyFileItEntities())
            {
                while (db.SHAREKEYs.Any(sk => sk.SHAREKEYCODE.Equals(testResult, StringComparison.CurrentCultureIgnoreCase))) {
                    testResult = result + curCnt.ToString().PadLeft(6,'0');
                    if (curCnt > 1000000) {
                        result = "";
                        testResult = "";
                        break;
                    }
                    curCnt++;
                }
                result = testResult;
            }

            return result;
        }

        public bool UpdateObject(SHAREKEY updated)
        {
            var result = false;

            APPUSERID = updated.APPUSERID;
            PRIMARYAPPUSERID = updated.PRIMARYAPPUSERID;
            PURCHASEDATE = updated.PURCHASEDATE;
            PROMOCODE = updated.PROMOCODE;
            LAST4CC = updated.LAST4CC;
            AMOUNT = updated.AMOUNT;
            SHAREKEYCODE = updated.SHAREKEYCODE;
            SALESREPID = updated.SALESREPID;
            PAYMENTTYPEID =updated.PAYMENTTYPEID;
      
            result = true;
            return result;
        }
    }
}
