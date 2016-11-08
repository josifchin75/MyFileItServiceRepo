<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportTester.aspx.cs" Inherits="MyFileItService.ReportTester" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .message {
            background-color: yellow;
            padding: 10px;
            border: 1px solid #000000;
            margin-top: 20px;
        }
        h3 {
            border-bottom: 1px solid #000000;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <h1>Report Tester</h1>
        <div>
            <h3>Team Event Player Document Status</h3>
            <asp:Button runat="server" ID="btnTeamEventPlayerDocumentStatus" Text="Run Report" OnClick="btnTeamEventPlayerDocumentStatus_Click" />
        </div>
        <div class="message">
            <asp:Label runat="server" ID="lblMessage"></asp:Label>
        </div>
    </form>
</body>
</html>
