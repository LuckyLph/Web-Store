<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="PointDistribution.aspx.cs" Inherits="Option_PointDistribution" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:SqlDataSource ID="sqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:AppConnectionString %>"></asp:SqlDataSource>
    <div>
        <div>Nos point de distribution</div>
        <div>
            <asp:DropDownList ID="DropDownListPtDistribution" runat="server" Width="235px" AutoPostBack="True" OnSelectedIndexChanged="DropDownListPtDistribution_SelectedIndexChanged">
                <asp:ListItem Value="-1">Sélectionner un point de distribution</asp:ListItem>
            </asp:DropDownList>
            <br />
            <br />
            Adresse du point de distribution:<br />
            <br />
            Coordonnées :<asp:TextBox ID="CoordonneesTextBox" runat="server" Enabled="False"></asp:TextBox>
            <br />
            <br />
            Ville :<asp:TextBox ID="VilleTextBox" runat="server" Enabled="False"></asp:TextBox>
            <br />
            <br />
            Pays :
            <asp:TextBox ID="PaysTextBox" runat="server" Enabled="False"></asp:TextBox>
            <br />
            <br />
            Code postal :<asp:TextBox ID="CodePostalTextBox" runat="server" Enabled="False"></asp:TextBox>
            <br />
            <br />
            <asp:Literal ID="Literal1" runat="server" ></asp:Literal>
            <br />
            <br />

        </div>
    </div>

</asp:Content>

