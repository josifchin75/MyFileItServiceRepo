using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MyFileItService
{
    public partial class ReportTester : System.Web.UI.Page
    {
        public string SERVICEUSER = "admin";
        public string SERVICEPASS = "admin";

        protected void Page_Load(object sender, EventArgs e)
        {
            lblMessage.Text = "";
        }

        protected void btnTeamEventPlayerDocumentStatus_Click(object sender, EventArgs e)
        {
            var svc = new MyFileItAppService();
            int teamEventId = 5;

            var result = svc.GetEventPlayerStatusReport(SERVICEUSER, SERVICEPASS, teamEventId);
            lblMessage.Text = result.ReportData;
        }
    }
}