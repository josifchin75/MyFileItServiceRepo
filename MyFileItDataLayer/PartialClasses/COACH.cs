using MyFileItDataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFileItDataLayer.Models
{
    public partial class COACH
    {
        public void SetNewID()
        {
            using (var db = new MyFileItEntities())
            {
                var ev = db.COACHes.OrderByDescending(o => o.ID).FirstOrDefault();
                this.ID = ev == null ? 1 : ev.ID + 1;
            }
        }

        public static bool ValidCoachName(string firstName, string lastName)
        {
            var result = true;
            using (var db = new MyFileItEntities())
            {
                result = !db.COACHes.Any(sr => sr.FIRSTNAME.Equals(firstName, StringComparison.CurrentCultureIgnoreCase) || sr.LASTNAME.Equals(lastName, StringComparison.CurrentCultureIgnoreCase));
            }
            return result;
        }

        public bool UpdateObject(COACH updated)
        {
            var result = false;

            ORGANIZATIONID = updated.ORGANIZATIONID;
            APPUSERID = updated.APPUSERID;
            FIRSTNAME = updated.FIRSTNAME;
            LASTNAME = updated.LASTNAME;
            ADDRESS1 = updated.ADDRESS1;
            ADDRESS2 = updated.ADDRESS2;
            CITY = updated.CITY;
            STATECODE = updated.STATECODE;
            ZIPCODE = updated.ZIPCODE;
            PHONE = updated.PHONE;
            EMAILADDRESS = updated.EMAILADDRESS;
            YEARCODE = updated.YEARCODE;
            SPORTTYPEID = updated.SPORTTYPEID;
            RELATIONSHIPTYPEID = updated.RELATIONSHIPTYPEID;
            COACHSTATUSID = updated.COACHSTATUSID;
            TEAMEVENTID = updated.TEAMEVENTID;

            result = true;
            return result;
        }
    }
}
