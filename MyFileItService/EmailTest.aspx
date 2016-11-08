<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EmailTest.aspx.cs" Inherits="MyFileItService.EmailTest" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Label runat="server" id="lblError"></asp:Label>
    <h1>Test Email</h1>
<table>
    <tr>
        <td>
            Send to email:
        </td>
        <td>
            <asp:TextBox runat="server" ID="emailAddress" ></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            Subject:
        </td>
        <td>
            <asp:TextBox runat="server" ID="subjectLine"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td>
            Message:
        </td>
        <td>
            <asp:TextBox runat="server" ID="message"></asp:TextBox>
        </td>
    </tr>
</table>
        <asp:Button runat="server" ID="btnSend" Text="Send Email" OnClick="btnSend_Click" />
        <asp:Button runat="server" ID="btnAsync" Text="Send Email ASYNC" OnClick="btnAsync_Click" />
    </div>
    </form>
</body>
</html>
