using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFileItDataLayer.Models
{
    public partial class MyFileItEntities
    {
        public Dictionary<int, string> GetReferenceData(string referenceTableName)
        {
            var result = new Dictionary<int, string>();
            switch (referenceTableName)
            {
                case "CoachStatus":
                    COACHSTATUS.ToList().ForEach(r => { result.Add(r.ID, r.NAME); });
                    break;
                case "RelationShipType":
                    RELATIONSHIPTYPEs.ToList().ForEach(r => { result.Add(r.ID, r.NAME); });
                    break;
                case "SportType":
                    SPORTTYPEs.ToList().ForEach(r => { result.Add(r.ID, r.NAME); });
                    break;
                case "AppUserType":
                    APPUSERTYPEs.ToList().ForEach(r => { result.Add(r.ID, r.NAME); });
                    break;
                case "AppUserStatus":
                    APPUSERSTATUS.ToList().ForEach(r => { result.Add(r.ID, r.NAME); });
                    break;
                case "OrganizationType":
                    ORGANIZATIONTYPEs.ToList().ForEach(r => { result.Add(r.ID, r.NAME); });
                    break;
                case "OrganizationStatus":
                    ORGANIZATIONSTATUS.ToList().ForEach(r => { result.Add(r.ID, r.NAME); });
                    break;
                case "TeamEventDocumentStatus":
                    TEAMEVENTDOCUMENTSTATUS.ToList().ForEach(r => { result.Add(r.ID, r.NAME); });
                    break;
                case "DocumentType":
                    DOCUMENTTYPEs.ToList().ForEach(r => { result.Add(r.ID, r.NAME); });
                    break;
                case "DocumentStatus":
                    DOCUMENTSTATUS.ToList().ForEach(r => { result.Add(r.ID, r.NAME); });
                    break;
                case "UserStageType":
                    USERSTAGETYPEs.ToList().ForEach(r => { result.Add(r.ID, r.NAME); });
                    break;
            }

            return result;
        }
    }
}
