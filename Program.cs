

using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using SkiaSharp;
using MySql.Data.MySqlClient;



/// <summary>
/// Classe principale du programme
/// </summary>
class Program
{
    /// <summary>
    /// chaîne de connexion à la base de données MySQL locale
    /// </summary>
    static string connectionString = "server=localhost;database=premierRenduPSI;user=root;password=Xiang92310;";

    static void Main(string[] args)
    {
        while (true)
        {
            // affiche le menu principal
            Console.WriteLine("\n*** Liv'in Paris metro ***");
            Console.WriteLine("1) Connexion");
            Console.WriteLine("2) Création compte");
            Console.WriteLine("3) Quitter");
            Console.Write("Choisissez une option : ");
            string choix = Console.ReadLine();

            // gère le choix de l'utilisateur
            switch (choix)
            {
                case "1":
                    Connexion();
                    break;
                case "2":
                    CreerCompte();
                    break;
                case "3":
                    Console.WriteLine("Au revoir, à bientôt!");
                    return;
                default:
                    Console.WriteLine("Option invalide, veuillez réessayer :(");
                    break;
                    //case "4":

            }
        }
    }

    /// <summary>
    /// permet à un utilisateur de se connecter (client ou cuisinier)
    /// </summary>
    static void Connexion()
    {
        while (true)
        {
            // demande des identifiants
            Console.Write("\nEmail : ");
            string email = Console.ReadLine();
            Console.Write("Mot de passe : ");
            string mdp = Console.ReadLine();

            // connexion à la base
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                // vérifie les identifiants dans les deux tables
                string requeteSql = @"SELECT 'Client' AS Role, idClient AS idUtilisateur, nom, prenom  FROM Client WHERE email = @Email AND motDePasse = @mdp
                                  UNION
                                  SELECT 'Cuisinier', idCuisinier AS idUtilisateur, nom, prenom FROM Cuisinier WHERE email = @Email AND motDePasse = @mdp"; 

                MySqlCommand commandesql = new MySqlCommand(requeteSql, connection); 
                commandesql.Parameters.AddWithValue("@Email", email); // paramètre sécurisé
                commandesql.Parameters.AddWithValue("@mdp", mdp); 

                using (MySqlDataReader lecteur = commandesql.ExecuteReader())
                {
                    if (lecteur.Read()) // si un utilisateur est trouvé
                    {
                        string role = lecteur["Role"].ToString();
                        int idUtilisateur = Convert.ToInt32(lecteur["idUtilisateur"]);
                        string prenom = lecteur["prenom"].ToString();
                        string nom = lecteur["nom"].ToString();

                        // message de bienvenue
                        Console.WriteLine($"\nBienvenue, {prenom} {nom} ({role}) !");
                        lecteur.Close();


                        // redirige vers le bon menu
                        if (role == "Client")
                            MenuClient(idUtilisateur);
                        else
                            MenuCuisinier(idUtilisateur);

                        return; // sortie de la méthode après connexion
                    }
                    else
                    {
                        Console.WriteLine("Email ou mot de passe incorrect.");
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// permet à un utilisateur de créer un compte client ou cuisinier
    /// </summary>
    static void CreerCompte()
    {
        Console.Write("\nVous êtes : 1) Client  2) Cuisinier\nChoix : ");
        string role = Console.ReadLine();
        
        // saisie des infos personnelles
        Console.Write("Nom : ");
        string nom = Console.ReadLine();
        Console.Write("Prénom : ");
        string prenom = Console.ReadLine();
        Console.Write("Email : ");
        string email = Console.ReadLine();
        Console.Write("Mot de passe : ");
        string mdp = Console.ReadLine();
        Console.Write("rue : ");
        string rue = Console.ReadLine();
        Console.Write("numMaison : ");
        string numMaison = Console.ReadLine();
        Console.Write("code Postal ? : ");
        string codePostal = Console.ReadLine();
        Console.Write("numéro de téléphone : ");
        string numTel = Console.ReadLine();
        Console.Write("Ville de résidence : ");
        string ville = Console.ReadLine();
        Console.Write("Le métro le plus proche de chez vous: ");
        string metroProche = Console.ReadLine();
        int totalCommande = 0; // initialisation à 0 pour un nouvel utilisateur

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            string requetesql = "";

            if (role == "1")  // Inscription en tant que Client
            {
                requetesql = "INSERT INTO Client (nom, prenom, email, motDePasse, rue, numMaison, codePostal, numTel, ville, totalCommande, metroProche) VALUES (@Nom, @Prenom, @Email, @motDePasse, @rue, @numMaison, @codePostal, @numTel, @ville, @totalCommande, @metroProche)";
            }
            else if (role == "2")  // Inscription en tant que Cuisinier
            {
                requetesql = "INSERT INTO Cuisinier (nom, prenom, email, motDePasse, rue, numMaison, codePostal, numTel, ville, totalCommande, metroProche) VALUES (@Nom, @Prenom, @Email, @motDePasse, @rue, @numMaison, @codePostal, @numTel, @ville, @totalCommande, @metroProche)";
            }
            else
            {
                Console.WriteLine("Choix invalide.");
                return;
            }

            // preparation de la requete SQL
            MySqlCommand commandesql = new MySqlCommand(requetesql, connection);
            commandesql.Parameters.AddWithValue("@Nom", nom);
            commandesql.Parameters.AddWithValue("@Prenom", prenom);
            commandesql.Parameters.AddWithValue("@Email", email);
            commandesql.Parameters.AddWithValue("@motDePasse", mdp);
            commandesql.Parameters.AddWithValue("@rue", rue);
            commandesql.Parameters.AddWithValue("@numMaison", numMaison);
            commandesql.Parameters.AddWithValue("@codePostal", codePostal);
            commandesql.Parameters.AddWithValue("@numTel", numTel);
            commandesql.Parameters.AddWithValue("@ville", ville);
            commandesql.Parameters.AddWithValue("@totalCommande", totalCommande);
            commandesql.Parameters.AddWithValue("@metroProche", metroProche);


            // execution de la requete
            int ligneaff = commandesql.ExecuteNonQuery();
            if (ligneaff > 0)
            {
                Console.WriteLine("Compte créé avec succès !");
            }
            else
            {
                Console.WriteLine("Erreur lors de la création du compte.");
            }


        }
    }

    /// <summary>
    /// menu affiché pour les utilisateurs clients connectés
    /// </summary>
    /// <param name="idClient"></param>
    static void MenuClient(int idClient)
    {
        List<string> platsCommandes = new List<string>(); // Stocker les noms des plats commandés
        double totalPrix = 0; // Stocke le total des commandes

        while (true)
        {
            // affiche le menu aux clients
            Console.WriteLine("\n*** Menu Client ***");
            Console.WriteLine("1) Ajouter un plat à la commande");
            Console.WriteLine("2) Voir les cuisiniers disponibles");
            Console.WriteLine("3) Régler la commande");
            Console.WriteLine("4) Se déconnecter");
            Console.Write("Choisissez une option : ");
            string choix = Console.ReadLine();

            switch (choix)
            {
                case "1":
                    totalPrix += AjouterPlatCommande(platsCommandes); // ajoute un plat à la commande
                    break;
                case "2":
                    VoirCuisiniers(); // affiche les cuisiniers dispos
                    break;
                case "3":
                    ReglerCommandes(idClient, platsCommandes, totalPrix); // procède au paiement 
                    platsCommandes.Clear(); // Réinitialiser après paiement
                    totalPrix = 0;
                    break;
                case "4":
                    return;  // deconnexion
                default:
                    Console.WriteLine("Option invalide, veuillez réessayer.");
                    break;
            }
        }
    }

    /// <summary>
    /// menu affiché pour les utilisateurs cuisiniers connectés
    /// </summary>
    /// <param name="idCuisinier"></param>
    static void MenuCuisinier(int idCuisinier)
    {
        while (true)
        {
            // affiche le menu
            Console.WriteLine("\n*** Menu Cuisinier ***");
            Console.WriteLine("1) Modifier mon menu");
            Console.WriteLine("2) Voir mes plats");
            Console.WriteLine("3) Voir mes clients");
            Console.WriteLine("4) Voir les commandes à préparer");
            Console.WriteLine("5) Mettre à jour le statut d'une commande");
            Console.WriteLine("6) Voir les commandes réalisées");
            Console.WriteLine("7) Se déconnecter");
            Console.Write("Choisissez une option : ");
            string choix = Console.ReadLine();

            switch (choix)
            {
                case "1":
                    ModifierMenu(idCuisinier);  // lancer la modif du menu
                    break;
                case "2":
                    VoirPlats(idCuisinier);   // afficher les plats
                    break;
                case "3":
                    VoirClients(idCuisinier); // voir liste des clients
                    break;
                case "4":
                    VoirCommandesAPreparer(idCuisinier); // voir commandes en attente
                    break;
                case "5":
                    Mettreàjourcommande(); // mise à jour du statut d'une commande
                    break;
                case "6":
                    VoirCommandesRealisee(idCuisinier); // voir commandes terminées
                    break;
                case "7":
                    return; // deconnexion
                default:
                    Console.WriteLine("Option invalide, veuillez réessayer.");
                    break;
            }
        }
    }

    /// <summary>
    /// permet au cuisinier d'ajouter un plat à son menu
    /// </summary>
    /// <param name="idCuisinier"></param>
    static void ModifierMenu(int idCuisinier)
    {
        // saisie des infos du plat
        Console.Write("\nNom du plat : ");
        string nomPlat = Console.ReadLine();
        Console.Write("Régime alimentaire : ");
        string regime = Console.ReadLine();
        Console.Write("Prix en euro : ");
        double prix = double.Parse(Console.ReadLine());
        Console.Write("Nationalité : ");
        string nationalite = Console.ReadLine();
        Console.Write("Date de fabrication (AAAA-MM-JJ) : ");
        string dateFabrication = Console.ReadLine();
        Console.Write("Date de péremption (AAAA-MM-JJ) : ");
        string datePeremption = Console.ReadLine();

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            // Insertion du plat avec idCuisinier
            string requetesql = "INSERT INTO Plat (nomPlat, regime, prix, nationalite, dateFabrication, datePeremption, idCuisinier) " +
                           "VALUES (@nomPlat, @regime, @prix, @nationalite, @dateFabrication, @datePeremption, @idCuisinier)";

            MySqlCommand commandesql = new MySqlCommand(requetesql, connection);
            commandesql.Parameters.AddWithValue("@nomPlat", nomPlat);
            commandesql.Parameters.AddWithValue("@regime", regime);
            commandesql.Parameters.AddWithValue("@prix", prix);
            commandesql.Parameters.AddWithValue("@nationalite", nationalite);
            commandesql.Parameters.AddWithValue("@dateFabrication", dateFabrication);
            commandesql.Parameters.AddWithValue("@datePeremption", datePeremption);
            commandesql.Parameters.AddWithValue("@idCuisinier", idCuisinier);

            int ligneaff = commandesql.ExecuteNonQuery(); // exécution
            if (ligneaff > 0)
            {
                Console.WriteLine("Plat ajouté !");
            }
            else
            {
                Console.WriteLine("Erreur lors de l'ajout du plat");
            }

            // Récupérer l'ID du plat inséré
            long idPlat = commandesql.LastInsertedId;

            // Ajout des ingrédients
            AjouterIngredients(idPlat, connection);

            connection.Close();
        }
    }

    /// <summary>
    /// affiche tous les plats d'un cuisinier donné
    /// </summary>
    /// <param name="idCuisinier"></param>
    static void VoirPlats(int idCuisinier)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            // Requête pour récupérer les plats du cuisinier avec idPlat
            string requetesql = "SELECT idPlat, nomPlat, regime, nationalite, prix FROM Plat WHERE idCuisinier = @idCuisinier;";
            MySqlCommand commandesql = new MySqlCommand(requetesql, connection);
            commandesql.Parameters.AddWithValue("@idCuisinier", idCuisinier);

            using (MySqlDataReader lecteur = commandesql.ExecuteReader())
            {
                if (!lecteur.HasRows)
                {
                    Console.WriteLine("Aucun plat trouvé pour ce cuisinier.");
                }
                else
                {
                    Console.WriteLine("\n--- Liste des plats ---");
                    while (lecteur.Read())
                    {
                        // lecture des informations
                        string nomPlat = lecteur["nomPlat"].ToString();
                        string regime = lecteur["regime"].ToString();
                        double prix = Convert.ToDouble(lecteur["prix"]);
                        string nationalite = lecteur["nationalite"].ToString();
                        int idPlat = Convert.ToInt32(lecteur["idPlat"]); // Récupérer idPlat

                        // Affichage des informations du plat
                        Console.WriteLine($"\nNom : {nomPlat}, Régime : {regime}, Prix : {prix} euro");
                        Console.WriteLine($"Nationalité : {nationalite}");

                        // Récupérer les ingrédients pour ce plat
                        List<string> ingredients = RecupererIngredients(idPlat);
                        if (ingredients.Count > 0)
                        {
                            Console.WriteLine($"Ingrédients : {string.Join(", ", ingredients)}");
                        }
                        else
                        {
                            Console.WriteLine("Ingrédients : Aucun");
                        }
                    }
                }
            }

            connection.Close();
        }
    }


    /// <summary>
    /// Affiche les cuisiniers dipsos et leurs plats
    /// </summary>
    static void VoirCuisiniers()
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            // jointure entre cuisinier et plat
            string requetesql = @"SELECT Cuisinier.nom AS nomCuisinier, Plat.idPlat, Plat.nomPlat, Plat.regime, Plat.prix FROM Cuisinier JOIN Plat ON Plat.idCuisinier = Cuisinier.idCuisinier";// AS pour éviter les confusions

            MySqlCommand commandesql = new MySqlCommand(requetesql, connection);
            MySqlDataReader lecteur = commandesql.ExecuteReader();

            Console.WriteLine("\n*** Cuisiniers Disponibles ***");
            while (lecteur.Read())
            {
                // lecture des données
                int idPlat = Convert.ToInt32(lecteur["idPlat"]);
                string nomCuisinier = lecteur["nomCuisinier"].ToString();
                string nomPlat = lecteur["nomPlat"].ToString();
                string regime = lecteur["regime"].ToString();
                double prix = Convert.ToDouble(lecteur["prix"]);

                // affichage 
                Console.WriteLine($"\nNom: {nomCuisinier}, Plat: {nomPlat}, Régime: {regime}, Prix: {prix} euro");

                // ingrédients du plat
                List<string> ingredients = RecupererIngredients(idPlat);
                if (ingredients.Count > 0)
                    Console.WriteLine($"Ingrédients : {string.Join(", ", ingredients)}");
                else
                    Console.WriteLine("Ingrédients : Aucun");
            }
        }
    }

    /// <summary>
    /// retourne la liste des ingrédients associés à un plat
    /// </summary>
    /// <param name="idPlat"></param>
    /// <returns></returns>
    static List<string> RecupererIngredients(int idPlat)
    {
        List<string> ingredients = new List<string>();

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            string requetesql = "SELECT nom FROM Ingredient WHERE idPlat = @idPlat";
            MySqlCommand commandesql = new MySqlCommand(requetesql, connection);
            commandesql.Parameters.AddWithValue("@idPlat", idPlat);

            using (MySqlDataReader lecteur = commandesql.ExecuteReader())
            {
                while (lecteur.Read())
                {
                    ingredients.Add(lecteur["nom"].ToString());
                }
            }
        }

        return ingredients;
    }

    /// <summary>
    /// permet au cuisinier de voir les clients qui ont commandé ses plats
    /// </summary>
    /// <param name="idCuisinier"></param>
    static void VoirClients(int idCuisinier)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            string requetesql = @"SELECT DISTINCT client.nom, client.prenom, client.email, client.numTel FROM Client JOIN Commande ON Client.idClient = Commande.idClient WHERE Commande.idCuisinier = @idCuisinier;";

            MySqlCommand commandesql = new MySqlCommand(requetesql, connection);
            commandesql.Parameters.AddWithValue("@idCuisinier", idCuisinier);

            using (MySqlDataReader lecteur = commandesql.ExecuteReader())
            {
                if (!lecteur.HasRows)
                {
                    Console.WriteLine("Aucun client trouvé.");
                }
                else
                {
                    Console.WriteLine("\n*** Liste des clients ***");
                    while (lecteur.Read())
                    {
                        Console.WriteLine($"Nom: {lecteur["nom"]}, Prénom: {lecteur["prenom"]}, Email: {lecteur["email"]}, Téléphone: {lecteur["numTel"]}");
                    }
                }
            }
        }
    }

    /// <summary>
    /// Ajoute un plat à la commande si le plat existe dans la base.
    /// </summary>
    /// <param name="platsCommandes"></param>
    /// <returns></returns>
    static double AjouterPlatCommande(List<string> platsCommandes)
    {
        Console.Write("\nEntrez le nom du plat que vous souhaitez commander : ");
        string nomPlat = Console.ReadLine();

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            // Vérifier si le plat existe dans la base 
            string requetesql = "SELECT prix FROM Plat WHERE nomPlat = @nomPlat";
            MySqlCommand commandesql = new MySqlCommand(requetesql, connection);
            commandesql.Parameters.AddWithValue("@nomPlat", nomPlat);

            using (MySqlDataReader lecteur = commandesql.ExecuteReader())
            {
                if (!lecteur.Read()) // Si aucun plat trouvé
                {
                    Console.WriteLine("Plat inexistant, veuillez recommencer.");
                    return 0;
                }

                double prix = Convert.ToDouble(lecteur["prix"]);
                platsCommandes.Add(nomPlat);

                Console.WriteLine($"Plat ajouté : {nomPlat} ({prix} euro)");
                return prix;
            }
        }
    }

    /// <summary>
    /// Finalise la commande d’un client en l’enregistrant dans la base de données
    /// </summary>
    /// <param name="idClient"></param>
    /// <param name="platsCommandes"></param>
    /// <param name="totalPrix"></param>
    static void ReglerCommandes(int idClient, List<string> platsCommandes, double totalPrix)
    {
        if (platsCommandes.Count == 0)
        {
            Console.WriteLine("Aucun plat commandé.");
            return;
        }

        string nomPlat = platsCommandes[0];  // on prend juste le premier plat

        // affichage du récapitulatif
        Console.WriteLine("\n*** Récapitulatif de la commande ***");
        Console.WriteLine($"Plat : {nomPlat}");
        Console.WriteLine($"Total à payer : {totalPrix} euro");

        // demande d'un commentaire facultatif
        Console.Write("\nSouhaitez-vous ajouter un commentaire pour le cuisinier ? (oui/non) : ");
        string reponse = Console.ReadLine().ToLower();
        string commentaire = "";

        if (reponse == "oui")
        {
            Console.Write("Écrivez votre commentaire (max 250 caractères) : ");
            commentaire = Console.ReadLine();
            if (commentaire.Length > 250)
            {
                commentaire = commentaire.Substring(0, 250);
                Console.WriteLine("Commentaire trop long, il a été tronqué.");
            }
        }

        Console.Write("\nConfirmez-vous le paiement ? (oui/non) : ");
        string confirmation = Console.ReadLine().ToLower();

        if (confirmation != "oui")
        {
            Console.WriteLine("Paiement annulé.");
            return;
        }

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            // récupère l’ID du cuisinier responsable du plat
            string requetecuisinier = "SELECT idCuisinier FROM Plat WHERE nomPlat = @nomPlat";
            MySqlCommand commandecuisinier = new MySqlCommand(requetecuisinier, connection);
            commandecuisinier.Parameters.AddWithValue("@nomPlat", nomPlat);

            object resultat = commandecuisinier.ExecuteScalar();

            if (resultat == null)
            {
                Console.WriteLine("Ce plat n'existe pas !");
                return;
            }

            int idCuisinier = Convert.ToInt32(resultat);

            // insertion dans la table Commande
            string requetesql = @"INSERT INTO Commande (nom, prix, tempsPreparation, statut, date, idClient, commentaire, idCuisinier)
                               VALUES (@nom, @prix, @tempsPreparation, @statut, @date, @idClient, @commentaire, @idCuisinier)";

            MySqlCommand commandesql = new MySqlCommand(requetesql, connection);
            commandesql.Parameters.AddWithValue("@nom", nomPlat);
            commandesql.Parameters.AddWithValue("@prix", totalPrix);
            commandesql.Parameters.AddWithValue("@tempsPreparation", 30); // temporaire
            commandesql.Parameters.AddWithValue("@statut", "en attente");
            commandesql.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd"));
            commandesql.Parameters.AddWithValue("@idClient", idClient);
            commandesql.Parameters.AddWithValue("@commentaire", commentaire);
            commandesql.Parameters.AddWithValue("@idCuisinier", idCuisinier);

            commandesql.ExecuteNonQuery();

            Console.WriteLine("\nPaiement effectué avec succès !");
            Console.WriteLine("Merci pour votre commande !");
        }
    }


    /// <summary>
    /// permet d’ajouter des ingrédients à un plat
    /// </summary>
    /// <param name="idPlat"></param>
    /// <param name="connection"></param>
    static void AjouterIngredients(long idPlat, MySqlConnection connection)
    {
        while (true)
        {
            Console.Write("\nNom de l'ingrédient (ou taper 'fin' pour arrêter) : ");
            string nomIngredient = Console.ReadLine();
            if (nomIngredient.ToLower() == "fin") 
                break;

           
            Console.Write("Origine : ");
            string origine = Console.ReadLine();
          
            // insertion de l’ingrédient
            string requetesql = "INSERT INTO ingredient (nom,  origine, idPlat) " +
                           "VALUES (@nom, @origine, @idPlat)";

            MySqlCommand commandesql = new MySqlCommand(requetesql, connection);
            commandesql.Parameters.AddWithValue("@nom", nomIngredient);
            commandesql.Parameters.AddWithValue("@origine", origine);
            commandesql.Parameters.AddWithValue("@idPlat", idPlat);

            commandesql.ExecuteNonQuery();
            Console.WriteLine("Ingrédient ajouté !");
        }
    }

    /// <summary>
    /// Affiche les commandes en attente assignées à un cuisinier
    /// </summary>
    /// <param name="idCuisinier"></param>
    static void VoirCommandesAPreparer(int idCuisinier)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            string requetesql = "SELECT idCommande, nom, prix, statut, date, idClient, commentaire FROM commande WHERE (idCuisinier = @idCuisinier) and commande.statut='en attente'";

            MySqlCommand commandesql = new MySqlCommand(requetesql, connection);
            commandesql.Parameters.AddWithValue("@idCuisinier", idCuisinier);

            using (MySqlDataReader lecteur = commandesql.ExecuteReader())
            {
                if (!lecteur.HasRows)
                {
                    Console.WriteLine("Aucune commande à préparer.");
                    return;
                }

                Console.WriteLine("\nCommandes à préparer :");
                while (lecteur.Read())
                {
                    Console.WriteLine($"Commande #{lecteur["idCommande"]} - {lecteur["nom"]} - {lecteur["prix"]} euro");
                    Console.WriteLine($"Client: {lecteur["idClient"]} | Statut: {lecteur["statut"]} | Date: {lecteur["date"]}"); // mettre le temps estimée et la distance
                    Console.WriteLine($"Commentaire: {lecteur["commentaire"]}\n");
                }
            }
            connection.Close();
        }
    }
 
    /// <summary>
    /// Affiche les commandes déjà réalisées par un cuisinier 
    /// </summary>
    /// <param name="idCuisinier"></param>
    static void VoirCommandesRealisee(int idCuisinier)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            string requetesql = "SELECT idCommande, nom, prix, statut, date, idClient, commentaire FROM commande WHERE (idCuisinier = @idCuisinier) not in (commande.statut='en attente')";

            MySqlCommand commandesql = new MySqlCommand(requetesql, connection);
            commandesql.Parameters.AddWithValue("@idCuisinier", idCuisinier);

            using (MySqlDataReader lecteur = commandesql.ExecuteReader())
            {
                if (!lecteur.HasRows)
                {
                    Console.WriteLine("Aucune commande à préparer.");
                    return;
                }

                Console.WriteLine("\nCommandes à préparer :");
                while (lecteur.Read())
                {
                    Console.WriteLine($"Commande #{lecteur["idCommande"]} - {lecteur["nom"]} - {lecteur["prix"]} euro");
                    Console.WriteLine($"Client: {lecteur["idClient"]} | Statut: {lecteur["statut"]} | Date: {lecteur["date"]}"); 
                    Console.WriteLine($"Commentaire: {lecteur["commentaire"]}\n");
                }
            }
            connection.Close();
        }
    }

    /// <summary>
    /// met à jour le statut d'une commande
    /// </summary>
    static void Mettreàjourcommande()
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            Console.WriteLine("Quelle est la commande à mettre à jour ?");
            int idCommande = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Quel est le nouveau statut de la commande ?");
            string nouveauStatut = Console.ReadLine();
            connection.Open();
            string requetesql = "UPDATE Commande SET statut = @statut WHERE idCommande =@idCommande ";
            MySqlCommand commandesql = new MySqlCommand(requetesql, connection);
            commandesql.Parameters.AddWithValue("@idCommande", idCommande);
            commandesql.Parameters.AddWithValue("@statut", nouveauStatut);
          
            int ligneaff = commandesql.ExecuteNonQuery();
            if (ligneaff > 0)
            {
                Console.WriteLine("Commande mise à jour avec succès ");
            }
            else
            {
                Console.WriteLine("Erreur lors de la mise à jour de la commande");
            }
        }
    }


    /// <summary>
    /// génère et affiche le graphe du métro parisien à partir du fichier Excel
    /// </summary>
    static void résultatGraphe()
    {
        // Charger les données depuis Excel
        var noeuds = ChargerNoeuds("MetroParis(1).xlsx");
        var graphe = new Graphe<string>();

        // Ajouter les noeuds au graphe
        foreach (var noeud in noeuds.Values)
        {
            graphe.AjouterNoeud(noeud);
        }

        // Charger et créer les liens
        var arcs = ChargerArcs("MetroParis(1).xlsx", noeuds);
        foreach (var arc in arcs)
        {
            graphe.AjouterLien(arc.Item1, arc.Item2, arc.Item3);
        }

        // Afficher le graphe avec SkiaSharp
        AfficherGraphe(graphe, "metro_paris.png");

    }

    /// <summary>
    /// Charge les noeuds (stations) 
    /// </summary>
    /// <param name="fichierExcel"></param>
    /// <returns></returns>
    static Dictionary<int, Noeud<string>> ChargerNoeuds(string fichierExcel)
    {
        var noeuds = new Dictionary<int, Noeud<string>>();
        string connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={fichierExcel};Extended Properties='Excel 12.0;HDR=YES;IMEX=1'";

        using (OleDbConnection connection = new OleDbConnection(connectionString))
        {
            connection.Open();
            OleDbCommand command = new OleDbCommand("SELECT * FROM [Noeuds$]", connection);
            OleDbDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                try
                {
                    int id = int.Parse(reader[0].ToString());
                    string libelleLigne = reader[1].ToString();
                    string libelleStation = reader[2].ToString();

                    // Conversion directe avec culture invariante
                    double longitude = double.Parse(reader[3].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                    double latitude = double.Parse(reader[4].ToString(), System.Globalization.CultureInfo.InvariantCulture);

                    string commune = reader[5].ToString();
                    string codeInsee = reader[6].ToString();

                    // ajout du noeud dans le dictionnaire
                    noeuds.Add(id, new Noeud<string>(id, libelleStation, libelleLigne, longitude, latitude, commune, codeInsee));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur ligne {noeuds.Count + 2}: {ex.Message}");
                }
            }
        }
        return noeuds;
    }


    /// <summary>
    /// Charge les arcs (liaisons) entre stations
    /// </summary>
    /// <param name="fichierExcel"></param>
    /// <param name="noeuds"></param>
    /// <returns></returns>
    static List<Tuple<Noeud<string>, Noeud<string>, double>> ChargerArcs(string fichierExcel, Dictionary<int, Noeud<string>> noeuds)
    {
        var arcs = new List<Tuple<Noeud<string>, Noeud<string>, double>>();
        string connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={fichierExcel};Extended Properties='Excel 12.0;HDR=YES;IMEX=1'";

        using (OleDbConnection connection = new OleDbConnection(connectionString))
        {
            connection.Open();
            OleDbCommand command = new OleDbCommand("SELECT * FROM [Arcs$]", connection);
            OleDbDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                int idStation = Convert.ToInt32(reader[0]);
                string station = reader[1].ToString();
                string precedentText = reader[2].ToString();
                string suivantText = reader[3].ToString();
                string tempsText = reader[4].ToString();

                // on ignore si aucun temps n'est défini
                if (string.IsNullOrEmpty(tempsText)) continue;

                double temps = Convert.ToDouble(tempsText);

                // Gérer les liens précédents
                if (!string.IsNullOrEmpty(precedentText))
                {
                    int idPrecedent;
                    if (int.TryParse(precedentText.Replace("=A", "").Replace("+1", ""), out idPrecedent))
                    {
                        if (noeuds.ContainsKey(idPrecedent))
                        {
                            arcs.Add(Tuple.Create(noeuds[idPrecedent], noeuds[idStation], temps));
                        }
                    }
                }

                // Gérer les liens suivants
                if (!string.IsNullOrEmpty(suivantText))
                {
                    int idSuivant;
                    if (int.TryParse(suivantText.Replace("=A", "").Replace("+1", ""), out idSuivant))
                    {
                        if (noeuds.ContainsKey(idSuivant))
                        {
                            arcs.Add(Tuple.Create(noeuds[idStation], noeuds[idSuivant], temps));
                        }
                    }
                }
            }
        }

        return arcs;
    }

   
   /// <summary>
   /// Affiche et sauvegarde le graphe sous forme d'image PNG à l'aide de SkiaSharp
   /// </summary>
   /// <param name="graphe"></param>
   /// <param name="nomFichier"></param>
    static void AfficherGraphe(Graphe<string> graphe, string nomFichier)
    {
        const int width = 2000;
        const int height = 2000;
        const int marge = 50;

        // calcul des bornes du graphe
        double minLon = graphe.Noeuds.Min(n => n.Longitude);
        double maxLon = graphe.Noeuds.Max(n => n.Longitude);
        double minLat = graphe.Noeuds.Min(n => n.Latitude);
        double maxLat = graphe.Noeuds.Max(n => n.Latitude);

        // création de la surface de dessin
        using (var surface = SKSurface.Create(new SKImageInfo(width, height)))
        {
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.White);

            // configuration des styles
            var paintLien = new SKPaint
            {
                Color = SKColors.Gray.WithAlpha(128),
                StrokeWidth = 3,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke
            };

            var paintNoeud = new SKPaint
            {
                Color = SKColors.Red,
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };

            var paintTexte = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true,
                TextSize = 24,
                TextAlign = SKTextAlign.Center
            };

            // dessin des liens
            foreach (var lien in graphe.Liens)
            {
                float x1 = marge + (float)((lien.Source.Longitude - minLon) / (maxLon - minLon) * (width - 2 * marge));
                float y1 = marge + (float)((maxLat - lien.Source.Latitude) / (maxLat - minLat) * (height - 2 * marge));
                float x2 = marge + (float)((lien.Destination.Longitude - minLon) / (maxLon - minLon) * (width - 2 * marge));
                float y2 = marge + (float)((maxLat - lien.Destination.Latitude) / (maxLat - minLat) * (height - 2 * marge));

                canvas.DrawLine(x1, y1, x2, y2, paintLien);
            }

            // dessin des noeuds
            foreach (var noeud in graphe.Noeuds)
            {
                float x = marge + (float)((noeud.Longitude - minLon) / (maxLon - minLon) * (width - 2 * marge));
                float y = marge + (float)((maxLat - noeud.Latitude) / (maxLat - minLat) * (height - 2 * marge));

                // dessin du cercle
                canvas.DrawCircle(x, y, 8, paintNoeud);

                // dessin du texte (libellé)
                canvas.DrawText(noeud.Libelle, x, y - 15, paintTexte);
            }

            // sauvegarde de l'image
            using (var image = surface.Snapshot())
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            using (var stream = File.OpenWrite(nomFichier))
            {
                data.SaveTo(stream);
            }
        }

        Console.WriteLine($"Carte du métro sauvegardée dans {nomFichier}");
    }

  
}
