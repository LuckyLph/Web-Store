using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.OleDb;

public partial class ModiferCommande : System.Web.UI.Page
{
    //<Louis-PhilippeH>
    //propriété qui représente notre classe de connexion et qui offre une passerelle vers la vraie connexion avec le pilote
    ConnexionBD bd = null;
    //propriété qui représente l'accès aux données (CRUD) pour un client
    ModeleClient modeleClient = null;

    /// <summary>
    /// Initialisation, première étape du cycle de vie donc on en profite pour créer notre modèle seulement s'il n'existe pas dans la session
    /// Après quoi, on ne le referra pas inutilement
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Init(object sender, EventArgs e)
    {
        //On ouvre pas une connexion si le modèle est déjà construit !
        //Il y a une nuance avec le postback ici car si on a frappé un problème dans le code, on aura mis le modèle à null dans la session
        //Ceci nous laisse donc une chance de le reconstruire !
        if (Session["modeleModifier"] == null)
        {
            //Gestion des exception essentielle, on gère du code "dangereux"
            try
            {
                //On effectue la connexion et on l'obtient en retour
                bd = new ConnexionBD();
                OleDbConnection connection = bd.ConnectToDB(sqlDataSource1.ConnectionString);

                //On Instancie notre modèle client en lui passant la connection reçue
                modeleClient = new ModeleClient(connection);

                //Si tout a fonctionné, on stocke notre modèle dans la session. C'est la meilleure façon car un PostBack va tout effacer ce qu'on vient de faire !
                Session["modeleModifier"] = modeleClient;
            }

            catch (Exception exc)
            {
                //message d'erreur comme quoi la BD ne sera pas disponible
                modeleClient.RollbackTransaction();
                System.Diagnostics.Debug.Write(exc);
            }
        }
    }

    /// <summary>
    /// Chargement de la page, il faut faire attention au PostBack pour ne pas faire des requêtes inutilement à chaque PostBack
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        Session["idCommande"] = Request.QueryString.ToString();
        ListerCommandes();
        HyperLinkModifierCommande.NavigateUrl = "Administration.aspx";
    }

    /// <summary>
    /// Déchargement de la page, Il faut faire le CommitChanges pour que nos changements soient concretement écrits dans la BD
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Unload(object sender, EventArgs e)
    {
        //!!!!!!!!!!!!!!!!!!!!!!!!
        //CECI EST ESSENTIEL LORSQU'ON A FAIT DES REQUETES EN MODIFICATION CAR ELLES ÉTAIENT EN MEMOIRE DANS LA TRANSACTION, IL FAUT LES ÉCRIRE DANS LA BD
        //!!!!!!!!!!!!!!!!!!!!!!!!
        try
        {
            if (Session["modeleModifier"] != null)
            {
                ((ModeleClient)Session["modeleModifier"]).CommitChanges();
            }
        }

        catch (Exception exc)
        {
            System.Diagnostics.Debug.Write(exc);
        }
    }

    /// <summary>
    /// Cette méthode liste tous les clients de la base de données et envoie les données en construction dans la table
    /// </summary>
    public void ListerCommandes()
    {
        //Gestion des exception essentielle, on gère du code "dangereux"
        try
        {
            //On s'assure que le modèle client est disponible
            if (Session["modeleModifier"] != null)
            {
                OleDbDataReader readerSelect = null;
                //On le récupère et on demande les enregistrements des clients selon la requête passée en paramètre
                ModeleClient modele = (ModeleClient)Session["modeleModifier"];
                if (!IsPostBack)
                {
                    string[] idCommandeTab = Session["idCommande"].ToString().Split('=');
                    int idCommande = Int32.Parse(idCommandeTab[1]);
                    readerSelect = modele.ReadClient("SELECT CommandeJeu.CodeJeu, Jeu.Titre, CommandeJeu.Quantité, Entrepot.Ville, Entrepot.Pays, Entrepot.IdEntrepot FROM Jeu INNER JOIN (CommandeJeu INNER JOIN Entrepot ON CommandeJeu.IdEntrepot = Entrepot.IdEntrepot) ON CommandeJeu.CodeJeu = Jeu.CodeJeu WHERE CommandeJeu.IdCommande = " + idCommande);
                    string jeuActuel = ConstruireDropDownList(DropDownListJeu, readerSelect);
                    readerSelect.Close();
                    readerSelect = modele.ReadClient("SELECT CommandeJeu.CodeJeu, Jeu.Titre, CommandeJeu.Quantité, Entrepot.Ville, Entrepot.Pays, Entrepot.IdEntrepot FROM Jeu INNER JOIN (CommandeJeu INNER JOIN Entrepot ON CommandeJeu.IdEntrepot = Entrepot.IdEntrepot) ON CommandeJeu.CodeJeu = Jeu.CodeJeu WHERE CommandeJeu.IdCommande = " + idCommande);
                    string entrepotActuel = ConstruireDropDownList(DropDownListEntrepot, readerSelect);
                    readerSelect.Close();
                    readerSelect = modele.ReadClient("SELECT CommandeJeu.CodeJeu, Jeu.Titre, CommandeJeu.Quantité, Entrepot.Ville, Entrepot.Pays, Entrepot.IdEntrepot FROM Jeu INNER JOIN (CommandeJeu INNER JOIN Entrepot ON CommandeJeu.IdEntrepot = Entrepot.IdEntrepot) ON CommandeJeu.CodeJeu = Jeu.CodeJeu WHERE CommandeJeu.IdCommande = " + idCommande);
                    ConstruireDropDownList(DropDownListQuantite, readerSelect);
                    readerSelect.Close();


                    readerSelect = modele.ReadClient("SELECT CodeJeu, Titre FROM Jeu WHERE NOT CodeJeu = '" + jeuActuel + "'");
                    CompleterDropDownList(DropDownListJeu, readerSelect);
                    readerSelect.Close();
                    readerSelect = modele.ReadClient("SELECT Ville, Pays, IdEntrepot FROM Entrepot WHERE NOT IdEntrepot = " + entrepotActuel);
                    CompleterDropDownList(DropDownListEntrepot, readerSelect);
                    readerSelect.Close();
                    CompleterDropDownList(DropDownListQuantite, readerSelect);
                }
            }
        }

        catch (Exception exc)
        {
            //Si le probleme provenait de la transaction du modele, on la Rollback
            ModeleClient modele = (ModeleClient)Session["modeleModifier"];
            modele.RollbackTransaction();
            //et on invalide le modele, il pourra être reconstruit en PostBack
            Session["modeleModifier"] = null;
            System.Diagnostics.Debug.Write(exc);
        }
    }
    public string ConstruireDropDownList(DropDownList liste, OleDbDataReader reader)
    {
        ListItem item = new ListItem();
        string stringToInsert = "";
        string stringToReturn = "";
        while (reader.Read())
        {
            if (liste == DropDownListJeu)
            {
                item = new ListItem();
                stringToInsert = reader[0].ToString();
                stringToReturn = stringToInsert;
                stringToInsert += " - " + reader[1].ToString();
                item.Value = reader[0].ToString();
                item.Text = stringToInsert;
                liste.Items.Add(item);
            }
            else if (liste == DropDownListEntrepot)
            {
                item = new ListItem();
                stringToInsert = reader[3].ToString();
                stringToInsert += " - " + reader[4].ToString();
                item.Value = reader[5].ToString();
                stringToReturn = item.Value;
                item.Text = stringToInsert;
                liste.Items.Add(item);
            }
            else if (liste == DropDownListQuantite)
            {
                item = new ListItem();
                stringToInsert = reader[2].ToString();
                item.Value = stringToInsert;
                item.Text = stringToInsert;
                liste.Items.Add(item);
            }
        }
        return stringToReturn;
    }
    public void CompleterDropDownList(DropDownList liste, OleDbDataReader reader)
    {
        ListItem item = new ListItem();
        string stringToInsert = "";
        if (!reader.IsClosed)
        {
            while (reader.Read())
            {
                if (liste == DropDownListJeu)
                {
                    item = new ListItem();
                    stringToInsert = reader[0].ToString();
                    stringToInsert += " - " + reader[1].ToString();
                    item.Value = reader[0].ToString();
                    item.Text = stringToInsert;
                    liste.Items.Add(item);
                }
                else if (liste == DropDownListEntrepot)
                {
                    item = new ListItem();
                    stringToInsert = reader[0].ToString();
                    stringToInsert += " - " + reader[1].ToString();
                    item.Value = reader[2].ToString();
                    item.Text = stringToInsert;
                    liste.Items.Add(item);
                }
            }
        }
        if (liste == DropDownListQuantite)
        {
            for (int i = 1; i <= 30; i++)
            {
                item = new ListItem();
                item.Text = i.ToString();
                item.Value = i.ToString();
                liste.Items.Add(item);
            }
        }
    }
    public void MettreAJourCommande()
    {
        //Gestion des exception essentielle, on gère du code "dangereux"
        try
        {
            //On s'assure que le modèle client est disponible
            if (Session["modeleModifier"] != null)
            {
                //On le récupère et on demande les enregistrements des clients selon la requête passée en paramètre
                ModeleClient modele = (ModeleClient)Session["modeleModifier"];
                string requestElementToSendJeu = DropDownListJeu.SelectedValue;
                string requestElementToSendEntrepot = DropDownListEntrepot.SelectedValue;
                string requestElementToSendQuantite = DropDownListQuantite.SelectedValue;
                string[] idCommandeTab = Session["idCommande"].ToString().Split('=');
                int idCommande = Int32.Parse(idCommandeTab[1]);
                //On peut maintenant faire notre requete
                int numRows = modele.CreateClient("UPDATE CommandeJeu SET CodeJeu = '" + requestElementToSendJeu + "', Quantité = '" + requestElementToSendQuantite + "', DateCommande = NOW(), IdEntrepot = '" + requestElementToSendEntrepot + "' WHERE IdCommande = " + idCommande);
                if (numRows == 1)
                {
                    LabelConfirmer.Text = "Commande modifiée avec succès !";
                    DropDownListJeu.Visible = false;
                    DropDownListEntrepot.Visible = false;
                    DropDownListQuantite.Visible = false;
                    LabelJeu.Visible = false;
                    LabelEntrepot.Visible = false;
                    LabelQuantite.Visible = false;
                    ButtonAnnuler.Visible = false;
                    ButtonModifier.Visible = false;
                }
                else
                {
                    LabelConfirmer.Text = "Erreur de modification !";
                }
            }
        }

        catch (Exception exc)
        {
            //Si le probleme provenait de la transaction du modele, on la Rollback
            ModeleClient modele = (ModeleClient)Session["modeleModifier"];
            modele.RollbackTransaction();
            //et on invalide le modele, il pourra être reconstruit en PostBack
            Session["modeleModifier"] = null;
            System.Diagnostics.Debug.Write(exc);
        }
    }
    public void SupprimerCommande()
    {
        //Gestion des exception essentielle, on gère du code "dangereux"
        try
        {
            //On s'assure que le modèle client est disponible
            if (Session["modeleModifier"] != null)
            {
                //On le récupère et on demande les enregistrements des clients selon la requête passée en paramètre
                ModeleClient modele = (ModeleClient)Session["modeleModifier"];
                string requestElementToSendJeu = DropDownListJeu.SelectedValue;
                string requestElementToSendEntrepot = DropDownListEntrepot.SelectedValue;
                string requestElementToSendQuantite = DropDownListQuantite.SelectedValue;
                string[] idCommandeTab = Session["idCommande"].ToString().Split('=');
                int idCommande = Int32.Parse(idCommandeTab[1]);
                //On peut maintenant faire notre requete
                int numRows = modele.CreateClient("DELETE FROM CommandeJeu WHERE IdCommande = " + idCommande);
                if (numRows == 1)
                {
                    LabelConfirmer.Text = "Commande effacée avec succès !";
                    DropDownListJeu.Visible = false;
                    DropDownListEntrepot.Visible = false;
                    DropDownListQuantite.Visible = false;
                    LabelJeu.Visible = false;
                    LabelEntrepot.Visible = false;
                    LabelQuantite.Visible = false;
                    ButtonAnnuler.Visible = false;
                    ButtonModifier.Visible = false;
                }
                else
                {
                    LabelConfirmer.Text = "Erreur de modification !";
                }
            }
        }

        catch (Exception exc)
        {
            //Si le probleme provenait de la transaction du modele, on la Rollback
            ModeleClient modele = (ModeleClient)Session["modeleModifier"];
            modele.RollbackTransaction();
            //et on invalide le modele, il pourra être reconstruit en PostBack
            Session["modeleModifier"] = null;
            System.Diagnostics.Debug.Write(exc);
        }
    }
    protected void ButtonAnnuler_Click(object sender, EventArgs e)
    {
        SupprimerCommande();
    }
    protected void ButtonModifier_Click(object sender, EventArgs e)
    {
        MettreAJourCommande();
    }
    protected void DropDownListJeu_SelectedIndexChanged(object sender, EventArgs e)
    {
        ButtonModifier.Enabled = true;
    }
    protected void DropDownListEntrepot_SelectedIndexChanged(object sender, EventArgs e)
    {
        ButtonModifier.Enabled = true;
    }
    protected void DropDownListQuantite_SelectedIndexChanged(object sender, EventArgs e)
    {
        ButtonModifier.Enabled = true;
    }
}