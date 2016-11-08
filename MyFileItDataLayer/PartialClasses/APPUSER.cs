using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFileItDataLayer.Models
{
    public partial class APPUSER
    {
        public List<SHAREKEY> AssociatedShareKeys
        {
            get
            {
                var result = new List<SHAREKEY>();
                using (var db = new MyFileItEntities())
                {
                    var relatedAppUserIds = new List<int?>();
                    db.APPUSERs.Where(au => au.PRIMARYAPPUSERID == ID).ToList().ForEach(au =>
                    {
                        relatedAppUserIds.Add(au.ID);
                    }); 
                    db.SHAREKEYs.Where(sk => sk.APPUSERID == ID || relatedAppUserIds.Contains(sk.APPUSERID)).ToList().ForEach(sk =>
                    {
                        result.Add(sk);
                    });
                }
                return result;
            }
        }

        public List<SHAREKEY> PurchasedShareKeys
        {
            get
            {
                var result = new List<SHAREKEY>();
                using (var db = new MyFileItEntities())
                {
                    db.SHAREKEYs.Where(sk => sk.PRIMARYAPPUSERID == ID).ToList().ForEach(sk =>
                    {
                        result.Add(sk);
                    });
                }
                return result;
            }
        }

        public List<COACH> Coaches
        {
            get
            {
                var result = new List<COACH>();
                using (var db = new MyFileItEntities())
                {
                    db.COACHes.Where(sk => sk.APPUSERID == ID).ToList().ForEach(sk =>
                    {
                        result.Add(sk);
                    });
                }
                return result;
            }
        }


        public void SetNewID()
        {
            using (var db = new MyFileItEntities())
            {
                var ev = db.APPUSERs.OrderByDescending(o => o.ID).FirstOrDefault();
                this.ID = ev == null ? 1 : ev.ID + 1;
                this.DATECREATED = DateTime.Now;
            }
        }

        public int GetNewAppUserOrganizationID()
        {
            int result = -1;
            using (var db = new MyFileItEntities())
            {
                var ev = db.APPUSERORGANIZATIONs.OrderByDescending(o => o.ID).FirstOrDefault();
                result = ev == null ? 1 : ev.ID + 1;
            }
            return result;
        }

        public string GenerateCabinetName()
        {
            var result = this.LASTNAME + "_" + this.FIRSTNAME;
            return result;
        }

        public string GetCabinetId()
        {
            //this is rigged!!! because of EF
            return this.EMAILADDRESS2;
        }

        public void SetCabinetId(string val)
        {
            this.EMAILADDRESS2 = val;
        }

        public static bool ValidUserName(string userName)
        {
            var result = true;
            using (var db = new MyFileItEntities())
            {
                result = !db.APPUSERs.Any(u => u.USERNAME.ToLower() == userName.ToLower());
            }

            return true;
        }

        public APPUSER PrimaryAppUser
        {
            get
            {
                APPUSER result = null;

                if (PRIMARYAPPUSERID != null)
                {
                    using (var db = new MyFileItEntities())
                    {
                        result = db.APPUSERs.Single(au => au.ID == PRIMARYAPPUSERID);
                    }
                }

                return result;
            }
        }

        public bool IsCoach
        {
            get
            {
                var result = false;
                using (var db = new MyFileItEntities())
                {
                    var coachAppUserTypeIds = db.APPUSERTYPEs.Where(au => au.ISCOACH == "1").Select(au => (int?)au.ID).ToList();
                    result = db.APPUSERORGANIZATIONs.Any(aug => aug.APPUSERID == ID && coachAppUserTypeIds.Contains(aug.APPUSERTYPEID));
                }
                return result;
            }
        }

        public string FullName
        {
            get
            {
                return FIRSTNAME + " " + LASTNAME;
            }
        }

        public APPUSERORGANIZATION CreateOrganizationAssociation(int organizationId, int appUserTypeId, DateTime startDate, DateTime expiresDate, int? yearCode, int sportTypeId)
        {
            var OrganizationAssociation = new APPUSERORGANIZATION()
            {
                ID = GetNewAppUserOrganizationID(),
                ORGANIZATIONID = organizationId,
                APPUSERID = ID,
                APPUSERTYPEID = appUserTypeId,
                STARTDATE = startDate,
                EXPIRESDATE = expiresDate,
                YEARCODE = yearCode,
                SPORTTYPEID = sportTypeId,
                DATECREATED = DateTime.Now
            };

            return OrganizationAssociation;
        }

        public bool UpdateObject(APPUSER updated)
        {
            var result = false;

            ID = updated.ID;
            USERNAME = updated.USERNAME;
            PASSWORD = updated.PASSWORD;
            PRIMARYAPPUSERID = updated.PRIMARYAPPUSERID;
            FIRSTNAME = updated.FIRSTNAME;
            LASTNAME = updated.LASTNAME;
            ADDRESS1 = updated.ADDRESS1;
            ADDRESS2 = updated.ADDRESS2;
            CITY = updated.CITY;
            STATECODE = updated.STATECODE;
            ZIPCODE = updated.ZIPCODE;
            PHONE = updated.PHONE;
            MOBILEPHONENUMBER = updated.MOBILEPHONENUMBER;
            EMAILADDRESS = updated.EMAILADDRESS;
            EMAILADDRESS2 = updated.EMAILADDRESS2;
            SEX = updated.SEX;
            BIRTHDATE = updated.BIRTHDATE;
            COMMENT = updated.COMMENT;
            RELATIONSHIPTYPEID = updated.RELATIONSHIPTYPEID;
            APPUSERTYPEID = updated.APPUSERTYPEID;
            SHAREKEYID = updated.SHAREKEYID;
            APPUSERSTATUSID = updated.APPUSERSTATUSID;

            result = true;
            return result;
        }
    }
}
