using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFileItDataLayer.Models
{
    public partial class FILECABINETDOCUMENT
    {
        public void SetNewID()
        {
            using (var db = new MyFileItEntities())
            {
                var doc = db.FILECABINETDOCUMENTs.OrderByDescending(d => d.ID).FirstOrDefault();
                this.ID = doc == null ? 1 : doc.ID + 1;
            }
        }

        public bool UpdateObject(FILECABINETDOCUMENT updated)
        {
            var result = false;

            DOCUMENTID = updated.DOCUMENTID;
            APPUSERID = updated.APPUSERID;
            SCANDATE = updated.SCANDATE;
            FIRSTNAME = updated.FIRSTNAME;
            LASTNAME = updated.LASTNAME;
            DOCUMENTTYPEID = updated.DOCUMENTTYPEID;
            COMMENT = updated.COMMENT;
            DOCUMENTDATE = updated.DOCUMENTDATE;
            DOCUMENTLOCATION = updated.DOCUMENTLOCATION;
            DOCUMENTSTATUSID = updated.DOCUMENTSTATUSID;

            result = true;

            return result;
        }
    }
}
