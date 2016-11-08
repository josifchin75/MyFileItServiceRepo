<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Tester.aspx.cs" Inherits="FileItService.Tester" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        h2 {
            border: solid 1px #000000;
            padding: 10px;
        }

        .output {
            width: 1100px;
        }

            .output img {
                width: 150px;
                float: left;
            }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table>
                <tr>
                    <td>Authentication User</td>
                    <td>
                        <asp:TextBox runat="server" ID="txtUser"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Authentication Pass</td>
                    <td>
                        <asp:TextBox runat="server" ID="txtPass"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>User to change</td>
                    <td>
                        <asp:TextBox runat="server" ID="txtTargetUser"></asp:TextBox>
                        <asp:CheckBox runat="server" ID="chkEnableUser" Text="User Active status to update" />
                        <asp:Button runat="server" ID="btnEnableUser" OnClick="btnEnableUser_Click" Text="Set User Active" />

                    </td>
                </tr>
                <tr>
                    <td>Change Password </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtNewPassword"></asp:TextBox>
                        <asp:Button runat="server" ID="btnChangePassword" OnClick="btnChangePassword_Click" Text="Change Password" />

                    </td>
                </tr>
                <tr>
                    <td>Add Cabinet ACcess:</td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlAccessType"></asp:DropDownList>
                        <asp:DropDownList runat="server" ID="ddlCabinets" AutoPostBack="true" OnSelectedIndexChanged="ddlCabinets_SelectedIndexChanged"></asp:DropDownList>
                        <asp:CheckBox runat="server" ID="chkAllowAccess" Text="Allow Access" />
                    </td>
                </tr>
            </table>
            <p>
                <asp:Button runat="server" ID="btnAuthenticate" OnClick="btnAuthenticate_Click" Text="Authenticate" />
                <asp:Button runat="server" ID="btnGetCabinets" OnClick="btnGetCabinets_Click" Text="Get Cabinets" />
                <asp:Button runat="server" ID="btnAddRemoveAccess" OnClick="btnAddRemoveAccess_Click" Text="Add / Remove Access" />
            </p>
            <p>
                <asp:Button runat="server" ID="btnReadDocs" OnClick="btnReadDocs_Click" Text="Read Cabinet Documents" />
                <asp:Button runat="server" ID="btnGetCabinet" OnClick="btnGetCabinet_Click" Text ="Get Cabinet" />
                <asp:Button runat="server" ID="btnCreateCabinet" OnClick="btnCreateCabinet_Click" Text="Create Cabinet" />
                <asp:Button runat="server" ID="btnGetDocsById" OnClick="btnGetDocsById_Click" Text="Get Docs By Id" />
            </p>
            <fieldset>
                <legend>Add User</legend>
                <table></table>
                <asp:Button runat="server" ID="btnAddUser" OnClick="btnAddUser_Click" Text="Add User" />
            </fieldset>
            <fieldset>
                <legend>Upload Document</legend>
                <div id="divIndexes" runat="server"></div>
                Upload File
                <asp:FileUpload runat="server" ID="FileUpload" />
                <asp:Button runat="server" ID="btnUploadImage" OnClick="btnUploadImage_Click" text="Upload Image" />
                
            </fieldset>
            <fieldset>
                <legend>Soft Delete Document</legend>
                <asp:Button runat="server" ID="btnDelete" Text="Delete Doc" OnClick="btnDelete_Click" />
            </fieldset>
            <h2>
                <asp:Label runat="server" ID="lblError"></asp:Label></h2>
            <asp:Panel ID="panOutput" runat="server" CssClass="output"></asp:Panel>
        </div>
    </form>
</body>
</html>
