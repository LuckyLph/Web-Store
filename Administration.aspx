<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Administration.aspx.cs" Inherits="Administration" %>
<!--<Louis-PhilippeH>-->
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:SqlDataSource ID="sqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:AppConnectionString %>"></asp:SqlDataSource>
    <asp:HyperLink ID="HyperLinkAdministration" runat="server">Passer une commande</asp:HyperLink>
    <asp:Table ID="TableAdmin" runat="server"></asp:Table>
    <asp:DropDownList ID="DropDownListAdmin" runat="server" AutoPostBack="True"></asp:DropDownList>
</asp:Content>

