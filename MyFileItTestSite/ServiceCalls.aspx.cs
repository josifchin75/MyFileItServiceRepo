using MyFileItTestSite.MyFileIt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MyFileItTestSite
{
    public partial class ServiceCalls : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            txtUserName.Text = "josifchin75@gmail.com";
            txtPassword.Text = "jopass12";
        }

        protected void btnAddOrganization_Click(object sender, EventArgs e)
        {
            SaveOrganization();
        }

        public bool SaveOrganization()
        {
            try
            {
                var c = new MyFileIt.CoachDTO();

                MyFileIt.MyFileItAppServiceClient obj = new MyFileIt.MyFileItAppServiceClient();
                MyFileIt.MyFileItResult result = new MyFileIt.MyFileItResult();
                MyFileIt.OrganizationDTO objOrganization = new MyFileIt.OrganizationDTO();
                objOrganization = setdata(objOrganization);
                if (Session["Organization"] != null)
                {
                    objOrganization.ID = Convert.ToInt16(Session["Organization"]);
                    result = obj.UpdateOrganization(appGlobal.username, appGlobal.password, objOrganization);
                    if (result.Success)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    result = obj.AddOrganization(appGlobal.username, appGlobal.password, objOrganization);
                    if (result.Success)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }


            }
            catch (Exception ex)
            {
                appGlobal.DisplayMsg(ex.Message.ToString(), this);
            }
            return false;
        }

        public OrganizationDTO setdata(OrganizationDTO obj)
        {
            try
            {
                obj.NAME = "FullOrgCabinet";
                obj.ADDRESS1 = "840 Sullivan Drive";
                obj.ADDRESS2 = "";
                obj.CITY = "Lansdale";
                obj.STATECODE = "";
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

        protected void btnDebugAdd_Click(object sender, EventArgs e)
        {

            MyFileIt.MyFileItAppServiceClient obj = new MyFileIt.MyFileItAppServiceClient();
            MyFileIt.MyFileItResult result = new MyFileIt.MyFileItResult();
            var message = "";
            var id = 10;
            var res = obj.GetOrganizations(appGlobal.username, appGlobal.password, id, null);
            //var addedOrgDTO = res.Organizations.First();
            var test = obj.GetOrganizations(appGlobal.username, appGlobal.password, id, null).Organizations;
            var addedOrgDTO = obj.GetOrganizations(appGlobal.username, appGlobal.password, id, null).Organizations.First();
            var orgDTO = new OrganizationDTO
            {
                ID = addedOrgDTO.ID, //THIS IS IMPORTANT FOR THE UPDATED TO WORK!!!
                NAME = "FullOrgCabinet",
                ADDRESS1 = "840 Sullivan Drive",
                ADDRESS2 = addedOrgDTO.ADDRESS2,
                CITY = "Lansdale test",
                STATECODE = addedOrgDTO.STATECODE,
                ZIPCODE = "19442",
                PHONE = "2151234567",
                EMAILADDRESS = "jo@mail.com",
                ORGANIZATIONTYPEID = 1,
                CONTACTPERSON = "Contact name",
                DATECREATED = addedOrgDTO.DATECREATED,
                ORGANIZATIONSTATUSID = 1,
                SALESREPID = 1,
                DIRECTORNAME = "Director Name",
                DIRECTOREMAIL = "Director@email.com",
                DIRECTORPHONE = "33344455555",
                ALLOWCOACHTOCREATEEVENTS = addedOrgDTO.ALLOWCOACHTOCREATEEVENTS,
                CCALLEMAILTODIRECTOR = addedOrgDTO.CCALLEMAILTODIRECTOR,
                CABINETID = addedOrgDTO.CABINETID,
                COMMENT = "comments",
                LOGOIMAGE = addedOrgDTO.LOGOIMAGE

            };
            var responseNew = obj.UpdateOrganization(appGlobal.username, appGlobal.password, orgDTO);
            if (responseNew.Success)
            {
                message = "Successful Update";
            }
            else
            {
                message = "Error on Update user";
            }
            var io = 0;
        }

        protected void btnSalesReps_Click(object sender, EventArgs e)
        {
            MyFileIt.MyFileItAppServiceClient obj = new MyFileIt.MyFileItAppServiceClient();
            MyFileIt.MyFileItResult result = new MyFileIt.MyFileItResult();
            var message = "";
            var res = obj.GetSalesReps(appGlobal.username, appGlobal.password, null, "", null);
            res.SalesReps.ToList().ForEach(sr =>
            {
                message += sr.FIRSTNAME + sr.LASTNAME;
            });
            lblMessage.Text = message;
        }

        protected void btnGetUsers_Click(object sender, EventArgs e)
        {
            MyFileIt.MyFileItAppServiceClient obj = new MyFileIt.MyFileItAppServiceClient();
            MyFileIt.MyFileItResult result = new MyFileIt.MyFileItResult();
            var message = "";
            var res = obj.GetAllAppUsers(appGlobal.username, appGlobal.password, 1);
            res.AppUsers.ToList().ForEach(au =>
            {
                message += au.USERNAME;
            });
            lblMessage.Text = message;
        }

        protected void btnAssociateUserToOrg_Click(object sender, EventArgs e)
        {
            MyFileIt.MyFileItAppServiceClient obj = new MyFileIt.MyFileItAppServiceClient();
            MyFileIt.MyFileItResult result = new MyFileIt.MyFileItResult();
            int appUserTypeId = 5;
            var message = "";
            var res = obj.AssociateAppUserToOrganization(appGlobal.username, appGlobal.password, 1, appUserTypeId, 2, DateTime.Now, DateTime.Now.AddDays(10), 2015, 1);

            lblMessage.Text = res.Success.ToString();
        }

        protected void btnRemoveUserOrg_Click(object sender, EventArgs e)
        {
            MyFileIt.MyFileItAppServiceClient obj = new MyFileIt.MyFileItAppServiceClient();
            MyFileIt.MyFileItResult result = new MyFileIt.MyFileItResult();
            var message = "";
            var res = obj.RemoveAppUserFromOrganization(appGlobal.username, appGlobal.password, 1, 2);
            lblMessage.Text = res.Success.ToString();
        }

        protected void btnAddRemoveOrganization_Click(object sender, EventArgs e)
        {
            MyFileIt.MyFileItAppServiceClient svc = new MyFileIt.MyFileItAppServiceClient();
            MyFileIt.MyFileItResult result = new MyFileIt.MyFileItResult();

            //get all the associations for the user
            int appUserIDLookup = 5; //this id is valid on my LOCAL DB, NOT server
            int newOrganizationID = 25;
            int oldOrganizationID = 21;
            DateTime startDate = DateTime.Now;
            DateTime expiresDate = DateTime.Now.AddDays(10);
            int yearCode = 2015;
            int sportTypeID = 2;
            int appUserTypeID = 5;
            AppUserDTO user = null;

            result = svc.GetAppUsers(appGlobal.username, appGlobal.password, appUserIDLookup, null);
            user = result.AppUsers.First();

            //loop all associated organization and remove specific organization
            //NOTE: I skipped all success / fail catch logic just to run the routine
            user.Organizations.Where(o => o.ID == oldOrganizationID || o.ID == newOrganizationID).ToList().ForEach(o =>
            {
                svc.RemoveAppUserFromOrganization(appGlobal.username, appGlobal.password, user.ID, o.ID);
            });

            //add the new organization link into the table
            result = svc.AssociateAppUserToOrganization(appGlobal.username, appGlobal.password, user.ID, appUserTypeID, newOrganizationID, startDate, expiresDate, yearCode, sportTypeID);
            lblMessage.Text = result.Success ? "Worked" : "Failed";
        }

        private void aftabTest()
        {
            MyFileIt.MyFileItAppServiceClient obj = new MyFileIt.MyFileItAppServiceClient();
            MyFileIt.MyFileItResult result = new MyFileIt.MyFileItResult();
            var message = "";

            int appUserIDLookup = 3;
            int newOrganizationID = 12;
            int oldOrganizationID = 10;
            DateTime startDate = DateTime.Now;
            DateTime expiresDate = DateTime.Now.AddDays(10);
            int yearCode = 2015;
            int sportTypeID = 2;
            int appUserTypeId = 5;
            AppUserDTO user = null;

            result = obj.GetAppUsers(appGlobal.username, appGlobal.password, appUserIDLookup, null);
            user = result.AppUsers.First();

            user.Organizations.Where(o => o.ID == oldOrganizationID || o.ID == newOrganizationID).ToList().ForEach(o =>
            {
                obj.RemoveAppUserFromOrganization(appGlobal.username, appGlobal.password, user.ID, o.ID);
            });

            //  add the new organization link into the table
            result = obj.AssociateAppUserToOrganization(appGlobal.username, appGlobal.password, user.ID, appUserTypeId, newOrganizationID, startDate, expiresDate, yearCode, sportTypeID);
            message = result.Success ? "Worked" : "Failed";
        }

        public void aftabTest2()
        {
            MyFileIt.MyFileItAppServiceClient obj = new MyFileIt.MyFileItAppServiceClient();
            MyFileIt.MyFileItResult result = new MyFileIt.MyFileItResult();

            result = obj.GetCoaches(appGlobal.username, appGlobal.password, null, "", 3, 12);

            lblMessage.Text = result.Success.ToString();

        }

        protected void btnTestAssociateOrg_Click(object sender, EventArgs e)
        {
            aftabTest2();
        }

        protected void btnLoginUserTest_Click(object sender, EventArgs e)
        {
            MyFileIt.MyFileItAppServiceClient obj = new MyFileIt.MyFileItAppServiceClient();
            MyFileIt.MyFileItResult result = new MyFileIt.MyFileItResult();
            var message = "";
            var res = obj.LoginAppUser(appGlobal.username, appGlobal.password, "admin@admin.com", "Admin1234");
            lblMessage.Text = res.Success.ToString();
        }

        protected void btnRemoveCoachFromUser_Click(object sender, EventArgs e)
        {
            MyFileIt.MyFileItAppServiceClient obj = new MyFileIt.MyFileItAppServiceClient();
            MyFileIt.MyFileItResult result = new MyFileIt.MyFileItResult();
            var message = "";
            var res = obj.RemoveCoachFromCurrentAppUser(appGlobal.username, appGlobal.password, 1);
            lblMessage.Text = res.Success.ToString();
        }

        protected void btnAddPaymentHistory_Click(object sender, EventArgs e)
        {
            MyFileItAppServiceClient obj = new MyFileItAppServiceClient();
            MyFileItResult result = new MyFileItResult();
            PaymentHistoryDTO objPayment = new PaymentHistoryDTO();

            objPayment.PRIMARYAPPUSERID = 2;
            objPayment.DATEPURCHASED = DateTime.UtcNow;
            objPayment.QUANTITY = 1;
            objPayment.ITEM = "ShareKey";
            objPayment.AMOUNT = Convert.ToDecimal("3.99"); ;
            objPayment.LAST4CC = "1111";
            objPayment.PROMOCODE = "";
            objPayment.AUTHORIZATIONNUMBER = "0000";
            //objPayment.NAME = "Test Name";

            result = obj.AddPaymentHistory(appGlobal.username, appGlobal.password, objPayment);
        }

        protected void btnGetTeamEventsText_Click(object sender, EventArgs e)
        {
            MyFileItAppServiceClient obj = new MyFileItAppServiceClient();
            MyFileItResult result = new MyFileItResult();
            result = obj.GetTeamEvents(appGlobal.username, appGlobal.password, null, null, "2016");
            var i = 0;
        }

        protected void btnShareKeyUpdate_Click(object sender, EventArgs e)
        {
            MyFileIt.MyFileItAppServiceClient obj = new MyFileIt.MyFileItAppServiceClient();
            MyFileIt.MyFileItResult result = new MyFileIt.MyFileItResult();
            FileCabinetDocumentDTO objf = new FileCabinetDocumentDTO();
            ShareKeyDTO objKey = new ShareKeyDTO();
            AppUserDTO user = new AppUserDTO();

            List<AppUserDTO> objUsers = new List<AppUserDTO>();
            result = obj.GetAppUsers(appGlobal.username, appGlobal.password, 2, "");
            user = result.AppUsers.First();
            user.SHAREKEYEXPIREDATE = DateTime.Now.AddDays(365);
            result = obj.UpdateAppUser(appGlobal.username, appGlobal.password, user);

            objKey.ID = 10;
            objKey.APPUSERID = null;
            objKey.AMOUNT = Convert.ToDecimal("3.99");
            objKey.DATECREATED = DateTime.Now;
            objKey.LAST4CC = "0002";
            objKey.PAYMENTTYPEID = 1;
            objKey.PRIMARYAPPUSERID = user.ID;
            objKey.PROMOCODE = "";
            objKey.PURCHASEDATE = DateTime.Now;
            objKey.SALESREPID = 1;
            objKey.SHAREKEYCODE = "20151022-0001";
            objKey.ApplicationUser = user;
            result = obj.UpdateShareKey(appGlobal.username, appGlobal.password, objKey);
        }

        protected void btnGetCoaches_Click(object sender, EventArgs e)
        {
            MyFileIt.MyFileItAppServiceClient obj = new MyFileIt.MyFileItAppServiceClient();
            MyFileIt.MyFileItResult result = new MyFileIt.MyFileItResult();
            var organizationId = 1;

            result = obj.GetCoachesByOrganizationId(appGlobal.username, appGlobal.password, organizationId);
            //these are all the coaches for that organization
            result.AppUsers.Count();
        }

        protected void btnGetdocumentThumbs_Click(object sender, EventArgs e)
        {
            MyFileIt.MyFileItAppServiceClient obj = new MyFileIt.MyFileItAppServiceClient();
            MyFileIt.MyFileItResult result = new MyFileIt.MyFileItResult();
            var idList = new List<int?>() { 59, 60 };
            result = obj.GetAppUserDocumentsThumbs(appGlobal.username, appGlobal.password, idList.ToArray());

            lblMessage.Text = "Count =" + result.Documents.Count();
        }

        protected void btnSendCoachEmail_Click(object sender, EventArgs e)
        {
            MyFileIt.MyFileItAppServiceClient obj = new MyFileIt.MyFileItAppServiceClient();
            MyFileIt.MyFileItResult result = new MyFileIt.MyFileItResult();
            var emailAddress = "josifchin75@gmail.com";
            string[] arr = new string[] { };
            result = obj.SendAddCoachEmail(appGlobal.username, appGlobal.password, emailAddress, "Subject", "This is the message", arr);
            lblMessage.Text = "Sent email:" + result.Success.ToString();
            ;
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            MyFileIt.MyFileItAppServiceClient obj = new MyFileIt.MyFileItAppServiceClient();
            MyFileIt.MyFileItResult result = new MyFileIt.MyFileItResult();
            result = obj.LoginAppUser(appGlobal.username, appGlobal.password, txtUserName.Text, txtPassword.Text);
            var show = result.AppUsers.First().ShowAds;
            lblMessage.Text = result.Success.ToString();
        }

    }
}