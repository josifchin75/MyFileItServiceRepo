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
    public class FileCabinetDocumentSingleDTO
    {
        [DataMember]
        public int FILECABINETDOCUMENTID { get; set; }
        [DataMember]
        public int? TEAMEVENTDOCUMENTID { get; set; }
        [DataMember]
        public int? VerifiedAppUserId { get; set; }
    }
}
