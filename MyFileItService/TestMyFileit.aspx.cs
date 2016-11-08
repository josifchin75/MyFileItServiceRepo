using MyFileItDataLayer.Models;
using MyFileItService.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MyFileItService
{
    public partial class TestMyFileit : System.Web.UI.Page
    {
        public string SERVICEUSER = "admin";
        public string SERVICEPASS = "admin";
        public string base64Image = ConfigurationSettings.base64TestString;



        protected void Page_Load(object sender, EventArgs e)
        {
            lblMessage.Text = "";
            if (!IsPostBack)
            {
                LoadDrops();
            }
        }

        private void LoadDrops()
        {
            var svc = new MyFileItAppService();
            var result = svc.GetOrganizations(SERVICEUSER, SERVICEPASS, null, null);
            ddlOrganizations.Items.Clear();
            result.Organizations.ForEach(o =>
            {
                ddlOrganizations.Items.Add(new ListItem(o.NAME + " - " + o.ID.ToString(), o.ID.ToString()));
            });

            result = svc.GetTeamEvents(SERVICEUSER, SERVICEPASS, 1, null, null);
            ddlTeamEvents.Items.Clear();
            result.TeamEvents.ForEach(te =>
            {
                ddlTeamEvents.Items.Add(new ListItem(te.NAME, te.ID.ToString()));
            });
            /*
            result = svc.GetTeamEventPlayerRosters(SERVICEUSER, SERVICEPASS, null, int.Parse(ddlTeamEvents.SelectedValue));
            ddlTeamEventPlayerRosters.Items.Clear();
            result.TeamEventPlayerRosters.ForEach(ter =>
            {
                ddlTeamEventPlayerRosters.Items.Add(new ListItem(ter.ID.ToString()));
            });
            */
            if (ddlOrganizations.SelectedValue != "")
            {
                result = svc.GetAllAppUsers(SERVICEUSER, SERVICEPASS, int.Parse(ddlOrganizations.SelectedValue));
                ddlAppUsers.Items.Clear();
                result.AppUsers.ForEach(u =>
                {
                    ddlAppUsers.Items.Add(new ListItem(u.USERNAME, u.ID.ToString()));
                });
            }
        }

        protected void btnAuth_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var b = svc.Authenticate("admin", "admin");
            lblMessage.Text = b.Success ? "TRUE" : "FALSE";
            lblMessage.Text += b.Message;
        }

        protected void btnCabs_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var b = svc.GetCabinets("admin", "admin", "admin", true);
            if (b.Count > 0)
            {
                b.ToList().ForEach(c => lblMessage.Text += "<br/>" + c.Key + "=" + c.Value);
            }
        }

        protected void btnDocs_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var d = svc.GetDocuments("admin", "admin", "houseofrep4101", null, false);
            //var d = svc.GetFamilyDocuments("admin","admin",)
            var i = 0;
        }

        protected void btnStatusTable_Click(object sender, EventArgs e)
        {
            var tablename = ddlReferenceTables.Value;
            lblMessage.Text = "";
            var svc = new MyFileItAppService();
            var response = svc.GetReferenceData(SERVICEUSER, SERVICEPASS, tablename);
            response.KeyValueData.ToList().ForEach(d =>
            {
                lblMessage.Text += d.Key + " = " + d.Value + "</br>";
            });
        }

        protected void btnRemoveOrganization_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var response = svc.RemoveOrganization(SERVICEUSER, SERVICEPASS, int.Parse(txtOrganizationId.Text));
            lblMessage.Text = "remove = " + response.Success.ToString();
        }

        protected void btnAddSharekeys_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();

            var response = svc.AddShareKey(SERVICEUSER, SERVICEPASS, 1, DateTime.Now, "PRMO", "1234", 100, 1, 2);
            lblMessage.Text = "Add Share key = " + response.Success;
        }

        protected void btnAddOrganization_Click(object sender, EventArgs e)
        {
            SaveOrganization();
        }

        public bool SaveOrganization()
        {
            try
            {
                var svc = new MyFileItAppService();
                var result = new MyFileItResult();
                var objOrganization = new OrganizationDTO();
                objOrganization = setdata(objOrganization);

                result = svc.AddOrganization(SERVICEUSER, SERVICEPASS, objOrganization);
                lblMessage.Text = result.Success ? "ADDED ORGANIZATION" : "ERROR ADDING ORGANIZATION";
            }
            catch (Exception ex)
            {
                // appGlobal.DisplayMsg(ex.Message.ToString(), this);
            }
            return false;
        }

        public OrganizationDTO setdata(OrganizationDTO obj)
        {
            try
            {
                obj.NAME = "NewTestAdditionalCabinetID";
                obj.ADDRESS1 = "840 Sullivan Drive";
                obj.ADDRESS2 = "";
                obj.CITY = "Lansdale";
                obj.STATECODE = "PA";
                obj.ZIPCODE = "19442";
                obj.PHONE = "2151234567";
                obj.EMAILADDRESS = "jo@mail.com";
                obj.ORGANIZATIONTYPEID = 1;//Convert.ToInt16(ddlOrganizationType.SelectedValue);
                obj.CONTACTPERSON = "Contact name";
                obj.DATECREATED = DateTime.UtcNow;
                obj.ORGANIZATIONSTATUSID = 1;
                obj.SALESREPID = 1;
                obj.DIRECTORNAME = "Director Name";
                obj.DIRECTOREMAIL = "Director@email.com";
                obj.DIRECTORPHONE = "33344455555";
                obj.ALLOWCOACHTOCREATEEVENTS = Convert.ToString(chkAllowCoach.Checked);
                obj.CCALLEMAILTODIRECTOR = Convert.ToString(chkCCEmail.Checked);
                obj.CABINETID = "0";
                obj.COMMENT = "comments";

            }
            catch (Exception ex)
            {

            }
            return obj;
        }

        protected void btnGetOrg_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var response = svc.GetOrganizations(SERVICEUSER, SERVICEPASS, null, txtOrganizationLookup.Text);
            var message = "";
            if (response.Success)
            {
                response.Organizations.ToList().ForEach(o =>
                {
                    message += o.NAME + "</br>";
                });
            }
            else
            {
                message = "Error on lookup";
            }
            lblMessage.Text = message;
        }

        protected void btnUpdateOrganization_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var response = svc.GetOrganizations(SERVICEUSER, SERVICEPASS, null, null);
            var message = "";
            var org = response.Organizations.First();
            org.ADDRESS1 += "++";
            /*OrganizationDTO dto = new OrganizationDTO()
            {
                ID = org.ID,
                NAME = org.NAME,
                ADDRESS1 = org.ADDRESS1,
                ADDRESS2 = org.ADDRESS2,
                CITY = org.CITY,
                STATECODE = org.STATECODE,
                ZIPCODE = org.ZIPCODE,
                EMAILADDRESS = org.EMAILADDRESS,
                ALLOWCOACHTOCREATEEVENTS = "False",

                CCALLEMAILTODIRECTOR = "True"
            };*/
            var result = svc.UpdateOrganization(SERVICEUSER, SERVICEPASS, org);
            message = result.Success.ToString() + " = " + result.Message;
            lblMessage.Text = message;
        }

        protected void btnGetUser_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();

            //var result3= svc.GetFamilyUsers(SERVICEUSER, SERVICEPASS, 21);
            string namelookup = "tes";

            var result4 = svc.GetCoachMembers(SERVICEUSER, SERVICEPASS, 21, null, null, namelookup, null);

            var result = svc.GetAppUsersByEmail(SERVICEUSER, SERVICEPASS, txtUsername.Text, txtEmail.Text, txtPassword.Text);
            var message = "";

            if (result.Success)
            {
                result.AppUsers.ForEach(a =>
                {
                    message = a.USERNAME + " = " + a.ID.ToString();
                });
                var primaryAppUserId = result.AppUsers.First().ID;
                var result2 = svc.GetFamilyUsers(SERVICEUSER, SERVICEPASS, primaryAppUserId);
                message += result2.Success;
            }
            else
            {
                message = "FAIL";
            }

            lblMessage.Text = message;
        }

        protected void btnAddAppUser_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var username = DateTime.Now.Ticks.ToString();
            var appUserObject = new AppUserDTO()
            {
                USERNAME = username,
                PASSWORD = "pass",
                FIRSTNAME = "first",
                LASTNAME = "last",
                ADDRESS1 = "add1",
                CITY = "cit",
                STATECODE = "PA",
                ZIPCODE = "19442",
                SEX = "M",
                EMAILADDRESS = "josifchin75@gmail.com",
                PRIMARYAPPUSERID = (chkIsPrimaryUser.Checked ? null : (int?)36)
            };
            var response = svc.AddAppUser(SERVICEUSER, SERVICEPASS, appUserObject);
            var message = "";
            if (response.Success)
            {
                var addedUser = svc.GetAppUsersByEmail(SERVICEUSER, SERVICEPASS, username, null, "pass").AppUsers.First();
                /*var addedUserDTO = new APPUSER
                {
                    ID = addedUser.ID, //THIS IS IMPORTANT FOR THE UPDATED TO WORK!!!
                    USERNAME = username,
                    PASSWORD = "pass",
                    FIRSTNAME = "first",
                    LASTNAME = "last",
                    ADDRESS1 = "add1",
                    CITY = "cit UPDATED",
                    STATECODE = "PA",
                    ZIPCODE = "19442",
                    SEX = "M",
                    PRIMARYAPPUSERID = 1
                };*/
                addedUser.CITY = "UPDATED CITY " + DateTime.Now.ToString();
                svc.UpdateAppUser(SERVICEUSER, SERVICEPASS, addedUser);
                message = "Successful Add";
            }
            else
            {
                message = "Error on Add user";
            }
            lblMessage.Text = message;
        }


        protected void btnUpload_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var cabDoc = new FileCabinetDocumentDTO()
            {
                APPUSERID = 9,
                SCANDATE = DateTime.Now,
                FIRSTNAME = "fff",
                LASTNAME = "lll",
                DOCUMENTTYPEID = 1,
                DOCUMENTDATE = DateTime.Now,
                //VERIFIEDDATE = DateTime.Now,
                //VERIFIEDAPPUSERID = 5,
                DOCUMENTSTATUSID = 1
            };
            var filename = "test.jpg";

            var result = svc.UploadFileCabinetDocument(SERVICEUSER, SERVICEPASS, 9, filename, base64Image, cabDoc);
            var message = "";

            if (result.Success)
            {
                message = "SUCCESS";
            }
            else
            {
                message = "FAIL";
            }

            lblMessage.Text = message;
        }

        protected void btnGetAllUsers_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var message = "";

            var result = svc.GetAllAppUsers(SERVICEUSER, SERVICEPASS, null);
            if (result.Success)
            {
                result.AppUsers.ToList().ForEach(u => message += u.USERNAME + "</br>");
            }
            else
            {
                message = "ERROR CALLING SERVIC";
            }

            lblMessage.Text = message;
        }

        protected void btnError_Click(object sender, EventArgs e)
        {
            throw new Exception("ERROR HAS OCCURRED");
        }

        protected void btnGetAppUSerMain_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var message = "";
            var nameLookup = "first";

            var result = svc.GetAppUsers(SERVICEUSER, SERVICEPASS, null, nameLookup);
            if (result.Success)
            {
                result.AppUsers.ToList().ForEach(u =>
                {
                    message += u.USERNAME + "</br>";
                    u.Organizations.ToList().ForEach(o =>
                    {
                        message += "-----" + o.NAME + "</br>";
                    });
                });
            }
            else
            {
                message = "ERROR CALLING SERVIC";
            }

            lblMessage.Text = message;
        }

        protected void btnAddSalesRep_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var message = "";
            var result = svc.AddSalesRep(SERVICEUSER, SERVICEPASS, new SalesRepDTO()
             {
                 FIRSTNAME = "Sales",
                 LASTNAME = "Rep",
                 ADDRESS1 = "108 Cornwall Drive",
                 CITY = "Chalfont",
                 STATECODE = "PA",
                 ZIPCODE = "18914",
                 SALESREPSTATUSID = 1,
                 EMAILADDRESS = "sales@me.com",
                 PASSWORD = "newpass"
             });
            message = result.Success ? "Added SalesRep" : "Error" + result.Message;
            lblMessage.Text = message;
        }

        protected void btnRemoveSalesRep_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var message = "";
            var result = svc.RemoveSalesRep(SERVICEUSER, SERVICEPASS, 2, 1);
            message = result.Success ? "Removed SalesRep" : "Error" + result.Message;
            lblMessage.Text = message;

        }

        protected void btnAddTeamEvent_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var message = "";
            var tevent = new TeamEventDTO()
            {
                NAME = txtTeamEventName.Text,
                ORGANIZATIONID = int.Parse(ddlOrganizations.SelectedValue),
                STARTDATE = DateTime.Now,
                EXPIRESDATE = DateTime.Now.AddDays(1),
                YEARCODE = 2015
            };
            var result = svc.AddTeamEvent(SERVICEUSER, SERVICEPASS, tevent);
            message = result.Success ? "Added Team Event" : "Error" + result.Message;
            lblMessage.Text = message;
        }

        protected void btnUpdateTeamEvent_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var message = "";
            var tevent = new TeamEventDTO()
            {
                ID = 3,
                NAME = txtTeamEventName.Text,
                ORGANIZATIONID = 37,
                STARTDATE = DateTime.Now,
                EXPIRESDATE = DateTime.Now.AddDays(1),
                YEARCODE = 2015
            };
            var result = svc.UpdateTeamEvent(SERVICEUSER, SERVICEPASS, tevent);
            message = result.Success ? "Updated Team Event" : "Error" + result.Message;
            lblMessage.Text = message;
        }

        protected void btnAddTeamEventDocument_Click(object sender, EventArgs e)
        {

            var svc = new MyFileItAppService();
            var message = "";
            var tevent = new TeamEventDocumentDTO()
            {
                DOCUMENTNAME = txtTeamEventDocumentName.Text,
                TEAMEVENTID = int.Parse(ddlTeamEvents.SelectedValue),
                SCANNAME = "scan name",
                VERIFIERNAME = "verifier name",
                PLAYERENTERDOCUMENTDATE = DateTime.Now,
                PLAYERENTEREXPIRATION = DateTime.Now.AddDays(10),
                TEAMEVENTDOCUMENTSTATUSID = 4,
                ENTERWHOSCANNED = "true",
                MUSTBEVERIFIED = "true",
                ISCOACHDOCUMENT = "true",
                ROSTERCLOSEDATE = DateTime.Now.AddDays(30)

            };
            var result = svc.AddTeamEventDocument(SERVICEUSER, SERVICEPASS, tevent);
            message = result.Success ? "Added Team Event Document" : "Error" + result.Message;
            lblMessage.Text = message;
        }
        protected void btnUpdateTeamEventDocument_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var message = "";
            var tevent = new TeamEventDocumentDTO()
            {
                ID = 1,
                DOCUMENTNAME = txtTeamEventDocumentName.Text + " UPDATED",
                TEAMEVENTID = 1,
                SCANNAME = "scan name",
                VERIFIERNAME = "verifier name",
                PLAYERENTERDOCUMENTDATE = DateTime.Now,
                PLAYERENTEREXPIRATION = DateTime.Now.AddDays(10),
                TEAMEVENTDOCUMENTSTATUSID = 4,
                ENTERWHOSCANNED = "true",
                MUSTBEVERIFIED = "true"
            };
            var result = svc.UpdateTeamEventDocument(SERVICEUSER, SERVICEPASS, tevent);
            message = result.Success ? "Updated Team Event Document" : "Error" + result.Message;
            lblMessage.Text = message;
        }

        protected void btnAddTeamEventPlayerRoster_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var message = "";
            var teventRoster = new TeamEventPlayerRosterDTO()
            {
                TEAMEVENTID = int.Parse(ddlTeamEvents.SelectedValue),
                APPUSERID = int.Parse(ddlAppUsers.SelectedValue),
                PLAYERPOSITION = "Position",
                JERSEYNUMBER = 10,
                USERSTAGETYPEID = 1
            };
            var result = svc.AddTeamEventPlayerRoster(SERVICEUSER, SERVICEPASS, teventRoster);
            message = result.Success ? "Added Team Event Player Roster" : "Error" + result.Message;
            lblMessage.Text = message;
        }

        protected void btnUpdateTeamEventPlayerRoster_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var message = "";

            var test = svc.GetTeamEventPlayerRosters(SERVICEUSER, SERVICEPASS, null, 3);
            return;
            var resultPlayer = svc.GetTeamEventPlayerRosters(SERVICEUSER, SERVICEPASS, int.Parse(ddlTeamEventPlayerRosters.SelectedValue), null).TeamEventPlayerRosters.First();

            resultPlayer.JERSEYNUMBER = resultPlayer.JERSEYNUMBER + 1;

            var result = svc.UpdateTeamEventPlayerRoster(SERVICEUSER, SERVICEPASS, resultPlayer);
            message = result.Success ? "Updated Team Event Roster" : "Error" + result.Message;
            lblMessage.Text = message;
        }

        protected void btnGetShareKeys_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var message = "";

            var result = svc.GetShareKeys(SERVICEUSER, SERVICEPASS, 1);
            result.ShareKeys.ToList().ForEach(s =>
            {
                lblMessage.Text += s.ID.ToString() + " " + (s.PROMOCODE ?? "NULL") + "<br/>";
            });
            //lblMessage.Text = message;
        }

        protected void btnGetAvailableSharekeys_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var message = "";
            var primaryAppUserId = 1;// 9;
            var promocode = "test";
            var result = svc.GetAvailableShareKeysByPromoCodeAndPrimaryUser(SERVICEUSER, SERVICEPASS, primaryAppUserId, promocode);
            result.ShareKeys.ToList().ForEach(s =>
            {
                lblMessage.Text += s.ID.ToString() + " " + (s.PROMOCODE ?? "NULL") + "<br/>";
            });
        }

        protected void btnGetFamilyDocs_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var message = "";
            var appuserid = 10;
            var teamEventId = 5;
            //var result = svc.GetFamilyDocuments(SERVICEUSER, SERVICEPASS, 1);
            var result = svc.GetAppUserDocuments(SERVICEUSER, SERVICEPASS, appuserid, teamEventId, new List<int?>(), true);
            lblMessage.Text = result.Success.ToString();
        }

        protected void btnGetDocsSpeedup_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var message = "";
            var appuserid = 10;
            int? teamEventId = 5;//null;//3;
            //var result = svc.GetFamilyDocuments(SERVICEUSER, SERVICEPASS, 1);
            var result = svc.GetAppUserDocumentsList(SERVICEUSER, SERVICEPASS, appuserid, teamEventId, new List<int?>());
            result = svc.GetAppUserDocumentsListNoImages(SERVICEUSER, SERVICEPASS, appuserid, teamEventId, new List<int?>());
            var doc = result.Documents.First();
            var result2 = svc.GetSingleDocument(SERVICEUSER, SERVICEPASS, new FileCabinetDocumentSingleDTO()
            {
                FILECABINETDOCUMENTID = doc.ID,
                TEAMEVENTDOCUMENTID = doc.TeamEventDocumentId,
                VerifiedAppUserId = doc.VerifiedAppUserId
            });

            lblMessage.Text = result.Success.ToString();
        }

        protected void btnGetAppUserOrgs_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var message = "";

            var result = svc.GetAppUserOrganizations(SERVICEUSER, SERVICEPASS, 6);
            lblMessage.Text = result.Organizations.Count.ToString() + " orgs found";
        }

        protected void btnGetTeamEvents_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var message = "";
            int? orgid = null;//14;
            int appUserId = 10;
            var result = svc.GetTeamEventsByAppUser(SERVICEUSER, SERVICEPASS, appUserId, orgid, null, "");
            lblMessage.Text = result.TeamEvents.Count.ToString() + " team events found";
        }

        protected void btnAssociateDocuments_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var message = "";
            int orgid = -1;//14;
            int appUserId = 10;
            int fileCabinetDocId = 1;
            int teamEventDocumentId = 1;// 7;
            var AssociateDocs = new List<AssociateDocumentDTO>();
            AssociateDocs.Add(new AssociateDocumentDTO()
            {
                appUserId = appUserId,
                fileCabinetDocumentId = fileCabinetDocId,
                organizationId = orgid,
                remove = false,
                comment = "test",
                emergency = false,
                teamEventDocumentId = teamEventDocumentId
            });
            var result = svc.AssociateDocumentsToTeamEventDocuments(SERVICEUSER, SERVICEPASS, AssociateDocs);
            lblMessage.Text = result.Success.ToString();
        }

        protected void btnGetTeamEventsWithUploads_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var message = "";

            int teamEventId = 1;
            var result = svc.GetTeamEventPlayersWithUploads(SERVICEUSER, SERVICEPASS, teamEventId);
            lblMessage.Text = result.AppUsers.Count.ToString() + " orgs found";
        }

        protected void btnGetAppUserFullLookup_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var message = "";
            var appUserId = 20;
            var teamEventId = 5;
            var firstName = "Jo";
            var lastName = "";
            string email = null;
            var sex = "";

            var result = svc.GetAppUsersByNameSexEmail(SERVICEUSER, SERVICEPASS, appUserId, teamEventId, firstName, lastName, email, sex);
            if (result.Success)
            {
                result.AppUsers.ToList().ForEach(u => message += u.USERNAME + "</br>");
            }
            else
            {
                message = "ERROR CALLING SERVIC";
            }

            lblMessage.Text = message;
        }

        protected void btnUpdateOrg_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var organizationId = 15;
            string nameLookup = null;
            var result = svc.GetOrganizations(SERVICEUSER, SERVICEPASS, organizationId, nameLookup);
            if (result.Success)
            {
                var org = result.Organizations.First();
                org.NAME += "..";
                result = svc.UpdateOrganization(SERVICEUSER, SERVICEPASS, org);
                lblMessage.Text = result.Success ? "Success" : "FAIL";
                //result.AppUsers.ToList().ForEach(u => message += u.USERNAME + "</br>");
            }
        }

        protected void btnTeamEventsByCoach_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var message = "";
            //
            int appUserid = 102;
            var result = svc.GetTeamEventsByCoach(SERVICEUSER, SERVICEPASS, appUserid, null);
            lblMessage.Text = result.TeamEvents.Count.ToString() + " events found";
        }

        protected void btnVerifyDocument_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var message = "";

            int appUserid = 34;
            int docid = 4;
            int teamEventId = 2;
            var result = svc.VerifyDocument(SERVICEUSER, SERVICEPASS, docid, appUserid, teamEventId);
            lblMessage.Text = result.Success.ToString();
        }

        protected void btnAddOrgDebug_Click(object sender, EventArgs e)
        {
            var message = "";
            var username = "family3@gmail.com";
            var svc = new MyFileItAppService();
            var appUserObject = new AppUserDTO()
            {
                USERNAME = username,
                EMAILADDRESS = username,
                PASSWORD = "1234",
                FIRSTNAME = "first",
                LASTNAME = "last",
                ADDRESS1 = "ADDRESS1",
                CITY = "CITY",
                STATECODE = "PA",
                ZIPCODE = "19442",
                SEX = "M",
                PRIMARYAPPUSERID = 0,
                APPUSERTYPEID = 4,
                BIRTHDATE = null,
                PHONE = "1234",
                RELATIONSHIPTYPEID = 2,
                DATECREATED = DateTime.UtcNow,
                APPUSERSTATUSID = 2
            };

            var response = svc.AddAppUser(SERVICEUSER, SERVICEPASS, appUserObject);
            lblMessage.Text = response.Success.ToString();
        }

        protected void btnEmergencyShare_Click(object sender, EventArgs e)
        {
            var message = "";
            var appUserId = 34;
            var docs = new List<int>() { 21, 22, 23 };
            var emergencyEmailAddress = "josifchin75@gmail.com";
            var emailMessage = "Additional message";

            var svc = new MyFileItAppService();
            var response = svc.AddEmergencyShare(SERVICEUSER, SERVICEPASS, appUserId, docs.ToArray(), emergencyEmailAddress, emailMessage);
            lblMessage.Text = response.Success.ToString();

        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var userName = "josifchin75@gmail.com";//"coach@coach.com";//"admin@admin.com";
            var pass = "jopass12";//"coach12"; //"Admin1234";

            userName = "";
            pass = "";
            userName = "Johndoe@gmail.com";
            pass = "johndoe12";
            userName = "sbutcher1@gmail.com";
            pass = "sandy9451";
            // userName = "skbutcher1@yahoo.com";
            //pass = "alas";//"Sandy12";

            var result = svc.LoginAppUser(SERVICEUSER, SERVICEPASS, userName, pass);
            lblMessage.Text = result.Success.ToString();
        }

        protected void btnAddCoach_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var coachDto = new CoachDTO()
            {
                APPUSERID = 20,
                TEAMEVENTID = 1,
                ORGANIZATIONID = 1,
                FIRSTNAME = "first",
                LASTNAME = "last",
                ADDRESS1 = ".",
                CITY = ".",
                STATECODE = ".",
                ZIPCODE = ".",
                SEX = "M"
            };
            var appUserId = 13;
            var organizationId = 1;
            var startDate = DateTime.Now;
            var expiresDate = DateTime.Now.AddYears(1);
            var yearCode = 2016;
            var sportTypeId = 1;

            var result = svc.AddCoach(SERVICEUSER, SERVICEPASS, appUserId, organizationId, startDate, expiresDate, yearCode, sportTypeId);
            //svc.AddCoach(SERVICEUSER, SERVICEPASS, coachDto);
            lblMessage.Text = result.Success.ToString();
        }

        protected void btnGetCoachesByEvent_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var eventId = 1;
            var result = svc.GetCoachesByTeamEventId(SERVICEUSER, SERVICEPASS, eventId);
            lblMessage.Text = result.Success.ToString();
        }

        protected void btnGetCoachesByOrganization_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var organizationId = 1;
            var result = svc.GetCoachesByOrganizationId(SERVICEUSER, SERVICEPASS, organizationId);
            result.Coaches.ForEach(c =>
            {
                //THIS IS THE EMAIL ADDRESS
                var emailAddress = c.EMAILADDRESS;
            });
            lblMessage.Text = result.Success.ToString();
        }

        protected void btnRejectDocumentShare_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var appUserId = 9;
            var teamEventId = 1;
            var result = svc.RejectTeamEventDocumentShare(SERVICEUSER, SERVICEPASS, appUserId, teamEventId);
            lblMessage.Text = result.Success.ToString();
        }

        protected void btnCheckExists_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var username = "skbutcher1@yahoo.com";
            var result = svc.CheckAppUserExists(SERVICEUSER, SERVICEPASS, username);
            lblMessage.Text = "user exists" + result;
        }

        protected void btnGetEmails_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var username = "josifchin75@gmail.com";
            var result = svc.GetSentEmails(SERVICEUSER, SERVICEPASS, username);
            lblMessage.Text = result.EmailLogs.Count.ToString();
        }

        protected void btnRemoveCoach_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var coachId = 3;
            var appUserId = 19;

            var result = svc.RemoveCoachFromCurrentAppUser(SERVICEUSER, SERVICEPASS, coachId);
            result = svc.RemoveAppUser(SERVICEUSER, SERVICEPASS, appUserId);
            lblMessage.Text = result.Success.ToString();
            /*
             CoachId= 3
AppuserId=19
result = obj.RemoveCoachFromCurrentAppUser(appGlobal.username, appGlobal.password, 3);
 result = obj.RemoveAppUser(appGlobal.username, appGlobal.password, 19);*/
        }

        protected void btnDeleteDocument_Click(object sender, EventArgs e)
        {
            var cabinetId = "Osifchin_Jonathan61701";
            var documentId = "000000A8";

            var svc = new MyFileItAppService();
            var coachId = 3;
            var appUserId = 9;

            var result = svc.DeleteAppUserDocument(SERVICEUSER, SERVICEPASS, appUserId, documentId);
            lblMessage.Text = result.Success.ToString();
        }

        protected void btnResendAssociateEmail_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var teamEventId = 5;
            var appUserId = 36;

            var result = svc.ResendAssociatedDocuments(SERVICEUSER, SERVICEPASS, appUserId, teamEventId);
            lblMessage.Text = result.Success.ToString();
        }

        protected void btnGetDocListOnly_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var docs = new List<int?>();

            docs.Add(59);
            docs.Add(60);

            var result = svc.GetAppUserDocumentsThumbs(SERVICEUSER, SERVICEPASS, docs);
            lblMessage.Text = result.Success.ToString();
        }

        protected void btnLoginSalesRep_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var emailAddress = "josifchin75@gmail.com";
            var password = "1234";

            var result = svc.LoginSalesRep(SERVICEUSER, SERVICEPASS, emailAddress, password);
            lblMessage.Text = result.Success.ToString();
        }

        protected void btnGetFamilyUsers_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var appUserId = 134;

            var result = svc.GetFamilyUsers(SERVICEUSER, SERVICEPASS, appUserId);
            lblMessage.Text = result.Success.ToString();
        }

        protected void btnSendShareReminders_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var result = svc.RunMissingDocumentShareEmails(SERVICEUSER, SERVICEPASS);
            lblMessage.Text = result.Success.ToString();
        }

        protected void btnInit_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            var result = svc.InitService();
            lblMessage.Text = "Ran INIT";
        }




    }
}