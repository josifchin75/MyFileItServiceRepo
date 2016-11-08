using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyFileItDataLayer.Helpers;

namespace MyFileItDataLayer.Models
{
    public partial class TEAMEVENT
    {
        public void SetNewID()
        {
            using (var db = new MyFileItEntities())
            {
                var ev = db.TEAMEVENTs.OrderByDescending(o => o.ID).FirstOrDefault();
                this.ID = ev == null ? 1 : ev.ID + 1;
            }
        }

        public void FormatData()
        {
            ISDELETED = ISDELETED.ConvertToFirebirdBoolean();
        }

        public bool UpdateObject(TEAMEVENT updated)
        {
            var result = false;

            ORGANIZATIONID = updated.ORGANIZATIONID;
            NAME = updated.NAME;
            YEARCODE = updated.YEARCODE;
            STARTDATE = updated.STARTDATE;
            EXPIRESDATE = updated.EXPIRESDATE;
            ISDELETED = updated.ISDELETED;

            result = true;

            return result;
        }

    }
}
