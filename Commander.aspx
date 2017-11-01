<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Commander.aspx.cs" Inherits="Commander" %>
<!--<Louis-PhilippeH>-->
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:SqlDataSource ID="sqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:AppConnectionString %>"></asp:SqlDataSource>
    <asp:Label ID="LabelJeu" runat="server" Text="Sélectionner votre jeu : "></asp:Label>
    <asp:DropDownList ID="DropDownListJeu" runat="server"></asp:DropDownList>
    <asp:Label ID="LabelConfirmer" runat="server"></asp:Label> <br />
    <asp:Label ID="LabelEntrepot" runat="server" Text="Sélectionner votre entrepôt : "></asp:Label>
    <asp:DropDownList ID="DropDownListEntrepot" runat="server"></asp:DropDownList> <br />
    <asp:Label ID="LabelQuantite" runat="server" Text="Sélectionner la quantité : "></asp:Label>
    <asp:DropDownList ID="DropDownListQuantite" runat="server"></asp:DropDownList> <br />
    <asp:Button ID="ButtonConfirmer" runat="server" Text="Commander" OnClick="ButtonConfirmer_Click" /> <br /> <br />
    <asp:HyperLink ID="HyperLinkCommander" runat="server">Retourner à l'administration</asp:HyperLink>
</asp:Content>

