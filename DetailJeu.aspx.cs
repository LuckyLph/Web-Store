using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.OleDb;

public partial class Accueil_DetailJeu : System.Web.UI.Page {
    //propriété qui représente notre classe de connexion et qui offre une passerelle vers la vraie connexion avec le pilote
    ConnexionBD bd = null;
    //propriété qui représente l'accès aux données (CRUD) pour un client
    ModeleClient modeleClient = null;

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

    protected void Page_Load(object sender, EventArgs e) {
        Session["CodeJeu"] = Request.QueryString.ToString();
        string[] codeJeuTab = Session["CodeJeu"].ToString().Split('=');
        string codeJeu = codeJeuTab[1];
        //Pas besoin de lister les clients à chaque postback !
        if (!IsPostBack) {
            //ListerGenreJeu();
            //ListerJeu(-1);
            //ListerNouveauxJeux();
            //ListerPointDistribution();
            ListerJeu(codeJeu);
        }
        
    }

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

    public void ListerJeu(string CodeJeu) {
        //Gestion des exception essentielle, on gère du code "dangereux"
        try {
            //On s'assure que le modèle client est disponible
            if (Session["modeleClient"] != null) {
                //On le récupère et on demande les enregistrements des clients selon la requête passée en paramètre
                ModeleClient modele = (ModeleClient)Session["modeleClient"];

                OleDbDataReader readerSelect = modele.ReadClient("SELECT Jeu.Titre, Jeu.Plateforme, Jeu.Prix, Jeu.Image, Jeu.Description, TypeJeu.Genre FROM Jeu INNER JOIN TypeJeu ON Jeu.IdGenre=TypeJeu.IdGenre WHERE Jeu.CodeJeu=" + CodeJeu);

                //On envoie notre table en construction
                //ConstruireListePtDistribution(readerSelect);
                AfficherJeu(readerSelect);

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


    private void AfficherJeu(OleDbDataReader reader) {
        Label1.Text = reader["Titre"].ToString();
        Label2.Text = reader["Genre"].ToString();
        Label3.Text = reader["Plateforme"].ToString();
        Label4.Text = reader["Prix"].ToString();
        Label5.Text = reader["Description"].ToString();
    }
}