<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ServiceCalls.aspx.cs" Inherits="MyFileItTestSite.ServiceCalls" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        form div {
            margin: 20px;
            border: solid 1px #ffffff;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h1>Login</h1>
            <table>
                <tr>
                    <td>User Name</td>
                    <td><asp:TextBox runat="server" ID="txtUserName"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Password</td>
                    <td><asp:TextBox runat="server" ID="txtPassword"></asp:TextBox></td>
                </tr>
                <tr>
                    <td></td>
                    <td><asp:Button runat="server" ID="btnLogin" Text="Login" OnClick="btnLogin_Click" /></td>
                </tr>
            </table>
            
        </div>
        <div>
            <h1>Service Calls</h1>
            <asp:Button runat="server" ID="btnAddOrganization" Text="Add Organization" OnClick="btnAddOrganization_Click" />
            <asp:CheckBox runat="server" ID="chkAllowCoach" Text="Allow Coach" />
            <asp:CheckBox runat="server" ID="chkCCEmail" Text="CC Email" />
        </div>
        <div>
            <asp:Button runat="server" ID="btnDebugAdd" Text="Debg Add" OnClick="btnDebugAdd_Click" />

        </div>
        <div>
            <asp:Button runat="server" ID="btnSalesReps" Text="Get Sales Reps" OnClick="btnSalesReps_Click" />
        </div>
        <div>
            <asp:Button runat="server" ID="btnGetUsers" Text="Get Users by Org" OnClick="btnGetUsers_Click" />
            <asp:Button runat="server" ID="btnAssociateUserToOrg" Text="Associate User To Organization" OnClick="btnAssociateUserToOrg_Click" />
            <asp:Button runat="server" ID="btnRemoveUserOrg" Text="Remove User From Org" OnClick="btnRemoveUserOrg_Click" />
            <asp:Button runat="server" ID="btnAddRemoveOrganization" Text="Remove and then Add Org to User" OnClick="btnAddRemoveOrganization_Click" />
        <asp:Button runat="server" ID="btnTestAssociateOrg" Text="Aftab Associate TEST" OnClick="btnTestAssociateOrg_Click" />
        </div>
        <div>
            <asp:Button runat="server" ID="btnLoginUserTest" Text="Login User Call" OnClick="btnLoginUserTest_Click" />
            <asp:Button runat="server" ID="btnRemoveCoachFromUser" Text="Remove Coach From User" OnClick="btnRemoveCoachFromUser_Click" />
        </div>
        <div>
            <asp:Button runat="server" ID="btnAddPaymentHistory" Text="Add Payment History" OnClick="btnAddPaymentHistory_Click" />
            <asp:Button runat="server" ID="btnGetTeamEventsText" Text="Get Team Events" OnClick="btnGetTeamEventsText_Click" />
              <asp:Button runat="server" ID="btnShareKeyUpdate" Text="Update Share Key" OnClick="btnShareKeyUpdate_Click" />
        </div>
        <div>
            <asp:Button runat="server" ID="btnGetCoaches" Text="Get Coaches BY Organization" OnClick="btnGetCoaches_Click" />
            <asp:Button runat="server" ID="btnSendCoachEmail" Text="Send Coach Email" OnClick="btnSendCoachEmail_Click" />
        </div>
        <div>
            <asp:Button runat="server" ID="btnGetdocumentThumbs" Text="Call Get Document THUMBS" OnClick="btnGetdocumentThumbs_Click" />
        </div>
        <h2>
            <asp:Label runat="server" ID="lblMessage"></asp:Label>
        </h2>
    </form>
</body>
</html>
