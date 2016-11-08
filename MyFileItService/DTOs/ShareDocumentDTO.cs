using MyFileItDataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using MyFileItService.Helpers;

namespace MyFileItService.DTOs
{
    public class ShareDocumentDTO
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public int? FILECABINETDOCUMENTID { get; set; }
        [DataMember]
        public int? TEAMEVENTDOCUMENTID { get; set; }
        [DataMember]
        public string DOCUMENTID { get; set; }
        [DataMember]
        public int? APPUSERID { get; set; }
        [DataMember]
        public DateTime? SCANDATE { get; set; }
        [DataMember]
        public string COMMENT { get; set; }
        [DataMember]
        public string LOCATION { get; set; }
        [DataMember]
        public int DOCUMENTTYPEID { get; set; }
        [DataMember]
        public int DOCUMENTSTATUSID { get; set; }
        [DataMember]
        public Nullable<System.DateTime> VERIFIEDDATE { get; set; }
        [DataMember]
        public int? VERIFIEDAPPUSERID { get; set; }
        [DataMember]
        public bool EMERGENCY { get; set; }
        [DataMember]
        public string EMERGENCYEMAILADDRESS { get; set; }
        [DataMember]
        public string CABINETID { get; set; }
        [DataMember]
        public DateTime? DOCUMENTDATE { get; set; }
        [DataMember]
        public DateTime? DATECREATED { get; set; }

        public ShareDocumentDTO() { }

        public ShareDocumentDTO(SHAREDOCUMENT shareDocumentEF)
        {
            ID = shareDocumentEF.ID;
            FILECABINETDOCUMENTID = shareDocumentEF.FILECABINETDOCUMENTID;
            TEAMEVENTDOCUMENTID = shareDocumentEF.TEAMEVENTDOCUMENTID;
            DOCUMENTID = shareDocumentEF.DOCUMENTID;
            APPUSERID = shareDocumentEF.APPUSERID;
            SCANDATE = shareDocumentEF.SCANDATE;
            COMMENT = shareDocumentEF.COMMENT;
            LOCATION = shareDocumentEF.LOCATION;
            DOCUMENTTYPEID = shareDocumentEF.DOCUMENTTYPEID;
            DOCUMENTSTATUSID = shareDocumentEF.DOCUMENTSTATUSID;
            VERIFIEDDATE = shareDocumentEF.VERIFIEDDATE;
            VERIFIEDAPPUSERID = shareDocumentEF.VERIFIEDAPPUSERID;

            EMERGENCY = shareDocumentEF.EMERGENCY.Equals("Y",StringComparison.CurrentCultureIgnoreCase) || shareDocumentEF.EMERGENCY.Equals("1",StringComparison.CurrentCultureIgnoreCase);
            EMERGENCYEMAILADDRESS = shareDocumentEF.EMERGENCYEMAILADDRESS;
            CABINETID = shareDocumentEF.CABINETID;
            DOCUMENTDATE = shareDocumentEF.DOCUMENTDATE;
            DATECREATED = shareDocumentEF.DATECREATED;
        }
        // User-defined conversion from dto to ef 
        public static implicit operator SHAREDOCUMENT(ShareDocumentDTO dto)
        {
            return new SHAREDOCUMENT()
            {
                ID = dto.ID,
                FILECABINETDOCUMENTID = dto.FILECABINETDOCUMENTID,
                TEAMEVENTDOCUMENTID = dto.TEAMEVENTDOCUMENTID,
                DOCUMENTID = dto.DOCUMENTID,
                APPUSERID = dto.APPUSERID,
                SCANDATE = dto.SCANDATE,
                COMMENT = dto.COMMENT,
                LOCATION = dto.LOCATION,
                DOCUMENTTYPEID = dto.DOCUMENTTYPEID,
                DOCUMENTSTATUSID = dto.DOCUMENTSTATUSID,
                VERIFIEDDATE = dto.VERIFIEDDATE,
                VERIFIEDAPPUSERID = dto.VERIFIEDAPPUSERID,
                EMERGENCY = dto.EMERGENCY.ConvertToFirebirdString(),
                EMERGENCYEMAILADDRESS = dto.EMERGENCYEMAILADDRESS,
                CABINETID = dto.CABINETID,
                DOCUMENTDATE = dto.DOCUMENTDATE,
                DATECREATED = dto.DATECREATED,
            };
        }
    }
}