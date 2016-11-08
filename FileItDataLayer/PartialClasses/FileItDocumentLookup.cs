using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileItDataLayer.Models
{
    public class FileItDocumentLookup
    {
        public int IndexNumber { get; set; }
        public string LookupValue { get; set; }
        public string Operator { get; set; }

        public string ToWhereClause() {
            var sb = new StringBuilder();
            if (IndexNumber == -1)
            {
                //temp hook to handle straight lookups
                sb.Append("FILENAME " + Operator + " '" + LookupValue.Replace("'", "''") + "'");
            }
            else
            {
                sb.Append("INDEX" + IndexNumber.ToString() + " " + Operator + " '" + LookupValue.Replace("'", "''") + "'");
            }
                return sb.ToString();
        }
    }

}
