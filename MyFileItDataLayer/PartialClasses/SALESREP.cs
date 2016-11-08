using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFileItDataLayer.Models
{
    public partial class SALESREP
    {
        public void SetNewID()
        {
            using (var db = new MyFileItEntities())
            {
                var ev = db.SALESREPs.OrderByDescending(o => o.ID).FirstOrDefault();
                this.ID = ev == null ? 1 : ev.ID + 1;
            }
        }

        public static bool ValidSalesRepName(string emailAddress) {
            var result = true;
            using (var db = new MyFileItEntities())
            {
                result = !db.SALESREPs.Any(sr => sr.EMAILADDRESS.ToLower() == emailAddress);
                //result = !db.SALESREPs.Any(sr => sr.FIRSTNAME.Equals(firstName, StringComparison.CurrentCultureIgnoreCase) && sr.LASTNAME.Equals(lastName, StringComparison.CurrentCultureIgnoreCase));
            }
            return result;
        }

        public bool UpdateObject(SALESREP updated)
        {
            var result = false;
            FIRSTNAME = updated.FIRSTNAME;
            LASTNAME = updated.LASTNAME;
            ADDRESS1 = updated.ADDRESS1;
            ADDRESS2 = updated.ADDRESS2;
            CITY = updated.CITY;
            STATECODE = updated.STATECODE;
            ZIPCODE = updated.ZIPCODE;
            PHONE = updated.PHONE;
            CONTACTPERSON = updated.CONTACTPERSON;
            EMAILADDRESS = updated.EMAILADDRESS;
            PASSWORD = updated.PASSWORD;
            SALESREPSTATUSID = updated.SALESREPSTATUSID;
            COMMENT = updated.COMMENT;
            DEACTIVATEDATE= updated.DEACTIVATEDATE;

            result = true;

            return result;
        }
    }
}
