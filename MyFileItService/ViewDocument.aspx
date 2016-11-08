<%@ Page Title="" Language="C#" MasterPageFile="~/MyFileIt.Master" AutoEventWireup="true" CodeBehind="ViewDocument.aspx.cs" Inherits="MyFileItService.ViewDocument" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <asp:Label ID="lblDocumentInfo" runat="server"></asp:Label>
        <asp:Image ID="imgMain" runat="server" />
        <%--<a id="lnkDownload" runat="server" href="#">Click to download</a>--%>
    </div>
</asp:Content>
