using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.OleDb;

public partial class Option_Commentaire : System.Web.UI.Page
{
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
    //Cette méthode effectue une mise à jour, mais "en memoire" dans la BD. Pour un impact réel dans le fichier de base de données
    //Il ne faut pas oublier d'appler le CommitChanges au déchargement de la page
    /// </summary>
    public void AjouterCommentaire(string nom, string prenom, string commentaire) {
        //Gestion des exception essentielle, on gère du code "dangereux"
        try {
            //On s'assure que le modèle client est disponible
            if (Session["modeleClient"] != null) {
                //On le récupère et on demande les enregistrements des clients selon la requête passée en paramètre
                ModeleClient modele = (ModeleClient)Session["modeleClient"];

                //On peut maintenant faire notre requete
                int numRows = modele.CreateClient("INSERT INTO Commentaire (nomVisiteur, prenomVisiteur, commentaireVisiteur, dateCreation, clientDestine) VALUES ('" + nom + "', '" + prenom + "', '" + commentaire + "', NOW(), '4')");
                if (numRows >= 1)
                {
                    //message de succès à l'écran
                    Label1.Visible = false;
                    Label2.Visible = false;
                    Label3.Visible = false;
                    TextBox1.Visible = false;
                    TextBox2.Visible = false;
                    TextBox3.Visible = false;
                    RequiredFieldValidator1.Visible = false;
                    RequiredFieldValidator2.Visible = false;
                    RequiredFieldValidator3.Visible = false;
                    Button1.Visible = false;
                    Label4.Text = "Opération réussie !";
                    
                }

                else
                {
                    Label1.Visible = false;
                    Label2.Visible = false;
                    Label3.Visible = false;
                    TextBox1.Visible = false;
                    TextBox2.Visible = false;
                    TextBox3.Visible = false;
                    RequiredFieldValidator1.Visible = false;
                    RequiredFieldValidator2.Visible = false;
                    RequiredFieldValidator3.Visible = false;
                    Button1.Visible = false;
                    Label4.Text = "Erreur lors de l'envoi du commentaire !";
                    //message d'erreur à l'écran
                }
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



    protected void Button1_Click(object sender, EventArgs e) {
        if(TextBox1.Text != "" && TextBox2.Text != "" && TextBox3.Text != "") {
            AjouterCommentaire(TextBox1.Text, TextBox2.Text, TextBox3.Text);
        }
    }
}