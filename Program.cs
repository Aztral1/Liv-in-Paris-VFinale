

using System;
            using System.Collections.Generic;
            using System.Data.OleDb;
            using System.IO;
            using System.Linq;
            using SkiaSharp;
            using MySql.Data.MySqlClient;




class Program
    {
        static string connectionString = "server=localhost;database=premierRenduPSI;user=root;password=;";

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
        while (true) // Boucle jusqu'à ce que les informations soient correctes
        {
            Console.Write("\nEmail : ");
            string email = Console.ReadLine();

            Console.Write("Mot de passe : ");
            string password = Console.ReadLine();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Vérifier si c'est un Client
                string queryClient = "SELECT * FROM Client WHERE email = @Email AND motDePasse = @Password";
                MySqlCommand cmdClient = new MySqlCommand(queryClient, connection);
                cmdClient.Parameters.AddWithValue("@Email", email);
                cmdClient.Parameters.AddWithValue("@Password", password);
                MySqlDataReader reader = cmdClient.ExecuteReader();

                if (reader.Read()) // Si un Client est trouvé
                {
                    Console.WriteLine($"\nBienvenue, {reader["prenom"]} {reader["nom"]} (Client) !");
                    reader.Close();
                    connection.Close();
                    MenuClient();
                    return;
                }
                reader.Close();

                // Vérifier si c'est un Cuisinier
                string queryCuisinier = "SELECT * FROM Cuisinier WHERE email = @Email AND motDePasse = @Password";
                MySqlCommand cmdCuisinier = new MySqlCommand(queryCuisinier, connection);
                cmdCuisinier.Parameters.AddWithValue("@Email", email);
                cmdCuisinier.Parameters.AddWithValue("@Password", password);
                reader = cmdCuisinier.ExecuteReader();

                if (reader.Read()) // Si un Cuisinier est trouvé
                {
                    Console.WriteLine($"\nBienvenue, {reader["prenom"]} {reader["nom"]} (Cuisinier) !");
                    reader.Close();
                    connection.Close();
                    MenuCuisinier();
                    return;
                }

                Console.WriteLine("Email ou mot de passe incorrect. Veuillez réessayer !");
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
            string password = Console.ReadLine();
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
                    query = "INSERT INTO Client (nom, prenom, email, motDePasse, rue, numMaison, codePostal, numTel, ville, totalCommande, metroProche) VALUES (@Nom, @Prenom, @Email, @Password, @rue, @numMaison, @codePostal, @numTel, @ville, @totalCommande, @metroProche)";
                }
                else if (role == "2")  // Inscription en tant que Cuisinier
                {
                    query = "INSERT INTO Cuisinier (nom, prenom, email, motDePasse, rue, numMaison, codePostal, numTel, ville, totalCommande, metroProche) VALUES (@Nom, @Prenom, @Email, @Password, @rue, @numMaison, @codePostal, @numTel, @ville, @totalCommande, @metroProche)";
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

        static void MenuClient()
        {
            while (true)
            {
                Console.WriteLine("\n--- Menu Client ---");
                Console.WriteLine("1. Commander un plat");
                Console.WriteLine("2. Voir les cuisiniers disponibles");
                Console.WriteLine("3. Se déconnecter");
                Console.Write("Choisissez une option : ");
                string choix = Console.ReadLine();

                switch (choix)
                {
                    case "1":
                        CommanderPlat();
                        break;
                    case "2":
                        VoirCuisiniers();
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("Option invalide, veuillez réessayer.");
                        break;
                }
            }
        }

    static void MenuCuisinier()
    {
        while (true)
        {
            Console.WriteLine("\n--- Menu Cuisinier ---");
            Console.WriteLine("1. Modifier mon menu");
            Console.WriteLine("2. Voir mes plats");
            Console.WriteLine("3. Voir mes clients");
            Console.WriteLine("4. Se déconnecter");
            Console.Write("Choisissez une option : ");
            string choix = Console.ReadLine();

            switch (choix)
            {
                case "1":
                    ModifierMenu();
                    break;
                case "2":
                    VoirPlats();
                    break;
                case "3":
                    VoirClients();
                    break;
                case "4":
                    return;
                default:
                    Console.WriteLine("Option invalide, veuillez réessayer.");
                    break;
            }
        }
    }
        static void ModifierMenu()
        {
            Console.Write("\nNom du plat : ");
            string nomPlat = Console.ReadLine();
            Console.Write("Régime alimentaire : ");
            string regime = Console.ReadLine();
            Console.Write("Prix (€) : ");
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

                // Vérifier si un plat existe déjà pour ce cuisinier
                string checkQuery = "SELECT idPlat FROM Plat WHERE idCuisinier = @idCuisinier";
                MySqlCommand checkCmd = new MySqlCommand(checkQuery, connection);
                object result = checkCmd.ExecuteScalar();

                string query;
                if (result != null)  
                {
                    query = "UPDATE Plat SET nomPlat = @nomPlat, regime = @regime, prix = @prix, " +
                            "nationalite = @nationalite, dateFabrication = @dateFabrication, datePeremption = @datePeremption " +
                            "WHERE idCuisinier = @idCuisinier";
                }
                else  
                {
                    query = "INSERT INTO Plat (idCuisinier, nomPlat, regime, prix, nationalite, dateFabrication, datePeremption) " +
                            "VALUES (@idCuisinier, @nomPlat, @regime, @prix, @nationalite, @dateFabrication, @datePeremption)";
                }

                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@nomPlat", nomPlat);
                command.Parameters.AddWithValue("@regime", regime);
                command.Parameters.AddWithValue("@prix", prix);
                command.Parameters.AddWithValue("@nationalite", nationalite);
                command.Parameters.AddWithValue("@dateFabrication", dateFabrication);
                command.Parameters.AddWithValue("@datePeremption", datePeremption);

                int rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine(rowsAffected > 0 ? "Plat ajouté/mis à jour avec succès !" : "Erreur lors de l'ajout/mise à jour du plat.");

                connection.Close();
            }
        }


        static void VoirPlats()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Plat WHERE idCuisinier = @idCuisinier";
                MySqlCommand command = new MySqlCommand(query, connection);



                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Console.WriteLine("\n--- Plat ---");
                    Console.WriteLine($"Nom: {reader["nomPlat"]}, Régime: {reader["regime"]}, Prix: {reader["prix"]}€");
                    Console.WriteLine($"Nationalité: {reader["nationalite"]}");
                    Console.WriteLine($"Date de fabrication: {reader["dateFabrication"]}, Date de péremption: {reader["datePeremption"]}");
                }

                if (!reader.HasRows)
                {
                    Console.WriteLine("Aucun plat trouvé pour ce cuisinier.");
                }

                connection.Close();
            }
        }

        static void CommanderPlat()
        {
            Console.Write("Entrez le nom du plat : ");
            string plat = Console.ReadLine();
            Console.WriteLine($"Commande passée pour {plat} !");
        }


    static void VoirCuisiniers()
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM Cuisinier";
            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataReader reader = command.ExecuteReader();



            Console.WriteLine("\n--- Cuisiniers Disponibles ---");
            while (reader.Read())
            {
                Console.WriteLine($"Nom: {reader["nom"]}");
            }
        }
    }



    static void VoirClients()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Client";
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = command.ExecuteReader();

                Console.WriteLine("\n--- Liste des Clients ---");
                while (reader.Read())
                {
                    Console.WriteLine($"ID: {reader["idClient"]}, Nom: {reader["nom"]}, Email: {reader["email"]}");
                }
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
   
