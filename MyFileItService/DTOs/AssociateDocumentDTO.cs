using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace MyFileItService.DTOs
{
    [DataContract]
    public class AssociateDocumentDTO
    {
        [DataMember]
        public int appUserId { get; set; }
        [DataMember]
        public int organizationId { get; set; }
        [DataMember]
        public int fileCabinetDocumentId { get; set; }
        [DataMember]
        public int teamEventDocumentId { get; set; }
        [DataMember]
        public string comment { get; set; }
        [DataMember]
        public bool emergency { get; set; }
        [DataMember]
        public bool remove { get; set; }
    }
}
