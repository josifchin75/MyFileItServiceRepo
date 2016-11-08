using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileItDataLayer.Models
{
    public class DocumentLookup
    {
        public int IndexNumber { get; set; }
        public string LookupValue { get; set; }
        public string Operator { get; set; }
    }
}
