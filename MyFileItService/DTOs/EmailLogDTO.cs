using MyFileItDataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace MyFileItService.DTOs
{
    [DataContract]
    public class EmailLogDTO
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string TOEMAILADDRESS { get; set; }
        [DataMember]
        public string SUBJECTLINE { get; set; }
        [DataMember]
        public string MESSAGE { get; set; }
       [DataMember]
        public DateTime? DATECREATED { get; set; }

        public EmailLogDTO() { }

        public EmailLogDTO(EMAILLOG emailLogEF)
        {
            ID = emailLogEF.ID;
            TOEMAILADDRESS = emailLogEF.TOEMAILADDRESS;
            SUBJECTLINE = emailLogEF.SUBJECTLINE;
            MESSAGE = emailLogEF.MESSAGE;
            DATECREATED = emailLogEF.DATECREATED;
        }
        // User-defined conversion from dto to ef 
        public static implicit operator EMAILLOG(EmailLogDTO dto)
        {
            return new EMAILLOG()
            {
                ID = dto.ID,
                TOEMAILADDRESS = dto.TOEMAILADDRESS,
                SUBJECTLINE = dto.SUBJECTLINE,
                MESSAGE = dto.MESSAGE,
                DATECREATED = dto.DATECREATED
            };
        }
    }
}