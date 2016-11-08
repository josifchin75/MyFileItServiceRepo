using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileItDataLayer.Models
{
    public partial class USER
    {
        public bool ActiveUserBool
        {
            get {
                return ACTIVEUSER.Equals("Y", StringComparison.CurrentCultureIgnoreCase) ? true : false;
            }
            set
            {
                ACTIVEUSER = value ? "Y" : "N";
            }
        }

        public Dictionary<string, string> Cabinets
        {
            get
            {
                var group = GroupName;
                var cabinets = new Dictionary<string, string>();
                using (var db = new FileItDataLayer.Models.SystemFileitEntities())
                {
                    var cabinetAccess = db.CABINETS_ACCESS
                        .Where(a =>
                            (a.ACCESSTYPE.Equals("User", StringComparison.CurrentCultureIgnoreCase) && a.ACCESSNAME == USERNAME)
                            ||
                            (a.ACCESSTYPE.Equals("Group", StringComparison.CurrentCultureIgnoreCase) && a.ACCESSNAME == group))
                            .Select(a => a.CABINETNAME)
                            .ToList();
                    db.CABINETS.Where(c => cabinetAccess.Contains(c.CABINETNAME)).ToList().ForEach(c =>
                    {
                        cabinets.Add(c.CABINETID, c.CABINETNAME);
                    });
                }
                return cabinets;
            }
        }

        public Dictionary<string, string> AvailableCabinets
        {
            get
            {
                var cabinets = new Dictionary<string, string>();
                using (var db = new FileItDataLayer.Models.SystemFileitEntities())
                {
                    db.CABINETS.ToList().ForEach(c =>
                        {
                            cabinets.Add(c.CABINETID, c.CABINETNAME);
                        });
                }
                return cabinets;
            }
        }

        public string GroupName
        {
            get
            {
                var groupName = "";
                using (var db = new FileItDataLayer.Models.SystemFileitEntities())
                {
                    groupName = db.GROUPS_USERS.Where(g => g.USERNAME == USERNAME).Select(g => g.GROUPNAME).First();
                }
                return groupName;
            }
        }

        public bool Authenticate(string password)
        {
            return (this.PASS == password && ActiveUserBool);
        }
    }
}
