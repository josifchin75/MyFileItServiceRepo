using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MyFileItService
{
    public partial class ViewDocument : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            setupPage();
        }

        protected void setupPage()
        {
            var guid = Request.Params["g"].ToString();
            lblDocumentInfo.Text = guid;
            //check for data
            var svc = new MyFileItAppService();
            var result = svc.GetEmergencyShare(ConfigurationSettings.ServiceUser, ConfigurationSettings.ServicePass, guid);
            var i = 0;

            if (result.Success)
            {
                var d = result.Documents.First();
                imgMain.Visible = true;
                imgMain.ImageUrl = "data:image/png;base64," + d.Base64Image;
                lblDocumentInfo.Text = "<h2>" + d.FIRSTNAME + " " + d.LASTNAME + "</h2>" + "<h3>" + d.DocumentTypeName + "<h3>" + "<i>" + d.COMMENT + "</i><p></p>";
            }
            else
            {
                lblDocumentInfo.Text = "This document is invalid or expired.";
                imgMain.Visible = false;
            }
        }
    }
}