using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyFileItDataLayer.Helpers;

namespace MyFileItDataLayer.Models
{
    public partial class ORGANIZATION
    {
        public MyFileItEntities DB { get; set; }

        public void SetNewID()
        {
            using (var db = new MyFileItEntities())
            {
                var org = db.ORGANIZATIONs.OrderByDescending(o => o.ID).FirstOrDefault();
                this.ID = org == null ? 1 : org.ID + 1;
            }
        }

        public void FormatData() {
            CCALLEMAILTODIRECTOR = CCALLEMAILTODIRECTOR.ConvertToFirebirdBoolean();
            ALLOWCOACHTOCREATEEVENTS = ALLOWCOACHTOCREATEEVENTS.ConvertToFirebirdBoolean();
        }

        public bool AddCabinet() {
            var result = false;

            //CABINETID = GenerateCabinetID();
            return result;
        }

 

        public bool UpdateObject(ORGANIZATION updated)
        {
            var result = false;
            NAME = updated.NAME;
            ADDRESS1 = updated.ADDRESS1;
            ADDRESS2 = updated.ADDRESS2;
            CITY = updated.CITY;
            STATECODE = updated.STATECODE;
            ZIPCODE = updated.ZIPCODE;
            PHONE = updated.PHONE;
            CONTACTPERSON = updated.CONTACTPERSON;
            EMAILADDRESS = updated.EMAILADDRESS;
            LOGOIMAGE = updated.LOGOIMAGE;
            COMMENT = updated.COMMENT;
            ORGANIZATIONTYPEID = updated.ORGANIZATIONTYPEID;
            ORGANIZATIONSTATUSID = updated.ORGANIZATIONSTATUSID;
            SALESREPID = updated.SALESREPID;
            DIRECTORNAME = updated.DIRECTORNAME;
            DIRECTOREMAIL = updated.DIRECTOREMAIL;
            DIRECTORPHONE = DIRECTORPHONE;
            ALLOWCOACHTOCREATEEVENTS = updated.ALLOWCOACHTOCREATEEVENTS;
            CCALLEMAILTODIRECTOR = updated.CCALLEMAILTODIRECTOR;
            WHOSPAYING = updated.WHOSPAYING;
            CABINETID = updated.CABINETID;

            result = true;

            return result;
        }

        public void CheckDB()
        {
        }
    }
}
