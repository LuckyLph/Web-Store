using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.OleDb;

public partial class Administration : System.Web.UI.Page
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
        if (Session["modeleJeu"] == null)
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
                Session["modeleJeu"] = modeleClient;
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
        ListerJeux();
        HyperLinkAdministration.NavigateUrl = "Commander.aspx";
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
            if (Session["modeleJeu"] != null)
            {
                ((ModeleClient)Session["modeleJeu"]).CommitChanges();
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
    public void ListerJeux()
    {
        //Gestion des exception essentielle, on gère du code "dangereux"
        try
        {
            //On s'assure que le modèle client est disponible
            if (Session["modeleJeu"] != null)
            {
                //On le récupère et on demande les enregistrements des clients selon la requête passée en paramètre
                ModeleClient modele = (ModeleClient)Session["modeleJeu"];
                OleDbDataReader readerSelect;

                //On envoie notre DropDownList en construction
                if (!IsPostBack)
                    ConstruireDropDownList(DropDownListAdmin);

                if (DropDownListAdmin.SelectedValue == "0")
                {
                    readerSelect = modele.ReadClient("SELECT CommandeJeu.CodeJeu, Jeu.Titre, CommandeJeu.Quantité, CommandeJeu.DateCommande, Jeu.Prix, Entrepot.Ville, Entrepot.Pays, CommandeJeu.IdCommande FROM Jeu INNER JOIN (CommandeJeu INNER JOIN Entrepot ON CommandeJeu.IdEntrepot = Entrepot.IdEntrepot) ON CommandeJeu.CodeJeu = Jeu.CodeJeu ORDER BY CommandeJeu.DateCommande ASC");
                }
                else
                {
                    readerSelect = modele.ReadClient("SELECT CommandeJeu.CodeJeu, Jeu.Titre, CommandeJeu.Quantité, CommandeJeu.DateCommande, Jeu.Prix, Entrepot.Ville, Entrepot.Pays, CommandeJeu.IdCommande FROM Jeu INNER JOIN (CommandeJeu INNER JOIN Entrepot ON CommandeJeu.IdEntrepot = Entrepot.IdEntrepot) ON CommandeJeu.CodeJeu = Jeu.CodeJeu ORDER BY CommandeJeu.DateCommande DESC");
                }
                //On envoie notre table en construction
                ConstruireTable(TableAdmin, readerSelect);

                //!!!!!!!!!!!!!!!!!!!!!!!!
                //CECI EST ESSENTIEL AVANT DE FAIRE UNE AUTRE REQUETE, CECI PERMETTRA UNE AUTRE
                //REQUÊTE SUR LA COMMANDE QUI A ÉTÉ OUVERTE DANS LE MODÈLE. MÊME SI C'ÉTAIT LA DERNIÈRE REQUÊTE DU LOT IL FAUT LE FAIRE !!!
                //!!!!!!!!!!!!!!!!!!!!!!!!
                readerSelect.Close();
            }
        }

        catch (Exception exc)
        {
            //Si le probleme provenait de la transaction du modele, on la Rollback
            ModeleClient modele = (ModeleClient)Session["modeleJeu"];
            modele.RollbackTransaction();
            //et on invalide le modele, il pourra être reconstruit en PostBack
            Session["modeleJeu"] = null;
            System.Diagnostics.Debug.Write(exc);
        }
    }


    /// <summary>
    /// Méthode qui construit la table dynamique située dans le formulaire .aspx
    /// </summary>
    /// <param name="table">l'objet table que l'on va construire dynamiquement</param>
    /// <param name="reader">le reader qui contient tous les enregistrements retenus par la requête de sélection</param>
    public void ConstruireTable(Table table, OleDbDataReader reader)
    {
        TableRow headerRow = new TableRow();
        TableHeaderCell cell = null;

        //extraction des champs pour créer l'entete
        for (int champ = 0; champ < reader.FieldCount; champ++)
        {
            cell = new TableHeaderCell();
            //On récupère le nom des champs ici
            cell.Text = reader.GetName(champ);
            //et ils deviennent le texte de nos entêtes
            headerRow.Cells.Add(cell);
        }

        //ajout d'une colonne qui ne vient pas de la base de données !
        cell = new TableHeaderCell();
        cell.Text = "Commandes";
        headerRow.Cells.Add(cell);

        //On ajoute la ligne des entêtes dans la table
        table.Rows.Add(headerRow);

        //extraction des enregistrements, on boucle le reader
        while (reader.Read())
        {
            TableRow row = new TableRow();
            TableCell tableCell = null;

            //Attention, on ne se rendra pas jusqu'au bout car la dernière entete ne provient pas de la BD !!!
            int numColumns = headerRow.Cells.Count - 1;

            //on se sert des noms de nos entetes comme clé pour récupérer nos enregistrements, encore une fois en ignorant la dernière qui a été créée manuellement !
            for (int i = 0; i < numColumns; i++)
            {
                tableCell = new TableCell();
                //on se sert du nom de l'entête ici comme clé
                tableCell.Text = reader[headerRow.Cells[i].Text].ToString();
                row.Cells.Add(tableCell);
            }

            //On popule la dernière colonne en créant un hyperlien qui enverra le id en GET
            HyperLink link = new HyperLink();
            //La valeur du id se trouve dans la première cellule de la rangée courante, d'où le [0] pour aller le chercher
            link.NavigateUrl = "ModiferCommande.aspx?id=" + row.Cells[7].Text;
            link.Text = "Modifier";

            //L'hyperlien étant un contrôle, on l'ajoute dans la cellule
            tableCell = new TableCell();
            tableCell.Controls.Add(link);
            row.Cells.Add(tableCell);

            //Et on ajoute notre ligne dans la table, on recommencera ce traitement pour tous les enregistrements
            table.Rows.Add(row);
        }
    }


    public void ConstruireDropDownList(DropDownList liste)
    {
        ListItem item = new ListItem();
        item.Value = "0";
        item.Text = "Croissant";
        liste.Items.Add(item);

        item = new ListItem();
        item.Value = "1";
        item.Text = "Decroissant";
        liste.Items.Add(item);
    }
}