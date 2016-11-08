using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyFileItDataLayer.Helpers;

namespace MyFileItDataLayer.Models
{
    public partial class TEAMEVENTDOCUMENT
    {
        public void SetNewID()
        {
            using (var db = new MyFileItEntities())
            {
                var ev = db.TEAMEVENTDOCUMENTs.OrderByDescending(o => o.ID).FirstOrDefault();
                this.ID = ev == null ? 1 : ev.ID + 1;
            }
        }

        public void FormatData()
        {
            ENTERWHOSCANNED = ENTERWHOSCANNED.ConvertToFirebirdBoolean();
            MUSTBEVERIFIED = MUSTBEVERIFIED.ConvertToFirebirdBoolean();
            ISCOACHDOCUMENT = ISCOACHDOCUMENT.ConvertToFirebirdBoolean();
        }

        public static bool ValidTeamEventDocumentName(string documentName)
        {
            var result = true;
            using (var db = new MyFileItEntities())
            {
                result = !db.TEAMEVENTDOCUMENTs.Any(d => d.DOCUMENTNAME.Equals(documentName, StringComparison.CurrentCultureIgnoreCase));
            }

            return true;
        }

        public bool UpdateObject(TEAMEVENTDOCUMENT updated)
        {
            var result = false;

            ID = updated.ID;
            TEAMEVENTID = updated.TEAMEVENTID;
            DOCUMENTNAME = updated.DOCUMENTNAME;
            PLAYERENTEREXPIRATION = updated.PLAYERENTEREXPIRATION;
            PLAYERENTERDOCUMENTDATE = updated.PLAYERENTERDOCUMENTDATE;
            ENTERWHOSCANNED = updated.ENTERWHOSCANNED;
            SCANNAME = updated.SCANNAME;
            MUSTBEVERIFIED = updated.MUSTBEVERIFIED;
            TEAMEVENTDOCUMENTSTATUSID = updated.TEAMEVENTDOCUMENTSTATUSID;
            VERIFIERNAME = updated.VERIFIERNAME;
            ISCOACHDOCUMENT = updated.ISCOACHDOCUMENT;
            ROSTERCLOSEDATE = updated.ROSTERCLOSEDATE;

            result = true;
            return result;
        }
    }
}
