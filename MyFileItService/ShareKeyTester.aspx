<%@ Page Title="" Language="C#" MasterPageFile="~/MyFileIt.Master" AutoEventWireup="true" CodeBehind="ShareKeyTester.aspx.cs" Inherits="MyFileItService.ShareKeyTester" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .message {
            font-size: 16pt;
            color: red;
        }

        td {
            text-align: right;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h1>Share Key Tester</h1>
    <p>Enter some information to add test share keys.</p>
    <div class="message">
        <asp:Label runat="server" ID="lblError"></asp:Label>
    </div>
    <table>
        <tr>
            <td>Organization:</td>
            <td><asp:DropDownList runat="server" ID="ddlOrganization"></asp:DropDownList></td>
        </tr>
        <tr>
            <td>Sales Rep:</td>
            <td><asp:DropDownList runat="server" ID="ddlSalesRep"></asp:DropDownList></td>
        </tr>
        <tr>
            <td>Promo Code:</td>
            <td>
                <asp:TextBox runat="server" ID="txtPromoCode"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>Number of keys:</td>
            <td>
                <asp:TextBox runat="server" ID="txtNumberKeys"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td></td>
            <td>
                <asp:Button runat="server" ID="btnGenerateKeys" Text="Generate Keys" OnClick="btnGenerateKeys_Click" />
            </td>
        </tr>
    </table>
</asp:Content>
