

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
            Console.WriteLine("4. Se déconnecter");
            Console.Write("Choisissez une option : ");
            string choix = Console.ReadLine();

            switch (choix)
            {
                case "1":
                    ModifierMenu(idCuisinier);  // ✅ Passe l'ID
                    break;
                case "2":
                    VoirPlats(idCuisinier);  // ✅ Passe l'ID
                    break;
                case "3":
                    VoirClients(idCuisinier);  
                    break;
                case "4":
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

            // Vérifier si un plat existe déjà pour ce cuisinier
            string checkQuery = "SELECT idPlat FROM Plat WHERE idPlat = @idCuisinier";  // ✅ Vérifie sur idPlat
            MySqlCommand checkCmd = new MySqlCommand(checkQuery, connection);
            checkCmd.Parameters.AddWithValue("@idCuisinier", idCuisinier);
            object result = checkCmd.ExecuteScalar();

            string query;
            if (result != null)
            {
                query = "UPDATE Plat SET nomPlat = @nomPlat, regime = @regime, prix = @prix, " +
                        "nationalite = @nationalite, dateFabrication = @dateFabrication, datePeremption = @datePeremption " +
                        "WHERE idPlat = @idCuisinier";  // ✅ Mise à jour
            }
            else
            {
                query = "INSERT INTO Plat (idPlat, nomPlat, regime, prix, nationalite, dateFabrication, datePeremption) " +
                        "VALUES (@idCuisinier, @nomPlat, @regime, @prix, @nationalite, @dateFabrication, @datePeremption)";  // ✅ Insertion
            }

            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@idCuisinier", idCuisinier);
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
    static void VoirPlats(int idCuisinier)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            // Correction de la requête pour bien récupérer les plats du cuisinier
            string query = "SELECT nomPlat, regime, nationalite, prix FROM Plat WHERE idPlat = @idCuisinier;";
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
                    while (reader.Read())
                    {
                        Console.WriteLine("\n--- Plat ---");
                        Console.WriteLine($"Nom: {reader["nomPlat"]}, Régime: {reader["regime"]}, Prix: {reader["prix"]} euro");
                        Console.WriteLine($"Nationalité: {reader["nationalite"]}");
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
            string query = "SELECT * FROM Cuisinier JOIN Plat ON idPlat = idCuisinier";
            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataReader reader = command.ExecuteReader();



            Console.WriteLine("\n--- Cuisiniers Disponibles ---");
            while (reader.Read())
            {
                Console.WriteLine($"Nom: {reader["nom"]}, Plat :{reader["nomPlat"]}, régime :{reader["regime"]}, Prix :{reader["prix"]}");
            }
        }
    }
    static void VoirClients(int idCuisinier)
    {
        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            string query = @"
            SELECT DISTINCT client.nom, client.prenom, client.email, client.numTel 
            FROM Client 
            JOIN Commande ON client.idClient = commande.idClient
            WHERE commande.idCommande = @idCuisinier;";

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

        Console.WriteLine("\n--- Récapitulatif de la commande ---");
        Console.WriteLine($"Plats : {string.Join(", ", platsCommandes)}");
        Console.WriteLine($"Total à payer : {totalPrix} euro");

        Console.Write("\nSouhaitez-vous ajouter un commentaire pour les cuisiniers ? (oui/non) : ");
        string reponse = Console.ReadLine().ToLower();
        string commentaire = "";

        if (reponse == "oui")
        {
            Console.Write("Écrivez votre commentaire (max 25 caractères) : ");
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

            // Insérer la commande dans la base de données
            string insertQuery = @"INSERT INTO Commande (nom, prix, tempsPreparation, statut, date, idClient, commentaire)
                               VALUES (@nom, @prix, @tempsPreparation, @statut, @date, @idClient, @commentaire)";

            MySqlCommand insertCmd = new MySqlCommand(insertQuery, connection);
            insertCmd.Parameters.AddWithValue("@nom", string.Join(", ", platsCommandes)); // Concatène les plats
            insertCmd.Parameters.AddWithValue("@prix", totalPrix);
            insertCmd.Parameters.AddWithValue("@tempsPreparation", 30); // Temps de préparation par défaut à modifier avec le temps du graphe
            insertCmd.Parameters.AddWithValue("@statut", "en attente");
            insertCmd.Parameters.AddWithValue("@date", DateTime.Now.ToString("yyyy-MM-dd")); // Date actuelle
            insertCmd.Parameters.AddWithValue("@idClient", idClient);
            insertCmd.Parameters.AddWithValue("@commentaire", commentaire);

            insertCmd.ExecuteNonQuery();

            Console.WriteLine("Paiement effectué avec succès ! Commande enregistrée.\nTemps d'attente estimée à : "+30+" minutes\nMerci et à bientôt !");
        }
    }
        static void AfficherChemin(List<Noeud<string>> chemin)
{
    if (chemin == null || chemin.Count == 0)
    {
        Console.WriteLine("⚠️ Aucun chemin trouvé");
        return;
    }

    Console.WriteLine($"\n🗺 Chemin de {chemin.First().Libelle} à {chemin.Last().Libelle} :");
    double total = 0;

    for (int i = 0; i < chemin.Count - 1; i++)
    {
        var lien = chemin[i].Liens.FirstOrDefault(l => l.Destination == chemin[i + 1])
                 ?? chemin[i + 1].Liens.First(l => l.Destination == chemin[i]);

        if (lien == null)
        {
            Console.WriteLine($"Erreur: Lien manquant entre {chemin[i].Libelle} et {chemin[i + 1].Libelle}");
            return;
        }

        total += lien.Poids;
        Console.WriteLine($"  {(i + 1).ToString().PadLeft(2)}. {chemin[i].Libelle} \u279C {chemin[i + 1].Libelle} ({lien.Poids} min)");
    }

    Console.WriteLine($"\n⏱ Total: {total} minutes | 🚉 {chemin.Count} stations");
    Console.WriteLine("------------------------------------------------");
}

static Dictionary<int, Noeud<string>> ChargerNoeuds(string fichierExcel)
{
    var noeuds = new Dictionary<int, Noeud<string>>();
    string connectionString = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={fichierExcel};Extended Properties='Excel 12.0;HDR=YES;IMEX=1'";

    // D'abord charger tous les noeuds de base
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
                double longitude = double.Parse(reader[3].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                double latitude = double.Parse(reader[4].ToString(), System.Globalization.CultureInfo.InvariantCulture);
                string commune = reader[5].ToString();
                string codeInsee = reader[6].ToString();

                // Initialisation avec temps de changement à 0 par défaut
                noeuds.Add(id, new Noeud<string>(id, libelleStation, libelleLigne, longitude, latitude, commune, codeInsee, 0));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur ligne noeud {noeuds.Count + 2}: {ex.Message}");
            }
        }
    }

    // Ensuite charger les temps de changement depuis l'onglet Arcs
    using (OleDbConnection connection = new OleDbConnection(connectionString))
    {
        connection.Open();
        OleDbCommand command = new OleDbCommand("SELECT * FROM [Arcs$] WHERE [Temps de Changement] IS NOT NULL", connection);
        OleDbDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            try
            {
                int idStation = Convert.ToInt32(reader[0]);
                if (noeuds.ContainsKey(idStation))
                {
                    double tempsChangement = Convert.ToDouble(reader["Temps de Changement"].ToString());
                    noeuds[idStation].TempsChangement = tempsChangement;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur chargement temps changement: {ex.Message}");
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
                    if (noeuds.ContainsKey(idPrecedent))
                    {
                        var precedent = noeuds[idPrecedent];
                        var current = noeuds[idStation];
                        arcs.Add(Tuple.Create(precedent, current, temps));
                        arcs.Add(Tuple.Create(current, precedent, temps));
                    }
                }

                // Gestion des liens suivants
                if (!string.IsNullOrEmpty(reader[3].ToString()))
                {
                    int idSuivant = ParseId(reader[3].ToString());
                    if (noeuds.ContainsKey(idSuivant))
                    {
                        var current = noeuds[idStation];
                        var suivant = noeuds[idSuivant];
                        arcs.Add(Tuple.Create(current, suivant, temps));
                        arcs.Add(Tuple.Create(suivant, current, temps));
                    }
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

// Nouveau: Ajoutez cette classe interne à Program.cs
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
   
