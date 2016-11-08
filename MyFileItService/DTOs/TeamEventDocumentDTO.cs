using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using MyFileItDataLayer.Models;

namespace MyFileItService.DTOs
{
    [DataContract]
    public class TeamEventDocumentDTO
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public int TEAMEVENTID { get; set; }
        [DataMember]
        public string DOCUMENTNAME { get; set; }
        [DataMember]
        public DateTime? PLAYERENTEREXPIRATION { get; set; }
        [DataMember]
        public DateTime? PLAYERENTERDOCUMENTDATE { get; set; }
        [DataMember]
        public string ENTERWHOSCANNED { get; set; }
        [DataMember]
        public string SCANNAME { get; set; }
        [DataMember]
        public string MUSTBEVERIFIED { get; set; }
        [DataMember]
        public Nullable<int> TEAMEVENTDOCUMENTSTATUSID { get; set; }
        [DataMember]
        public string VERIFIERNAME { get; set; }
        [DataMember]
        public string ISCOACHDOCUMENT { get; set; }
        [DataMember]
        public DateTime? ROSTERCLOSEDATE { get; set; }
        [DataMember]
        public int? DocumentId { get; set; }

        public TeamEventDocumentDTO() { }

        public TeamEventDocumentDTO(TEAMEVENTDOCUMENT teamEventDocumentEF, int? appUserId)
        {
            ID = teamEventDocumentEF.ID;
            TEAMEVENTID = teamEventDocumentEF.TEAMEVENTID;
            DOCUMENTNAME = teamEventDocumentEF.DOCUMENTNAME;
            PLAYERENTEREXPIRATION = teamEventDocumentEF.PLAYERENTEREXPIRATION;
            PLAYERENTERDOCUMENTDATE = teamEventDocumentEF.PLAYERENTERDOCUMENTDATE;
            ENTERWHOSCANNED = teamEventDocumentEF.ENTERWHOSCANNED;
            SCANNAME = teamEventDocumentEF.SCANNAME;
            MUSTBEVERIFIED = teamEventDocumentEF.MUSTBEVERIFIED;
            TEAMEVENTDOCUMENTSTATUSID = teamEventDocumentEF.TEAMEVENTDOCUMENTSTATUSID;
            VERIFIERNAME = teamEventDocumentEF.VERIFIERNAME;
            ROSTERCLOSEDATE = teamEventDocumentEF.ROSTERCLOSEDATE;
            ISCOACHDOCUMENT = teamEventDocumentEF.ISCOACHDOCUMENT;

            //incude the document id that is associated to the teameventid
            if (appUserId != null)
            {
                var documentObject = teamEventDocumentEF.SHAREDOCUMENTs.SingleOrDefault(sd => sd.TEAMEVENTDOCUMENTID == teamEventDocumentEF.ID && sd.APPUSERID == appUserId);
                if (documentObject != null)
                {
                    DocumentId = documentObject.FILECABINETDOCUMENTID;
                }
            }
        }

        // User-defined conversion from dto to ef 
        public static implicit operator TEAMEVENTDOCUMENT(TeamEventDocumentDTO dto)
        {
            return new TEAMEVENTDOCUMENT()
            {
                ID = dto.ID,
                TEAMEVENTID = dto.TEAMEVENTID,
                DOCUMENTNAME = dto.DOCUMENTNAME,
                PLAYERENTEREXPIRATION = dto.PLAYERENTEREXPIRATION,
                PLAYERENTERDOCUMENTDATE = dto.PLAYERENTERDOCUMENTDATE,
                ENTERWHOSCANNED = dto.ENTERWHOSCANNED,
                SCANNAME = dto.SCANNAME,
                MUSTBEVERIFIED = dto.MUSTBEVERIFIED,
                TEAMEVENTDOCUMENTSTATUSID = dto.TEAMEVENTDOCUMENTSTATUSID,
                VERIFIERNAME = dto.VERIFIERNAME,
                ISCOACHDOCUMENT = dto.ISCOACHDOCUMENT,
                ROSTERCLOSEDATE = dto.ROSTERCLOSEDATE
            };
        }
    }
}