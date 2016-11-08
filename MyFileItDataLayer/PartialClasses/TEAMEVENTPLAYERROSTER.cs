using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFileItDataLayer.Models
{
    public partial class TEAMEVENTPLAYERROSTER
    {
        public void SetNewID()
        {
            using (var db = new MyFileItEntities())
            {
                var ev = db.TEAMEVENTPLAYERROSTERs.OrderByDescending(o => o.ID).FirstOrDefault();
                this.ID = ev == null ? 1 : ev.ID + 1;
            }
        }
        public static bool ValidTeamEventPlayerRoster(int teamEventId, int appUserId, ref string error)
        {
            var result = true;
            using (var db = new MyFileItEntities())
            {
                result = !db.TEAMEVENTPLAYERROSTERs.Any(r => r.APPUSERID == appUserId && r.TEAMEVENTID == teamEventId);
                if (!result)
                {
                    error = "Player is already on the roster.";
                }
            }

            return result;
        }

        public bool UpdateObject(TEAMEVENTPLAYERROSTER updated)
        {
            var result = false;

            ID = updated.ID;
            TEAMEVENTID = updated.TEAMEVENTID;
            APPUSERID = updated.APPUSERID;
            PLAYERPOSITION = updated.PLAYERPOSITION;
            JERSEYNUMBER = updated.JERSEYNUMBER;
            DATECREATED = updated.DATECREATED;

            result = true;
            return result;
        }
    }


}
