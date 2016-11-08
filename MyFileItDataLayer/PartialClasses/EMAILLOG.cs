using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFileItDataLayer.Models
{
    public partial class EMAILLOG
    {
        public void SetNewID()
        {
            using (var db = new MyFileItEntities())
            {
                var ev = db.EMAILLOGs.OrderByDescending(o => o.ID).FirstOrDefault();
                this.ID = ev == null ? 1 : ev.ID + 1;
            }
        }
    }
}
