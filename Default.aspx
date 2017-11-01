<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Accueil_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
        <asp:SqlDataSource ID="sqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:AppConnectionString %>"></asp:SqlDataSource>
        <div>
            <div>
                <div>Ce site vise à vous faire acheter des jeux et vous permet en même temps de voir l'inventaire grandiose de jeu que nous avons</div>

            </div>
        </div>
        <asp:DropDownList ID="GenreJeuxDropDownList" runat="server" AutoPostBack="True" OnSelectedIndexChanged="GenreJeuxDropDownList_SelectedIndexChanged" Width="164px">
            <asp:ListItem Value="-1">Tout les genres</asp:ListItem>
        </asp:DropDownList>
        <asp:Table ID="TableJeu" runat="server" BorderStyle="Solid" BorderWidth="2px" CellPadding="5" CellSpacing="10" GridLines="Both">
        </asp:Table>
        <br />
        <asp:TextBox ID="TextBox1" runat="server" Width="409px"></asp:TextBox>
        <br />
        <asp:TextBox ID="TextBox2" runat="server" Width="406px"></asp:TextBox>
        <br />
        <asp:TextBox ID="TextBox3" runat="server" Width="407px"></asp:TextBox>
        <br />
        <asp:TextBox ID="TextBox4" runat="server" Width="405px"></asp:TextBox>
</asp:Content>

