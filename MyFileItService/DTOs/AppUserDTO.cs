using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using MyFileItDataLayer.Models;

namespace MyFileItService.DTOs
{
    [DataContract]
    public class AppUserDTO
    {
        [DataMember]
        public int ID { get; set; }
        [DataMember]
        public string USERNAME { get; set; }
        [DataMember]
        public string PASSWORD { get; set; }
        [DataMember]
        public Nullable<int> PRIMARYAPPUSERID { get; set; }
        [DataMember]
        public Nullable<int> APPUSERID { get; set; }
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
        public string MOBILEPHONENUMBER { get; set; }
        [DataMember]
        public string EMAILADDRESS { get; set; }
        [DataMember]
        public string EMAILADDRESS2 { get; set; }
        [DataMember]
        public string SEX { get; set; }
        [DataMember]
        public Nullable<System.DateTime> BIRTHDATE { get; set; }
        [DataMember]
        public string COMMENT { get; set; }
        [DataMember]
        public Nullable<int> RELATIONSHIPTYPEID { get; set; }
        [DataMember]
        public Nullable<int> APPUSERTYPEID { get; set; }
        [DataMember]
        public Nullable<int> SHAREKEYID { get; set; }
        [DataMember]
        public Nullable<System.DateTime> SHAREKEYEXPIREDATE { get; set; }
        [DataMember]
        public int NumberOfPurchasedShareKeys { get; set; }
        [DataMember]
        public Nullable<int> SALESREPID { get; set; }
        [DataMember]
        public Nullable<int> APPUSERSTATUSID { get; set; }
        [DataMember]
        public Nullable<System.DateTime> DATECREATED { get; set; }

        [DataMember]
        public string CABINETID { get; set; }

        [DataMember]
        public string AppUserStatus { get; set; }

        [DataMember]
        public string RelationShipType { get; set; }

        [DataMember]
        public List<CoachDTO> Coaches { get; set; }

        [DataMember]
        public bool IsCoach { get; set; }

        [DataMember]
        public bool ShowAds { get; set; }

        [DataMember]
        public bool RemindUserForSignUp { get; set; }

        [DataMember]
        public int DaysLeftInTrial { get; set; }

        [DataMember]
        public List<OrganizationDTO> Organizations { get; set; }

        [DataMember]
        public List<ShareKeyDTO> ShareKeys { get; set; }


        public AppUserDTO() { }

        public AppUserDTO(APPUSER appUserEF)
        {
            ID = appUserEF.ID;
            USERNAME = appUserEF.USERNAME;
            PASSWORD = appUserEF.PASSWORD;
            PRIMARYAPPUSERID = appUserEF.PRIMARYAPPUSERID;
            FIRSTNAME = appUserEF.FIRSTNAME;
            LASTNAME = appUserEF.LASTNAME;
            ADDRESS1 = appUserEF.ADDRESS1;
            ADDRESS2 = appUserEF.ADDRESS2;
            CITY = appUserEF.CITY;
            STATECODE = appUserEF.STATECODE;
            ZIPCODE = appUserEF.ZIPCODE;
            PHONE = appUserEF.PHONE;
            MOBILEPHONENUMBER = appUserEF.MOBILEPHONENUMBER;
            EMAILADDRESS = appUserEF.EMAILADDRESS;
            EMAILADDRESS2 = appUserEF.EMAILADDRESS2;
            SEX = appUserEF.SEX;
            BIRTHDATE = appUserEF.BIRTHDATE;
            COMMENT = appUserEF.COMMENT;
            RELATIONSHIPTYPEID = appUserEF.RELATIONSHIPTYPEID;
            RelationShipType = appUserEF.RELATIONSHIPTYPE != null ? appUserEF.RELATIONSHIPTYPE.NAME : null;
            APPUSERTYPEID = appUserEF.APPUSERTYPEID;
            SHAREKEYID = appUserEF.SHAREKEYID;
            SHAREKEYEXPIREDATE = appUserEF.SHAREKEYEXPIREDATE;
            APPUSERSTATUSID = appUserEF.APPUSERSTATUSID;
            AppUserStatus = appUserEF.APPUSERSTATU != null ? appUserEF.APPUSERSTATU.NAME : null;
            DATECREATED = appUserEF.DATECREATED;

            CABINETID = appUserEF.GetCabinetId();

            ShareKeys = appUserEF.AssociatedShareKeys.Select(sk =>
                new ShareKeyDTO(sk, true)
            ).ToList();
            //ShareKeys = appUserEF.SHAREKEYs.Where(sk => sk.APPUSERID == appUserEF.ID).Select(sk => new ShareKeyDTO(sk, true)).ToList();

            NumberOfPurchasedShareKeys = appUserEF.PurchasedShareKeys.Count;

            Organizations = new List<OrganizationDTO>();
            appUserEF.APPUSERORGANIZATIONs.ToList().ForEach(auo =>
            {
                Organizations.Add(new OrganizationDTO(auo.ORGANIZATION, APPUSERTYPEID));
            });

            Coaches = new List<CoachDTO>();
            appUserEF.Coaches.ForEach(c => {
                Coaches.Add(new CoachDTO(c));
            });

            IsCoach = appUserEF.IsCoach;//Coaches.Any();

            ShowAds = !ShareKeys.Any();

            RemindUserForSignUp = !ShareKeys.Any();// && DATECREATED < DateTime.Now.AddDays(-90);
            var expirationDate = ((DateTime)DATECREATED).AddDays(ConfigurationSettings.TrialExpirationDays);
            DaysLeftInTrial = ShareKeys.Any() ? -9999 : (expirationDate - DateTime.Now).Days;
        }

        // User-defined conversion from dto to ef 
        public static implicit operator APPUSER(AppUserDTO dto)
        {
            return new APPUSER()
            {
                ID = dto.ID,
                USERNAME = dto.USERNAME,
                PASSWORD = dto.PASSWORD,
                PRIMARYAPPUSERID = dto.PRIMARYAPPUSERID,
                FIRSTNAME = dto.FIRSTNAME,
                LASTNAME = dto.LASTNAME,
                ADDRESS1 = dto.ADDRESS1,
                ADDRESS2 = dto.ADDRESS2,
                CITY = dto.CITY,
                STATECODE = dto.STATECODE,
                ZIPCODE = dto.ZIPCODE,
                PHONE = dto.PHONE,
                MOBILEPHONENUMBER = dto.MOBILEPHONENUMBER,
                EMAILADDRESS = dto.EMAILADDRESS,
                EMAILADDRESS2 = dto.EMAILADDRESS2,
                SEX = dto.SEX,
                BIRTHDATE = dto.BIRTHDATE,
                COMMENT = dto.COMMENT,
                RELATIONSHIPTYPEID = dto.RELATIONSHIPTYPEID,
                APPUSERTYPEID = dto.APPUSERTYPEID,
                SHAREKEYID = dto.SHAREKEYID,
                SHAREKEYEXPIREDATE = dto.SHAREKEYEXPIREDATE,
                APPUSERSTATUSID = dto.APPUSERSTATUSID,
                DATECREATED = dto.DATECREATED,
            };
        }
    }
}