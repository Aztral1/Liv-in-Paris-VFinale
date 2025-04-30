

using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using SkiaSharp;
using MySql.Data.MySqlClient;
using System.Diagnostics;



/// <summary>
/// Classe principale du programme
/// </summary>
class Program
{
    /// <summary>
    /// chaîne de connexion à la base de données MySQL locale
    /// </summary>
    public static string connectionString = "server=localhost;database=premierRenduPSI;user=root;password=Xiang92310;";

    static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("\n--- Bienvenue ---");
            Console.WriteLine("1. Connexion");
            Console.WriteLine("2. Créer un compte");
            Console.WriteLine("3. Quitter");
            Console.WriteLine("4) Test chemin, dessin chemin, distance et temps");

            Console.Write("Choisissez une option : ");
            string choix = Console.ReadLine();

            switch (choix)
            {
                case "1":
                    Connexion();
                    break;
                case "2":
                    CreerCompte();
                    break;
                case "3":
                    Console.WriteLine("Au revoir !");
                    return;
                case "4":

                    // Initialisation et chargement des données
                    var graphe = new Graphe<string>();
                    var noeuds = ChargerNoeuds("MetroParis(1).xlsx");
                    var arcs = ChargerArcs("MetroParis(1).xlsx", noeuds);

                    foreach (var noeud in noeuds.Values) graphe.AjouterNoeud(noeud);
                    foreach (var arc in arcs) graphe.AjouterLien(arc.Item1, arc.Item2, arc.Item3);

                    // Menu interactif
                    while (true)
                    {
                        Console.Clear();
                        Console.WriteLine("=== PLANIFICATEUR DE TRAJET MÉTRO PARISIEN ===");
                        Console.WriteLine("\n1. Rechercher un trajet");
                        Console.WriteLine("2. Quitter");
                        Console.Write("\nVotre choix : ");

                        string choix2 = Console.ReadLine();

                        if (choix2 == "2") break;

                        if (choix2 == "1")
                        {
                            Console.WriteLine("\nAlgorithmes disponibles :");
                            Console.WriteLine("1. Dijkstra (recommandé)");
                            Console.WriteLine("2. Bellman-Ford");
                            Console.WriteLine("3. Floyd-Marshall");
                            Console.Write("\nChoisissez un algorithme (1-3) : ");
                            string choixAlgo = Console.ReadLine();

                            List<Noeud<string>> chemin = null;
                            string algoUtilisé = "";
                            Console.WriteLine("De quelle station partez-vous ?");
                            var nomdépart = Console.ReadLine()?.Trim().ToUpper();
                            var départ = noeuds.Values.FirstOrDefault(n => n.Libelle.ToUpper().Contains(nomdépart));

                            if (départ == null)
                            {
                                Console.WriteLine($"Aucune station contenant '{nomdépart}' n'a été trouvée.");
                                continue; // ou return selon votre flux
                            }

                            Console.WriteLine("Vers quelle station allez-vous ?");
                            var nomarrivée = Console.ReadLine()?.Trim().ToUpper();
                            var arrivée = noeuds.Values.FirstOrDefault(n => n.Libelle.ToUpper().Contains(nomarrivée));

                            if (arrivée == null)
                            {
                                Console.WriteLine($"Aucune station contenant '{nomarrivée}' n'a été trouvée.");
                                continue; // ou return selon votre flux
                            }
                            switch (choixAlgo)
                            {
                                case "1":
                                    chemin = Chemin<string>.Dijsktra(graphe, départ, arrivée);
                                    algoUtilisé = "Dijkstra";
                                    break;
                                case "2":
                                    var distancesBF = Chemin<string>.BellmanFord(graphe, départ);
                                    chemin = Chemin<string>.ReconstruireCheminBellmanFord(distancesBF, graphe, départ, arrivée);
                                    algoUtilisé = "Bellman-Ford";
                                    break;
                                case "3":

                                    var (distancesFW, predecesseursFW) = Chemin<string>.FloydWarshall(graphe);
                                    chemin = Chemin<string>.ReconstruireCheminFloydWarshall(predecesseursFW, départ, arrivée);
                                    algoUtilisé = "Floyd-Warshall";
                                    break;
                                default:
                                    Console.WriteLine("Choix invalide, utilisation de Dijkstra par défaut.");
                                    chemin = Chemin<string>.Dijsktra(graphe, départ, arrivée);
                                    algoUtilisé = "Dijkstra";
                                    break;
                            }

                            // Affichage des résultats
                            if (chemin.Count > 0)
                            {
                                Console.WriteLine($"\n CHEMIN TROUVÉ ({chemin.Count} stations) - Algorithme: {algoUtilisé}");
                                AfficherChemin(chemin);
                                graphe.AfficherGraphe("metro_paris_chemin.png", chemin);
                                Process.Start(new ProcessStartInfo { FileName = "metro_paris_chemin.png", UseShellExecute = true });
                            }
                            else
                            {
                                Console.WriteLine("\nAUCUN CHEMIN TROUVÉ");
                                Console.WriteLine($"Entre {départ.Libelle} et {arrivée.Libelle}");
                                Process.Start(new ProcessStartInfo { FileName = "metro_paris.png", UseShellExecute = true });
                            }

                            Console.WriteLine("\nAppuyez sur une touche pour continuer...");
                            Console.ReadKey();
                        }
                    }
                    break;

                default:
                    Console.WriteLine("Option invalide, veuillez réessayer.");
                    break;
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
            Console.Write("\nEmail : ");
            string email = Console.ReadLine();

            Console.Write("Mot de passe : ");
            string password = Console.ReadLine();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Test Admin
                string queryAdmin = "SELECT * FROM Administrateur WHERE email = @Email AND motDePasse = @Password";
                MySqlCommand cmdAdmin = new MySqlCommand(queryAdmin, connection);
                cmdAdmin.Parameters.AddWithValue("@Email", email);
                cmdAdmin.Parameters.AddWithValue("@Password", password);

                using (MySqlDataReader reader = cmdAdmin.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Console.WriteLine($"\nBienvenue Admin {reader["prenom"]} {reader["nom"]} !");
                        reader.Close();
                        
                        Admin.MenuAdmin();
                        return;
                    }
                }

                // Test Client
                string queryClient = "SELECT * FROM Client WHERE email = @Email AND motDePasse = @Password";
                MySqlCommand cmdClient = new MySqlCommand(queryClient, connection);
                cmdClient.Parameters.AddWithValue("@Email", email);
                cmdClient.Parameters.AddWithValue("@Password", password);

                using (MySqlDataReader reader = cmdClient.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int idClient = Convert.ToInt32(reader["idClient"]);
                        Console.WriteLine($"\nBienvenue {reader["prenom"]} {reader["nom"]} (Client) !");
                        reader.Close();
                        MenuClient(idClient);
                        return;
                    }
                }

                // Test Cuisinier
                string queryCuisinier = "SELECT * FROM Cuisinier WHERE email = @Email AND motDePasse = @Password";
                MySqlCommand cmdCuisinier = new MySqlCommand(queryCuisinier, connection);
                cmdCuisinier.Parameters.AddWithValue("@Email", email);
                cmdCuisinier.Parameters.AddWithValue("@Password", password);

                using (MySqlDataReader reader = cmdCuisinier.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        int idCuisinier = Convert.ToInt32(reader["idCuisinier"]);
                        Console.WriteLine($"\nBienvenue {reader["prenom"]} {reader["nom"]} (Cuisinier) !");
                        reader.Close();
                        MenuCuisinier(idCuisinier);
                        return;
                    }
                }

                Console.WriteLine("Email ou mot de passe incorrect. Veuillez réessayer !");
            }
        }
    }


    /// <summary>
    /// permet à un utilisateur de créer un compte client ou cuisinier
    /// </summary>
    static void CreerCompte()
    {
        Console.Write("\nVous êtes : 1. Client  2. Cuisinier  3. Administrateur\nChoix : ");
        string role = Console.ReadLine();

        if (role == "3")
        {
            Console.Write("Code de vérification administrateur : ");
            string code = Console.ReadLine();

            if (code != "0000")
            {
                Console.WriteLine("Code incorrect. Accès refusé.");
                return;
            }

            Console.WriteLine("Connexion en tant qu'administrateur réussie !");
            Admin.MenuAdmin();
            return;
        }

        Console.Write("Nom : ");
        string nom = Console.ReadLine();
        Console.Write("Prénom : ");
        string prenom = Console.ReadLine();
        Console.Write("Email : ");
        string email = Console.ReadLine();
        Console.Write("Mot de passe : ");
        string password = Console.ReadLine();
        Console.Write("Rue : ");
        string rue = Console.ReadLine();
        Console.Write("Numéro de maison : ");
        string numMaison = Console.ReadLine();
        Console.Write("Code postal : ");
        string codePostal = Console.ReadLine();
        Console.Write("Numéro de téléphone : ");
        string numTel = Console.ReadLine();
        Console.Write("Ville de résidence : ");
        string ville = Console.ReadLine();
        Console.Write("Métro le plus proche : ");
        string metroProche = Console.ReadLine();
        int totalCommande = 0;

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            string query = "";

            if (role == "1") // Client
            {
                query = "INSERT INTO Client (nom, prenom, email, motDePasse, rue, numMaison, codePostal, numTel, ville, totalCommande, metroProche) " +
                        "VALUES (@Nom, @Prenom, @Email, @Password, @rue, @numMaison, @codePostal, @numTel, @ville, @totalCommande, @metroProche)";
            }
            else if (role == "2") // Cuisinier
            {
                query = "INSERT INTO Cuisinier (nom, prenom, email, motDePasse, rue, numMaison, codePostal, numTel, ville, totalCommande, metroProche) " +
                        "VALUES (@Nom, @Prenom, @Email, @Password, @rue, @numMaison, @codePostal, @numTel, @ville, @totalCommande, @metroProche)";
            }
            else
            {
                Console.WriteLine("Choix invalide.");
                return;
            }

            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@Nom", nom);
            command.Parameters.AddWithValue("@Prenom", prenom);
            command.Parameters.AddWithValue("@Email", email);
            command.Parameters.AddWithValue("@Password", password);
            command.Parameters.AddWithValue("@rue", rue);
            command.Parameters.AddWithValue("@numMaison", numMaison);
            command.Parameters.AddWithValue("@codePostal", codePostal);
            command.Parameters.AddWithValue("@numTel", numTel);
            command.Parameters.AddWithValue("@ville", ville);
            command.Parameters.AddWithValue("@totalCommande", totalCommande);
            command.Parameters.AddWithValue("@metroProche", metroProche);

            int rowsAffected = command.ExecuteNonQuery();
            Console.WriteLine(rowsAffected > 0 ? "Compte créé avec succès !" : "Erreur lors de la création du compte.");
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
            Console.WriteLine("7) Trouver chemin plus court client");
            Console.WriteLine("8) Se déconnecter");
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
                    CalculerCheminVersClient(idCuisinier);
                    break;

                case "8":
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

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            // Récupérer les infos du client
            string requeteClient = "SELECT metroProche FROM Client WHERE idClient = @idClient";
            MySqlCommand commandeClient = new MySqlCommand(requeteClient, connection);
            commandeClient.Parameters.AddWithValue("@idClient", idClient);
            string metroClient = commandeClient.ExecuteScalar()?.ToString();

            // Récupérer l'ID du cuisinier et son métro proche
            string requeteCuisinier = "SELECT idCuisinier FROM Plat WHERE nomPlat = @nomPlat";
            MySqlCommand commandeCuisinier = new MySqlCommand(requeteCuisinier, connection);
            commandeCuisinier.Parameters.AddWithValue("@nomPlat", nomPlat);
            int idCuisinier = Convert.ToInt32(commandeCuisinier.ExecuteScalar());

            string requeteMetroCuisinier = "SELECT metroProche FROM Cuisinier WHERE idCuisinier = @idCuisinier";
            MySqlCommand commandeMetroCuisinier = new MySqlCommand(requeteMetroCuisinier, connection);
            commandeMetroCuisinier.Parameters.AddWithValue("@idCuisinier", idCuisinier);
            string metroCuisinier = commandeMetroCuisinier.ExecuteScalar()?.ToString();

            // Charger le graphe pour calculer le trajet
            var graphe = new Graphe<string>();
            var noeuds = ChargerNoeuds("MetroParis(1).xlsx");
            var arcs = ChargerArcs("MetroParis(1).xlsx", noeuds);

            foreach (var noeud in noeuds.Values) graphe.AjouterNoeud(noeud);
            foreach (var arc in arcs) graphe.AjouterLien(arc.Item1, arc.Item2, arc.Item3);

            // Trouver les stations correspondantes
            var depart = noeuds.Values.FirstOrDefault(n => n.Libelle.ToString().ToUpper().Contains(metroClient.ToUpper()));
            var arrivee = noeuds.Values.FirstOrDefault(n => n.Libelle.ToString().ToUpper().Contains(metroCuisinier.ToUpper()));

            double distance = 0;
            double tempsTrajet = 0;

            if (depart != null && arrivee != null)
            {
                var chemin = Chemin<string>.Dijsktra(graphe, depart, arrivee);
                if (chemin.Count > 0)
                {
                    for (int i = 0; i < chemin.Count - 1; i++)
                    {
                        var current = chemin[i];
                        var next = chemin[i + 1];
                        var lien = current.Liens.FirstOrDefault(l => l.Destination == next)
                                ?? next.Liens.First(l => l.Destination == current);

                        distance += CalculerDistanceHaversine(
                            current.Latitude, current.Longitude,
                            next.Latitude, next.Longitude);

                        tempsTrajet += lien.Poids;

                        if (i < chemin.Count - 1 && !current.Lignes.Intersect(next.Lignes).Any())
                        {
                            tempsTrajet += current.TempsChangement;
                        }
                    }
                }
            }

            // Afficher le récapitulatif
            Console.WriteLine("\n*** Récapitulatif de la commande ***");
            Console.WriteLine($"Plat : {nomPlat}");
            Console.WriteLine($"Total à payer : {totalPrix} euro");
            Console.WriteLine($"Distance entre vous et le cuisinier : {distance:0.00} km");
            Console.WriteLine($"Temps estimé de livraison : {tempsTrajet} minutes");

            // Ajouter les étapes manquantes

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

            // Insertion dans la table commande
            string requeteCommande = @"INSERT INTO Commande 
            (nom, prix, tempsPreparation, statut, date, idClient, commentaire, idCuisinier) 
            VALUES (@nom, @prix, @tempsPreparation, @statut, @date, @idClient, @commentaire, @idCuisinier)";

            MySqlCommand commande = new MySqlCommand(requeteCommande, connection);
            commande.Parameters.AddWithValue("@nom", nomPlat);
            commande.Parameters.AddWithValue("@prix", totalPrix);
            commande.Parameters.AddWithValue("@tempsPreparation", 30); // Valeur par défaut
            commande.Parameters.AddWithValue("@statut", "en attente");
            commande.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd"));
            commande.Parameters.AddWithValue("@idClient", idClient);
            commande.Parameters.AddWithValue("@commentaire", commentaire);
            commande.Parameters.AddWithValue("@idCuisinier", idCuisinier);
            commande.ExecuteNonQuery();

            Console.WriteLine("\nPaiement effectué avec succès !");
            Console.WriteLine("Votre commande a été enregistrée.");
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

            string requetesql = @"SELECT commande.idCommande, commande.nom, commande.prix, commande.statut, commande.date, commande.commentaire,
       client.idClient, client.nom AS nomClient, client.prenom, client.metroProche AS metroClient,
       cuisinier.metroProche AS metroCuisinier
FROM commande
JOIN client ON commande.idClient = client.idClient
JOIN cuisinier ON commande.idCuisinier = cuisinier.idCuisinier
WHERE commande.idCuisinier = @idCuisinier AND commande.statut = 'en attente'
";

            MySqlCommand commandesql = new MySqlCommand(requetesql, connection);
            commandesql.Parameters.AddWithValue("@idCuisinier", idCuisinier);

            // Charger le graphe une seule fois
            var graphe = new Graphe<string>();
            var noeuds = ChargerNoeuds("MetroParis(1).xlsx");
            var arcs = ChargerArcs("MetroParis(1).xlsx", noeuds);

            foreach (var noeud in noeuds.Values) graphe.AjouterNoeud(noeud);
            foreach (var arc in arcs) graphe.AjouterLien(arc.Item1, arc.Item2, arc.Item3);

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
                    string metroClient = lecteur["metroClient"].ToString();
                    string metroCuisinier = lecteur["metroCuisinier"].ToString();

                    var depart = noeuds.Values.FirstOrDefault(n => n.Libelle.ToString().ToUpper().Contains(metroCuisinier.ToUpper()));
                    var arrivee = noeuds.Values.FirstOrDefault(n => n.Libelle.ToString().ToUpper().Contains(metroClient.ToUpper()));

                    double distance = 0;
                    double tempsTrajet = 0;

                    if (depart != null && arrivee != null)
                    {
                        var chemin = Chemin<string>.Dijsktra(graphe, depart, arrivee);
                        if (chemin.Count > 0)
                        {
                            for (int i = 0; i < chemin.Count - 1; i++)
                            {
                                var current = chemin[i];
                                var next = chemin[i + 1];
                                var lien = current.Liens.FirstOrDefault(l => l.Destination == next)
                                         ?? next.Liens.First(l => l.Destination == current);

                                distance += CalculerDistanceHaversine(
                                    current.Latitude, current.Longitude,
                                    next.Latitude, next.Longitude);

                                tempsTrajet += lien.Poids;

                                if (i < chemin.Count - 1 && !current.Lignes.Intersect(next.Lignes).Any())
                                {
                                    tempsTrajet += current.TempsChangement;
                                }
                            }
                        }
                    }

                    Console.WriteLine($"\nCommande #{lecteur["idCommande"]} - {lecteur["nom"]} - {lecteur["prix"]} euro");
                    Console.WriteLine($"Client: {lecteur["prenom"]} {lecteur["nomClient"]}");
                    Console.WriteLine($"Distance: {distance:0.00} km | Temps estimé: {tempsTrajet} minutes");
                    Console.WriteLine($"Statut: {lecteur["statut"]} | Date: {lecteur["date"]}");
                    Console.WriteLine($"Commentaire: {lecteur["commentaire"]}\n");
                }
            }
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
                    Console.WriteLine("Aucune commande réalisée.");
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
    /// Charge les noeuds (stations) 
    /// </summary>
    /// <param name="fichierExcel"></param>
    /// <returns></returns>
    

    static double DegresToRadians(double deg) => deg * (Math.PI / 180);
    static double CalculerDistanceHaversine(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // Rayon terrestre en km
        var dLat = DegresToRadians(lat2 - lat1);
        var dLon = DegresToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(DegresToRadians(lat1)) * Math.Cos(DegresToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }
    static void AfficherChemin(List<Noeud<string>> chemin)
    {
        Console.WriteLine($"\n Chemin de {chemin.First().Libelle} à {chemin.Last().Libelle} :");
        double tempsTotal = 0;
        double tempsTrajet = 0;
        double distanceTotale = 0;
        int nbChangements = 0;
        string ligneActuelle = chemin[0].Lignes.First();  // Prend la première ligne disponible

        for (int i = 0; i < chemin.Count - 1; i++)
        {
            var current = chemin[i];
            var next = chemin[i + 1];

            var lien = current.Liens.FirstOrDefault(l => l.Destination == next)
                     ?? next.Liens.First(l => l.Destination == current);

            if (lien == null)
            {
                Console.WriteLine($"Erreur: Lien manquant entre {current.Libelle} et {next.Libelle}");
                return;
            }

            double distanceSegment = CalculerDistanceHaversine(
                current.Latitude, current.Longitude,
                next.Latitude, next.Longitude);

            distanceTotale += distanceSegment;

            // Vérifie si les stations partagent une ligne commune
            var lignesCommunes = current.Lignes.Intersect(next.Lignes).ToList();
            if (lignesCommunes.Count == 0) // Changement de ligne
            {
                Console.WriteLine($"  {(i + 1).ToString().PadLeft(2)}. {current.Libelle} -> {next.Libelle} ({lien.Poids} min, {distanceSegment:0.00} km)");
                Console.WriteLine($"     [CHANGEMENT: {ligneActuelle} -> {next.Lignes.First()} | +{current.TempsChangement} min]");
                tempsTotal += lien.Poids + current.TempsChangement;
                tempsTrajet += lien.Poids;
                nbChangements++;
                ligneActuelle = next.Lignes.First();
            }
            else // Même ligne
            {
                Console.WriteLine($"  {(i + 1).ToString().PadLeft(2)}. {current.Libelle} -> {next.Libelle} ({lien.Poids} min, {distanceSegment:0.00} km)");
                tempsTotal += lien.Poids;
                tempsTrajet += lien.Poids;
            }
        }

        Console.WriteLine($"\n SYNTHÈSE DU TRAJET:");
        Console.WriteLine($"• Temps de trajet: {tempsTrajet} minutes");
        Console.WriteLine($"• Temps de changement: {tempsTotal - tempsTrajet} minutes");
        Console.WriteLine($"• Temps total: {tempsTotal} minutes");
        Console.WriteLine($"• Distance totale: {distanceTotale:0.00} km");
        Console.WriteLine($"• Stations: {chemin.Count}");
        Console.WriteLine($"• Changements: {nbChangements}");
        Console.WriteLine("------------------------------------------------");
    }

    static void CalculerCheminVersClient(int idCuisinier)
    {
        using (MySqlConnection connection = new MySqlConnection(Program.connectionString))
        {
            connection.Open();

            // Affiche les clients du cuisinier
            string requeteClients = @"SELECT DISTINCT client.idClient, client.nom, client.prenom, client.metroProche
                                  FROM Client
                                  JOIN Commande ON Client.idClient = Commande.idClient
                                  WHERE Commande.idCuisinier = @idCuisinier";

            MySqlCommand cmd = new MySqlCommand(requeteClients, connection);
            cmd.Parameters.AddWithValue("@idCuisinier", idCuisinier);

            var clients = new Dictionary<int, string>();
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                Console.WriteLine("\n--- Clients ayant passé commande ---");
                while (reader.Read())
                {
                    int id = Convert.ToInt32(reader["idClient"]);
                    string nom = reader["nom"].ToString();
                    string prenom = reader["prenom"].ToString();
                    string metro = reader["metroProche"].ToString();

                    Console.WriteLine($"ID: {id} | {prenom} {nom} - Métro: {metro}");
                    clients[id] = metro;
                }
            }

            if (clients.Count == 0)
            {
                Console.WriteLine("Aucun client trouvé.");
                return;
            }

            // Saisie de l'id du client cible
            Console.Write("\nEntrez l’ID du client pour calculer le chemin : ");
            if (!int.TryParse(Console.ReadLine(), out int idClient) || !clients.ContainsKey(idClient))
            {
                Console.WriteLine("ID invalide.");
                return;
            }

            // Récupérer le métro du cuisinier
            string metroCuisinier = "";
            string reqMetroC = "SELECT metroProche FROM Cuisinier WHERE idCuisinier = @idCuisinier";
            MySqlCommand cmdMetroC = new MySqlCommand(reqMetroC, connection);
            cmdMetroC.Parameters.AddWithValue("@idCuisinier", idCuisinier);
            metroCuisinier = cmdMetroC.ExecuteScalar()?.ToString();

            string metroClient = clients[idClient];

            // Calcul du chemin avec graphe
            var graphe = new Graphe<string>();
            var noeuds = ChargerNoeuds("MetroParis(1).xlsx");
            var arcs = ChargerArcs("MetroParis(1).xlsx", noeuds);

            foreach (var noeud in noeuds.Values) graphe.AjouterNoeud(noeud);
            foreach (var arc in arcs) graphe.AjouterLien(arc.Item1, arc.Item2, arc.Item3);

            var depart = noeuds.Values.FirstOrDefault(n => n.Libelle.ToUpper().Contains(metroCuisinier.ToUpper()));
            var arrivee = noeuds.Values.FirstOrDefault(n => n.Libelle.ToUpper().Contains(metroClient.ToUpper()));

            if (depart == null || arrivee == null)
            {
                Console.WriteLine("Erreur : station introuvable.");
                return;
            }

            var chemin = Chemin<string>.Dijsktra(graphe, depart, arrivee);
            if (chemin.Count == 0)
            {
                Console.WriteLine("Aucun chemin trouvé.");
                return;
            }

            AfficherChemin(chemin);
            graphe.AfficherGraphe("chemin_cuisinier_client.png", chemin);
            Process.Start(new ProcessStartInfo { FileName = "chemin_cuisinier_client.png", UseShellExecute = true });
        }
    }

    static Dictionary<int, Noeud<string>> ChargerNoeuds(string fichierExcel)
    {
        var noeuds = new Dictionary<int, Noeud<string>>();
        var stationsParNom = new Dictionary<string, List<Noeud<string>>>();
        string connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={fichierExcel};Extended Properties='Excel 12.0;HDR=YES;IMEX=1'";

        using (OleDbConnection connection = new OleDbConnection(connectionString))
        {
            connection.Open();
            OleDbCommand command = new OleDbCommand("SELECT * FROM [Noeuds$]", connection);
            OleDbDataReader reader = command.ExecuteReader();

            // 1. Chargement initial des nœuds
            while (reader.Read())
            {
                int id = int.Parse(reader[0].ToString());
                string libelleLigne = reader[1].ToString();
                string libelleStation = reader[2].ToString().Trim();
                double longitude = double.Parse(reader[3].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                double latitude = double.Parse(reader[4].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                string commune = reader[5].ToString();
                string codeInsee = reader[6].ToString();
                double tempsChangement = reader.FieldCount > 7 ? double.Parse(reader[7].ToString()) : 0;

                var noeud = new Noeud<string>(
                    id,
                    libelleStation,
                    libelleLigne,
                    longitude,
                    latitude,
                    commune,
                    codeInsee,
                    tempsChangement);

                noeuds.Add(id, noeud);

                // Ajout au regroupement par nom de station
                if (!stationsParNom.ContainsKey(libelleStation))
                {
                    stationsParNom[libelleStation] = new List<Noeud<string>>();
                }
                stationsParNom[libelleStation].Add(noeud);
            }

            // 2. Création des liens de correspondance
            foreach (var groupe in stationsParNom.Where(g => g.Value.Count > 1))
            {
                var stations = groupe.Value;
                for (int i = 0; i < stations.Count; i++)
                {
                    for (int j = i + 1; j < stations.Count; j++)
                    {
                        // Création d'un lien bidirectionnel avec le temps de changement
                        stations[i].AjouterLien(stations[j], stations[i].TempsChangement, true);
                    }
                }
            }
        }

        return noeuds;
    }

    static List<Tuple<Noeud<string>, Noeud<string>, double>> ChargerArcs(string fichierExcel, Dictionary<int, Noeud<string>> noeuds)
    {
        var arcs = new HashSet<Tuple<Noeud<string>, Noeud<string>, double>>(new ArcEqualityComparer());
        string connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={fichierExcel};Extended Properties='Excel 12.0;HDR=YES;IMEX=1'";

        using (OleDbConnection connection = new OleDbConnection(connectionString))
        {
            connection.Open();
            OleDbCommand command = new OleDbCommand("SELECT * FROM [Arcs$]", connection);
            OleDbDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                try
                {
                    int idStation = Convert.ToInt32(reader[0]);
                    if (!noeuds.ContainsKey(idStation)) continue;

                    string tempsText = reader[4].ToString();
                    if (string.IsNullOrEmpty(tempsText)) continue;
                    double temps = Convert.ToDouble(tempsText);


                    // Gestion des liens précédents
                    if (!string.IsNullOrEmpty(reader[2].ToString()))
                    {
                        int idPrecedent = ParseId(reader[2].ToString());


                        var precedent = noeuds[idPrecedent];
                        var current = noeuds[idStation];
                        arcs.Add(Tuple.Create(precedent, current, temps));
                        arcs.Add(Tuple.Create(current, precedent, temps));

                    }

                    // Gestion des liens suivants
                    if (!string.IsNullOrEmpty(reader[3].ToString()))
                    {
                        int idSuivant = ParseId(reader[3].ToString());

                        var current = noeuds[idStation];
                        var suivant = noeuds[idSuivant];
                        arcs.Add(Tuple.Create(current, suivant, temps));
                        arcs.Add(Tuple.Create(suivant, current, temps));

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors du chargement d'un arc: {ex.Message}");
                }
            }
        }
        return arcs.ToList();
    }

    // Nouvelle classe pour éviter les doublons

    class ArcEqualityComparer : IEqualityComparer<Tuple<Noeud<string>, Noeud<string>, double>>
    {
        public bool Equals(Tuple<Noeud<string>, Noeud<string>, double> x, Tuple<Noeud<string>, Noeud<string>, double> y)
        {
            return x.Item1.Id == y.Item1.Id && x.Item2.Id == y.Item2.Id;
        }

        public int GetHashCode(Tuple<Noeud<string>, Noeud<string>, double> obj)
        {
            return obj.Item1.Id.GetHashCode() ^ obj.Item2.Id.GetHashCode();
        }
    }

    // Nouvelle classe pour éviter les doublons

    static int ParseId(string text)
    {
        if (text.StartsWith("=A") || text.StartsWith("=D") || text.StartsWith("=C"))
            return int.Parse(text.Substring(3).Replace("+1", ""));
        return int.Parse(text);
    }
}

class Admin
{
    public static void MenuAdmin()
    {
        while (true)
        {
            Console.WriteLine("\n--- Menu Administrateur ---");
            Console.WriteLine("1. Gérer les clients");
            Console.WriteLine("2. Gérer les cuisiniers");
            Console.WriteLine("3. Gérer les commandes");
            Console.WriteLine("4. Voir tous les plats");
            Console.WriteLine("5. Quitter");
            Console.Write("Choisissez une option : ");
            string choix = Console.ReadLine();

            switch (choix)
            {
                case "1":
                    GérerClients();
                    break;
                case "2":
                    GérerCuisiniers();
                    break;
                case "3":
                    GérerCommandes();
                    break;
                case "4":
                    VoirTousLesPlats();
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Option invalide.");
                    break;
            }
        }
    }

    static void GérerClients()
    {
        Console.WriteLine("\n--- Tous les Clients ---");
        using (var connection = new MySqlConnection(Program.connectionString))
        {
            connection.Open();
            var cmd = new MySqlCommand("SELECT * FROM Client", connection);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader["idClient"]}, Nom: {reader["nom"]}, Email: {reader["email"]}");
                }
            }

            Console.Write("\nSouhaitez-vous modifier (m) ou supprimer (s) un client ? (ou Entrée pour revenir) : ");
            string action = Console.ReadLine().ToLower();

            if (action == "m")
            {
                Console.Write("ID du client à modifier : ");
                int id = int.Parse(Console.ReadLine());
                Console.Write("Nouveau nom : ");
                string nouveauNom = Console.ReadLine();

                var update = new MySqlCommand("UPDATE Client SET nom = @nom WHERE idClient = @id", connection);
                update.Parameters.AddWithValue("@nom", nouveauNom);
                update.Parameters.AddWithValue("@id", id);
                update.ExecuteNonQuery();

                Console.WriteLine("Client modifié.");
            }
            else if (action == "s")
            {
                Console.Write("ID du client à supprimer : ");
                int id = int.Parse(Console.ReadLine());

                var delete = new MySqlCommand("DELETE FROM Client WHERE idClient = @id", connection);
                delete.Parameters.AddWithValue("@id", id);
                delete.ExecuteNonQuery();

                Console.WriteLine("Client supprimé.");
            }
        }
    }

    static void GérerCuisiniers()
    {
        Console.WriteLine("\n--- Tous les Cuisiniers ---");
        using (var connection = new MySqlConnection(Program.connectionString))
        {
            connection.Open();
            var cmd = new MySqlCommand("SELECT * FROM Cuisinier", connection);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader["idCuisinier"]}, Nom: {reader["nom"]}, Email: {reader["email"]}");
                }
            }

            Console.Write("\nSouhaitez-vous modifier (m) ou supprimer (s) un cuisinier ? (ou Entrée pour revenir) : ");
            string action = Console.ReadLine().ToLower();

            if (action == "m")
            {
                Console.Write("ID du cuisinier à modifier : ");
                int id = int.Parse(Console.ReadLine());
                Console.Write("Nouveau nom : ");
                string nouveauNom = Console.ReadLine();

                var update = new MySqlCommand("UPDATE Cuisinier SET nom = @nom WHERE idCuisinier = @id", connection);
                update.Parameters.AddWithValue("@nom", nouveauNom);
                update.Parameters.AddWithValue("@id", id);
                update.ExecuteNonQuery();

                Console.WriteLine("Cuisinier modifié.");
            }
            else if (action == "s")
            {
                Console.Write("ID du cuisinier à supprimer : ");
                int id = int.Parse(Console.ReadLine());

                var delete = new MySqlCommand("DELETE FROM Cuisinier WHERE idCuisinier = @id", connection);
                delete.Parameters.AddWithValue("@id", id);
                delete.ExecuteNonQuery();

                Console.WriteLine("Cuisinier supprimé.");
            }
        }
    }

    static void GérerCommandes()
    {
        Console.WriteLine("\n--- Toutes les Commandes ---");
        using (var connection = new MySqlConnection(Program.connectionString))
        {
            connection.Open();
            var cmd = new MySqlCommand("SELECT * FROM Commande", connection);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"Commande #{reader["idCommande"]} - Nom: {reader["nom"]}, Prix: {reader["prix"]}, Client ID: {reader["idClient"]}");
                }
            }

            Console.Write("\nSouhaitez-vous modifier (m) ou supprimer (s) une commande ? (ou Entrée pour revenir) : ");
            string action = Console.ReadLine().ToLower();

            if (action == "m")
            {
                Console.Write("ID de la commande à modifier : ");
                int id = int.Parse(Console.ReadLine());
                Console.Write("Nouveau statut : ");
                string nouveauStatut = Console.ReadLine();

                var update = new MySqlCommand("UPDATE Commande SET statut = @statut WHERE idCommande = @id", connection);
                update.Parameters.AddWithValue("@statut", nouveauStatut);
                update.Parameters.AddWithValue("@id", id);
                update.ExecuteNonQuery();

                Console.WriteLine("Commande modifiée.");
            }
            else if (action == "s")
            {
                Console.Write("ID de la commande à supprimer : ");
                int id = int.Parse(Console.ReadLine());

                var delete = new MySqlCommand("DELETE FROM Commande WHERE idCommande = @id", connection);
                delete.Parameters.AddWithValue("@id", id);
                delete.ExecuteNonQuery();

                Console.WriteLine("Commande supprimée.");
            }
        }
    }

    static void VoirTousLesPlats()
    {
        using (MySqlConnection connection = new MySqlConnection(Program.connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM Plat";
            MySqlCommand command = new MySqlCommand(query, connection);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                Console.WriteLine("\n--- Tous les Plats ---");
                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader["idPlat"]}, Nom: {reader["nomPlat"]}, Prix: {reader["prix"]}, Cuisinier ID: {reader["idCuisinier"]}");
                }
            }
        }
    }
}




