using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using MyFileItDataLayer.Models;

namespace MyFileItService.DTOs
{
    public class CoachDTO
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public int? ORGANIZATIONID { get; set; }
        [DataMember]
        public int? APPUSERID { get; set; }
        [DataMember]
        public string FIRSTNAME { get; set; }
        [DataMember]
        public string LASTNAME { get; set; }
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
        public string EMAILADDRESS { get; set; }
        [DataMember]
        public string SEX { get; set; }
        [DataMember]
        public int? YEARCODE { get; set; }
        [DataMember]
        public int? SPORTTYPEID { get; set; }
        [DataMember]
        public int? RELATIONSHIPTYPEID { get; set; }
        [DataMember]
        public int? COACHSTATUSID { get; set; }
        [DataMember]
        public DateTime? DATECREATED { get; set; }
        [DataMember]
        public int TEAMEVENTID { get; set; }



        public CoachDTO() { }

        public CoachDTO(COACH coachEF, string emailOverride = "")
        {
            ID = coachEF.ID;
            ORGANIZATIONID = coachEF.ORGANIZATIONID;
            APPUSERID = coachEF.APPUSERID;
            FIRSTNAME = coachEF.FIRSTNAME;
            LASTNAME = coachEF.LASTNAME;
            ADDRESS1 = coachEF.ADDRESS1;
            ADDRESS2 = coachEF.ADDRESS2;
            CITY = coachEF.CITY;
            STATECODE = coachEF.STATECODE;
            ZIPCODE = coachEF.ZIPCODE;
            PHONE = coachEF.PHONE;
            EMAILADDRESS = emailOverride.Length > 0 ? emailOverride : coachEF.EMAILADDRESS;
            SEX = coachEF.SEX;
            YEARCODE = coachEF.YEARCODE;
            SPORTTYPEID = coachEF.SPORTTYPEID;
            RELATIONSHIPTYPEID = coachEF.RELATIONSHIPTYPEID;
            COACHSTATUSID = coachEF.COACHSTATUSID;
            TEAMEVENTID = int.Parse(coachEF.TEAMEVENTID.ToString());
        }
        // User-defined conversion from dto to ef 
        public static implicit operator COACH(CoachDTO dto)
        {
            return new COACH()
            {
                ID = dto.ID,
                ORGANIZATIONID = dto.ORGANIZATIONID,
                APPUSERID = dto.APPUSERID,
                FIRSTNAME = dto.FIRSTNAME,
                LASTNAME = dto.LASTNAME,
                ADDRESS1 = dto.ADDRESS1,
                ADDRESS2 = dto.ADDRESS2,
                CITY = dto.CITY,
                STATECODE = dto.STATECODE,
                ZIPCODE = dto.ZIPCODE,
                PHONE = dto.PHONE,
                EMAILADDRESS = dto.EMAILADDRESS,
                SEX = dto.SEX,
                YEARCODE = dto.YEARCODE,
                SPORTTYPEID = dto.SPORTTYPEID,
                RELATIONSHIPTYPEID = dto.RELATIONSHIPTYPEID,
                COACHSTATUSID = dto.COACHSTATUSID,
                TEAMEVENTID = dto.TEAMEVENTID
            };
        }

    }
}