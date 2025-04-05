

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
            Console.WriteLine("4) Test chemin, dessin chemin, distance et temps");
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
                default:Console.WriteLine("Option invalide, veuillez réessayer :(");
                    break;
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
