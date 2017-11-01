<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="ModiferCommande.aspx.cs" Inherits="ModiferCommande" %>
<!--<Louis-PhilippeH>-->
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:SqlDataSource ID="sqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:AppConnectionString %>"></asp:SqlDataSource>
    <asp:Label ID="LabelJeu" runat="server" Text="Sélectionner votre jeu : "></asp:Label>
    <asp:DropDownList ID="DropDownListJeu" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropDownListJeu_SelectedIndexChanged"></asp:DropDownList>
    <asp:Label ID="LabelConfirmer" runat="server"></asp:Label> <br />
    <asp:Label ID="LabelEntrepot" runat="server" Text="Sélectionner votre entrepôt : "></asp:Label>
    <asp:DropDownList ID="DropDownListEntrepot" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropDownListEntrepot_SelectedIndexChanged"></asp:DropDownList> <br />
    <asp:Label ID="LabelQuantite" runat="server" Text="Sélectionner la quantité : "></asp:Label>
    <asp:DropDownList ID="DropDownListQuantite" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropDownListQuantite_SelectedIndexChanged"></asp:DropDownList> <br />
    <asp:Button ID="ButtonModifier" runat="server" Text="Modifier" OnClick="ButtonModifier_Click" Enabled="False" />
    <asp:Button ID="ButtonAnnuler" OnClick="ButtonAnnuler_Click" runat="server" Text="Annuler la commande" /> <br /> <br />
    <asp:HyperLink ID="HyperLinkModifierCommande" runat="server">Retourner à l'administration</asp:HyperLink>
</asp:Content>

