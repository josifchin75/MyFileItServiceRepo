using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MyFileItService
{
    public partial class EmailTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblError.Text = "";

            if (!IsPostBack)
            {
                emailAddress.Text = "josifchin75@gmail.com";
                subjectLine.Text = "TEST EMAIL";
                message.Text = "This is the message";
            }
        }

        protected void btnSend_Click(object sender, EventArgs e)
        {
            string error = "";
            var result = MyFileItService.Helpers.EmailHelper.SendEmail(
                 emailAddress.Text,
                 new List<string>() { "test@c.com" },
                 new List<string>(),
                 subjectLine.Text,
                 message.Text,
                 true,
                 new List<string>(),
                 ref error
                 );
            lblError.Text = "Sent EMAIL = " + result.ToString() + " " + error;
        }

        protected void btnAsync_Click(object sender, EventArgs e)
        {
            string error = "";
            MyFileItService.Helpers.EmailHelper.SendEmailAsync(
                          emailAddress.Text,
                          new List<string>(),
                          new List<string>(),
                          subjectLine.Text,
                          message.Text,
                          true,
                          new List<string>(),
                          ref error
                          );

            lblError.Text = "PAST SEND";
        }
    }
}