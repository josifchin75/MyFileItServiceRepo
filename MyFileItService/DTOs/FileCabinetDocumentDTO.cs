using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using MyFileItDataLayer.Models;
using MyFileItService.Helpers;

namespace MyFileItService.DTOs
{
    [DataContract]
    public class FileCabinetDocumentDTO
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string CABINETID { get; set; }
        [DataMember]
        public string DOCUMENTID { get; set; }
        [DataMember]
        public int? APPUSERID { get; set; }
        [DataMember]
        public Nullable<System.DateTime> SCANDATE { get; set; }
        [DataMember]
        public string FIRSTNAME { get; set; }
        [DataMember]
        public string LASTNAME { get; set; }
        [DataMember]
        public int DOCUMENTTYPEID { get; set; }
        [DataMember]
        public string COMMENT { get; set; }
        [DataMember]
        public Nullable<System.DateTime> DOCUMENTDATE { get; set; }
       
        [DataMember]
        public string DOCUMENTLOCATION { get; set; }
        [DataMember]
        public int DOCUMENTSTATUSID { get; set; }
        [DataMember]
        public Nullable<System.DateTime> DATECREATED { get; set; }

        [DataMember]
        public int? VerifiedAppUserId { get; set; }

        [DataMember]
        public string VerifiedAppUserName { get; set; }

        [DataMember]
        public int? TeamEventDocumentId { get; set; }

        [DataMember]
        public string Base64Image { get; set; }
        [DataMember]
        public string Base64ImageThumb { get; set; }

        [DataMember]
        public string DocumentTypeName { get; set; }

        [DataMember]
        public bool CanDelete { get; set; }

        public FileCabinetDocumentDTO() { }

        public FileCabinetDocumentDTO(FILECABINETDOCUMENT fileCabinetDocumentEF)
        {
            ID = fileCabinetDocumentEF.ID;
            CABINETID = fileCabinetDocumentEF.CABINETID;
            DOCUMENTID = fileCabinetDocumentEF.DOCUMENTID;
            APPUSERID = fileCabinetDocumentEF.APPUSERID;
            SCANDATE = fileCabinetDocumentEF.SCANDATE;
            FIRSTNAME = fileCabinetDocumentEF.FIRSTNAME;
            LASTNAME = fileCabinetDocumentEF.LASTNAME;
            DOCUMENTTYPEID = fileCabinetDocumentEF.DOCUMENTTYPEID;
            DocumentTypeName = fileCabinetDocumentEF.DOCUMENTTYPE.NAME;
            COMMENT = fileCabinetDocumentEF.COMMENT;
            DOCUMENTDATE = fileCabinetDocumentEF.DOCUMENTDATE;
            DOCUMENTLOCATION = fileCabinetDocumentEF.DOCUMENTLOCATION;
            DOCUMENTSTATUSID = fileCabinetDocumentEF.DOCUMENTSTATUSID;
            DATECREATED = fileCabinetDocumentEF.DATECREATED;
            CanDelete = !fileCabinetDocumentEF.SHAREDOCUMENTs.Any();
        }

        public static implicit operator FILECABINETDOCUMENT(FileCabinetDocumentDTO dto)
        {
            return new FILECABINETDOCUMENT()
            {
                ID = dto.ID,
                CABINETID = dto.CABINETID,
                DOCUMENTID = dto.DOCUMENTID,
                APPUSERID = dto.APPUSERID,
                SCANDATE = dto.SCANDATE,
                FIRSTNAME = dto.FIRSTNAME,
                LASTNAME = dto.LASTNAME,
                DOCUMENTTYPEID = dto.DOCUMENTTYPEID,
                COMMENT = dto.COMMENT,
                DOCUMENTDATE = dto.DOCUMENTDATE,   
                DOCUMENTLOCATION = dto.DOCUMENTLOCATION,
                DOCUMENTSTATUSID = dto.DOCUMENTSTATUSID,
                DATECREATED = dto.DATECREATED
            };
        }

        public Dictionary<int, string> FileItIndexInformation()
        {
            var result = new Dictionary<int, string>();

            var currentDate = (DateTime?)DateTime.Now;

            result.Add(1, APPUSERID.ToString());
            result.Add(2, SCANDATE.ConvertToFirebirdDate());
            result.Add(3, FIRSTNAME);
            result.Add(4, LASTNAME);
            result.Add(5, DOCUMENTTYPEID.ToString());
            //add the document type to the data
            result.Add(6, GetDocumentType(DOCUMENTTYPEID));
            result.Add(7, DOCUMENTDATE.ConvertToFirebirdDate());
            result.Add(8,  currentDate.ConvertToFirebirdDate()); //removed from table to be used in share verification
            result.Add(9, null);//VERIFIEDAPPUSERID == null ?  null : VERIFIEDAPPUSERID.ToString());

            return result;
        }

        private string GetDocumentType(int documentTypeId)
        {
            var result = "";

            using (var db = new MyFileItEntities())
            {
                var obj = db.DOCUMENTSTATUS.SingleOrDefault(d => d.ID == documentTypeId);
                if (obj != null)
                {
                    result = obj.NAME;
                }
            }
            return result;
        }

    }
}