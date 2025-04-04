

using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using SkiaSharp;
using MySql.Data.MySqlClient;




class Program
{
    static string connectionString = "server=localhost;database=premierRenduPSI;user=root;password=Xiang92310;";

    static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("\n--- Bienvenue ---");
            Console.WriteLine("1. Connexion");
            Console.WriteLine("2. Créer un compte");
            Console.WriteLine("3. Quitter");
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
                default:
                    Console.WriteLine("Option invalide, veuillez réessayer.");
                    break;
            }
        }
    }
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
                string query = @"SELECT 'Client' AS Role, idClient AS idUtilisateur, nom, prenom 
                             FROM Client 
                             WHERE email = @Email AND motDePasse = @Password
                             UNION
                             SELECT 'Cuisinier', idCuisinier AS idUtilisateur, nom, prenom 
                             FROM Cuisinier 
                             WHERE email = @Email AND motDePasse = @Password";

                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@Email", email);
                command.Parameters.AddWithValue("@Password", password);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string role = reader["Role"].ToString();
                        int idUtilisateur = Convert.ToInt32(reader["idUtilisateur"]);
                        string prenom = reader["prenom"].ToString();
                        string nom = reader["nom"].ToString();

                        Console.WriteLine($"\nBienvenue, {prenom} {nom} ({role}) !");
                        reader.Close();

                        if (role == "Client")
                            MenuClient(idUtilisateur);
                        else
                            MenuCuisinier(idUtilisateur);

                        return;
                    }
                    else
                    {
                        Console.WriteLine("Email ou mot de passe incorrect.");
                    }
                }
            }
        }
    }
    static void CreerCompte()
    {
        Console.Write("\nVous êtes : 1. Client  2. Cuisinier\nChoix : ");
        string role = Console.ReadLine();

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
        int totalCommande = 0;

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            string query = "";

            if (role == "1")  // Inscription en tant que Client
            {
                query = "INSERT INTO Client (nom, prenom, email, motDePasse, rue, numMaison, codePostal, numTel, ville, totalCommande, metroProche) VALUES (@Nom, @Prenom, @Email, @motDePasse, @rue, @numMaison, @codePostal, @numTel, @ville, @totalCommande, @metroProche)";
            }
            else if (role == "2")  // Inscription en tant que Cuisinier
            {
                query = "INSERT INTO Cuisinier (nom, prenom, email, motDePasse, rue, numMaison, codePostal, numTel, ville, totalCommande, metroProche) VALUES (@Nom, @Prenom, @Email, @motDePasse, @rue, @numMaison, @codePostal, @numTel, @ville, @totalCommande, @metroProche)";
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
            command.Parameters.AddWithValue("@motDePasse", mdp);
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
    static void MenuClient(int idClient)
    {
        List<string> platsCommandes = new List<string>(); // Stocker les noms des plats commandés
        double totalPrix = 0; // Stocke le total des commandes

        while (true)
        {
            Console.WriteLine("\n--- Menu Client ---");
            Console.WriteLine("1. Ajouter un plat à la commande");
            Console.WriteLine("2. Voir les cuisiniers disponibles");
            Console.WriteLine("3. Régler la commande");
            Console.WriteLine("4. Se déconnecter");
            Console.Write("Choisissez une option : ");
            string choix = Console.ReadLine();

            switch (choix)
            {
                case "1":
                    totalPrix += AjouterPlatCommande(platsCommandes);
                    break;
                case "2":
                    VoirCuisiniers();
                    break;
                case "3":
                    ReglerCommandes(idClient, platsCommandes, totalPrix);
                    platsCommandes.Clear(); // Réinitialiser après paiement
                    totalPrix = 0;
                    break;
                case "4":
                    return;
                default:
                    Console.WriteLine("Option invalide, veuillez réessayer.");
                    break;
            }
        }
    }
    static void MenuCuisinier(int idCuisinier)
    {
        while (true)
        {
            Console.WriteLine("\n--- Menu Cuisinier ---");
            Console.WriteLine("1. Modifier mon menu");
            Console.WriteLine("2. Voir mes plats");
            Console.WriteLine("3. Voir mes clients");
            Console.WriteLine("4. Voir les commandes à préparer");
            Console.WriteLine("5. Mettre à jour le statut d'une commande");
            Console.WriteLine("6. Se déconnecter");
            Console.Write("Choisissez une option : ");
            string choix = Console.ReadLine();

            switch (choix)
            {
                case "1":
                    ModifierMenu(idCuisinier);  
                    break;
                case "2":
                    VoirPlats(idCuisinier);  
                    break;
                case "3":
                    VoirClients(idCuisinier);
                    break;
                case "4":
                    VoirCommandesAPreparer(idCuisinier);
                    break;
                case "5":
                    Mettreàjourcommande();
                    break;
                case "6":
                    return;
                default:
                    Console.WriteLine("Option invalide, veuillez réessayer.");
                    break;
            }
        }
    }
    static void ModifierMenu(int idCuisinier)
    {
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
            string query = "INSERT INTO Plat (nomPlat, regime, prix, nationalite, dateFabrication, datePeremption, idCuisinier) " +
                           "VALUES (@nomPlat, @regime, @prix, @nationalite, @dateFabrication, @datePeremption, @idCuisinier)";

            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@nomPlat", nomPlat);
            command.Parameters.AddWithValue("@regime", regime);
            command.Parameters.AddWithValue("@prix", prix);
            command.Parameters.AddWithValue("@nationalite", nationalite);
            command.Parameters.AddWithValue("@dateFabrication", dateFabrication);
            command.Parameters.AddWithValue("@datePeremption", datePeremption);
            command.Parameters.AddWithValue("@idCuisinier", idCuisinier);

            int rowsAffected = command.ExecuteNonQuery();
            Console.WriteLine(rowsAffected > 0 ? "Plat ajouté avec succès !" : "Erreur lors de l'ajout du plat.");

            // Récupérer l'ID du plat inséré
            long idPlat = command.LastInsertedId;

            // Ajout des ingrédients
            AjouterIngredients(idPlat, connection);

            connection.Close();
        }
    }
    static void VoirPlats(int idCuisinier)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            // Requête pour récupérer les plats du cuisinier avec idPlat
            string query = "SELECT idPlat, nomPlat, regime, nationalite, prix FROM Plat WHERE idCuisinier = @idCuisinier;";
            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@idCuisinier", idCuisinier);

            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (!reader.HasRows)
                {
                    Console.WriteLine("Aucun plat trouvé pour ce cuisinier.");
                }
                else
                {
                    Console.WriteLine("\n--- Liste des plats ---");
                    while (reader.Read())
                    {
                        string nomPlat = reader["nomPlat"].ToString();
                        string regime = reader["regime"].ToString();
                        double prix = Convert.ToDouble(reader["prix"]);
                        string nationalite = reader["nationalite"].ToString();
                        int idPlat = Convert.ToInt32(reader["idPlat"]); // Récupérer idPlat

                        // Affichage des informations du plat
                        Console.WriteLine($"\nNom : {nomPlat}, Régime : {regime}, Prix : {prix}€");
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

    static void VoirCuisiniers()
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            string query = @"
        SELECT 
            Cuisinier.nom AS nomCuisinier,
            Plat.idPlat,
            Plat.nomPlat,
            Plat.regime,
            Plat.prix
        FROM Cuisinier
        JOIN Plat ON Plat.idCuisinier = Cuisinier.idCuisinier";

            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataReader reader = command.ExecuteReader();

            Console.WriteLine("\n--- Cuisiniers Disponibles ---");
            while (reader.Read())
            {
                int idPlat = Convert.ToInt32(reader["idPlat"]);
                string nomCuisinier = reader["nomCuisinier"].ToString();
                string nomPlat = reader["nomPlat"].ToString();
                string regime = reader["regime"].ToString();
                double prix = Convert.ToDouble(reader["prix"]);

                Console.WriteLine($"\nNom: {nomCuisinier}, Plat: {nomPlat}, Régime: {regime}, Prix: {prix} euro");

                // Appelle la nouvelle fonction
                List<string> ingredients = RecupererIngredients(idPlat);
                if (ingredients.Count > 0)
                    Console.WriteLine($"Ingrédients : {string.Join(", ", ingredients)}");
                else
                    Console.WriteLine("Ingrédients : Aucun");
            }
        }
    }

    static List<string> RecupererIngredients(int idPlat)
    {
        List<string> ingredients = new List<string>();

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT nom FROM Ingredient WHERE idPlat = @idPlat";
            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@idPlat", idPlat);

            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    ingredients.Add(reader["nom"].ToString());
                }
            }
        }

        return ingredients;
    }



    static void VoirClients(int idCuisinier)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            string query = @"
        SELECT DISTINCT client.nom, client.prenom, client.email, client.numTel 
        FROM Client 
        JOIN Commande ON Client.idClient = Commande.idClient
        WHERE Commande.idCuisinier = @idCuisinier;";

            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@idCuisinier", idCuisinier);

            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (!reader.HasRows)
                {
                    Console.WriteLine("Aucun client trouvé.");
                }
                else
                {
                    Console.WriteLine("\n--- Liste des clients ---");
                    while (reader.Read())
                    {
                        Console.WriteLine($"Nom: {reader["nom"]}, Prénom: {reader["prenom"]}, Email: {reader["email"]}, Téléphone: {reader["numTel"]}");
                    }
                }
            }
        }
    }

    static double AjouterPlatCommande(List<string> platsCommandes)
    {
        Console.Write("\nEntrez le nom du plat que vous souhaitez commander : ");
        string nomPlat = Console.ReadLine();

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            // Vérifier si le plat existe
            string checkQuery = "SELECT prix FROM Plat WHERE nomPlat = @nomPlat";
            MySqlCommand checkCmd = new MySqlCommand(checkQuery, connection);
            checkCmd.Parameters.AddWithValue("@nomPlat", nomPlat);

            using (MySqlDataReader reader = checkCmd.ExecuteReader())
            {
                if (!reader.Read()) // Si aucun plat trouvé
                {
                    Console.WriteLine("Plat inexistant, veuillez recommencer.");
                    return 0;
                }

                double prix = Convert.ToDouble(reader["prix"]);
                platsCommandes.Add(nomPlat);

                Console.WriteLine($"Plat ajouté : {nomPlat} ({prix} euro)");
                return prix;
            }
        }
    }
    static void ReglerCommandes(int idClient, List<string> platsCommandes, double totalPrix)
    {
        if (platsCommandes.Count == 0)
        {
            Console.WriteLine("Aucun plat commandé.");
            return;
        }

        string nomPlat = platsCommandes[0]; // Un seul plat pour simplifier

        Console.WriteLine("\n--- Récapitulatif de la commande ---");
        Console.WriteLine($"Plat : {nomPlat}");
        Console.WriteLine($"Total à payer : {totalPrix} euro");

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

            // 👉 Étape : récupérer l'idCuisinier qui a fait ce plat
            string queryCuisinier = "SELECT idCuisinier FROM Plat WHERE nomPlat = @nomPlat";
            MySqlCommand cmd = new MySqlCommand(queryCuisinier, connection);
            cmd.Parameters.AddWithValue("@nomPlat", nomPlat);

            object result = cmd.ExecuteScalar();

            if (result == null)
            {
                Console.WriteLine("⚠️ Ce plat n'existe pas !");
                return;
            }

            int idCuisinier = Convert.ToInt32(result);

            // 👉 Insertion de la commande avec l'idCuisinier récupéré
            string insertQuery = @"INSERT INTO Commande (nom, prix, tempsPreparation, statut, date, idClient, commentaire, idCuisinier)
                               VALUES (@nom, @prix, @tempsPreparation, @statut, @date, @idClient, @commentaire, @idCuisinier)";

            MySqlCommand insertCmd = new MySqlCommand(insertQuery, connection);
            insertCmd.Parameters.AddWithValue("@nom", nomPlat);
            insertCmd.Parameters.AddWithValue("@prix", totalPrix);
            insertCmd.Parameters.AddWithValue("@tempsPreparation", 30); // temporaire
            insertCmd.Parameters.AddWithValue("@statut", "en attente");
            insertCmd.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd"));
            insertCmd.Parameters.AddWithValue("@idClient", idClient);
            insertCmd.Parameters.AddWithValue("@commentaire", commentaire);
            insertCmd.Parameters.AddWithValue("@idCuisinier", idCuisinier);

            insertCmd.ExecuteNonQuery();

            Console.WriteLine("\n✅ Paiement effectué avec succès !");
            Console.WriteLine("Merci pour votre commande !");
        }
    }

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
          

            string query = "INSERT INTO ingredient (nom,  origine, idPlat) " +
                           "VALUES (@nom, @origine, @idPlat)";

            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@nom", nomIngredient);
            cmd.Parameters.AddWithValue("@origine", origine);
            cmd.Parameters.AddWithValue("@idPlat", idPlat);

            cmd.ExecuteNonQuery();
            Console.WriteLine("Ingrédient ajouté !");
        }
    }

    static void VoirCommandesAPreparer(int idCuisinier)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT idCommande, nom, prix, statut, date, idClient, commentaire " +
                           "FROM commande WHERE idCuisinier = @idCuisinier";

            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@idCuisinier", idCuisinier);

            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (!reader.HasRows)
                {
                    Console.WriteLine("Aucune commande à préparer.");
                    return;
                }

                Console.WriteLine("\nCommandes à préparer :");
                while (reader.Read())
                {
                    Console.WriteLine($"Commande #{reader["idCommande"]} - {reader["nom"]} - {reader["prix"]}€");
                    Console.WriteLine($"Client: {reader["idClient"]} | Statut: {reader["statut"]} | Date: {reader["date"]}"); // mettre le temps estimée et la distance
                    Console.WriteLine($"Commentaire: {reader["commentaire"]}\n");
                }
            }
            connection.Close();
        }
    }

    static void Mettreàjourcommande()
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            Console.WriteLine("Quelle est la commande à mettre à jour ?");
            int idCommande = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Quel est le nouveau statut de la commande ?");
            string nouveauStatut = Console.ReadLine();
            connection.Open();
            string query = "UPDATE Commande SET statut = @statut WHERE idCommande =@idCommande ";
            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@idCommande", idCommande);
            command.Parameters.AddWithValue("@statut", nouveauStatut);
            int rowsAffected = command.ExecuteNonQuery();
            Console.WriteLine(rowsAffected > 0 ? "Commande mise à jour avec succès !" : "Erreur lors de la mise à jour de la commande.");
        }
    }


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

    // La méthode AfficherGraphe reste identique à votre version originale
    static void AfficherGraphe(Graphe<string> graphe, string nomFichier)
    {
        const int width = 2000;
        const int height = 2000;
        const int marge = 50;

        // 1. Calcul des bornes du graphe
        double minLon = graphe.Noeuds.Min(n => n.Longitude);
        double maxLon = graphe.Noeuds.Max(n => n.Longitude);
        double minLat = graphe.Noeuds.Min(n => n.Latitude);
        double maxLat = graphe.Noeuds.Max(n => n.Latitude);

        // 2. Création de la surface de dessin
        using (var surface = SKSurface.Create(new SKImageInfo(width, height)))
        {
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.White);

            // 3. Configuration des styles
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

            // 4. Dessin des liens
            foreach (var lien in graphe.Liens)
            {
                float x1 = marge + (float)((lien.Source.Longitude - minLon) / (maxLon - minLon) * (width - 2 * marge));
                float y1 = marge + (float)((maxLat - lien.Source.Latitude) / (maxLat - minLat) * (height - 2 * marge));
                float x2 = marge + (float)((lien.Destination.Longitude - minLon) / (maxLon - minLon) * (width - 2 * marge));
                float y2 = marge + (float)((maxLat - lien.Destination.Latitude) / (maxLat - minLat) * (height - 2 * marge));

                canvas.DrawLine(x1, y1, x2, y2, paintLien);
            }

            // 5. Dessin des noeuds
            foreach (var noeud in graphe.Noeuds)
            {
                float x = marge + (float)((noeud.Longitude - minLon) / (maxLon - minLon) * (width - 2 * marge));
                float y = marge + (float)((maxLat - noeud.Latitude) / (maxLat - minLat) * (height - 2 * marge));

                // Dessin du cercle
                canvas.DrawCircle(x, y, 8, paintNoeud);

                // Dessin du texte (libellé)
                canvas.DrawText(noeud.Libelle, x, y - 15, paintTexte);
            }

            // 6. Sauvegarde de l'image
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
