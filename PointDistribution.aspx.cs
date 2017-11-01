using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.OleDb;

public partial class Option_PointDistribution : System.Web.UI.Page {
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
    protected void Page_Init(object sender, EventArgs e) {
        //On ouvre pas une connexion si le modèle est déjà construit !
        //Il y a une nuance avec le postback ici car si on a frappé un problème dans le code, on aura mis le modèle à null dans la session
        //Ceci nous laisse donc une chance de le reconstruire !
        if (Session["modeleClient"] == null) {
            //Gestion des exception essentielle, on gère du code "dangereux"
            try {
                //On effectue la connexion et on l'obtient en retour
                bd = new ConnexionBD();
                OleDbConnection connection = bd.ConnectToDB(sqlDataSource1.ConnectionString);

                //On Instancie notre modèle client en lui passant la connection reçue
                modeleClient = new ModeleClient(connection);

                //Si tout a fonctionné, on stocke notre modèle dans la session. C'est la meilleure façon car un PostBack va tout effacer ce qu'on vient de faire !
                Session["modeleClient"] = modeleClient;
            }

            catch (Exception exc) {
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
    protected void Page_Load(object sender, EventArgs e) {
        //Pas besoin de lister les clients à chaque postback !
        if (!IsPostBack) {
            ListerPointDistribution();
        }
    }

    /// <summary>
    /// Déchargement de la page, Il faut faire le CommitChanges pour que nos changements soient concretement écrits dans la BD
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Unload(object sender, EventArgs e) {
        //!!!!!!!!!!!!!!!!!!!!!!!!
        //CECI EST ESSENTIEL LORSQU'ON A FAIT DES REQUETES EN MODIFICATION CAR ELLES ÉTAIENT EN MEMOIRE DANS LA TRANSACTION, IL FAUT LES ÉCRIRE DANS LA BD
        //!!!!!!!!!!!!!!!!!!!!!!!!
        try {
            if (Session["modeleClient"] != null) {
                ((ModeleClient)Session["modeleClient"]).CommitChanges();
            }
        }

        catch (Exception exc) {
            System.Diagnostics.Debug.Write(exc);
        }
    }

    /// <summary>
    /// Cette méthode liste tous les clients de la base de données et envoie les données en construction dans la table
    /// </summary>
    public void ListerPointDistribution() {
        //Gestion des exception essentielle, on gère du code "dangereux"
        try {
            //On s'assure que le modèle client est disponible
            if (Session["modeleClient"] != null) {
                //On le récupère et on demande les enregistrements des clients selon la requête passée en paramètre
                ModeleClient modele = (ModeleClient)Session["modeleClient"];

                OleDbDataReader readerSelect = modele.ReadClient("SELECT * FROM PointDistribution");

                //On envoie notre table en construction
                ConstruireListePtDistribution(readerSelect);

                //!!!!!!!!!!!!!!!!!!!!!!!!
                //CECI EST ESSENTIEL AVANT DE FAIRE UNE AUTRE REQUETE, CECI PERMETTRA UNE AUTRE
                //REQUÊTE SUR LA COMMANDE QUI A ÉTÉ OUVERTE DANS LE MODÈLE. MÊME SI C'ÉTAIT LA DERNIÈRE REQUÊTE DU LOT IL FAUT LE FAIRE !!!
                //!!!!!!!!!!!!!!!!!!!!!!!!
                readerSelect.Close();
            }
        }

        catch (Exception exc) {
            //Si le probleme provenait de la transaction du modele, on la Rollback
            ModeleClient modele = (ModeleClient)Session["modeleClient"];
            modele.RollbackTransaction();
            //et on invalide le modele, il pourra être reconstruit en PostBack
            Session["modeleClient"] = null;
            System.Diagnostics.Debug.Write(exc);
        }
    }

    public void ListerInfoPtDistribution(int cle) {
        //Gestion des exception essentielle, on gère du code "dangereux"
        try {
            //On s'assure que le modèle client est disponible
            if (Session["modeleClient"] != null) {
                //On le récupère et on demande les enregistrements des clients selon la requête passée en paramètre
                ModeleClient modele = (ModeleClient)Session["modeleClient"];

                OleDbDataReader readerSelect = modele.ReadClient("SELECT Adresse, Ville, CodePostal, Pays FROM PointDistribution WHERE IdPointDistribution =" + cle.ToString());

                //On envoie notre table en construction
                AfficherInfoPtDistribution(readerSelect);

                //!!!!!!!!!!!!!!!!!!!!!!!!
                //CECI EST ESSENTIEL AVANT DE FAIRE UNE AUTRE REQUETE, CECI PERMETTRA UNE AUTRE
                //REQUÊTE SUR LA COMMANDE QUI A ÉTÉ OUVERTE DANS LE MODÈLE. MÊME SI C'ÉTAIT LA DERNIÈRE REQUÊTE DU LOT IL FAUT LE FAIRE !!!
                //!!!!!!!!!!!!!!!!!!!!!!!!
                readerSelect.Close();
            }
        }

        catch (Exception exc) {
            //Si le probleme provenait de la transaction du modele, on la Rollback
            ModeleClient modele = (ModeleClient)Session["modeleClient"];
            modele.RollbackTransaction();
            //et on invalide le modele, il pourra être reconstruit en PostBack
            Session["modeleClient"] = null;
            System.Diagnostics.Debug.Write(exc);
        }
    }




    private void ConstruireListePtDistribution(OleDbDataReader reader) {
        while (reader.Read()) {
            ListItem item = new ListItem();
            item.Text = reader["Ville"].ToString();
            item.Value = reader["IdPointDistribution"].ToString();
            DropDownListPtDistribution.Items.Add(item);
        }
    }


    protected void DropDownListPtDistribution_SelectedIndexChanged(object sender, EventArgs e) {
        int clePrimaire = DropDownListPtDistribution.SelectedIndex;
        if (clePrimaire > 0) {
            ListerInfoPtDistribution(clePrimaire);
        }
        else {
            CoordonneesTextBox.Text = "";
            VilleTextBox.Text = "";
            PaysTextBox.Text = "";
            CodePostalTextBox.Text = "";
            Literal1.Text = "";
        }
    }


    private void AfficherInfoPtDistribution(OleDbDataReader reader) {
        reader.Read();
        CoordonneesTextBox.Text = reader["Adresse"].ToString();
        VilleTextBox.Text = reader["Ville"].ToString();
        PaysTextBox.Text = reader["Pays"].ToString();
        CodePostalTextBox.Text = reader["CodePostal"].ToString();

        Literal1.Text = @"<iframe src=""https://www.google.ca/maps/place/" + VilleTextBox.Text + @""" width=""600"" height=""450""></iframe>";
    }
}