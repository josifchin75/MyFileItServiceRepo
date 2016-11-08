using MyFileItDataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace MyFileItService.DTOs
{
    [DataContract]
    public class OrganizationDTO2
    {
        [DataMember]
        public int ID { get; set; }
    }

    [DataContract]
    public class OrganizationDTO
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string USERNAME { get; set; }
        [DataMember]
        public string PASSWORD { get; set; }
        [DataMember]
        public int? APPUSERTYPEID { get; set; }
        [DataMember]
        public string NAME { get; set; }
        [DataMember]
        public string ADDRESS1 { get; set; }
        [DataMember]
        public string ADDRESS2 { get; set; }
        [DataMember]
        public string CITY { get; set; }
        [DataMember]
        public string STATECODE { get; set; }
        [DataMember]
        public string ZIPCODE { get; set; }
        [DataMember]
        public string PHONE { get; set; }
        [DataMember]
        public string CONTACTPERSON { get; set; }
        [DataMember]
        public string EMAILADDRESS { get; set; }
        [DataMember]
        public string LOGOIMAGE { get; set; }
        [DataMember]
        public string COMMENT { get; set; }
        [DataMember]
        public Nullable<int> ORGANIZATIONTYPEID { get; set; }
        [DataMember]
        public Nullable<int> ORGANIZATIONSTATUSID { get; set; }
        [DataMember]
        public Nullable<int> SALESREPID { get; set; }
        [DataMember]
        public string DIRECTORNAME { get; set; }
        [DataMember]
        public string DIRECTOREMAIL { get; set; }
        [DataMember]
        public string DIRECTORPHONE { get; set; }
        [DataMember]
        public string ALLOWCOACHTOCREATEEVENTS { get; set; }
        [DataMember]
        public string CCALLEMAILTODIRECTOR { get; set; }
        [DataMember]
        public string WHOSPAYING { get; set; }
        [DataMember]
        public string CABINETID { get; set; }
        [DataMember]
        public DateTime? DATECREATED { get; set; }

        public OrganizationDTO()
        {
        }

        public OrganizationDTO(ORGANIZATION organizationEF, int? appUserTypeId)
        {
            ID = organizationEF.ID;
            USERNAME = organizationEF.USERNAME;
            PASSWORD = organizationEF.PASSWORD;
            APPUSERTYPEID = appUserTypeId;
            NAME = organizationEF.NAME;
            ADDRESS1 = organizationEF.ADDRESS1;
            ADDRESS2 = organizationEF.ADDRESS2;
            CITY = organizationEF.CITY;
            STATECODE = organizationEF.STATECODE;
            ZIPCODE = organizationEF.ZIPCODE;
            PHONE = organizationEF.PHONE;
            CONTACTPERSON = organizationEF.CONTACTPERSON;
            EMAILADDRESS = organizationEF.EMAILADDRESS;
            LOGOIMAGE = organizationEF.LOGOIMAGE;
            COMMENT = organizationEF.COMMENT;
            ORGANIZATIONTYPEID = organizationEF.ORGANIZATIONTYPEID;
            ORGANIZATIONSTATUSID = organizationEF.ORGANIZATIONSTATUSID;
            SALESREPID = organizationEF.SALESREPID;
            DIRECTORNAME = organizationEF.DIRECTORNAME;
            DIRECTOREMAIL = organizationEF.DIRECTOREMAIL;
            DIRECTORPHONE = organizationEF.DIRECTORPHONE;
            ALLOWCOACHTOCREATEEVENTS = organizationEF.ALLOWCOACHTOCREATEEVENTS;
            CCALLEMAILTODIRECTOR = organizationEF.CCALLEMAILTODIRECTOR;
            WHOSPAYING = organizationEF.WHOSPAYING;
            CABINETID = organizationEF.CABINETID;
        }

        // User-defined conversion from dto to ef 
        public static implicit operator ORGANIZATION(OrganizationDTO dto)
        {
            return new ORGANIZATION()
            {
                ID = dto.ID,
                USERNAME = dto.USERNAME,
                PASSWORD = dto.PASSWORD,
                NAME = dto.NAME,
                ADDRESS1 = dto.ADDRESS1,
                ADDRESS2 = dto.ADDRESS2,
                CITY = dto.CITY,
                STATECODE = dto.STATECODE,
                ZIPCODE = dto.ZIPCODE,
                PHONE = dto.PHONE,
                CONTACTPERSON = dto.CONTACTPERSON,
                EMAILADDRESS = dto.EMAILADDRESS,
                LOGOIMAGE = dto.LOGOIMAGE,
                COMMENT = dto.COMMENT,
                ORGANIZATIONTYPEID = dto.ORGANIZATIONTYPEID,
                ORGANIZATIONSTATUSID = dto.ORGANIZATIONSTATUSID,
                SALESREPID = dto.SALESREPID,
                DIRECTORNAME = dto.DIRECTORNAME,
                DIRECTOREMAIL = dto.DIRECTOREMAIL,
                DIRECTORPHONE = dto.DIRECTORPHONE,
                ALLOWCOACHTOCREATEEVENTS = dto.ALLOWCOACHTOCREATEEVENTS,
                CCALLEMAILTODIRECTOR = dto.CCALLEMAILTODIRECTOR,
                WHOSPAYING = dto.WHOSPAYING,
                CABINETID = dto.CABINETID
            };
        }
    }
}