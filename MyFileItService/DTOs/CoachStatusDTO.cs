using MyFileItDataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace MyFileItService.DTOs
{
    [DataContract]
    public class CoachStatusDTO
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string NAME { get; set; }
        [DataMember]
        public bool ALLOWADDEVENT { get; set; }
    }
}