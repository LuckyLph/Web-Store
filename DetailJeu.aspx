<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="DetailJeu.aspx.cs" Inherits="Accueil_DetailJeu" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:SqlDataSource ID="sqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:AppConnectionString %>"></asp:SqlDataSource>
    <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
    <br />
    <div>
         <div>

             <asp:Label ID="Label2" runat="server" Text="Label"></asp:Label>
             <br />
             <br />
             <asp:Label ID="Label3" runat="server" Text="Label"></asp:Label>
             <br />
             <br />
             <asp:Label ID="Label4" runat="server" Text="Label"></asp:Label>
             <br />
             <br />
             <asp:Label ID="Label5" runat="server" Text="Label"></asp:Label>
             <br />
             <br />
             <asp:HyperLink ID="HyperLink1" runat="server" ImageUrl="~/Accueil/Default.aspx">retour</asp:HyperLink>

         </div>
    </div>
</asp:Content>

