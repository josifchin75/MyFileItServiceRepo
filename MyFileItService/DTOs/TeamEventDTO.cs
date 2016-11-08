using MyFileItDataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace MyFileItService.DTOs
{
    [DataContract]
    public class TeamEventDTO
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public int ORGANIZATIONID { get; set; }
        [DataMember]
        public string OrganizationName { get; set; }
        [DataMember]
        public string NAME { get; set; }
        [DataMember]
        public Nullable<int> YEARCODE { get; set; }
        [DataMember]
        public Nullable<System.DateTime> STARTDATE { get; set; }
        [DataMember]
        public Nullable<System.DateTime> EXPIRESDATE { get; set; }
        [DataMember]
        public Nullable<System.DateTime> DATECREATED { get; set; }
        [DataMember]
        public string ISDELETED { get; set; }

        public TeamEventDTO() { }

        public TeamEventDTO(TEAMEVENT teamEventEF)
        {
            ID = teamEventEF.ID;
            ORGANIZATIONID = teamEventEF.ORGANIZATIONID;

            var organizationName = "";
            using (var db = new MyFileItEntities()) {
                organizationName = db.ORGANIZATIONs.Single(o => o.ID == teamEventEF.ORGANIZATIONID).NAME;
            }

            OrganizationName = organizationName;//teamEventEF.ORGANIZATION.NAME;
            NAME = teamEventEF.NAME;
            YEARCODE = teamEventEF.YEARCODE;
            STARTDATE = teamEventEF.STARTDATE;
            EXPIRESDATE = teamEventEF.EXPIRESDATE;
            DATECREATED = teamEventEF.DATECREATED;
            ISDELETED = teamEventEF.ISDELETED;
        }

        // User-defined conversion from dto to ef 
        public static implicit operator TEAMEVENT(TeamEventDTO dto)
        {
            return new TEAMEVENT()
            {
                ID = dto.ID,
                ORGANIZATIONID = dto.ORGANIZATIONID,
                NAME = dto.NAME,
                YEARCODE = dto.YEARCODE,
                STARTDATE = dto.STARTDATE,
                EXPIRESDATE = dto.EXPIRESDATE,
                DATECREATED = dto.DATECREATED,
                ISDELETED = dto.ISDELETED
            };
        }
    }
}