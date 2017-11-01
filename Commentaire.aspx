<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Commentaire.aspx.cs" Inherits="Option_Commentaire" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <asp:SqlDataSource ID="sqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:AppConnectionString %>"></asp:SqlDataSource>
    <div>
         <div>
             <asp:Label ID="Label1" runat="server" Text="Prénom: "></asp:Label>
             <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
             <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="TextBox1" EnableClientScript="False" ErrorMessage="Vous devez inscrire un prénom !" ForeColor="Red"></asp:RequiredFieldValidator>
             <br />
             <br />
             <asp:Label ID="Label2" runat="server" Text="Nom: "></asp:Label>
             <asp:TextBox ID="TextBox2" runat="server"></asp:TextBox>
             <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="TextBox2" EnableClientScript="False" ErrorMessage="Vous devez inscrire un nom !" ForeColor="Red"></asp:RequiredFieldValidator>
             <br />
             <br />
             <asp:Label ID="Label3" runat="server" Text="Commentaires: "></asp:Label>
             <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="TextBox3" EnableClientScript="False" ErrorMessage="Vous devez inscrire un commentaire !" ForeColor="Red"></asp:RequiredFieldValidator>
             <br />
             <asp:TextBox ID="TextBox3" runat="server" Height="96px" TextMode="MultiLine" Width="255px"></asp:TextBox>
             <br />
             <br />
             <br />
             <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Soumettre le commentaire" />
             <br />
             <br />
             <asp:Label ID="Label4" runat="server"></asp:Label>
             <br />
             <br />
             <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/Accueil/Default.aspx">Retour vers l&#39;accueil</asp:HyperLink>
             <br />


         </div>
    </div>
    
</asp:Content>

