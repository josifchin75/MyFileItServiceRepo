using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using MyFileItDataLayer.Models;

namespace MyFileItService.DTOs
{
    [DataContract]
    public class TeamEventPlayerRosterDTO
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public int TEAMEVENTID { get; set; }
        [DataMember]
        public Nullable<int> APPUSERID { get; set; }
        [DataMember]
        public string PLAYERPOSITION { get; set; }
        [DataMember]
        public Nullable<int> JERSEYNUMBER { get; set; }
        [DataMember]
        public Nullable<System.DateTime> DATECREATED { get; set; }
        [DataMember]
        public int? USERSTAGETYPEID { get; set; }
        [DataMember]
        public string PlayerName { get; set; }
        [DataMember]
        public string Sex { get; set; }
        [DataMember]
        public bool RequiresVerification { get; set; }

        public TeamEventPlayerRosterDTO()
        {
        }

        public TeamEventPlayerRosterDTO(TEAMEVENTPLAYERROSTER teamEventPlayerRosterEF)
        {
            ID = teamEventPlayerRosterEF.ID;
            TEAMEVENTID = teamEventPlayerRosterEF.TEAMEVENTID;
            APPUSERID = teamEventPlayerRosterEF.APPUSERID;
            PLAYERPOSITION = teamEventPlayerRosterEF.PLAYERPOSITION;
            JERSEYNUMBER = teamEventPlayerRosterEF.JERSEYNUMBER;
            USERSTAGETYPEID = teamEventPlayerRosterEF.USERSTAGETYPEID;
            DATECREATED = teamEventPlayerRosterEF.DATECREATED;
           
            PlayerName = teamEventPlayerRosterEF.APPUSER.FIRSTNAME + " " + teamEventPlayerRosterEF.APPUSER.LASTNAME;
            Sex = teamEventPlayerRosterEF.APPUSER.SEX;
            RequiresVerification = false;
            
            //using (var db = new MyFileItEntities()) {
            //get the team Event documents
            //get all sharedocuments for teamEventDocumentId  -> get all FileCabinetDocument with FILECABINETDOCUMENTID where VerifiedAppUserId = null
            //if any then RequiresVerification = true;
                var fileCabinetDocumentIds = new List<int>();
                teamEventPlayerRosterEF.TEAMEVENT.TEAMEVENTDOCUMENTs.ToList().ForEach(ted =>
                {
                    //var l = ted.SHAREDOCUMENTs.ToList();
                    ted.SHAREDOCUMENTs.Where(sdd => sdd.APPUSERID == teamEventPlayerRosterEF.APPUSERID).ToList().ForEach(sd =>
                    {
                        if (sd.VERIFIEDAPPUSERID == null) {
                            RequiresVerification = true;
                        }
                        /*if (sd.FILECABINETDOCUMENT.VERIFIEDAPPUSERID == null) {
                            RequiresVerification = true;
                        }*/
                    });
                });
            //}
 
        }

        // User-defined conversion from dto to ef 
        public static implicit operator TEAMEVENTPLAYERROSTER(TeamEventPlayerRosterDTO dto)
        {
            return new TEAMEVENTPLAYERROSTER()
            {
                ID = dto.ID,
                TEAMEVENTID = dto.TEAMEVENTID,
                APPUSERID = dto.APPUSERID,
                PLAYERPOSITION = dto.PLAYERPOSITION,
                JERSEYNUMBER = dto.JERSEYNUMBER,
                USERSTAGETYPEID = dto.USERSTAGETYPEID,
                DATECREATED = dto.DATECREATED
            };
        }
    }
}