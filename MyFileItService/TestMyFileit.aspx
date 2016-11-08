<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestMyFileit.aspx.cs" Inherits="MyFileItService.TestMyFileit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .error {
            color: red;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h2>
                <asp:Label ID="lblMessage" runat="server" CssClass="error"></asp:Label>
            </h2>
            <asp:Button runat="server" ID="btnAuth" OnClick="btnAuth_Click" Text="Authorize" />
            <asp:Button runat="server" ID="btnCabs" OnClick="btnCabs_Click" Text="Get Cabinets" />
            <asp:Button runat="server" ID="btnDocs" OnClick="btnDocs_Click" Text="Get Documents" />
            <asp:Button runat="server" ID="btnInit" OnClick="btnInit_Click" Text="Init Service" />

            <h2>Get Reference Tables</h2>
            <select runat="server" id="ddlReferenceTables">
                <option value="CoachStatus">Coach Status</option>
                <option value="RelationShipType">RelationShipType</option>
                <option value="SportType">SportType</option>
                <option value="AppUserType">AppUserType</option>
                <option value="AppUserStatus">AppUserStatus</option>
                <option value="OrganizationType">OrganizationType</option>
                <option value="OrganizationStatus">OrganizationStatus</option>
                <option value="TeamEventDocumentStatus">TeamEventDocumentStatus</option>
                <option value="DocumentType">DocumentType</option>
                <option value="DocumentStatus">DocumentStatus</option>
                <option value="UserStageType">UserStageType</option>
            </select>
            <div>
                <p>
                    Organizations:
                <asp:DropDownList runat="server" ID="ddlOrganizations"></asp:DropDownList>
                </p>
                <p>
                    Team Events
                    <asp:DropDownList runat="server" ID="ddlTeamEvents"></asp:DropDownList>
                </p>
            </div>
            <div>
                <asp:Button runat="server" ID="btnAddCoach" OnClick="btnAddCoach_Click" Text="Add Coach" />
            </div>
            <asp:Button runat="server" ID="btnStatusTable" OnClick="btnStatusTable_Click" Text="Get Reference Data" />
            <h2>Remove Organization</h2>
            <asp:TextBox runat="server" ID="txtOrganizationId" Text="1"></asp:TextBox>
            <asp:Button runat="server" ID="btnRemoveOrganization" Text="Remove Organization" OnClick="btnRemoveOrganization_Click" />
            <h2>lookup organization</h2>
            <asp:TextBox runat="server" ID="txtOrganizationLookup" Text="1"></asp:TextBox>
            <asp:Button runat="server" ID="btnGetOrg" Text="Get Organization" OnClick="btnGetOrg_Click" />
            <asp:Button runat="server" ID="btnUpdateOrg" Text="Update Organization" OnClick="btnUpdateOrg_Click" />
            <asp:Button runat="server" ID="btnAddOrgDebug" Text="Add Organization Debug" OnClick="btnAddOrgDebug_Click" />
            <div>
                <h2>add sharekeys</h2>
                <asp:Button runat="server" ID="btnAddSharekeys" Text="Add Sharekeys" OnClick="btnAddSharekeys_Click" />
                <asp:Button runat="server" ID="btnGetShareKeys" Text="Get ShareKeys" OnClick="btnGetShareKeys_Click" />
                <asp:Button runat="server" ID="btnGetAvailableSharekeys" Text="Get Available ShareKeys" OnClick="btnGetAvailableSharekeys_Click" />
                <asp:Button runat="server" ID="btnEmergencyShare" Text="Send Emergency Share" OnClick="btnEmergencyShare_Click" />
            </div>

            <div>
                <h1>Service Calls</h1>
                <asp:Button runat="server" ID="btnAddOrganization" Text="Add Organization" OnClick="btnAddOrganization_Click" />
                <asp:Button runat="server" ID="btnUpdateOrganization" Text="Update Organization" OnClick="btnUpdateOrganization_Click" />
                <asp:CheckBox runat="server" ID="chkAllowCoach" Text="Allow Coach" />
                <asp:CheckBox runat="server" ID="chkCCEmail" Text="CC Email" />
            </div>
            <div>
                <h1>Get Users</h1>
                UserName:
                <asp:TextBox runat="server" ID="txtUsername"></asp:TextBox>
                email:
                <asp:TextBox runat="server" ID="txtEmail"></asp:TextBox>
                pass:
                <asp:TextBox runat="server" ID="txtPassword"></asp:TextBox>
                <asp:Button runat="server" ID="btnGetUser" Text="Get User By Email" OnClick="btnGetUser_Click" />
                <asp:Button runat="server" ID="btnGetAllUsers" Text="Get All Users" OnClick="btnGetAllUsers_Click" />
                <asp:Button runat="server" ID="btnGetAppUSerMain" Text="Get App Users MAin" OnClick="btnGetAppUSerMain_Click" />
                <asp:Button runat="server" ID="btnGetAppUserFullLookup" Text="Get App Users Name + EMail" OnClick="btnGetAppUserFullLookup_Click" />
                <asp:Button runat="server" ID="btnCheckExists" Text="Check User Exists" OnClick="btnCheckExists_Click" />
                <asp:Button runat="server" ID="btnGetEmails" Text="Get Sent Emails" OnClick="btnGetEmails_Click" />
                <asp:Button runat="server" ID="btnGetFamilyUsers" Text="Get Family Users" OnClick="btnGetFamilyUsers_Click" />

            </div>
            <div>
                <h1>Add USer</h1>
                <asp:Button runat="server" ID="btnAddAppUser" Text="Add USer" OnClick="btnAddAppUser_Click" />
                <asp:CheckBox ID="chkIsPrimaryUser" runat="server" Checked="True" Text="Is primary user" />
                <asp:Button runat="server" ID="btnLogin" Text="Login User" OnClick="btnLogin_Click" />
                <asp:Button runat="server" ID="btnRemoveCoach" Text="Remove Coach" OnClick="btnRemoveCoach_Click" />
            </div>
            <div>
                <h1>Upload Document</h1>
                <asp:Button runat="server" ID="btnUpload" Text="Upload Document" OnClick="btnUpload_Click" />
            </div>
            <div>
                <h1>Sales Rep</h1>
                <asp:Button runat="server" ID="btnAddSalesRep" Text="Add Sales Rep" OnClick="btnAddSalesRep_Click" />
                <asp:Button runat="server" ID="btnRemoveSalesRep" Text="Remove Sales Rep" OnClick="btnRemoveSalesRep_Click" />
                <asp:Button ID="btnLoginSalesRep" runat="server" OnClick="btnLoginSalesRep_Click" Text="Login Sales Rep" />
            </div>
            <div>
                <h1>Team Events</h1>
                <asp:TextBox runat="server" ID="txtTeamEventName"></asp:TextBox>
                <asp:Button runat="server" ID="btnAddTeamEvent" Text="Add Team Event" OnClick="btnAddTeamEvent_Click" />
                <asp:Button runat="server" ID="btnUpdateTeamEvent" Text="Update Team Event" OnClick="btnUpdateTeamEvent_Click" />
                <asp:Button runat="server" ID="btnGetTeamEvents" Text="Get Team Event By User" OnClick="btnGetTeamEvents_Click" />
                <asp:Button runat="server" ID="btnTeamEventsByCoach" Text="Get Team Event By Coach" OnClick="btnTeamEventsByCoach_Click" />

                <asp:Button runat="server" ID="btnGetTeamEventsWithUploads" Text="Get Team Event With Uploads" OnClick="btnGetTeamEventsWithUploads_Click" />
            </div>
            <div>
                <h1>Team Event Docs</h1>
                <asp:TextBox runat="server" ID="txtTeamEventDocumentName"></asp:TextBox>
                <asp:Button runat="server" ID="btnAddTeamEventDocument" Text="Add Team Event Document" OnClick="btnAddTeamEventDocument_Click" />
                <asp:Button runat="server" ID="btnUpdateTeamEventDocument" Text="Update Team Event Document" OnClick="btnUpdateTeamEventDocument_Click" />
                <asp:Button runat="server" ID="btnVerifyDocument" Text="Verify Document" OnClick="btnVerifyDocument_Click" />
                <asp:Button runat="server" ID="btnRejectDocumentShare" Text="Reject Document Share" OnClick="btnRejectDocumentShare_Click" />
                <asp:Button runat="server" ID="btnDeleteDocument" Text="Delete Document" OnClick="btnDeleteDocument_Click" />
            </div>
            <div>
                <h1>Team Event Player Roster</h1>
                <asp:DropDownList runat="server" ID="ddlAppUsers"></asp:DropDownList>
                <asp:Button runat="server" ID="btnAddTeamEventPlayerRoster" Text="Add Team Event Player Roster" OnClick="btnAddTeamEventPlayerRoster_Click" />
                <asp:Button runat="server" ID="btnUpdateTeamEventPlayerRoster" Text="Update Team Event Player Roster" OnClick="btnUpdateTeamEventPlayerRoster_Click" />
                <asp:DropDownList runat="server" ID="ddlTeamEventPlayerRosters"></asp:DropDownList>

            </div>
            <div>
                <asp:Button runat="server" ID="btnGetFamilyDocs" Text="Get Family Docs" OnClick="btnGetFamilyDocs_Click" />
                 <asp:Button runat="server" ID="btnGetDocsSpeedup" Text="Get Family Docs SPEEDUP" OnClick="btnGetDocsSpeedup_Click" />
                <asp:Button runat="server" ID="btnGetDocListOnly" Text="Get Docs by specified list" OnClick="btnGetDocListOnly_Click" />
            </div>
            <div>
                <h1>Associate Docs Array</h1>
                <asp:Button runat="server" ID="btnAssociateDocuments" Text="Associate Multiple Docs" OnClick="btnAssociateDocuments_Click" />
                <asp:Button runat="server" ID="btnResendAssociateEmail" Text="Resend Associate Email" OnClick="btnResendAssociateEmail_Click" />
            </div>
            <div>
                <h1>Test get app user organizations</h1>
                <asp:Button runat="server" ID="btnGetAppUserOrgs" Text="Get User Orgs" OnClick="btnGetAppUserOrgs_Click" />
            </div>
            <div>
                <h1>Coaches</h1>
                <asp:Button runat="server" ID="btnGetCoachesByEvent" Text="Get Coaches by Event Id" OnClick="btnGetCoachesByEvent_Click" />
                <asp:Button runat="server" ID="btnGetCoachesByOrganization" Text="Get Coaches by Organization Id" OnClick="btnGetCoachesByOrganization_Click" />
            </div>
            <div>
                <h1>Service Check Methods</h1>
                <asp:Button runat="server" ID="btnSendShareReminders" Text="Send Share Reminders" OnClick="btnSendShareReminders_Click" />
            </div>
            <div>
                <h1>ERRORS</h1>
                <asp:Button runat="server" ID="btnError" Text="Throw Error" OnClick="btnError_Click" />
            </div>
        </div>
    </form>
</body>
</html>
