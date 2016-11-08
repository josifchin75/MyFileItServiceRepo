using MyFileItDataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace MyFileItService.DTOs
{
    [DataContract]
    public class SalesRepDTO
    {
        [DataMember]
        public int ID { get; set; }
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
        public string CONTACTPERSON { get; set; }
        [DataMember]
        public string EMAILADDRESS { get; set; }
        [DataMember]
        public string PASSWORD { get; set; }
        [DataMember]
        public int? SALESREPSTATUSID { get; set; }
        [DataMember]
        public string COMMENT { get; set; }
        [DataMember]
        public DateTime? DEACTIVATEDATE { get; set; }
        [DataMember]
        public DateTime? DATECREATED { get; set; }

        public SalesRepDTO() { }

        public SalesRepDTO(SALESREP salesRepEF)
        {
            ID = salesRepEF.ID;
            FIRSTNAME = salesRepEF.FIRSTNAME;
            LASTNAME = salesRepEF.LASTNAME;
            ADDRESS1 = salesRepEF.ADDRESS1;
            ADDRESS2 = salesRepEF.ADDRESS2;
            CITY = salesRepEF.CITY;
            STATECODE = salesRepEF.STATECODE;
            ZIPCODE = salesRepEF.ZIPCODE;
            PHONE = salesRepEF.PHONE;
            CONTACTPERSON = salesRepEF.CONTACTPERSON;
            EMAILADDRESS = salesRepEF.EMAILADDRESS;
            PASSWORD = salesRepEF.PASSWORD;
            SALESREPSTATUSID = salesRepEF.SALESREPSTATUSID;
            COMMENT = salesRepEF.COMMENT;
            DEACTIVATEDATE = salesRepEF.DEACTIVATEDATE;
        }
        // User-defined conversion from dto to ef 
        public static implicit operator SALESREP(SalesRepDTO dto)
        {
            return new SALESREP()
            {
                ID = dto.ID,
                FIRSTNAME = dto.FIRSTNAME,
                LASTNAME = dto.LASTNAME,
                ADDRESS1 = dto.ADDRESS1,
                ADDRESS2 = dto.ADDRESS2,
                CITY = dto.CITY,
                STATECODE = dto.STATECODE,
                ZIPCODE = dto.ZIPCODE,
                PHONE = dto.PHONE,
                CONTACTPERSON = dto.CONTACTPERSON,
                EMAILADDRESS = dto.EMAILADDRESS,
                PASSWORD = dto.PASSWORD,
                SALESREPSTATUSID = dto.SALESREPSTATUSID,
                COMMENT = dto.COMMENT,
                DEACTIVATEDATE = dto.DEACTIVATEDATE,
                DATECREATED = dto.DATECREATED
            };
        }
    }
}