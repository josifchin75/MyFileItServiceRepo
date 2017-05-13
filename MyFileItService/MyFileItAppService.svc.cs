using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using MyFileItDataLayer.Models;
using MyFileItService.DTOs;
using System.Data.Entity.Validation;
using System.Data.Entity;
using MyFileItService.Helpers;

namespace MyFileItService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "MyFileItService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select MyFileItService.svc or MyFileItService.svc.cs at the Solution Explorer and start debugging.
    public class MyFileItAppService : IMyFileItAppService
    {
        bool DebugMode = false;

        public bool InitService()
        {
            //call some methods to force the service to come alive
            ExceptionHelper.LogError("Initialized Service.");
            AllowAccess("testuser", "testpass");
            using (var db = new MyFileItEntities())
            {
                var user = db.APPUSERs.FirstOrDefault();
            }
            //ExceptionHelper.LogError("Checking For reminders to run/");
            CheckIfRemindersShouldRun();

            //call the login 
            // ExceptionHelper.LogError("About to call login service.");
            // LoginAppUser("testuser", "testpass", "appusername", "appuserpass");
            // ExceptionHelper.LogError("Called login service.");
            return true;
        }

        private void CheckIfRemindersShouldRun()
        {
            if (DateTime.Now.Hour == ConfigurationSettings.ReminderCheckHour24)
            {
                var lastReminderCheck = ConfigurationSettings.LastReminderCheck;
                if (lastReminderCheck == null || ((DateTime)lastReminderCheck).Date < DateTime.Now.Date)
                {
                    var result = SendMissingDocumentShares();
                    ConfigurationSettings.LastReminderCheck = DateTime.Now;
                }
            }
        }

        public MyFileItResult GetReferenceData(string user, string pass, string referenceTableName)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    result.KeyValueData = db.GetReferenceData(referenceTableName);
                    result.Success = result.KeyValueData.Any();
                }
            }
            return result;
        }

        /************************************************************
         * FILEDOCUMENT UPLOAD STUFF 
         * ********************************************************/
        public MyFileItResult UploadFileCabinetDocument(string user, string pass, int appUserId, string filename, string base64Image, FileCabinetDocumentDTO doc)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                try
                {
                    using (var db = new MyFileItEntities())
                    {
                        var appUser = db.APPUSERs.Single(o => o.ID == appUserId);
                        FileItMainService.FileItDocument[] documents = new FileItMainService.FileItDocument[1];
                        documents[0] = new FileItMainService.FileItDocument()
                        {
                            CabinetID = appUser.GetCabinetId(),
                            FileName = filename,
                            ImageBase64 = base64Image,
                            IndexInformation = doc.FileItIndexInformation()
                        };

                        using (var svc = new FileItMainService.FileItServiceClient())
                        {
                            var response = svc.UploadDocuments(ConfigurationSettings.ServiceUser, ConfigurationSettings.ServicePass, appUser.GetCabinetId(), documents);
                            result.Success = response.Success;
                            result.Message = response.Message;

                            if (result.Success)
                            {
                                var documentEF = new FILECABINETDOCUMENT()
                                {
                                    CABINETID = appUser.GetCabinetId(),
                                    DOCUMENTID = response.FileNameFileItID.Single(f => f.Key == filename).Value,
                                    APPUSERID = doc.APPUSERID,
                                    SCANDATE = doc.SCANDATE,
                                    FIRSTNAME = doc.FIRSTNAME,
                                    LASTNAME = doc.LASTNAME,
                                    DOCUMENTTYPEID = doc.DOCUMENTTYPEID,
                                    COMMENT = doc.COMMENT,
                                    DOCUMENTDATE = doc.DOCUMENTDATE,
                                    //VERIFIEDDATE = doc.VERIFIEDDATE,
                                    //VERIFIEDAPPUSERID = doc.VERIFIEDAPPUSERID,
                                    DOCUMENTLOCATION = doc.DOCUMENTLOCATION,
                                    DOCUMENTSTATUSID = doc.DOCUMENTSTATUSID == 0 ? db.DOCUMENTSTATUS.First().ID : doc.DOCUMENTSTATUSID
                                };

                                documentEF.SetNewID();
                                documentEF.DATECREATED = DateTime.Now;

                                db.FILECABINETDOCUMENTs.Add(documentEF);
                                result.Success = SaveDBChanges(db);
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHelper.LogError(ex);
                    result.Message = ex.ToString();
                }
            }
            return result;
        }

        public MyFileItResult VerifyDocument(string user, string pass, int documentId, int verifyAppUserId, int teamEventDocumentId)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    var share = db.SHAREDOCUMENTs.Single(sd => sd.FILECABINETDOCUMENTID == documentId && sd.TEAMEVENTDOCUMENTID == teamEventDocumentId);
                    share.VERIFIEDAPPUSERID = verifyAppUserId;
                    share.VERIFIEDDATE = DateTime.Now;

                    //var doc = db.FILECABINETDOCUMENTs.Single(d => d.ID == documentId);
                    //doc.VERIFIEDAPPUSERID = verifyAppUserId;
                    //doc.VERIFIEDDATE = DateTime.Now;
                    result.Success = SaveDBChanges(db);
                }
            }
            return result;
        }

        public MyFileItResult AddEmergencyShare(string user, string pass, int appUserId, int[] fileCabinetDocumentIds, string emergencyEmailAddress, string emailMessage)
        {
            var result = new MyFileItResult();
            var success = true;
            var message = "";
            string error = "";

            if (AllowAccess(user, pass))
            {

                using (var db = new MyFileItEntities())
                {
                    //get all filecabinetdocuments
                    var fileCabinetDocuments = db.FILECABINETDOCUMENTs.Where(fcd => fileCabinetDocumentIds.Contains(fcd.ID)).ToList();
                    var appUser = db.APPUSERs.Single(au => au.ID == appUserId);
                    var shares = new List<SHAREDOCUMENT>();
                    //loop and add association
                    fileCabinetDocuments.ForEach(fcd =>
                    {
                        var newShare = new SHAREDOCUMENT(appUserId, fcd.ID, null, emailMessage, true, emergencyEmailAddress, ref error);

                        result.Message += error;
                        //error checking happens in the ef object    
                        if (result.Message == null || result.Message.Length == 0)
                        {
                            db.SHAREDOCUMENTs.Add(newShare);
                            shares.Add(newShare);
                            if (!SaveDBChanges(db))
                            {
                                success = false;
                            }
                        }
                    });

                    if (success)
                    {
                        //send email
                        success = EmailHelper.SendEmergencyShareEmail(appUser, emergencyEmailAddress, emailMessage, fileCabinetDocuments, shares);
                    }

                    result.Success = success;
                }

            }

            result.Success = success;
            result.Message = message;
            return result;
        }

        public MyFileItResult GetEmergencyShare(string user, string pass, string guid)
        {
            var result = new MyFileItResult();

            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    var shareDocument = db.SHAREDOCUMENTs.SingleOrDefault(sd => sd.VIEWDOCUMENTKEY.Equals(guid, StringComparison.CurrentCultureIgnoreCase));
                    if (shareDocument != null)
                    {
                        if (shareDocument.ISACTIVE == true.ConvertToFirebirdString() && shareDocument.DATECREATED > DateTime.Now.AddHours(-4))
                        {
                            shareDocument.ISACTIVE = false.ConvertToFirebirdString();
                            shareDocument.VIEWED = true.ConvertToFirebirdString();
                            shareDocument.VIEWDATE = DateTime.Now;

                            var fd = db.FILECABINETDOCUMENTs.Single(fcb => fcb.ID == shareDocument.FILECABINETDOCUMENTID);

                            var idLookups = new List<FileItMainService.FileItDocumentIdLookup>();

                            idLookups.Add(new FileItMainService.FileItDocumentIdLookup()
                            {
                                CabinetId = fd.CABINETID,
                                DocumentId = fd.DOCUMENTID
                            });

                            var r = new FileItMainService.FileItResponse();
                            using (var svc = new FileItMainService.FileItServiceClient())
                            {
                                r = svc.GetDocumentsById(ConfigurationSettings.ServiceUser, ConfigurationSettings.ServicePass, idLookups.ToArray(), true, false);
                            }

                            r.Documents.ToList().ForEach(d =>
                           {
                               var fileitDoc = new FileCabinetDocumentDTO(fd);
                               fileitDoc.Base64Image = d.ImageBase64;
                               fileitDoc.Base64ImageThumb = d.WebImageThumbBase64;

                               result.Documents.Add(fileitDoc);
                           });
                            // result.Success = true;
                            result.Success = SaveDBChanges(db);
                        }
                        else
                        {
                            result.Message = "This document is no longer active.";
                        }
                    }
                    else
                    {
                        result.Message = "Emergency Share is not found";
                    }
                }
            }
            return result;
        }

        public MyFileItResult AssociateDocumentsToTeamEventDocuments(string user, string pass, List<AssociateDocumentDTO> associations)
        {
            var result = new MyFileItResult();
            var success = true;
            var message = "";

            if (AllowAccess(user, pass))
            {
                associations.ForEach(a =>
                {
                    var resp = AssociateDocumentToTeamEventDocument(user, pass, a.appUserId, a.organizationId, a.fileCabinetDocumentId, a.teamEventDocumentId, a.comment, a.emergency, a.remove);
                    if (!resp.Success)
                    {
                        success = false;
                    }
                    message += resp.Message;
                });
            }

            result.Success = success;
            result.Message = message;
            return result;
        }

        public MyFileItResult ResendAssociatedDocuments(string user, string pass, int appUserId, int? teamEventId)
        {
            var result = new MyFileItResult();
            var success = true;
            var message = "";

            if (AllowAccess(user, pass))
            {
                //send an email
                var emails = new List<string>();
                searchCoachesByTeamEventId(int.Parse(teamEventId.ToString())).ForEach(c =>
                {
                    emails.Add(c.EMAILADDRESS);
                });

                var emailString = "";
                emails.ForEach(e =>
                {
                    emailString += e;
                });

                if (emails.Any())
                {
                    using (var db = new MyFileItEntities())
                    {
                        //get the shares
                        List<int> ids = db.TEAMEVENTDOCUMENTs.Where(ted => ted.TEAMEVENTID == teamEventId).Select(ted => ted.ID).ToList();

                        List<int?> teamEventDocumentIds = new List<int?>();
                        for (int i = 0; i < ids.Count(); i++)
                        {
                            teamEventDocumentIds.Add((int?)ids[i]);
                        }

                        db.SHAREDOCUMENTs
                            .Where(sd => sd.APPUSERID == appUserId && teamEventDocumentIds.Contains(sd.TEAMEVENTDOCUMENTID))
                            .ToList().ForEach(newShare =>
                            {
                                newShare.APPUSER = db.APPUSERs.Single(au => au.ID == newShare.APPUSERID);
                                newShare.TEAMEVENTDOCUMENT = db.TEAMEVENTDOCUMENTs.Include(ted => ted.TEAMEVENT).Single(ted => ted.ID == newShare.TEAMEVENTDOCUMENTID);

                                EmailHelper.SendDocumentAssociatedEmail(emails, newShare);
                            });
                    }
                }
            }

            result.Success = success;
            result.Message = message;
            return result;
        }

        public MyFileItResult AssociateDocumentToTeamEventDocument(string user, string pass, int appUserId, int organizationId, int fileCabinetDocumentId, int teamEventDocumentId, string comment, bool emergency, bool remove)
        {
            //TODO: this should be altered to not include the organization id, it can be inferred from event document - jo 10Jan2016
            var result = new MyFileItResult();
            string error = "";
            string emergencyEmailAddress = "";
            int eventId = -1;

            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    var share = db.SHAREDOCUMENTs.SingleOrDefault(s => s.TEAMEVENTDOCUMENTID == teamEventDocumentId && s.FILECABINETDOCUMENTID == fileCabinetDocumentId);

                    if (!remove)
                    {
                        if (share == null)
                        {
                            if (organizationId == -1 || organizationId == null)
                            {
                                eventId = db.TEAMEVENTDOCUMENTs.Single(d => d.ID == teamEventDocumentId).TEAMEVENTID;
                                organizationId = db.TEAMEVENTs.Single(te => te.ID == eventId).ORGANIZATIONID;
                            }
                            var newShare = new SHAREDOCUMENT(appUserId, fileCabinetDocumentId, teamEventDocumentId, comment, emergency, emergencyEmailAddress, ref error);

                            result.Message = error;
                            //error checking happens in the ef object    
                            if (result.Message == null || result.Message.Length == 0)
                            {
                                db.SHAREDOCUMENTs.Add(newShare);
                                result.Success = SaveDBChanges(db);
                            }
                            if (result.Success)
                            {
                                //send an email
                                var emails = new List<string>();
                                searchCoachesByTeamEventId(eventId).ForEach(c =>
                                {
                                    emails.Add(c.EMAILADDRESS);
                                });

                                //emails.Add("josifchin75@gmail.com");

                                var emailString = "";
                                emails.ForEach(e =>
                                {
                                    emailString += e;
                                });
                                //ExceptionHelper.LogError("About to send associate email to " + emailString);
                                if (emails.Any())
                                {
                                    newShare.APPUSER = db.APPUSERs.Single(au => au.ID == newShare.APPUSERID);
                                    newShare.TEAMEVENTDOCUMENT = db.TEAMEVENTDOCUMENTs.Include(ted => ted.TEAMEVENT).Single(ted => ted.ID == newShare.TEAMEVENTDOCUMENTID);

                                    EmailHelper.SendDocumentAssociatedEmail(emails, newShare);
                                    //ExceptionHelper.LogError("Just sent associate email");
                                }
                            }
                        }
                        else
                        {
                            result.Message = "Share already exists";
                        }
                    }
                    else
                    {
                        //remove the share
                        db.SHAREDOCUMENTs.Remove(share);
                        result.Success = SaveDBChanges(db);
                    }
                }
            }
            return result;
        }

        public MyFileItResult RejectTeamEventDocumentShare(string user, string pass, int appUserId, int teamEventDocumentId)
        {
            var result = new MyFileItResult();
            var success = true;
            var message = "";

            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    var share = db.SHAREDOCUMENTs.SingleOrDefault(s => s.APPUSERID == appUserId && s.TEAMEVENTDOCUMENTID == teamEventDocumentId);
                    if (share != null)
                    {
                        var appUser = db.APPUSERs.Single(u => u.ID == appUserId);
                        var teamEventDocument = db.TEAMEVENTDOCUMENTs.Include(ted => ted.TEAMEVENT).Single(ted => ted.ID == teamEventDocumentId);
                        db.SHAREDOCUMENTs.Remove(share);
                        result.Success = SaveDBChanges(db);
                        if (success)
                        {
                            //send reset email
                            EmailHelper.SendRejectDocumentEmail(appUser, teamEventDocument);
                        }
                    }
                    else
                    {
                        message = "Invalid share id";
                    }
                }
            }

            result.Success = success;
            result.Message = message;
            return result;
        }

        /************************************************************
         * ORGANIZATIONS 
         * ********************************************************/

        public MyFileItResult GetAppUserOrganizations(string user, string pass, int appUserId)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    var organizations = db.APPUSERORGANIZATIONs.Where(ao => ao.APPUSERID == appUserId).Select(ao => ao.ORGANIZATIONID).ToList();
                    db.ORGANIZATIONs.Where(o => organizations.Contains(o.ID)).ToList().ForEach(o =>
                    {
                        result.Organizations.Add(new OrganizationDTO(o, null));
                        result.Success = true;
                    });
                }
            }
            return result;
        }

        public MyFileItResult GetOrganizations(string user, string pass, int? organizationId, string nameLookup)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    if (organizationId != null)
                    {
                        db.ORGANIZATIONs.Where(o => o.ID == organizationId).ToList().ForEach(o =>
                        {
                            result.Organizations.Add(new OrganizationDTO(o, null));
                            result.Success = true;
                        });
                    }
                    else
                    {
                        db.ORGANIZATIONs.Where(o => o.NAME.ToLower().IndexOf(nameLookup.ToLower()) > -1 || nameLookup == null).ToList().ForEach(o =>
                        {
                            result.Organizations.Add(new OrganizationDTO(o, null));
                            result.Success = true;
                        });
                    }
                }
            }
            return result;
        }

        public MyFileItResult AddOrganization(string user, string pass, OrganizationDTO organization)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {

                    var organizationEF = (ORGANIZATION)organization;
                    organizationEF.SetNewID();
                    organizationEF.DATECREATED = DateTime.Now;
                    organizationEF.FormatData();

                    db.ORGANIZATIONs.Add(organizationEF);

                    result.Success = SaveDBChanges(db);
                    if (result.Success)
                    {
                        //actually add the cabinet to the system and update the record.
                        using (var svc = new FileItMainService.FileItServiceClient())
                        {
                            var response = svc.CreateCabinet(ConfigurationSettings.ServiceUser, ConfigurationSettings.ServicePass, DefaultMyFileItTemplate(organization.NAME), organization.NAME);
                            organizationEF.CABINETID = response.Cabinet.CabinetId;
                            db.Entry(organizationEF).State = EntityState.Modified;
                            result.Success = SaveDBChanges(db);
                        }
                        //result.Success = organization.AddCabinet();
                    }
                }
            }
            return result;
        }

        public MyFileItResult UpdateOrganization(string user, string pass, OrganizationDTO organization)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    //make sure the cabinetid is NOT lost
                    organization.CABINETID = db.ORGANIZATIONs.Single(or => or.ID == organization.ID).CABINETID;
                }

                using (var db = new MyFileItEntities())
                {
                    var o = (ORGANIZATION)organization;
                    o.FormatData();

                    //o.UpdateObject(organization);
                    db.Entry(o).State = EntityState.Modified;
                    result.Success = SaveDBChanges(db);
                }

            }
            return result;

            /*
             {
"user": "admin",
"pass": "admin",
"add": 1,
"organization": {
"NAME": "Test ADD",
"ADDRESS1":"address 12",
"CITY": "Hatbor2o",
"STATECODE": "PA",
"ZIPCODE": "19040"
}
}
             */
        }

        public MyFileItResult RemoveOrganization(string user, string pass, int organizationId)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    var org = db.ORGANIZATIONs.SingleOrDefault(o => o.ID == organizationId);
                    //should this delete the associated ref's?
                    if (org != null && !org.APPUSERORGANIZATIONs.Any())
                    {
                        db.ORGANIZATIONs.Remove(org);
                    }
                    result.Success = SaveDBChanges(db);
                }
            }
            return result;
        }

        /************************************************************
         * TEAMEVENTS 
         * ********************************************************/
        public MyFileItResult GetTeamEvents(string user, string pass, int? organizationId, int? teamEventId, string name)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    if (teamEventId != null)
                    {
                        db.TEAMEVENTs.Where(e => e.ID == teamEventId && e.ISDELETED != "Y").ToList().ForEach(e =>
                        {
                            result.TeamEvents.Add(new TeamEventDTO(e));
                            result.Success = true;
                        });
                    }
                    else
                    {
                        //this is slow, but it works
                        db.TEAMEVENTs.Where(e => (e.ORGANIZATIONID == organizationId || organizationId == null) && e.ISDELETED != "Y").ToList().ForEach(e =>
                        {
                            if (name == null || name.Length == 0 || e.NAME.ToLower().IndexOf(name.ToLower()) > -1)
                            {
                                result.TeamEvents.Add(new TeamEventDTO(e));
                                result.Success = true;
                            }
                        });
                    }
                }
            }
            return result;
        }

        public MyFileItResult GetTeamEventsByAppUser(string user, string pass, int appUserId, int? organizationId, int? teamEventId, string name)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    var organizationIds = db.APPUSERORGANIZATIONs.Where(ao => ao.APPUSERID == appUserId).Select(ao => ao.ORGANIZATIONID).ToList();
                    if (organizationId != null)
                    {
                        organizationIds = new List<int>() { int.Parse(organizationId.ToString()) };
                    }

                    if (teamEventId != null)
                    {
                        db.TEAMEVENTs.Where(e => e.ID == teamEventId && e.ISDELETED != "Y").ToList().ForEach(e =>
                        {
                            result.TeamEvents.Add(new TeamEventDTO(e));
                            result.Success = true;
                        });
                    }
                    else
                    {
                        //this is slow, but it works
                        db.TEAMEVENTs.Where(e => organizationIds.Contains(e.ORGANIZATIONID) && e.ISDELETED != "Y").ToList().ForEach(e =>
                        {
                            if (name == null || name.Length == 0 || e.NAME.ToLower().IndexOf(name.ToLower()) > -1)
                            {
                                result.TeamEvents.Add(new TeamEventDTO(e));
                                result.Success = true;
                            }
                        });
                    }
                }
            }
            return result;
        }

        public MyFileItResult GetTeamEventsByCoach(string user, string pass, int appUserId, string name)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities()) 
                {
                    var eventIds = db.COACHes.Where(c => c.APPUSERID == appUserId).Select(c => c.TEAMEVENTID).ToList();
                    db.TEAMEVENTs.Where(e => eventIds.Contains(e.ID) && e.ISDELETED != "Y" && e.ISDELETED != "1").ToList().ForEach(e =>
                    {
                        result.TeamEvents.Add(new TeamEventDTO(e));
                        result.Success = true;
                    });
                }
            }
            return result;
        }

        public MyFileItResult ValidateTeamEvent(string user, string pass, string teamEventName)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    if (db.TEAMEVENTs.Any(te => te.NAME.Equals(teamEventName, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        result.Message = "Team event already exists.";
                    }
                    else
                    {
                        result.Success = true;
                    }
                }
            }
            return result;
        }

        public MyFileItResult AddTeamEvent(string user, string pass, TeamEventDTO teamEvent)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass) && teamEvent != null)
            {
                using (var db = new MyFileItEntities())
                {
                    result = ValidateTeamEvent(user, pass, teamEvent.NAME);
                    if (result.Success)
                    {
                        var teamEventEF = (TEAMEVENT)teamEvent;
                        teamEventEF.DATECREATED = DateTime.Now;
                        teamEventEF.SetNewID();
                        teamEventEF.FormatData();
                        db.TEAMEVENTs.Add(teamEventEF);
                        result.Success = SaveDBChanges(db);

                        teamEventEF = db.TEAMEVENTs.Single(te => te.ID == teamEventEF.ID);
                        //include the TeamEvent in the result
                        result.TeamEvents.Add(new TeamEventDTO(teamEventEF));
                    }
                }
            }
            return result;
        }

        public MyFileItResult UpdateTeamEvent(string user, string pass, TeamEventDTO teamEvent)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    var teamEventEF = (TEAMEVENT)teamEvent;
                    db.Entry(teamEventEF).State = EntityState.Modified;
                    result.Success = SaveDBChanges(db);
                }
            }
            return result;
        }

        public MyFileItResult RemoveTeamEvent(string user, string pass, int teamEventId)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    var ev = db.TEAMEVENTs.SingleOrDefault(e => e.ID == teamEventId);
                    //should this delete the associated ref's?
                    if (ev != null && !ev.TEAMEVENTPLAYERROSTERs.Any())
                    {
                        db.TEAMEVENTs.Remove(ev);
                    }
                    result.Success = SaveDBChanges(db);
                }
            }
            return result;
        }

        /************************************************************
        * TEAMEVENTDOCUMENT
        * ********************************************************/
        public MyFileItResult GetAppUserTeamEventDocumentsByTeamEvent(string user, string pass, int appUserId, int teamEventId)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    db.TEAMEVENTDOCUMENTs.Where(e => e.TEAMEVENTID == teamEventId).ToList().ForEach(e =>
                    {
                        result.TeamEventDocuments.Add(new TeamEventDocumentDTO(e, appUserId));
                        result.Success = true;
                    });

                }
            }
            return result;
        }

        public MyFileItResult GetTeamEventDocuments(string user, string pass, int? teamEventDocumentId, string nameLookup)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    if (teamEventDocumentId != null)
                    {
                        db.TEAMEVENTDOCUMENTs.Where(e => e.ID == teamEventDocumentId).ToList().ForEach(e =>
                        {
                            result.TeamEventDocuments.Add(new TeamEventDocumentDTO(e, null));
                            result.Success = true;
                        });
                    }
                    else
                    {
                        db.TEAMEVENTDOCUMENTs.Where(e => e.DOCUMENTNAME.IndexOf(nameLookup) > -1).ToList().ForEach(e =>
                        {
                            result.TeamEventDocuments.Add(new TeamEventDocumentDTO(e, null));
                            result.Success = true;
                        });
                    }
                }
            }
            return result;
        }

        public MyFileItResult AddTeamEventDocument(string user, string pass, TeamEventDocumentDTO teamEventDocument)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass) && teamEventDocument != null)
            {
                using (var db = new MyFileItEntities())
                {
                    if (TEAMEVENTDOCUMENT.ValidTeamEventDocumentName(teamEventDocument.DOCUMENTNAME))
                    {
                        var teamEventDocumentEF = (TEAMEVENTDOCUMENT)teamEventDocument;
                        teamEventDocumentEF.SetNewID();
                        teamEventDocumentEF.FormatData();
                        db.TEAMEVENTDOCUMENTs.Add(teamEventDocumentEF);

                        result.TeamEventDocuments.Add(new TeamEventDocumentDTO(teamEventDocumentEF, null));
                        result.Success = SaveDBChanges(db);
                    }
                    else
                    {
                        result.Message = "Invalid Team Event Document Name";
                    }
                }
            }
            return result;
        }

        public MyFileItResult UpdateTeamEventDocument(string user, string pass, TeamEventDocumentDTO teamEventDocument)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    //var doc = db.TEAMEVENTDOCUMENTs.Single(d => d.ID == teamEventDocument.ID);
                    //doc.UpdateObject(teamEventDocument);
                    var doc = (TEAMEVENTDOCUMENT)teamEventDocument;
                    doc.FormatData();
                    db.Entry(doc).State = EntityState.Modified;
                    result.Success = SaveDBChanges(db);
                }
            }
            return result;
        }

        public MyFileItResult RemoveTeamEventDocument(string user, string pass, int teamEventDocumentId)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    var doc = db.TEAMEVENTDOCUMENTs.SingleOrDefault(u => u.ID == teamEventDocumentId);
                    //should this delete the associated ref's?
                    if (doc != null)
                    {
                        db.TEAMEVENTDOCUMENTs.Remove(doc);
                    }
                    result.Success = SaveDBChanges(db);
                }
            }
            return result;
        }


        /************************************************************
        * TEAMEVENTPLAYERROSTER
        * ********************************************************/
        public MyFileItResult GetTeamEventPlayerRosters(string user, string pass, int? teamEventPlayerRosterId, int? teamEventId)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    if (teamEventPlayerRosterId != null)
                    {
                        db.TEAMEVENTPLAYERROSTERs.Where(e => e.ID == teamEventPlayerRosterId).ToList().ForEach(e =>
                        {
                            result.TeamEventPlayerRosters.Add(new TeamEventPlayerRosterDTO(e));
                            result.Success = true;
                        });
                    }
                    else
                    {
                        if (teamEventId != null)
                        {
                            var teamEventIdLookup = int.Parse(teamEventId.ToString());
                            db.TEAMEVENTPLAYERROSTERs.Where(e => e.TEAMEVENTID == teamEventIdLookup).ToList().ForEach(e =>
                            //db.TEAMEVENTPLAYERROSTERs.Where(e => e.TEAMEVENTID == 0).ToList().ForEach(e =>
                            {
                                result.TeamEventPlayerRosters.Add(new TeamEventPlayerRosterDTO(e));
                                result.Success = true;
                            });
                        }
                    }
                    result.Success = true;
                }
            }
            return result;
        }

        public MyFileItResult GetTeamEventPlayersWithUploads(string user, string pass, int teamEventId)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    //get all sharedocument files with teameventdocumentid in teamevent, but not in teameventplayerroster table
                    var rosterIds = db.TEAMEVENTPLAYERROSTERs.Where(roster => roster.TEAMEVENTID == teamEventId).Select(roster => roster.APPUSERID).ToList();
                    var teamEventDocumentIds = db.TEAMEVENTDOCUMENTs.Where(ted => ted.TEAMEVENTID == teamEventId).Select(ted => ted.ID.ToString()).ToList();

                    var docs = db.SHAREDOCUMENTs
                        .Where(sd => sd.TEAMEVENTDOCUMENTID != null && teamEventDocumentIds.Contains(sd.TEAMEVENTDOCUMENTID.ToString()) && !rosterIds.Contains(sd.APPUSERID))
                        //.Include("APPUSER")
                        .Select(sd => sd.APPUSER)
                        .ToList();
                    docs.ForEach(d =>
                    {
                        if (!result.AppUsers.Any(u => u.ID == d.ID))
                        {
                            result.AppUsers.Add(new AppUserDTO(d));
                        }
                    });
                    result.Success = true;
                }
            }
            return result;
        }

        public MyFileItResult AddTeamEventPlayerRoster(string user, string pass, TeamEventPlayerRosterDTO teamEventPlayerRoster)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass) && teamEventPlayerRoster != null)
            {
                using (var db = new MyFileItEntities())
                {
                    var message = "";
                    if (TEAMEVENTPLAYERROSTER.ValidTeamEventPlayerRoster(teamEventPlayerRoster.TEAMEVENTID, int.Parse(teamEventPlayerRoster.APPUSERID.ToString()), ref message))
                    {
                        var teamEventPlayerRosterEF = (TEAMEVENTPLAYERROSTER)teamEventPlayerRoster;
                        teamEventPlayerRosterEF.SetNewID();
                        teamEventPlayerRosterEF.DATECREATED = DateTime.Now;
                        db.TEAMEVENTPLAYERROSTERs.Add(teamEventPlayerRosterEF);
                        result.Success = SaveDBChanges(db);
                    }
                    else
                    {
                        result.Message = message;
                    }
                }
            }
            return result;
        }

        public MyFileItResult UpdateTeamEventPlayerRoster(string user, string pass, TeamEventPlayerRosterDTO teamEventPlayerRoster)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    //var roster = db.TEAMEVENTPLAYERROSTERs.Single(d => d.ID == teamEventPlayerRoster.ID);
                    //roster.UpdateObject(teamEventPlayerRoster);
                    var roster = (TEAMEVENTPLAYERROSTER)teamEventPlayerRoster;
                    db.Entry(roster).State = EntityState.Modified;
                    result.Success = SaveDBChanges(db);
                }
            }
            return result;
        }

        public MyFileItResult RemoveTeamEventPlayerRoster(string user, string pass, int teamEventPlayerRosterId)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    var roster = db.TEAMEVENTPLAYERROSTERs.SingleOrDefault(u => u.ID == teamEventPlayerRosterId);
                    //should this delete the associated ref's?
                    if (roster != null)
                    {
                        db.TEAMEVENTPLAYERROSTERs.Remove(roster);
                    }
                    result.Success = SaveDBChanges(db);
                }
            }
            return result;
        }
        /************************************************************
         * APPUSERs 
         * ********************************************************/
        public bool CheckAppUserExists(string user, string pass, string appUserName)
        {
            var result = true;
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    result = db.APPUSERs.Any(au => au.USERNAME.Trim().ToLower() == appUserName.Trim().ToLower());
                }
            }
            return result;
        }

        public MyFileItResult LoginAppUser(string user, string pass, string appUserName, string appUserPass)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    var userObj = db.APPUSERs.FirstOrDefault(au => au.USERNAME.ToLower() == appUserName.ToLower() && au.PASSWORD.Equals(appUserPass) && au.PRIMARYAPPUSERID == null);

                    if (userObj != null)
                    {
                        result.Success = true;
                        result.AppUsers.Add(new AppUserDTO(userObj));
                    }
                }
            }
            return result;
        }

        public MyFileItResult ForgotPassword(string user, string pass, string emailAddress)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    var userObj = db.APPUSERs.FirstOrDefault(au => au.EMAILADDRESS.Equals(emailAddress, StringComparison.CurrentCultureIgnoreCase));
                    if (userObj != null)
                    {
                        if (EmailHelper.SendForgotPassword(userObj))
                        {
                            result.Success = true;
                            result.Message = "Email Has Been Sent.";
                        }
                        else
                        {
                            result.Message = "There was an issue with the email server, please try again later.";
                        }
                    }
                    else
                    {
                        result.Message = "Email address was not found in the system.";
                    }
                }
            }
            return result;
        }

        public string GetInvitationToShareEmailText(string user, string pass)
        {
            var result = "";
            if (AllowAccess(user, pass))
            {
                result = EmailHelper.GetInviteToShareEmailMessage();
            }
            return result;
        }

        public MyFileItResult SendInvitationEmail(string user, string pass, string emailAddress, string message)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                result.Success = EmailHelper.SendInvitationEmail(emailAddress, message);
            }
            return result;
        }

        public MyFileItResult RunMissingDocumentShareEmails(string user, string pass)
        {
            var result = new MyFileItResult();
            //result.Success = true;
            //var cnt = 0;
            if (AllowAccess(user, pass))
            {
                result = SendMissingDocumentShares();
                /*
                 select
                    o.name,
                    a.ID as appuserid,
                    a.emailaddress as email,
                    e.name as eventname,
                    td.documentname as docname ,
                    s.documentid
                from teameventplayerroster r
                left join teamevent e on e.id = r.TEAMEVENTID
                left join APPUSER a on a.id = r.APPUSERID
                left join teameventdocument td  on td.id = e.id
                left join sharedocument s on s.teameventdocumentid = td.id
                left join organization o on o.id = e.organizationid
                where s.documentid is null`
                 */
            }
            
            return result;
        }

        private MyFileItResult SendMissingDocumentShares()
        {
            var result = new MyFileItResult();
            int cnt = 0;
            result.Success = true;
            //get the emails to send
            using (var db = new MyFileItEntities())
            {
                var emails = db.TEAMEVENTPLAYERROSTERs.Where(
                         r => !r.APPUSER.SHAREDOCUMENTs.Any(sd => sd.TEAMEVENTDOCUMENT.TEAMEVENTID == r.TEAMEVENTID)
                    )
                    .ToList();
                emails.ForEach(e =>
                {
                    if (!EmailHelper.SendShareReminderEmail(e.APPUSER, e.TEAMEVENT.ORGANIZATION, e.TEAMEVENT))
                    {
                        result.Success = false;
                    }
                });
                cnt++;
            }
            result.Message = "Sent " + cnt.ToString() + " reminder emails.";


            return result;
        }

        public MyFileItResult GetSentEmails(string user, string pass, string toEmailAddress)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    db.EMAILLOGs.Where(e => e.TOEMAILADDRESS.ToLower().Trim() == toEmailAddress.Trim().ToLower()).ToList().ForEach(e =>
                    {
                        result.EmailLogs.Add(new EmailLogDTO(e));
                    });
                }
                result.Success = true;
            }
            return result;
        }

        public MyFileItResult GetAppUsers(string user, string pass, int? appUserId, string nameLookup)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    if (appUserId != null)
                    {
                        db.APPUSERs.Where(a => a.ID == appUserId).ToList().ForEach(a =>
                        {
                            result.AppUsers.Add(new AppUserDTO(a));
                            result.Success = true;
                        });
                    }
                    else
                    {
                        db.APPUSERs.Where(a => a.FIRSTNAME.IndexOf(nameLookup) > -1 || a.LASTNAME.IndexOf(nameLookup) > -1).ToList().ForEach(a =>
                        {
                            result.AppUsers.Add(new AppUserDTO(a));
                            result.Success = true;
                        });
                    }
                }
            }
            return result;
        }

        public MyFileItResult GetAllAppUsers(string user, string pass, int? organizationId)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    var appUserList = new List<int>();
                    if (organizationId != null)
                    {
                        db.APPUSERORGANIZATIONs.Where(auo => auo.ORGANIZATIONID == organizationId).ToList().ForEach(o =>
                        {
                            appUserList.Add(o.APPUSERID);
                        });
                    }

                    db.APPUSERs.Where(au => !appUserList.Any() || appUserList.Contains(au.ID)).ToList().ForEach(a =>
                    {
                        result.AppUsers.Add(new AppUserDTO(a));
                        result.Success = true;
                    });
                }
            }
            return result;
        }

        public MyFileItResult GetAppUsersByEmail(string user, string pass, string userName, string emailAddress, string userPassword)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {

                    db.APPUSERs.Where(a =>
                        ((a.USERNAME.Equals(userName, StringComparison.CurrentCultureIgnoreCase) && emailAddress == null)
                        ||
                        (a.EMAILADDRESS.Equals(emailAddress, StringComparison.CurrentCultureIgnoreCase) && userName == null))
                        && a.PASSWORD.Equals(userPassword)
                        ).ToList().ForEach(a =>
                    {
                        result.AppUsers.Add(new AppUserDTO(a));
                        result.Success = true;
                    });

                }
            }
            return result;
        }

        public MyFileItResult GetAppUsersByPhoneNumber(string user, string pass, string phoneNumber)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    db.APPUSERs.Where(a =>
                            a.PHONE.ToLower() == phoneNumber.ToLower()
                        ).ToList().ForEach(a =>
                        {
                            result.AppUsers.Add(new AppUserDTO(a));
                            result.Success = true;
                        });
                }
            }
            return result;
        }

        public MyFileItResult GetAppUsersByNameSexEmail(string user, string pass, int appUserId, int teamEventId, string firstName, string lastName, string parentEmailAddress, string sex)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                if (firstName.Length > 0 || lastName.Length > 0 || parentEmailAddress.Length > 0 || sex.Length > 0)
                {
                    using (var db = new MyFileItEntities())
                    {
                        //TODO: This may not scale!!!
                        var orgIds = GetAppUserOrganizations(user, pass, appUserId).Organizations.Select(o => o.ID).ToList();
                        var orgUserIds = db.APPUSERORGANIZATIONs.Where(aug => orgIds.Contains(aug.ORGANIZATIONID)).Select(aug => aug.APPUSERID).ToList();
                        var existingRosterAppUserIds = db.TEAMEVENTPLAYERROSTERs.Where(t => t.TEAMEVENTID == teamEventId).Select(t => t.APPUSERID).ToList();

                        var orgUsers = db.APPUSERs.Where(au => orgUserIds.Contains(au.ID) && !existingRosterAppUserIds.Contains(au.ID));

                        var primaryAppUserIds = db.APPUSERs.Where(au => au.EMAILADDRESS.Equals(parentEmailAddress, StringComparison.CurrentCultureIgnoreCase) && au.PRIMARYAPPUSERID == null).Select(au => au.ID).ToList();
                        orgUsers.Where(
                                u => u.FIRSTNAME.ToLower().IndexOf(firstName.ToLower()) > -1
                                || u.LASTNAME.ToLower().IndexOf(lastName.ToLower()) > -1
                                || u.SEX.ToLower().IndexOf(sex.ToLower()) > -1
                                || primaryAppUserIds.Contains(u.ID)
                                )
                            .ToList()
                            .ForEach(au =>
                            {
                                result.AppUsers.Add(new AppUserDTO(au));
                                result.Success = true;
                            });
                    }
                }
            }
            return result;
        }

        public MyFileItResult GetFamilyUsers(string user, string pass, int primaryAppUserId)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    //not sure if this should include the primary app user
                    db.APPUSERs.Include(a => a.SHAREKEYs).Where(a => a.ID == primaryAppUserId || a.PRIMARYAPPUSERID == primaryAppUserId).ToList().ForEach(a =>
                        {
                            result.AppUsers.Add(new AppUserDTO(a));
                            result.Success = true;
                        });
                }
            }
            return result;
        }

        public MyFileItResult GetCoachMembers(string user, string pass, int appUserId, int? organizationId, int? teamEventId, string nameLookup, string parentEmailAddress)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                nameLookup = nameLookup == null ? "" : nameLookup;

                using (var db = new MyFileItEntities())
                {
                    //TODO: This may not scale!!!
                    var orgIds = organizationId == null ? GetAppUserOrganizations(user, pass, appUserId).Organizations.Select(o => o.ID).ToList() : new List<int>() { int.Parse(organizationId.ToString()) };
                    var orgUserIds = db.APPUSERORGANIZATIONs.Where(aug => orgIds.Contains(aug.ORGANIZATIONID)).Select(aug => aug.APPUSERID).ToList();
                    var teamEventUserIds = teamEventId == null ? new List<int?>() : db.TEAMEVENTPLAYERROSTERs.Where(r => r.TEAMEVENTID == teamEventId).Select(r => r.APPUSERID).ToList();
                    var orgUsers = db.APPUSERs.Where(au => orgUserIds.Contains(au.ID) && (teamEventUserIds.Contains(au.ID) || teamEventId == null)).ToList();
                    var primaryAppUserIds = db.APPUSERs.Where(au => au.EMAILADDRESS.Equals(parentEmailAddress, StringComparison.CurrentCultureIgnoreCase) && au.PRIMARYAPPUSERID == null).Select(au => au.ID).ToList();

                    orgUsers.Where(
                            u => (u.FIRSTNAME.ToLower().IndexOf(nameLookup.ToLower()) > -1 || nameLookup.Length == 0)
                            || (u.LASTNAME.ToLower().IndexOf(nameLookup.ToLower()) > -1 || nameLookup.Length == 0)
                            || (primaryAppUserIds.Contains(u.ID))
                            )
                        .ToList()
                        .ForEach(au =>
                        {
                            result.AppUsers.Add(new AppUserDTO(au));
                            result.Success = true;
                        });
                }
            }
            return result;
        }

        public FileCabinetDocumentDTO GetSingleDocument(string user, string pass, FileCabinetDocumentSingleDTO lookup)
        {
            FileCabinetDocumentDTO result = null;

            var idLookups = new List<FileItMainService.FileItDocumentIdLookup>();
            using (var db = new MyFileItEntities())
            {
                var filedoc = db.FILECABINETDOCUMENTs.Single(fd => fd.ID == lookup.FILECABINETDOCUMENTID);
                //var teamEventDoc = db.TEAMEVENTDOCUMENTs.Single(te => te.ID == lookup.TEAMEVENTDOCUMENTID);
                idLookups.Add(new FileItMainService.FileItDocumentIdLookup()
                {
                    CabinetId = filedoc.CABINETID,
                    DocumentId = filedoc.DOCUMENTID
                });

                var r = new FileItMainService.FileItResponse();
                using (var svc = new FileItMainService.FileItServiceClient())
                {
                    r = svc.GetDocumentsById(ConfigurationSettings.ServiceUser, ConfigurationSettings.ServicePass, idLookups.ToArray(), true, false);
                }
                var returnedDocument = r.Documents.First();

                result = new FileCabinetDocumentDTO(filedoc);
                result.Base64Image = returnedDocument.ImageBase64;
                result.Base64ImageThumb = returnedDocument.WebImageThumbBase64;
                result.TeamEventDocumentId = lookup.TEAMEVENTDOCUMENTID;
                result.DocumentTypeName = lookup.TEAMEVENTDOCUMENTID != null ? db.TEAMEVENTDOCUMENTs.Single(ted => ted.ID == lookup.TEAMEVENTDOCUMENTID).DOCUMENTNAME : null;
                result.VerifiedAppUserName = GetVerifiedAppUserName(db, result.VerifiedAppUserId);
            }
            return result;
        }

        public MyFileItResult GetAppUserDocumentsList(string user, string pass, int appUserId, int? teamEventId, List<int?> downloadedDocumentIds)
        {
            MyFileItResult result = null;

            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    var userIds = new List<int?>();
                    userIds.Add((int?)appUserId);
                    Debug("Got Users: " + userIds.Count);

                    result = FindDocumentsByIdList(userIds, teamEventId, downloadedDocumentIds, false);
                    result.Documents.ForEach(d =>
                    {
                        d.Base64Image = null;
                        d.Base64ImageThumb = null;
                    });
                    //this needs to get fixed, not sure where to check for success
                    result.Success = true;
                }
            }
            return result;
        }

        public MyFileItResult GetAppUserDocumentsThumbs(string user, string pass, List<int?> lookupDocumentIds)
        {
            MyFileItResult result = null;

            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    result = GetDocumentThumbsByIdList(lookupDocumentIds);

                    //this needs to get fixed, not sure where to check for success
                    result.Success = true;
                }
            }
            return result;
        }

        private MyFileItResult GetDocumentThumbsByIdList(List<int?> lookupDocumentIds)
        {
            var result = new MyFileItResult();
            var idLookups = new List<FileItMainService.FileItDocumentIdLookup>();

            using (var db = new MyFileItEntities())
            {
                // do them one at a time for now ...
                db.FILECABINETDOCUMENTs.Where(fd => lookupDocumentIds.Contains(fd.ID)).ToList().ForEach(fd =>
                {
                    idLookups = new List<FileItMainService.FileItDocumentIdLookup>();
                    idLookups.Add(new FileItMainService.FileItDocumentIdLookup()
                    {
                        CabinetId = fd.CABINETID,
                        DocumentId = fd.DOCUMENTID
                    });

                    var r = new FileItMainService.FileItResponse();
                    using (var svc = new FileItMainService.FileItServiceClient())
                    {
                        r = svc.GetDocumentsById(ConfigurationSettings.ServiceUser, ConfigurationSettings.ServicePass, idLookups.ToArray(), true, false);
                    }

                    r.Documents.ToList().ForEach(d =>
                        {
                            var fileitDoc = new FileCabinetDocumentDTO(fd);
                            fileitDoc.Base64Image = d.ImageBase64;
                            fileitDoc.Base64ImageThumb = d.WebImageThumbBase64;
                            result.Documents.Add(fileitDoc);
                        });
                });
            }
            return result;
        }

        public MyFileItResult GetAppUserDocumentsListNoImages(string user, string pass, int appUserId, int? teamEventId, List<int?> downloadedDocumentIds)
        {
            MyFileItResult result = null;

            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    var userIds = new List<int?>();
                    userIds.Add((int?)appUserId);
                    Debug("Got Users: " + userIds.Count);

                    result = FindDocumentsByIdListNoImages(userIds, teamEventId, downloadedDocumentIds);
                    result.Documents.ForEach(d =>
                    {
                        d.Base64Image = null;
                        d.Base64ImageThumb = null;
                    });
                    //this needs to get fixed, not sure where to check for success
                    result.Success = true;
                }
            }
            return result;
        }



        public MyFileItResult GetAppUserDocuments(string user, string pass, int appUserId, int? teamEventId, List<int?> downloadedDocumentIds, bool? thumbsOnly)
        {
            var result = new MyFileItResult();
            thumbsOnly = thumbsOnly ?? false;

            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {

                    var userIds = new List<int?>();
                    userIds.Add((int?)appUserId);
                    Debug("Got Users: " + userIds.Count);

                    result = FindDocumentsByIdList(userIds, teamEventId, downloadedDocumentIds, (bool)thumbsOnly);

                }
                result.Success = true;
            }
            return result;
        }

        public MyFileItResult GetFamilyDocuments(string user, string pass, int primaryAppUserId)
        {
            Debug("Getting Family Docs" + primaryAppUserId);

            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    var userIds = new List<int?>();
                    db.APPUSERs.Where(a => a.ID == primaryAppUserId || a.PRIMARYAPPUSERID == primaryAppUserId).ToList().ForEach(a =>
                    {
                        userIds.Add(a.ID);
                    });
                    Debug("Got Users: " + userIds.Count);

                    result = FindDocumentsByIdList(userIds, null, new List<int?>());
                }
            }
            return result;
        }

        public MyFileItResult DeleteAppUserDocument(string user, string pass, int appUserId, string documentId)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    var appUser = db.APPUSERs.Single(u => u.ID == appUserId);
                    var cabinetId = appUser.GetCabinetId();
                    var r = new FileItMainService.FileItResponse();
                    using (var svc = new FileItMainService.FileItServiceClient())
                    {
                        r = svc.DeleteDocument(ConfigurationSettings.ServiceUser, ConfigurationSettings.ServicePass, cabinetId, documentId);
                    }
                    result.Success = r.Success;
                }
            }
            return result;
        }

        public class ShareDocumentTeamEvent
        {
            public int? FileCabinetDocumentId { get; set; }
            public int? TeamEventDocumentId { get; set; }
            public int? VerifiedAppUserId { get; set; }
        }

        //private List<FileCabinetDocumentSingleDTO> GetDocumentLookupList(List<int?> userIds, int? teamEventId)
        //{
        //    var result = new List<FileCabinetDocumentSingleDTO>();
        //    var teamEventDocumentIds = new List<int?>();
        //    var shareFileCabinetDocumentIds = new List<int>();
        //    var shareDocumentVerifieds = new Dictionary<int, int?>();
        //    var shareDocumentTeamEventIds = new List<ShareDocumentTeamEvent>();
        //    var teamEventDocumentList = new Dictionary<int, int>();

        //    using (var db = new MyFileItEntities())
        //    {
        //        if (teamEventId != null)
        //        {
        //            teamEventDocumentIds = db.TEAMEVENTDOCUMENTs.Where(ted => ted.TEAMEVENTID == teamEventId).Select(ted => (int?)ted.ID).ToList();
        //            //shareFileCabinetDocumentIds = 
        //            db.SHAREDOCUMENTs
        //            .Where(sd => userIds.Contains(sd.APPUSERID) && teamEventDocumentIds.Contains(sd.TEAMEVENTDOCUMENTID))
        //                //.Select(sd => sd.FILECABINETDOCUMENTID)
        //            .ToList()
        //            .ForEach(sfd =>
        //            {
        //                if (!shareFileCabinetDocumentIds.Contains(int.Parse(sfd.FILECABINETDOCUMENTID.ToString())))
        //                {
        //                    shareFileCabinetDocumentIds.Add(int.Parse(sfd.FILECABINETDOCUMENTID.ToString()));
        //                }

        //                if (!shareDocumentVerifieds.ContainsKey((int)sfd.FILECABINETDOCUMENTID))
        //                {
        //                    shareDocumentVerifieds.Add((int)sfd.FILECABINETDOCUMENTID, sfd.VERIFIEDAPPUSERID);
        //                }

        //                shareDocumentTeamEventIds.Add(new ShareDocumentTeamEvent()
        //                {
        //                    FileCabinetDocumentId = sfd.FILECABINETDOCUMENTID,
        //                    TeamEventDocumentId = sfd.TEAMEVENTDOCUMENTID,
        //                    VerifiedAppUserId = sfd.VERIFIEDAPPUSERID
        //                });
        //                /* if (!shareDocumentTeamEventIds.ContainsKey((int)sfd.FILECABINETDOCUMENTID))
        //                 {
        //                     shareDocumentTeamEventIds.Add((int)sfd.FILECABINETDOCUMENTID, sfd.TEAMEVENTDOCUMENTID);
        //                 }*/
        //                teamEventDocumentList.Add((int)sfd.TEAMEVENTDOCUMENTID, (int)sfd.FILECABINETDOCUMENTID);
        //            });
        //        }

        //        db.FILECABINETDOCUMENTs.Where(fd => userIds.Contains(fd.APPUSERID)
        //                && (teamEventId == null || shareFileCabinetDocumentIds.Contains(fd.ID))
        //                )
        //            .ToList()
        //            .ForEach(fd =>
        //        {
        //            result.Add(new FileCabinetDocumentSingleDTO()
        //            {
        //                 FILECABINETDOCUMENTID = 
        //                CABINETID = fd.CABINETID,
        //                DOCUMENTID = fd.DOCUMENTID
        //            });
        //        });
        //    }

        //    return result;
        //}

        private MyFileItResult FindDocumentsByIdListNoImages(List<int?> userIds, int? teamEventId, List<int?> skipDocumentList)
        {
            var result = new MyFileItResult();
            var teamEventDocumentIds = new List<int?>();
            var shareFileCabinetDocumentIds = new List<int>();
            var shareDocumentVerifieds = new Dictionary<int, int?>();
            var shareDocumentTeamEventIds = new List<ShareDocumentTeamEvent>();
            var teamEventDocumentList = new Dictionary<int, int>();

            using (var db = new MyFileItEntities())
            {
                if (teamEventId != null)
                {
                    teamEventDocumentIds = db.TEAMEVENTDOCUMENTs.Where(ted => ted.TEAMEVENTID == teamEventId).Select(ted => (int?)ted.ID).ToList();
                    db.SHAREDOCUMENTs
                    .Where(sd => userIds.Contains(sd.APPUSERID) && teamEventDocumentIds.Contains(sd.TEAMEVENTDOCUMENTID))
                    .ToList()
                    .ForEach(sfd =>
                    {
                        if (!shareFileCabinetDocumentIds.Contains(int.Parse(sfd.FILECABINETDOCUMENTID.ToString())))
                        {
                            shareFileCabinetDocumentIds.Add(int.Parse(sfd.FILECABINETDOCUMENTID.ToString()));
                        }

                        if (!shareDocumentVerifieds.ContainsKey((int)sfd.FILECABINETDOCUMENTID))
                        {
                            shareDocumentVerifieds.Add((int)sfd.FILECABINETDOCUMENTID, sfd.VERIFIEDAPPUSERID);
                        }

                        shareDocumentTeamEventIds.Add(new ShareDocumentTeamEvent()
                        {
                            FileCabinetDocumentId = sfd.FILECABINETDOCUMENTID,
                            TeamEventDocumentId = sfd.TEAMEVENTDOCUMENTID,
                            VerifiedAppUserId = sfd.VERIFIEDAPPUSERID
                        });
                        teamEventDocumentList.Add((int)sfd.TEAMEVENTDOCUMENTID, (int)sfd.FILECABINETDOCUMENTID);
                    });
                }

                db.FILECABINETDOCUMENTs.Where(fd => userIds.Contains(fd.APPUSERID)
                        && (teamEventId == null || shareFileCabinetDocumentIds.Contains(fd.ID))
                        && !skipDocumentList.Contains(fd.ID)
                        )
                    .ToList()
                    .ForEach(fd =>
                    {
                        Debug("Getting Doc: " + fd.DOCUMENTID);
                        //get the document base 64
                        if (teamEventId == null || !teamEventDocumentList.Any(docList => docList.Value == fd.ID))
                        {
                            var fileitDoc = new FileCabinetDocumentDTO(fd);
                            fileitDoc.Base64Image = null;
                            fileitDoc.Base64ImageThumb = null;
                            if (teamEventId != null)
                            {
                                fileitDoc.TeamEventDocumentId = shareDocumentTeamEventIds.First(sdt => sdt.FileCabinetDocumentId == fd.ID).TeamEventDocumentId;
                                fileitDoc.VerifiedAppUserId = shareDocumentVerifieds[fd.ID];
                            }
                            result.Documents.Add(fileitDoc);
                        }
                        else
                        {
                            //catch any docs with identical images!
                            teamEventDocumentList.Where(docList => docList.Value == fd.ID).ToList().ForEach(
                                td =>
                                {
                                    //don't duplicate the docs!!!
                                    if (!result.Documents.Any(doc => doc.TeamEventDocumentId == td.Key && doc.ID == fd.ID))
                                    {
                                        var fileitDoc = new FileCabinetDocumentDTO(fd);
                                        fileitDoc.Base64Image = null;
                                        fileitDoc.Base64ImageThumb = null;
                                        if (teamEventId != null)
                                        {
                                            fileitDoc.TeamEventDocumentId = td.Key;
                                            var documentEventObject = shareDocumentTeamEventIds.SingleOrDefault(t => (int)t.TeamEventDocumentId == td.Key && (int)t.FileCabinetDocumentId == fd.ID);
                                            fileitDoc.VerifiedAppUserId = documentEventObject == null ? null : documentEventObject.VerifiedAppUserId;
                                            var teamEventDocId = td.Key;//shareDocumentTeamEventIds.Single(t => (int)t.TeamEventDocumentId == td.Key && (int)t.FileCabinetDocumentId == fd.ID).TeamEventDocumentId;
                                            fileitDoc.DocumentTypeName = db.TEAMEVENTDOCUMENTs.Single(ted => ted.ID == teamEventDocId).DOCUMENTNAME;//get the TeamEventDocumentType
                                        }

                                        result.Documents.Add(fileitDoc);
                                    }
                                });
                        }

                        //set the verified user name
                        result.Documents.ForEach(d =>
                        {
                            d.VerifiedAppUserName = GetVerifiedAppUserName(db, d.VerifiedAppUserId);
                        });
                        result.Success = true;

                    });
            }
            return result;
        }

        private MyFileItResult FindDocumentsByIdList(List<int?> userIds, int? teamEventId, List<int?> skipDocumentList, bool thumbsOnly = false)
        {
            var result = new MyFileItResult();
            var teamEventDocumentIds = new List<int?>();
            var shareFileCabinetDocumentIds = new List<int>();
            var shareDocumentVerifieds = new Dictionary<int, int?>();
            var shareDocumentTeamEventIds = new List<ShareDocumentTeamEvent>();
            var teamEventDocumentList = new Dictionary<int, int>();

            using (var db = new MyFileItEntities())
            {
                if (teamEventId != null)
                {
                    teamEventDocumentIds = db.TEAMEVENTDOCUMENTs.Where(ted => ted.TEAMEVENTID == teamEventId).Select(ted => (int?)ted.ID).ToList();
                    //shareFileCabinetDocumentIds = 
                    db.SHAREDOCUMENTs
                    .Where(sd => userIds.Contains(sd.APPUSERID) && teamEventDocumentIds.Contains(sd.TEAMEVENTDOCUMENTID))
                        //.Select(sd => sd.FILECABINETDOCUMENTID)
                    .ToList()
                    .ForEach(sfd =>
                    {
                        if (!shareFileCabinetDocumentIds.Contains(int.Parse(sfd.FILECABINETDOCUMENTID.ToString())))
                        {
                            shareFileCabinetDocumentIds.Add(int.Parse(sfd.FILECABINETDOCUMENTID.ToString()));
                        }

                        if (!shareDocumentVerifieds.ContainsKey((int)sfd.FILECABINETDOCUMENTID))
                        {
                            shareDocumentVerifieds.Add((int)sfd.FILECABINETDOCUMENTID, sfd.VERIFIEDAPPUSERID);
                        }

                        shareDocumentTeamEventIds.Add(new ShareDocumentTeamEvent()
                        {
                            FileCabinetDocumentId = sfd.FILECABINETDOCUMENTID,
                            TeamEventDocumentId = sfd.TEAMEVENTDOCUMENTID,
                            VerifiedAppUserId = sfd.VERIFIEDAPPUSERID
                        });
                        /* if (!shareDocumentTeamEventIds.ContainsKey((int)sfd.FILECABINETDOCUMENTID))
                         {
                             shareDocumentTeamEventIds.Add((int)sfd.FILECABINETDOCUMENTID, sfd.TEAMEVENTDOCUMENTID);
                         }*/
                        teamEventDocumentList.Add((int)sfd.TEAMEVENTDOCUMENTID, (int)sfd.FILECABINETDOCUMENTID);
                    });
                }

                db.FILECABINETDOCUMENTs.Where(fd => userIds.Contains(fd.APPUSERID)
                        && (teamEventId == null || shareFileCabinetDocumentIds.Contains(fd.ID))
                        && !skipDocumentList.Contains(fd.ID)
                        )
                    .ToList()
                    .ForEach(fd =>
                {
                    Debug("Getting Doc: " + fd.DOCUMENTID);
                    //get the document base 64
                    var lookups = new List<FileItMainService.FileItDocumentLookup>();
                    var lookup = new FileItMainService.FileItDocumentLookup()
                    {
                        IndexNumber = -1,
                        LookupValue = fd.DOCUMENTID,
                        Operator = "="
                    };
                    lookups.Add(lookup);

                    var idLookups = new List<FileItMainService.FileItDocumentIdLookup>();

                    idLookups.Add(new FileItMainService.FileItDocumentIdLookup()
                    {
                        CabinetId = fd.CABINETID,
                        DocumentId = fd.DOCUMENTID
                    });

                    Debug("About to get doc from " + idLookups.First().CabinetId);

                    var r = new FileItMainService.FileItResponse();
                    using (var svc = new FileItMainService.FileItServiceClient())
                    {
                        r = svc.GetDocumentsById(ConfigurationSettings.ServiceUser, ConfigurationSettings.ServicePass, idLookups.ToArray(), true, false);
                    }
                    Debug("Result from doc lookup = " + r.Success.ToString());
                    //var r = GetDocuments(user, pass, fd.CABINETID, lookups.ToArray(), true);
                    if (r.Success)
                    {
                        Debug("adding doc to dto result");
                        r.Documents.ToList().ForEach(d =>
                        {
                            if (teamEventId == null || !teamEventDocumentList.Any(docList => docList.Value == fd.ID))
                            {
                                var fileitDoc = new FileCabinetDocumentDTO(fd);
                                fileitDoc.Base64Image = thumbsOnly ? null : d.ImageBase64;
                                fileitDoc.Base64ImageThumb = d.WebImageThumbBase64;
                                if (teamEventId != null)
                                {
                                    fileitDoc.TeamEventDocumentId = shareDocumentTeamEventIds.First(sdt => sdt.FileCabinetDocumentId == fd.ID).TeamEventDocumentId;
                                    fileitDoc.VerifiedAppUserId = shareDocumentVerifieds[fd.ID];
                                }
                                result.Documents.Add(fileitDoc);
                            }
                            else
                            {
                                //catch any docs with identical images!
                                teamEventDocumentList.Where(docList => docList.Value == fd.ID).ToList().ForEach(
                                    td =>
                                    {
                                        //don't duplicate the docs!!!
                                        if (!result.Documents.Any(doc => doc.TeamEventDocumentId == td.Key && doc.ID == fd.ID))
                                        {
                                            var fileitDoc = new FileCabinetDocumentDTO(fd);
                                            fileitDoc.Base64Image = thumbsOnly ? null : d.ImageBase64;
                                            fileitDoc.Base64ImageThumb = d.WebImageThumbBase64;
                                            if (teamEventId != null)
                                            {
                                                fileitDoc.TeamEventDocumentId = td.Key;
                                                var documentEventObject = shareDocumentTeamEventIds.SingleOrDefault(t => (int)t.TeamEventDocumentId == td.Key && (int)t.FileCabinetDocumentId == fd.ID);
                                                fileitDoc.VerifiedAppUserId = documentEventObject == null ? null : documentEventObject.VerifiedAppUserId;
                                                var teamEventDocId = td.Key;//shareDocumentTeamEventIds.Single(t => (int)t.TeamEventDocumentId == td.Key && (int)t.FileCabinetDocumentId == fd.ID).TeamEventDocumentId;
                                                fileitDoc.DocumentTypeName = db.TEAMEVENTDOCUMENTs.Single(ted => ted.ID == teamEventDocId).DOCUMENTNAME;//get the TeamEventDocumentType
                                            }

                                            result.Documents.Add(fileitDoc);
                                        }
                                    });
                            }
                        });

                        //set the verified user name
                        result.Documents.ForEach(d =>
                        {
                            d.VerifiedAppUserName = GetVerifiedAppUserName(db, d.VerifiedAppUserId);
                        });
                        result.Success = true;
                    }
                });
            }
            return result;
        }

        private string GetVerifiedAppUserName(MyFileItEntities db, int? id)
        {
            string result = null;
            if (id != null)
            {
                result = db.APPUSERs.Single(u => u.ID == id).FullName;
            }
            return result;
        }



        public MyFileItResult AddAppUser(string user, string pass, AppUserDTO appUser, int? autoSignUpOrganizationId = null)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass) && appUser != null)
            {
                using (var db = new MyFileItEntities())
                {
                    if (APPUSER.ValidUserName(appUser.USERNAME))
                    {
                        if (appUser.APPUSERTYPEID == null)
                        {
                            //default to a player
                            var appUserType = db.APPUSERTYPEs.Where(au => au.PLAYERUSER == "1").First();
                            appUser.APPUSERTYPEID = appUserType.ID;
                        }
                        var isPlayerUser = db.APPUSERTYPEs.Single(au => au.ID == appUser.APPUSERTYPEID).PLAYERUSER.ConvertToFirebirdBoolean() == true.ConvertToFirebirdString();
                        var appUserEF = (APPUSER)appUser;
                        //appUserEF.DATECREATED = DateTime.Now;
                        appUserEF.SetNewID();

                        db.APPUSERs.Add(appUserEF);
                        result.Success = SaveDBChanges(db);
                        if (result.Success)
                        {
                            //include the appuser in the result
                            result.AppUsers.Add(new AppUserDTO(appUserEF));
                            if (appUser.PRIMARYAPPUSERID == null)
                            {
                                using (var svc = new FileItMainService.FileItServiceClient())
                                {
                                    var newCabinet = appUserEF.GenerateCabinetName();
                                    var response = svc.CreateCabinet(
                                        ConfigurationSettings.ServiceUser,
                                        ConfigurationSettings.ServicePass,
                                        DefaultMyFileItTemplate(newCabinet),
                                        newCabinet);
                                    //this is bullshit but it doesn't work right now to update the cabinetid column
                                    appUserEF.SetCabinetId(response.Cabinet.CabinetId);
                                }
                            }
                            else
                            {
                                var primaryCabinetId = db.APPUSERs.Single(au => au.ID == appUser.PRIMARYAPPUSERID).GetCabinetId();
                                appUserEF.SetCabinetId(primaryCabinetId);
                                appUserEF.USERNAME += "_" + appUser.FIRSTNAME;
                                appUserEF.PASSWORD = "_" + appUser.LASTNAME;
                            }
                            db.Entry(appUserEF).State = EntityState.Modified;
                            result.Success = SaveDBChanges(db);
                            //send an email out
                            if (result.Success)
                            {
                                string organizationName = null;
                                if (autoSignUpOrganizationId != null)
                                {
                                    organizationName = db.ORGANIZATIONs.Single(o => o.ID == (int)autoSignUpOrganizationId).NAME;
                                }
                                EmailHelper.SendSignUpEmail(appUserEF, isPlayerUser, autoSignUpOrganizationId != null, organizationName);
                            }
                        }
                    }
                    else
                    {
                        result.Message = "Invalid User Name";
                    }
                }
            }
            return result;
        }

        public MyFileItResult AssociateAppUserToOrganization(string user, string pass, int appUserId, int appUserTypeId, int organizationId, DateTime startDate, DateTime expiresDate, int? yearCode, int sportTypeId)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    var relationship = db.APPUSERORGANIZATIONs.SingleOrDefault(auo => auo.APPUSERID == appUserId && auo.ORGANIZATIONID == organizationId);

                    if (relationship == null)
                    {
                        var appUser = db.APPUSERs.SingleOrDefault(au => au.ID == appUserId);
                        var organization = db.ORGANIZATIONs.SingleOrDefault(o => o.ID == organizationId);
                        if (appUser != null)
                        {
                            if (organization != null)
                            {
                                db.APPUSERORGANIZATIONs.Add(appUser.CreateOrganizationAssociation(organizationId, appUserTypeId, startDate, expiresDate, yearCode, sportTypeId));
                                result.Success = SaveDBChanges(db);
                                if (result.Success)
                                {
                                    var appUserTypeName = db.APPUSERTYPEs.Single(aut => aut.ID == appUserTypeId).NAME;
                                    //EmailHelper.SendAssociateUserToOrganizationEmail(appUser, organization, appUserTypeName);
                                }
                            }
                            else
                            {
                                result.Message = "Organization not found.";
                            }
                        }
                        else
                        {
                            result.Message = "Application User Not Found";
                        }
                    }
                    else
                    {
                        result.Message = "User Organization assocation already exists.";
                    }
                }
            }
            return result;
        }

        public MyFileItResult SendAddCoachEmail(string user, string pass, string emailAddress, string subject, string bodyMessage, List<string> ccEmailAddresses)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                result.Success = EmailHelper.SendAssociateUserToOrganizationEmail(emailAddress, subject, bodyMessage, ccEmailAddresses);
            }
            return result;
        }

        public MyFileItResult RemoveAppUserFromOrganization(string user, string pass, int appUserId, int organizationId)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    var relationship = db.APPUSERORGANIZATIONs.SingleOrDefault(auo => auo.APPUSERID == appUserId && auo.ORGANIZATIONID == organizationId);


                    if (relationship != null)
                    {
                        db.APPUSERORGANIZATIONs.Remove(relationship);
                        result.Success = SaveDBChanges(db);
                    }
                    else
                    {
                        result.Message = "User Organization association does not exist.";
                    }
                }
            }
            return result;
        }

        public MyFileItResult UpdateAppUser(string user, string pass, AppUserDTO appUser)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                string cabinetId = null;
                DateTime? dateCreated;
                string userName;
                string passWord;

                using (var db = new MyFileItEntities())
                {
                    //this seems crazy, but you can't retrieve the appuser object AND change it to modified. jo 29Jan2016
                    var appUserObject = db.APPUSERs.Single(u => u.ID == appUser.ID);
                    cabinetId = appUserObject.GetCabinetId();
                    dateCreated = appUserObject.DATECREATED;
                    userName = appUserObject.USERNAME;
                    passWord = appUserObject.PASSWORD;
                }
                using (var db = new MyFileItEntities())
                {
                    //var appuserObject = db.APPUSERs.Single(u => u.ID == appUser.ID);
                    var appUserEF = (APPUSER)appUser;

                    appUserEF.SetCabinetId(cabinetId);
                    appUserEF.DATECREATED = dateCreated;
                    appUserEF.USERNAME = userName;
                    appUserEF.PASSWORD = passWord;
                    //appuserObject.UpdateObject(appUser);
                    db.Entry(appUserEF).State = EntityState.Modified;
                    result.Success = SaveDBChanges(db);
                    //return the appuser
                    if (result.Success)
                    {
                        EmailHelper.SendChangeAccountEmail(appUserEF);
                        //include the appuser in the result
                        result.AppUsers.Add(new AppUserDTO(appUserEF));
                    }
                }
            }
            return result;
        }

        public MyFileItResult RemoveAppUser(string user, string pass, int appUserId)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    var userObj = db.APPUSERs.SingleOrDefault(u => u.ID == appUserId);
                    //should this delete the associated ref's?
                    db.APPUSERORGANIZATIONs.Where(auo => auo.APPUSERID == appUserId).ToList().ForEach(auo =>
                    {
                        db.APPUSERORGANIZATIONs.Remove(auo);
                    });
                    if (userObj != null)
                    {
                        db.APPUSERs.Remove(userObj);
                    }
                    result.Success = SaveDBChanges(db);
                }
            }
            return result;
        }

        public MyFileItResult UpdateAppUserStatus(string user, string pass, int appUserId, int appUserStatusId)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    var userObj = db.APPUSERs.SingleOrDefault(u => u.ID == appUserId);
                    //should this delete the associated ref's?
                    if (userObj != null)
                    {
                        userObj.APPUSERSTATUSID = appUserStatusId;
                    }
                    result.Success = SaveDBChanges(db);
                }
            }
            return result;
        }

        /************************************************************
        * SALESREPs
        * ********************************************************/
        public MyFileItResult GetSalesReps(string user, string pass, int? salesRepId, string nameLookup, int? salesRepStatusId)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    if (salesRepId != null)
                    {
                        db.SALESREPs.Where(a => a.ID == salesRepId).ToList().ForEach(a =>
                        {
                            result.SalesReps.Add(new SalesRepDTO(a));
                            result.Success = true;
                        });
                    }
                    else
                    {
                        db.SALESREPs.Where(a => a.FIRSTNAME.IndexOf(nameLookup) > -1 || a.LASTNAME.IndexOf(nameLookup) > -1).ToList().ForEach(a =>
                        {
                            result.SalesReps.Add(new SalesRepDTO(a));
                            result.Success = true;
                        });
                        db.SALESREPs.Where(a => a.SALESREPSTATUSID == salesRepStatusId).ToList().ForEach(a =>
                        {
                            result.SalesReps.Add(new SalesRepDTO(a));
                            result.Success = true;
                        });

                    }
                }
            }
            return result;
        }

        public MyFileItResult LoginSalesRep(string user, string pass, string emailAddress, string password)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    if (emailAddress != null && emailAddress.Length > 0 && password != null && password.Length > 0)
                    {
                        db.SALESREPs.Where(a => a.EMAILADDRESS.ToLower().Trim() == emailAddress.ToLower().Trim() && a.PASSWORD == password).ToList().ForEach(a =>
                        {
                            result.SalesReps.Add(new SalesRepDTO(a));
                            result.Success = true;
                        });
                    }
                }
            }
            return result;
        }

        public MyFileItResult AddSalesRep(string user, string pass, SalesRepDTO salesRep)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass) && salesRep != null)
            {
                using (var db = new MyFileItEntities())
                {
                    if (SALESREP.ValidSalesRepName(salesRep.EMAILADDRESS))
                    {
                        var salesRepEF = (SALESREP)salesRep;
                        salesRepEF.DATECREATED = DateTime.Now;
                        salesRepEF.SetNewID();
                        db.SALESREPs.Add(salesRepEF);
                        result.Success = SaveDBChanges(db);
                    }
                    else
                    {
                        result.Message = "Invalid Name";
                    }
                }
            }
            return result;
        }

        public MyFileItResult UpdateSalesRep(string user, string pass, SalesRepDTO salesRep)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    //var rep = db.SALESREPs.SingleOrDefault(r => r.ID == salesRep.ID);
                    //rep.UpdateObject(salesRep);
                    var rep = (SALESREP)salesRep;
                    db.Entry(rep).State = EntityState.Modified;
                    result.Success = SaveDBChanges(db);
                }
            }
            return result;
        }

        public MyFileItResult ChangeSalesRepStatus(string user, string pass, int salesRepId, int salesRepStatusId)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    var rep = db.SALESREPs.SingleOrDefault(r => r.ID == salesRepId);
                    if (rep != null)
                    {
                        rep.SALESREPSTATUSID = salesRepStatusId;
                        result.Success = SaveDBChanges(db);
                    }
                }
            }
            return result;
        }

        public MyFileItResult RemoveSalesRep(string user, string pass, int salesRepId, int salesRepIdReplacement)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    db.ORGANIZATIONs.Where(o => o.SALESREPID == salesRepId).ToList().ForEach(o =>
                    {
                        o.SALESREPID = salesRepIdReplacement;
                    });
                    result.Success = SaveDBChanges(db);
                    if (result.Success)
                    {
                        db.SALESREPs.RemoveRange(db.SALESREPs.Where(sr => sr.ID == salesRepId));
                        result.Success = SaveDBChanges(db);
                    }
                }
            }
            return result;

        }

        /************************************************************
         * COACHs 
         * ********************************************************/
        public MyFileItResult GetCoaches(string user, string pass, int? coachId, string nameLookup, int? appUserId, int? organizationId)
        {
            var result = new MyFileItResult();
            var addedCoachIds = new List<int>();

            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    if (coachId != null)
                    {
                        db.COACHes.Where(a => a.ID == coachId).ToList().ForEach(a =>
                        {
                            result.Coaches.Add(new CoachDTO(a));
                            result.Success = true;
                        });
                    }
                    else
                    {
                        db.COACHes.Where(a => a.FIRSTNAME.IndexOf(nameLookup) > -1 || a.LASTNAME.IndexOf(nameLookup) > -1).ToList().ForEach(a =>
                        {
                            result.Coaches.Add(new CoachDTO(a));
                            addedCoachIds.Add(a.ID);
                            result.Success = true;
                        });
                        db.COACHes.Where(a => (a.APPUSERID == appUserId) && !addedCoachIds.Contains(a.ID)).ToList().ForEach(a =>
                        {
                            addedCoachIds.Add(a.ID);
                            result.Coaches.Add(new CoachDTO(a));
                            result.Success = true;
                        });
                        db.COACHes.Where(a => (a.ORGANIZATIONID == organizationId) && !addedCoachIds.Contains(a.ID)).ToList().ForEach(a =>
                        {
                            addedCoachIds.Add(a.ID);
                            result.Coaches.Add(new CoachDTO(a));
                            result.Success = true;
                        });
                    }
                }
            }
            return result;
        }

        public MyFileItResult GetCoachesByOrganizationId(string user, string pass, int organizationId)
        {
            var result = new MyFileItResult();
            var addedCoachIds = new List<int>();

            if (AllowAccess(user, pass))
            {
                result.AppUsers = searchCoachesByOrganizationId(organizationId);
                result.Success = true;
            }
            return result;
        }

        private List<AppUserDTO> searchCoachesByOrganizationId(int organizationId)
        {
            var result = new List<AppUserDTO>();
            using (var db = new MyFileItEntities())
            {
                var appUserTypes = db.APPUSERTYPEs.Where(aut => aut.ISCOACH == "1").Select(aut => (int?)aut.ID).ToList();
                db.APPUSERORGANIZATIONs.Where(auo => auo.ORGANIZATIONID == organizationId && auo.APPUSERTYPEID != null && appUserTypes.Contains(auo.APPUSERTYPEID))
                    .ToList().ForEach(a =>
                    {
                        var appUser = db.APPUSERs.Single(au => au.ID == a.APPUSERID);
                        result.Add(new AppUserDTO(appUser));
                    });
                //db.COACHes.Where(a => a.ORGANIZATIONID == organizationId).ToList().ForEach(a =>
                //{
                //    var appUser = db.APPUSERs.Single(au => au.ID == a.APPUSERID);
                //    result.Add(new CoachDTO(a, appUser.EMAILADDRESS));
                //});
            }
            return result;
        }

        public MyFileItResult GetCoachesByTeamEventId(string user, string pass, int teamEventId)
        {
            var result = new MyFileItResult();
            var addedCoachIds = new List<int>();

            if (AllowAccess(user, pass))
            {
                result.Coaches = searchCoachesByTeamEventId(teamEventId);
                result.Success = true;
            }
            return result;
        }

        private List<CoachDTO> searchCoachesByTeamEventId(int teamEventId)
        {
            var result = new List<CoachDTO>();
            using (var db = new MyFileItEntities())
            {
                db.COACHes.Where(a => a.TEAMEVENTID == teamEventId && a.APPUSERID != null).ToList().ForEach(a =>
                {
                    var appUser = db.APPUSERs.Single(au => au.ID == a.APPUSERID);
                    result.Add(new CoachDTO(a, appUser.EMAILADDRESS));
                });
            }
            return result;
        }

        public MyFileItResult AddCoach(string user, string pass, int appUserId, int organizationId, DateTime startDate, DateTime expiresDate, int? yearCode, int sportTypeId)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    //associate the user to the organization with coach status
                    var coachUserTypeId = db.APPUSERTYPEs.First(aut => aut.ISCOACH == "1").ID;
                    AssociateAppUserToOrganization(user, pass, appUserId, coachUserTypeId, organizationId, startDate, expiresDate, yearCode, sportTypeId);
                    result.Success = SaveDBChanges(db);
                }
            }
            return result;
        }

        public MyFileItResult RemoveCoach(string user, string pass, int appUserId, int organizationId)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    //find any team event lookups
                    db.COACHes.Where(c => c.APPUSERID == appUserId && c.ORGANIZATIONID == organizationId).ToList().ForEach(c =>
                    {
                        db.COACHes.Remove(c);
                    });
                    var coachUserTypeId = db.APPUSERTYPEs.First(aut => aut.ISCOACH == "1").ID;
                    var appUserOrganization = db.APPUSERORGANIZATIONs.FirstOrDefault(auo => auo.APPUSERID == appUserId && auo.ORGANIZATIONID == organizationId && auo.APPUSERTYPEID == coachUserTypeId);
                    if (appUserOrganization != null)
                    {
                        db.APPUSERORGANIZATIONs.Remove(appUserOrganization);
                    }
                    result.Success = SaveDBChanges(db);
                }
            }
            return result;
        }

        //public MyFileItResult UpdateCoach(string user, string pass, CoachDTO coach)
        //{
        //    var result = new MyFileItResult();
        //    if (AllowAccess(user, pass))
        //    {
        //        using (var db = new MyFileItEntities())
        //        {
        //            //var c = db.COACHes.SingleOrDefault(r => r.ID == coach.ID);
        //            //c.UpdateObject(coach);
        //            var c = (COACH)coach;
        //            db.Entry(c).State = EntityState.Modified;
        //            result.Success = SaveDBChanges(db);
        //        }
        //    }
        //    return result;
        //}

        //public MyFileItResult AssociateCoachToUser(string user, string pass, int coachId, int appUserId)
        //{
        //    var result = new MyFileItResult();
        //    if (AllowAccess(user, pass))
        //    {
        //        using (var db = new MyFileItEntities())
        //        {
        //            var c = db.COACHes.SingleOrDefault(r => r.ID == coachId);
        //            var userObj = db.APPUSERs.SingleOrDefault(u => u.ID == appUserId);
        //            if (c != null && userObj != null)
        //            {
        //                //fail the call if the user id is not set
        //                c.APPUSERID = appUserId;
        //                result.Success = SaveDBChanges(db);
        //            }
        //            else
        //            {
        //                result.Message = "Please pass in a valid appuserid";
        //            }

        //        }
        //    }
        //    return result;
        //}

        public MyFileItResult RemoveCoachFromCurrentAppUser(string user, string pass, int coachId)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    var c = db.COACHes.SingleOrDefault(r => r.ID == coachId);
                    if (c != null)
                    {
                        //delete the coach instead
                        db.COACHes.Remove(c);
                    }
                    result.Success = SaveDBChanges(db);
                }
            }
            return result;
        }

        public MyFileItResult AssociateCoachToTeamEvent(string user, string pass, CoachDTO coach)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass) && coach != null)
            {
                using (var db = new MyFileItEntities())
                {
                    //check for dups
                    if (coach.TEAMEVENTID == null || !db.COACHes.Any(c => c.APPUSERID == coach.APPUSERID && c.TEAMEVENTID == coach.TEAMEVENTID))
                    {
                        var coachEF = (COACH)coach;
                        coachEF.DATECREATED = DateTime.Now;
                        coachEF.SetNewID();
                        db.COACHes.Add(coachEF);
                        result.Success = SaveDBChanges(db);
                    }
                    else
                    {
                        var duplicateCoach = db.COACHes.First(c => c.APPUSERID == coach.APPUSERID && c.TEAMEVENTID == coach.TEAMEVENTID);
                        var duplicateUser = db.APPUSERs.Single(au => au.ID == duplicateCoach.APPUSERID);
                        var duplicateEvent = db.TEAMEVENTs.Single(e => e.ID == coach.TEAMEVENTID);
                        result.Message = duplicateUser.FullName + " is already associated to event(" + duplicateEvent.NAME + ")";
                    }
                }
            }
            return result;
        }

        public MyFileItResult RemoveCoachFromTeamEvent(string user, string pass, int appUserId, int teamEventId)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    var coach = db.COACHes.Single(c => c.APPUSERID == appUserId && c.TEAMEVENTID == teamEventId);
                    db.COACHes.Remove(coach);
                    result.Success = SaveDBChanges(db);
                }
            }
            return result;
        }

        //public MyFileItResult AssociateCoachToTeamEvent(string user, string pass, int appUserId, int teamEventId)
        //{
        //    var result = new MyFileItResult();
        //    if (AllowAccess(user, pass))
        //    {
        //        using (var db = new MyFileItEntities())
        //        {
        //            //var c = db.COACHes.SingleOrDefault(r => r.ID == coachId);
        //            var teamEventObj = db.TEAMEVENTs.SingleOrDefault(u => u.ID == teamEventId);

        //            //if (c != null && teamEventId != null)
        //            //{
        //            //    c.APPUSERID = teamEventId;
        //            //}
        //            result.Success = SaveDBChanges(db);
        //        }
        //    }
        //    return result;
        //}

        public MyFileItResult RemoveCoachFromCurrentTeamEvent(string user, string pass, int coachId)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    var c = db.COACHes.SingleOrDefault(r => r.ID == coachId);
                    if (c != null)
                    {
                        c.TEAMEVENTID = null;
                    }
                    result.Success = SaveDBChanges(db);
                }
            }
            return result;
        }

        public List<CoachStatusDTO> GetCoachStatuses(string user, string pass)
        {
            var result = new List<CoachStatusDTO>();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    db.COACHSTATUS.ToList().ForEach(c =>
                    {
                        result.Add(new CoachStatusDTO()
                        {
                            ID = c.ID,
                            NAME = c.NAME,
                            ALLOWADDEVENT = c.ALLOWADDEVENT == "Y"
                        });
                    });
                }
            }
            return result;
        }

        /************************************************************
         * SHAREKEYs 
         * ********************************************************/
        public MyFileItResult AddShareKey(string user, string pass, int primaryAppUserId, DateTime purchaseDate, string promoCode, string last4Digits, decimal amount, int salesRepId, int numKeys)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    //validate the promocode if it exists
                    string errorMessage = "";
                    if (ValidShareKeyPromoCode(promoCode, ref errorMessage, db))
                    {
                        SHAREKEY sk = null;
                        for (var i = 0; i < numKeys; i++)
                        {
                            //this is slow due to the id issues with firebird
                            sk = new SHAREKEY(primaryAppUserId, purchaseDate, promoCode, last4Digits, amount, salesRepId);
                            db.SHAREKEYs.Add(sk);
                            result.Success = SaveDBChanges(db);
                            //fk issue??
                            sk.SALESREPID = salesRepId;
                            result.Success = SaveDBChanges(db);
                        }
                    }
                    else
                    {
                        result.Message = errorMessage;
                    }
                }
            }
            return result;
        }

        private bool ValidShareKeyPromoCode(string promoCode, ref string message, MyFileItEntities db)
        {
            var result = false;
            if (promoCode == null || promoCode.Length == 0)
            {
                result = true;
            }
            else
            {
                //test if it exists
                if (db.SHAREKEYs.Any(sk => sk.PROMOCODE.ToLower() == promoCode.ToLower()))
                {
                    message = promoCode + " already exists in the system.";
                }
                else
                {
                    result = true;
                }
            }

            return result;
        }

        public MyFileItResult AssociateShareKeyToUser(string user, string pass, int appUserId, int shareKeyId)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    var sharekey = db.SHAREKEYs.SingleOrDefault(sk => sk.ID == shareKeyId);
                    if (sharekey != null)
                    {
                        var appUser = db.APPUSERs.SingleOrDefault(au => au.ID == appUserId);
                        if (appUser != null)
                        {
                            sharekey.APPUSERID = appUserId;
                            db.Entry(sharekey).State = EntityState.Modified;
                            result.Success = SaveDBChanges(db);
                        }
                        else
                        {
                            result.Message = "User not found.";
                        }
                    }
                    else
                    {
                        result.Message = "Share key not found";
                    }

                }
            }
            return result;
        }

        public MyFileItResult UpdateShareKey(string user, string pass, ShareKeyDTO shareKey)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    var c = (SHAREKEY)shareKey;
                    db.Entry(c).State = EntityState.Modified;
                    result.Success = SaveDBChanges(db);
                }
            }
            return result;
        }

        public MyFileItResult GetAvailableShareKeysByPromoCodeAndPrimaryUser(string user, string pass, int primaryAppUserId, string promocode)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    db.SHAREKEYs.Where(sk => sk.APPUSERID == null && (sk.PRIMARYAPPUSERID == primaryAppUserId
                       ||
                       (sk.PROMOCODE.Equals(promocode, StringComparison.CurrentCultureIgnoreCase) && sk.PROMOCODE.Length > 0)))
                        // .Include(sk => sk.APPUSER)
                       .ToList()
                       .ForEach(sk =>
                       {
                           result.ShareKeys.Add(new ShareKeyDTO(sk, false));
                       });
                    result.Success = true;
                }
            }
            return result;
        }

        public MyFileItResult GetShareKeys(string user, string pass, int primaryAppUserId)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    db.SHAREKEYs.Where(sk => sk.PRIMARYAPPUSERID == primaryAppUserId).Include(sk => sk.APPUSER).ToList().ForEach(sk =>
                    {
                        result.ShareKeys.Add(new ShareKeyDTO(sk));
                    });
                    result.Success = SaveDBChanges(db);
                }
            }
            return result;
        }

        /****************************************
         * PaymentHistory
         * **************************************/
        public MyFileItResult AddPaymentHistory(string user, string pass, PaymentHistoryDTO paymentHistory)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass) && paymentHistory != null)
            {
                using (var db = new MyFileItEntities())
                {

                    var paymentHistoryEF = (PAYMENTHISTORY)paymentHistory;
                    //paymentHistoryEF.DATEPURCHASED = DateTime.Now;
                    paymentHistoryEF.SetNewID();
                    db.PAYMENTHISTORies.Add(paymentHistoryEF);
                    result.Success = SaveDBChanges(db);

                }
            }
            return result;
        }

        public MyFileItResult GetPaymentHistory(string user, string pass, int primaryAppUserId)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {
                using (var db = new MyFileItEntities())
                {
                    db.PAYMENTHISTORies.Where(ph => ph.PRIMARYAPPUSERID == primaryAppUserId).ToList().ForEach(ph =>
                    {
                        result.PaymentHistories.Add(new PaymentHistoryDTO(ph));
                    });
                    result.Success = true;
                }
            }
            return result;
        }

        /***********************************************
         * REPORTS
         * ********************************************/
        public MyFileItResult GetEventPlayerStatusReport(string user, string pass, int teamEventId)
        {
            var result = new MyFileItResult();
            if (AllowAccess(user, pass))
            {

                using (var db = new MyFileItEntities())
                {
                    //get the report data
                    var reportHelper = new ReportHelper(db);
                    var data = reportHelper.CreateEventPlayerDocumentStatusReport(teamEventId);
                    result.ReportData = data.ConvertToJSONString();
                    result.Success = true;
                }
            }
            return result;
        }
        /*********************************************************************************/

        /***********************************************
         * BASE METHODS
         * ********************************************/


        private bool SaveDBChanges(MyFileItEntities db)
        {
            var success = false;
            try
            {
                db.SaveChanges();
                success = true;
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    ExceptionHelper.LogError(String.Format("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State));
                    foreach (var ve in eve.ValidationErrors)
                    {
                        ExceptionHelper.LogError(String.Format("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage));
                    }
                }
                throw;
            }
            return success;
        }

        private bool AllowAccess(string user, string pass)
        {
            var response = false;

            using (var svc = new FileItMainService.FileItServiceClient())
            {
                response = svc.Authenticate(user, pass).Success;
            }
            return response;
        }

        public FileItMainService.FileItResponse Authenticate(string user, string pass)
        {
            var response = new FileItMainService.FileItResponse();
            using (var svc = new FileItMainService.FileItServiceClient())
            {
                response = svc.Authenticate(user, pass);
            }

            return response;
        }

        public Dictionary<string, string> GetCabinets(string user, string pass, string targetuser, bool allavailable)
        {
            var response = new Dictionary<string, string>();
            using (var svc = new FileItMainService.FileItServiceClient())
            {
                response = svc.GetCabinets(ConfigurationSettings.ServiceUser, ConfigurationSettings.ServicePass, targetuser, allavailable);
            }
            return response;
        }

        public FileItMainService.FileItCabinet GetCabinet(string user, string pass, string cabinetId)
        {
            var response = new FileItMainService.FileItCabinet();
            using (var svc = new FileItMainService.FileItServiceClient())
            {
                response = GetCabinet(ConfigurationSettings.ServiceUser, ConfigurationSettings.ServicePass, cabinetId);
            }
            return response;
        }

        public FileItMainService.FileItResponse GetDocuments(string user, string pass, string cabinetid, FileItMainService.FileItDocumentLookup[] lookups, bool includeThumbs)
        {
            var response = new FileItMainService.FileItResponse();
            using (var svc = new FileItMainService.FileItServiceClient())
            {
                response = svc.GetDocuments(ConfigurationSettings.ServiceUser, ConfigurationSettings.ServicePass, cabinetid, lookups, includeThumbs);
            }
            return response;
        }

        public FileItMainService.FileItResponse UploadDocuments(string user, string pass, string cabinetId, FileItMainService.FileItDocument[] documents)
        {
            var response = new FileItMainService.FileItResponse();
            using (var svc = new FileItMainService.FileItServiceClient())
            {
                response = svc.UploadDocuments(ConfigurationSettings.ServiceUser, ConfigurationSettings.ServicePass, cabinetId, documents);
            }
            return response;
        }




        public FileItMainService.FileItTemplate DefaultMyFileItTemplate(string organizationName)
        {
            var templateName = organizationName.Replace(" ", "") + "template";
            var templateDefinitions = new FileItMainService.FileItTemplateDefinition[9];

            templateDefinitions[0] = CreateTemplateDefinition(templateName, "Application User ID", 1, 10, "Number", false, false, false);
            templateDefinitions[1] = CreateTemplateDefinition(templateName, "Scan Date", 2, 10, "Date", false, false, false);
            templateDefinitions[2] = CreateTemplateDefinition(templateName, "First Name", 3, 100, "Text", false, false, false);
            templateDefinitions[3] = CreateTemplateDefinition(templateName, "Last Name", 4, 100, "Text", false, false, false);
            templateDefinitions[4] = CreateTemplateDefinition(templateName, "Document Type ID", 5, 10, "Number", false, false, false);
            templateDefinitions[5] = CreateTemplateDefinition(templateName, "Document Type", 6, 100, "Text", false, false, false);
            templateDefinitions[6] = CreateTemplateDefinition(templateName, "Document Date", 7, 10, "Date", false, false, false);
            templateDefinitions[7] = CreateTemplateDefinition(templateName, "Verified Date", 8, 10, "Date", false, false, false);
            templateDefinitions[8] = CreateTemplateDefinition(templateName, "Verified Application User ID", 9, 10, "Number", false, false, false);

            var response = new FileItMainService.FileItTemplate()
            {
                TemplateName = templateName,
                TemplateDefinitions = templateDefinitions
            };
            return response;
        }

        private FileItMainService.FileItTemplateDefinition CreateTemplateDefinition(string templateName, string indexName, int indexNum, int maxLength, string dataType, bool mustFill, bool mustEnter, bool hideColumn)
        {
            return new FileItMainService.FileItTemplateDefinition()
            {
                TEMPLATENAME = templateName,
                INDEXNAME = indexName,
                INDEXNUM = (short)indexNum,
                MAXLENGTH = (short)maxLength,
                DATATYPE = dataType,
                MUSTFILL = mustFill ? "Yes" : "No",
                MUSTENTER = mustEnter ? "Yes" : "No",
                HIDECOLUMN = hideColumn ? "Yes" : "No"
            };
        }

        private void Debug(string message)
        {
            if (DebugMode)
            {
                ExceptionHelper.LogError(message);
            }
        }

    }
}
