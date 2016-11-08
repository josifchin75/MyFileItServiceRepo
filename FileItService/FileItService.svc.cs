using FileItDataLayer.Models;
using FileItService.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using FileItService.Helpers;

namespace FileItService
{

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "FileItService" in code, svc and config file together.

    public class FileItService : IFileItService
    {
        private string FileItDataPath = System.Configuration.ConfigurationManager.AppSettings["FileItDataPath"].ToString();
        private string FileItEmptyCabinetPath = System.Configuration.ConfigurationManager.AppSettings["FileItEmptyCabinetFilePath"].ToString();

        public bool InitService()
        {
            ExceptionHelper.LogError("Initialized Main Service.");
            //call the login 
            // ExceptionHelper.LogError("About to call login service.");
            // LoginAppUser("testuser", "testpass", "appusername", "appuserpass");
            // ExceptionHelper.LogError("Called login service.");
            return true;
        }

        public FileItResponse Authenticate(string user, string pass)
        {
            var response = new FileItResponse();

            using (var db = new FileItDataLayer.Models.SystemFileitEntities())
            {
                var userObj = db.USERS.FirstOrDefault(u => u.USERNAME.Equals(user, StringComparison.CurrentCultureIgnoreCase));
                if (userObj != null)
                {
                    response.Success = userObj.Authenticate(pass);
                }
                else
                {
                    response.Message = "User was not found";
                }
            }
            return response;
        }

        public FileItResponse SetCabinetAccess(string user, string pass, string accesstype, string targetuser, string cabinetid, bool allowaccess)
        {
            var response = new FileItResponse();
            using (var db = new FileItDataLayer.Models.SystemFileitEntities())
            {
                var cabinet = db.CABINETS.FirstOrDefault(c => c.CABINETID == cabinetid);
                if (cabinet != null)
                {
                    response.Success = cabinet.SetAccess(accesstype, targetuser, allowaccess);
                }
            }
            return response;
        }

        public FileItResponse AddUpdateUser(string user, string pass, FileItUser userobj)
        {
            var response = new FileItResponse();
            using (var db = new FileItDataLayer.Models.SystemFileitEntities())
            {
                var loginUser = db.USERS.FirstOrDefault(u => u.USERNAME.Equals(user, StringComparison.CurrentCultureIgnoreCase));
                if (loginUser.Authenticate(pass))
                {
                    var userDb = db.USERS.FirstOrDefault(u => u.USERNAME == userobj.UserName);
                    if (userDb == null)
                    {
                        userDb = new FileItDataLayer.Models.USER();
                    }
                    if (userDb != null)
                    {
                        if (userobj.MergeWithDB(userDb))
                        {
                            db.SaveChanges();
                            response.Success = true;
                        }
                    }
                }
            }
            return response;
        }

        public FileItResponse SetUserActive(string user, string pass, string targetuser, bool active)
        {
            var response = new FileItResponse();
            using (var db = new FileItDataLayer.Models.SystemFileitEntities())
            {
                var userObj = db.USERS.FirstOrDefault(u => u.USERNAME.Equals(user, StringComparison.CurrentCultureIgnoreCase));
                var targetUserObj = db.USERS.FirstOrDefault(u => u.USERNAME.Equals(targetuser, StringComparison.CurrentCultureIgnoreCase));
                if (userObj != null && targetUserObj != null && userObj.Authenticate(pass))
                {
                    targetUserObj.ActiveUserBool = active;
                    db.SaveChanges();
                    response.Success = true;
                }
            }

            return response;
        }

        public FileItResponse SetUserPassword(string user, string pass, string targetuser, string newpass)
        {
            var response = new FileItResponse();
            using (var db = new FileItDataLayer.Models.SystemFileitEntities())
            {
                var userObj = db.USERS.FirstOrDefault(u => u.USERNAME.Equals(user, StringComparison.CurrentCultureIgnoreCase));
                var targetUserObj = db.USERS.FirstOrDefault(u => u.USERNAME.Equals(targetuser, StringComparison.CurrentCultureIgnoreCase));
                if (userObj != null && targetUserObj != null && userObj.Authenticate(pass))
                {
                    targetUserObj.PASS = newpass;
                    db.SaveChanges();
                    response.Success = true;
                }
            }
            return response;
        }

        public Dictionary<string, string> GetCabinets(string user, string pass, string targetuser, bool allavailable)
        {
            var response = new Dictionary<string, string>();
            using (var db = new FileItDataLayer.Models.SystemFileitEntities())
            {
                var userObj = db.USERS.FirstOrDefault(u => u.USERNAME.Equals(user, StringComparison.CurrentCultureIgnoreCase));
                var targetUserObj = db.USERS.FirstOrDefault(u => u.USERNAME.Equals(targetuser, StringComparison.CurrentCultureIgnoreCase));
                if (userObj != null && targetUserObj != null && userObj.Authenticate(pass))
                {
                    response = allavailable ? targetUserObj.AvailableCabinets : targetUserObj.Cabinets;
                }
            }
            return response;
        }

        public FileItCabinet GetCabinet(string user, string pass, string cabinetId)
        {
            FileItCabinet response = null;
            using (var db = new FileItDataLayer.Models.SystemFileitEntities())
            {
                var userObj = db.USERS.FirstOrDefault(u => u.USERNAME.Equals(user, StringComparison.CurrentCultureIgnoreCase));
                if (userObj != null && userObj.Authenticate(pass))
                {
                    var cabinet = db.CABINETS.Single(c => c.CABINETID.Equals(cabinetId));

                    var def = new List<FileItTemplateDefinition>();
                    var template = new FileItTemplate();
                    template.ConvertFromTEMPLATE(cabinet.Template);
                    response = new FileItCabinet()
                    {
                        CabinetId = cabinet.CABINETID,
                        CabinetName = cabinet.CABINETNAME,
                        Template = template
                    };
                }
            }
            return response;
        }

        public FileItResponse CreateCabinet(string user, string pass, FileItTemplate template, string cabinetName)
        {
            var response = new FileItResponse();
            var cabinetId = "";
            try
            {
                response.Success = CABINET.Create(cabinetName, ref cabinetId, user, template.ConvertToTemplate(cabinetId), FileItDataPath, FileItEmptyCabinetPath);
                response.Cabinet = new FileItCabinet()
                {
                    CabinetId = cabinetId,
                    CabinetName = cabinetName
                };
            }
            catch (Exception err)
            {
                response.Message = err.ToString();
            }
            return response;
        }

        public FileItResponse DeleteDocument(string user, string pass, string cabinetid, string documentId)
        {
            var response = new FileItResponse();
            response.Documents = new List<DTOs.FileItDocument>();
            
            using (var db = new FileItDataLayer.Models.SystemFileitEntities())
            {
                var userObj = db.USERS.FirstOrDefault(u => u.USERNAME.Equals(user, StringComparison.CurrentCultureIgnoreCase));
                if (userObj != null && userObj.Authenticate(pass))
                {
                    var cabinet = db.CABINETS.Single(c => c.CABINETID.Equals(cabinetid, StringComparison.CurrentCultureIgnoreCase));
                    response.Success = cabinet.DeleteDocument(documentId); 
                }
            }

            return response;
        }

        public FileItResponse GetDocuments(string user, string pass, string cabinetid, List<DTOs.FileItDocumentLookup> lookups, bool includeThumbs)
        {
            var response = new FileItResponse();
            response.Documents = new List<DTOs.FileItDocument>();

            using (var db = new FileItDataLayer.Models.SystemFileitEntities())
            {
                Debug("Getting user: " + user);
                var userObj = db.USERS.FirstOrDefault(u => u.USERNAME.Equals(user, StringComparison.CurrentCultureIgnoreCase));
                if (userObj != null && userObj.Authenticate(pass))
                {
                    Debug("Getting cabinet" + cabinetid);
                    var cabinet = db.CABINETS.Single(c => c.CABINETID.Equals(cabinetid, StringComparison.CurrentCultureIgnoreCase));
                    var template = new FileItTemplate();
                    template.ConvertFromTEMPLATE(cabinet.Template);
                    Debug("Getting template" + cabinet.Template);

                    response.Cabinet = new FileItCabinet()
                    {
                        CabinetId = cabinet.CABINETID,
                        CabinetName = cabinet.CABINETNAME,
                        Template = template
                    };
                    var result = cabinet.GetDocuments(ConvertLookups(lookups), includeThumbs);
                    result.ToList().ForEach(d =>
                    {
                        response.Documents.Add(new DTOs.FileItDocument()
                        {
                            FileName = d.FILENAME,
                            ImageBase64 = d.ImageBase64,
                            WebImageBase64 = d.WebImageBase64,
                            WebImageBase64Src = d.WebImageBase64Src,
                            WebImageThumbBase64 = d.WebImageThumbBase64,
                            WebImageThumbBase64Src = d.WebImageThumbBase64Src,
                            IndexInformation = d.Indexes
                        });
                    });
                }
            }


            return response;
        }


      

        public FileItResponse GetDocumentsById(string user, string pass, List<FileItDocumentIdLookup> documentIds, bool includeThumbs, bool includeDeleted)
        {
            var response = new FileItResponse();
            response.Documents = new List<DTOs.FileItDocument>();
            using (var db = new FileItDataLayer.Models.SystemFileitEntities())
            {
                var userObj = db.USERS.FirstOrDefault(u => u.USERNAME.Equals(user, StringComparison.CurrentCultureIgnoreCase));
                if (userObj != null && userObj.Authenticate(pass))
                {
                    Debug("Getting docs by id IN METHOD");
                    //load the lists
                    var cabinets = new Dictionary<string, List<string>>();
                    var currentCabinet = "";

                    documentIds.ToList().ForEach(d =>
                    {
                        if (!cabinets.ContainsKey(d.CabinetId))
                        {
                            cabinets.Add(d.CabinetId, new List<string>());
                        }
                        cabinets[d.CabinetId].Add(d.DocumentId);
                    });

                    Debug("Looping Cabinets" + cabinets.Count.ToString());
                    cabinets.ToList().ForEach(c =>
                    {
                        Debug("PROCESSING CABINET:" + c.Key + " ITEMS=" + c.Value.Count);
                        Debug("******************");
                        currentCabinet = c.Key;
                        
                        var cabinet = db.CABINETS.Single(cab => cab.CABINETID.Equals(c.Key, StringComparison.CurrentCultureIgnoreCase));
                        if (response.Cabinet == null)
                        {
                            var template = new FileItTemplate();
                            template.ConvertFromTEMPLATE(cabinet.Template);
                            response.Cabinet = new FileItCabinet()
                            {
                                CabinetId = cabinet.CABINETID,
                                CabinetName = cabinet.CABINETNAME,
                                Template = template
                            };
                        }
                        Debug("About to get documents: " + cabinet.CABINETNAME + " " + c.Value.Count);
                        //get the documents
                        cabinet.GetDocumentsByIds(c.Value, includeThumbs,includeDeleted).ToList().ForEach(doc =>
                        {
                            Debug("in loop adding docs to response COUNT=" + response.Documents.Count);
                            response.Documents.Add(new DTOs.FileItDocument()
                            {
                                FileName = doc.FILENAME,
                                ImageBase64 = doc.ImageBase64,
                                WebImageBase64 = doc.WebImageBase64,
                                WebImageBase64Src = doc.WebImageBase64Src,
                                WebImageThumbBase64 = doc.WebImageThumbBase64,
                                WebImageThumbBase64Src = doc.WebImageThumbBase64Src,
                                IndexInformation = doc.Indexes
                            });
                        });
                        Debug("Responses loaded: " + response.Documents.Count());
                        response.Success = true;
                    });
                }
            }

            return response;
        }

        private List<FileItDataLayer.Models.FileItDocumentLookup> ConvertLookups(List<DTOs.FileItDocumentLookup> lookups)
        {
            var result = new List<FileItDataLayer.Models.FileItDocumentLookup>();
            if (lookups != null && lookups.Count > 0)
            {
                lookups.ForEach(l =>
                {
                    result.Add(new FileItDataLayer.Models.FileItDocumentLookup()
                    {
                        Operator = l.Operator,
                        IndexNumber = l.IndexNumber,
                        LookupValue = l.LookupValue
                    });
                });
            }

            return result;
        }

        public FileItResponse UploadDocuments(string user, string pass, string cabinetId, DTOs.FileItDocument[] documents)
        {
            var response = new FileItResponse();
            response.FileNameFileItID = new Dictionary<string, string>();

            using (var db = new FileItDataLayer.Models.SystemFileitEntities())
            {
                Debug("Uploading to cabinet: " + cabinetId + " " + documents.Length.ToString() + " docs");
                var cab = db.CABINETS.Single(c => c.CABINETID.Equals(cabinetId, StringComparison.CurrentCultureIgnoreCase));
                response.Success = true;
                documents.ToList().ForEach(d =>
                {
                    var filename = FileItDataLayer.Models.FileItDocument.GetNextFileName();
                    Debug("Got filename: " + filename);
                    var doc = new FileItDataLayer.Models.FileItDocument()
                    {
                        FILENAME = filename,
                        ARCHIVED = "N",
                        DELETED = false,
                        ImageBase64 = d.ImageBase64,
                        EXTENSION = System.IO.Path.GetExtension(d.FileName).Replace(".",""),
                        INDEXEDON = DateTime.Now,
                        PUBLIC = true,
                        VERSION_NO = 1,
                        VERSIONID = "",
                        USERID = user,
                        STATIONID = AppGlobal.StationID,
                        BATCHNO = "BATCH",
                        Indexes = d.IndexInformation
                    };
                    if (!cab.Add(doc))
                    {
                        Debug("Failed to add doc.");
                        response.Success = false;
                    }
                    else
                    {
                        //TODO: retain the doc id and return
                        response.FileNameFileItID.Add(d.FileName, filename);
                    }
                });
            }
            return response;
        }

        public void Debug(string message)
        {
            if (AppGlobal.DebugMode)
            {
                ExceptionHelper.LogError(message);
            }
        }
    }
}
