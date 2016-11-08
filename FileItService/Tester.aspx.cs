using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using FileItService.Helpers;
using FileItService.DTOs;

namespace FileItService
{
    public partial class Tester : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {


            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            this.lblError.Text = "";

            if (!IsPostBack)
            {
                txtUser.Text = "admin";
                txtPass.Text = "admin";
                txtTargetUser.Text = "admin";

                LoadAccessType();
                LoadCabinets();

            }
            LoadIndexDiv();
        }

        private void LoadAccessType()
        {
            ddlAccessType.Items.Add(new ListItem("User"));
            ddlAccessType.Items.Add(new ListItem("Group"));
        }

        private void LoadCabinets()
        {
            ddlCabinets.Items.Clear();
            var svc = new FileItService();
            var result = svc.GetCabinets(this.txtUser.Text, this.txtPass.Text, this.txtTargetUser.Text, true);
            foreach (var key in result.Keys)
            {
                ddlCabinets.Items.Add(new ListItem(key));
            }
        }

        protected void btnAuthenticate_Click(object sender, EventArgs e)
        {
            var svc = new FileItService();
            var result = svc.Authenticate(this.txtUser.Text, this.txtPass.Text);
            this.lblError.Text = result.Success ? "TRUE" : "FALSE";
        }

        protected void btnGetCabinets_Click(object sender, EventArgs e)
        {
            var svc = new FileItService();
            var result = svc.GetCabinets(this.txtUser.Text, this.txtPass.Text, this.txtTargetUser.Text, false);
            var s = "";
            foreach (var key in result.Keys)
            {
                s += key + " = " + result[key] + "<br/>";
                ddlCabinets.Items.Add(new ListItem(key));
            }
            this.lblError.Text = s;
        }

        protected void btnAddRemoveAccess_Click(object sender, EventArgs e)
        {
            var svc = new FileItService();
            var result = svc.SetCabinetAccess(txtUser.Text, txtPass.Text, ddlAccessType.SelectedValue, txtTargetUser.Text, ddlCabinets.SelectedValue, chkAllowAccess.Checked);

            this.lblError.Text = result.Success ? "TRUE" : "FALSE";
        }

        protected void btnEnableUser_Click(object sender, EventArgs e)
        {
            var svc = new FileItService();
            var result = svc.SetUserActive(txtUser.Text, txtPass.Text, txtTargetUser.Text, this.chkEnableUser.Checked);

            this.lblError.Text = result.Success ? "TRUE" : "FALSE";
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            var svc = new FileItService();
            var result = svc.SetUserPassword(txtUser.Text, txtPass.Text, txtTargetUser.Text, txtNewPassword.Text);

            this.lblError.Text = result.Success ? "TRUE" : "FALSE";
        }

        protected void btnAddUser_Click(object sender, EventArgs e)
        {
            var svc = new FileItService();
            var obj = new FileItUser();
            obj.UserName = "TEST";
            var s = obj.GetPropertyValue("UserName");
            obj.SetPropertyValue("UserName", "CHANGED");
            var result = svc.AddUpdateUser(txtUser.Text, txtPass.Text, obj);
            this.lblError.Text = result.Success ? "TRUE" : "FALSE";
        }

        protected void btnReadDocs_Click(object sender, EventArgs e)
        {
            var cab = this.ddlCabinets.SelectedValue;
            var svc = new FileItService();
            var lookups = new List<FileItDocumentLookup>();
            /*lookups.Add(new FileItDocumentLookup()
            {
                IndexNumber = 2,
                LookupValue = "Test",
                Operator = "="
            });*/
            var result = svc.GetDocuments(txtUser.Text, txtPass.Text, cab, lookups, false);

            result.Documents.ForEach(d =>
            {
                System.Web.UI.HtmlControls.HtmlImage img = new System.Web.UI.HtmlControls.HtmlImage();
                img.Src = d.WebImageBase64Src;
                img.Attributes.Add("title", GenerateToolTip(d, result.Cabinet));
                panOutput.Controls.Add(img);
            });

        }

        private string GenerateToolTip(FileItDocument d, FileItCabinet c)
        {
            var sb = new System.Text.StringBuilder();
            d.IndexInformation.Keys.ToList().ForEach(k =>
            {
                sb.Append(c.Template.TemplateDefinitions.Single(td => td.INDEXNUM == k).INDEXNAME + " = " + d.IndexInformation[k] + Environment.NewLine);
            });
            return sb.ToString();
        }

        protected void btnGetCabinet_Click(object sender, EventArgs e)
        {
            var cab = this.ddlCabinets.SelectedValue;
            var svc = new FileItService();
            var lookups = new Dictionary<string, string>();
            var result = svc.GetCabinet(txtUser.Text, txtPass.Text, cab);
            this.lblError.Text = result.CabinetName;
        }

        protected void btnUploadImage_Click(object sender, EventArgs e)
        {
            var svc = new FileItService();
            var cab = svc.GetCabinet(this.txtUser.Text, this.txtPass.Text, ddlCabinets.SelectedValue);
            if (cab == null)
            {
                return;
            }
            var doc = new FileItDocument();
            doc.IndexInformation = new Dictionary<int, string>();
            doc.CabinetID = ddlCabinets.SelectedValue;

            cab.Template.TemplateDefinitions.ForEach(td =>
            {
                doc.IndexInformation.Add(td.INDEXNUM, Request["Index" + td.INDEXNUM.ToString()]);
            });
            var b64 = "";
            if ((FileUpload.PostedFile != null) && (FileUpload.PostedFile.ContentLength > 0))
            {
                b64 = Convert.ToBase64String(FileUpload.FileBytes);
            }

            doc.ImageBase64 = b64;
            doc.FileName = FileUpload.FileName;

            var docs = new FileItDocument[1];
            docs[0] = doc;

            svc.UploadDocuments(this.txtUser.Text, txtPass.Text, ddlCabinets.SelectedValue, docs);
            HtmlImage img = new HtmlImage();
            img.Src = "data:image/png;base64," + b64;
            panOutput.Controls.Add(img);

        }

        protected void ddlCabinets_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadIndexDiv();
        }

        private void LoadIndexDiv()
        {
            divIndexes.Controls.Clear();

            var svc = new FileItService();
            var cab = svc.GetCabinet(this.txtUser.Text, this.txtPass.Text, ddlCabinets.SelectedValue);
            if (cab == null)
            {
                return;
            }
            var h = new HtmlGenericControl("h3");
            h.InnerHtml = cab.CabinetName;
            divIndexes.Controls.Add(h);
            var tbl = new HtmlTable();
            cab.Template.TemplateDefinitions.ForEach(td =>
            {
                var tr = new HtmlTableRow();
                var td1 = new HtmlTableCell();
                var td2 = new HtmlTableCell();
                td1.InnerHtml = td.INDEXNAME;
                var txt = new HtmlInputText();
                txt.ID = "Index" + td.INDEXNUM.ToString();
                txt.Attributes.Add("Index", td.INDEXNUM.ToString());
                if (td.MAXLENGTH != null)
                {
                    txt.MaxLength = int.Parse(td.MAXLENGTH.ToString());
                }
                td2.Controls.Add(txt);
                tr.Controls.Add(td1);
                tr.Controls.Add(td2);
                tbl.Controls.Add(tr);
            });

            /* var trFile = new HtmlTableRow();
             var tdFile1 = new HtmlTableCell();
             var tdFile2 = new HtmlTableCell();
             tdFile1.InnerHtml = "Select File";
             var txtFile = new HtmlInputFile();
             txtFile.ID = "FileUpload";

             tdFile2.Controls.Add(txtFile);
             trFile.Controls.Add(tdFile1);
             trFile.Controls.Add(tdFile2);
             tbl.Controls.Add(trFile);*/

            divIndexes.Controls.Add(tbl);
        }

        protected void btnCreateCabinet_Click(object sender, EventArgs e)
        {
            var svc = new FileItService();

            var cablookup = "FullOrgCabinet59721";
            //cablookup = "houseofrep4101";
            var c = svc.GetCabinet(txtUser.Text, txtPass.Text, cablookup);
            var ticks = DateTime.Now.Ticks.ToString();
            var cabinetName = "My Test Cabinet" + ticks.Substring(ticks.Length - 5, 5);
            var result = svc.CreateCabinet(this.txtUser.Text, this.txtPass.Text, c.Template, cabinetName);

            if (result.Success)
            {
                lblError.Text = "ADDED CABINET";
            }
            else
            {
                lblError.Text = result.Message;
            }
        }

        protected void btnGetDocsById_Click(object sender, EventArgs e)
        {
            var svc = new FileItService();
            var ids = new List<FileItDocumentIdLookup>();
            ids.Add(new FileItDocumentIdLookup()
            {
                CabinetId = "houseofrep4101",
                DocumentId = "10000100"
            });

            var result = svc.GetDocumentsById(txtUser.Text, txtPass.Text, ids, false, false);
            panOutput.Controls.Clear();

            result.Documents.ForEach(d =>
            {
                System.Web.UI.HtmlControls.HtmlImage img = new System.Web.UI.HtmlControls.HtmlImage();
                img.Src = d.WebImageBase64Src;
                img.Attributes.Add("title", GenerateToolTip(d, result.Cabinet));
                panOutput.Controls.Add(img);
            });
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            var cabinetId = "Osifchin_Jonathan61701";
            var svc = new FileItService();
            var documentId = "000000A8";
            var result = svc.DeleteDocument(txtUser.Text, txtPass.Text, cabinetId, documentId);
            lblError.Text = "DELETED =" + result.Success.ToString();
        }
    }
}