using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.OleDb;

public partial class Commander : System.Web.UI.Page
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
        if (Session["modeleCommandes"] == null)
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
                Session["modeleCommandes"] = modeleClient;
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
        ListerCommandes();
        HyperLinkCommander.NavigateUrl = "Administration.aspx";
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
            if (Session["modeleCommandes"] != null)
            {
                ((ModeleClient)Session["modeleCommandes"]).CommitChanges();
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
            if (Session["modeleCommandes"] != null)
            {
                //On le récupère et on demande les enregistrements des clients selon la requête passée en paramètre
                ModeleClient modele = (ModeleClient)Session["modeleCommandes"];

                if (!IsPostBack)
                {
                    OleDbDataReader readerSelectJeu = modele.ReadClient("SELECT CodeJeu, Titre FROM Jeu");
                    ConstruireDropDownList(DropDownListJeu, readerSelectJeu);
                    readerSelectJeu.Close();
                    OleDbDataReader readerSelectEntrepot = modele.ReadClient("SELECT IdEntrepot, Ville, Pays FROM Entrepot");
                    ConstruireDropDownList(DropDownListEntrepot, readerSelectEntrepot);
                    readerSelectEntrepot.Close();
                }
            }
        }

        catch (Exception exc)
        {
            //Si le probleme provenait de la transaction du modele, on la Rollback
            ModeleClient modele = (ModeleClient)Session["modeleCommandes"];
            modele.RollbackTransaction();
            //et on invalide le modele, il pourra être reconstruit en PostBack
            Session["modeleCommandes"] = null;
            System.Diagnostics.Debug.Write(exc);
        }
    }


    /// <summary>
    //Cette méthode effectue une mise à jour, mais "en memoire" dans la BD. Pour un impact réel dans le fichier de base de données
    //Il ne faut pas oublier d'appler le CommitChanges au déchargement de la page
    /// </summary>
    public void AjouterCommande()
    {
        //Gestion des exception essentielle, on gère du code "dangereux"
        try
        {
            //On s'assure que le modèle client est disponible
            if (Session["modeleCommandes"] != null)
            {
                //On le récupère et on demande les enregistrements des clients selon la requête passée en paramètre
                ModeleClient modele = (ModeleClient)Session["modeleCommandes"];
                string requestElementToSendJeu = DropDownListJeu.SelectedValue;
                string requestElementToSendEntrepot = DropDownListEntrepot.SelectedValue;
                string requestElementToSendQuantite = DropDownListQuantite.SelectedValue;
                //On peut maintenant faire notre requete
                int numRows = modele.CreateClient("INSERT INTO CommandeJeu (CodeJeu, Quantité, DateCommande, IdEntrepot) VALUES ('" + requestElementToSendJeu + "', '" + requestElementToSendQuantite + "', NOW(), '" + requestElementToSendEntrepot + "')");
                if (numRows == 1)
                {
                    LabelConfirmer.Text = "Commande réussie !";
                    DropDownListJeu.Visible = false;
                    DropDownListEntrepot.Visible = false;
                    DropDownListQuantite.Visible = false;
                    LabelJeu.Visible = false;
                    LabelEntrepot.Visible = false;
                    LabelQuantite.Visible = false;
                    ButtonConfirmer.Visible = false;
                }
                else
                {
                    LabelConfirmer.Text = "Erreur de commande !";
                }
            }
        }

        catch (Exception exc)
        {
            //Si le probleme provenait de la transaction du modele, on la Rollback
            ModeleClient modele = (ModeleClient)Session["modeleCommandes"];
            modele.RollbackTransaction();
            //et on invalide le modele, il pourra être reconstruit en PostBack
            Session["modeleCommandes"] = null;
            System.Diagnostics.Debug.Write(exc);
        }
    }
    public void ConstruireDropDownList(DropDownList liste, OleDbDataReader reader)
    {
        ListItem item = new ListItem();
        string stringToInsert = "";
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
                stringToInsert = reader[1].ToString();
                stringToInsert += " - " + reader[2].ToString();
                item.Value = reader[0].ToString();
                item.Text = stringToInsert;
                liste.Items.Add(item);
            }
        }
        if (DropDownListQuantite.Items.Count == 0)
        {
            for (int i = 1; i <= 30; i++)
            {
                item = new ListItem();
                item.Text = i.ToString();
                item.Value = i.ToString();
                DropDownListQuantite.Items.Add(item);
            }
        }
    }
    protected void ButtonConfirmer_Click(object sender, EventArgs e)
    {
        AjouterCommande();
    }
}