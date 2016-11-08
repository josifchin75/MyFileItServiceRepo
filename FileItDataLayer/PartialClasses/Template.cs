using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileItDataLayer.Models
{
    partial class TEMPLATE
    {
        private List<TEMPLATE_DEFINITION> _TemplateDefinitions { get; set;}
        public List<TEMPLATE_DEFINITION> TemplateDefinitions
        {
            get
            {
                if (_TemplateDefinitions == null)
                {
                    var result = new List<TEMPLATE_DEFINITION>();
                    using (var db = new SystemFileitEntities())
                    {
                        result = db.TEMPLATE_DEFINITION.Where(td => td.TEMPLATENAME.Equals(this.TEMPLATENAME, StringComparison.CurrentCultureIgnoreCase)).ToList();
                    }
                    _TemplateDefinitions = result;
                }
                return _TemplateDefinitions;
            }
            set {
                _TemplateDefinitions = value;
            }
        }

        //public Dictionary<string, string> TemplateDefinitionsDictionary
        //{
        //    get {
        //        var d = new Dictionary<string, string>();
        //        TemplateDefinitions.ForEach(td => {
        //            d.Add(td.INDEXNAME, td.DATATYPE);
        //        });
        //        return d;
        //    }
        //}

        internal void CreateNewTemplateRecord()
        {
            throw new NotImplementedException();
        }
    }
}
