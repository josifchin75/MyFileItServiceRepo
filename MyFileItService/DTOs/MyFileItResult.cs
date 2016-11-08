using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace MyFileItService.DTOs
{
    [DataContract]
    public class MyFileItResult
    {
        [DataMember]
        public bool Success { get; set; }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public Dictionary<int, string> KeyValueData { get; set; }

        [DataMember]
        public List<OrganizationDTO> Organizations { get; set; }

        [DataMember]
        public List<TeamEventDTO> TeamEvents { get; set; }

        [DataMember]
        public List<TeamEventDocumentDTO> TeamEventDocuments { get; set; }

        [DataMember]
        public List<TeamEventPlayerRosterDTO> TeamEventPlayerRosters { get; set; }

        [DataMember]
        public List<AppUserDTO> AppUsers { get; set; }

        [DataMember]
        public List<SalesRepDTO> SalesReps { get; set; }

        [DataMember]
        public List<CoachDTO> Coaches { get; set; }

        [DataMember]
        public List<ShareKeyDTO> ShareKeys { get; set; }

        [DataMember]
        public List<FileCabinetDocumentDTO> Documents { get; set; }

        [DataMember]
        public List<PaymentHistoryDTO> PaymentHistories { get; set; }

        [DataMember]
        public List<EmailLogDTO> EmailLogs { get; set; }

        [DataMember]
        public string ReportData { get; set; }

        public MyFileItResult()
        {
            KeyValueData = new Dictionary<int, string>();
            Organizations = new List<OrganizationDTO>();
            TeamEvents = new List<TeamEventDTO>();
            TeamEventDocuments = new List<TeamEventDocumentDTO>();
            TeamEventPlayerRosters = new List<TeamEventPlayerRosterDTO>();
            AppUsers = new List<AppUserDTO>();
            SalesReps = new List<SalesRepDTO>();
            Coaches = new List<CoachDTO>();
            ShareKeys = new List<ShareKeyDTO>();
            Documents = new List<FileCabinetDocumentDTO>();
            PaymentHistories = new List<PaymentHistoryDTO>();
            EmailLogs = new List<EmailLogDTO>();
        }
    }
}