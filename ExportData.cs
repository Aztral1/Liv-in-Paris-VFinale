using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Xml.Serialization;
using MySql.Data.MySqlClient;

/// <summary>
/// Classe responsable de l'exportation des données vers différents formats
/// </summary>
public static class ExportData
{
    /// <summary>
    /// Exporte les données des clients au format JSON
    /// </summary>
    /// <param name="filePath">Chemin du fichier de sortie</param>
    public static void ExportClientsToJson(string filePath)
    {
        var clients = GetClientsFromDatabase();
        string donneesJson = JsonSerializer.Serialize(clients, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, donneesJson);
        Console.WriteLine($"Données des clients exportées avec succès vers {filePath}");
    }

    /// <summary>
    /// Exporte les données des clients au format XML
    /// </summary>
    /// <param name="filePath">Chemin du fichier de sortie</param>
    public static void ExportClientsToXml(string filePath)
    {
        var clients = GetClientsFromDatabase();
        XmlSerializer convertisseurXml = new XmlSerializer(typeof(List<ClientData>));
        using (TextWriter writer = new StreamWriter(filePath))
        {
            convertisseurXml.Serialize(writer, clients);
        }
        Console.WriteLine($"Données des clients exportées avec succès vers {filePath}");
    }

    /// <summary>
    /// Exporte les données des cuisiniers au format JSON
    /// </summary>
    /// <param name="filePath">Chemin du fichier de sortie</param>
    public static void ExportCuisiniersToJson(string filePath)
    {
        var cuisiniers = GetCuisiniersFromDatabase();
        string donneesJson = JsonSerializer.Serialize(cuisiniers, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, donneesJson);
        Console.WriteLine($"Données des cuisiniers exportées avec succès vers {filePath}");
    }

    /// <summary>
    /// Exporte les données des cuisiniers au format XML
    /// </summary>
    /// <param name="filePath">Chemin du fichier de sortie</param>
    public static void ExportCuisiniersToXml(string filePath)
    {
        var cuisiniers = GetCuisiniersFromDatabase();
        XmlSerializer convertisseurXml = new XmlSerializer(typeof(List<CuisinierData>));
        using (TextWriter writer = new StreamWriter(filePath))
        {
            convertisseurXml.Serialize(writer, cuisiniers);
        }
        Console.WriteLine($"Données des cuisiniers exportées avec succès vers {filePath}");
    }

    /// <summary>
    /// Exporte les données des plats au format JSON
    /// </summary>
    /// <param name="filePath">Chemin du fichier de sortie</param>
    public static void ExportPlatsToJson(string filePath)
    {
        var plats = GetPlatsFromDatabase();
        string donneesJson = JsonSerializer.Serialize(plats, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, donneesJson);
        Console.WriteLine($"Données des plats exportées avec succès vers {filePath}");
    }

    /// <summary>
    /// Exporte les données des plats au format XML
    /// </summary>
    /// <param name="filePath">Chemin du fichier de sortie</param>
    public static void ExportPlatsToXml(string filePath)
    {
        var plats = GetPlatsFromDatabase();
        XmlSerializer convertisseurXml = new XmlSerializer(typeof(List<PlatData>));
        using (TextWriter writer = new StreamWriter(filePath))
        {
            convertisseurXml.Serialize(writer, plats);
        }
        Console.WriteLine($"Données des plats exportées avec succès vers {filePath}");
    }

    /// <summary>
    /// Exporte les données des commandes au format JSON
    /// </summary>
    /// <param name="filePath">Chemin du fichier de sortie</param>
    public static void ExportCommandesToJson(string filePath)
    {
        var commandes = GetCommandesFromDatabase();
        string donneesJson = JsonSerializer.Serialize(commandes, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, donneesJson);
        Console.WriteLine($"Données des commandes exportées avec succès vers {filePath}");
    }

    /// <summary>
    /// Exporte les données des commandes au format XML
    /// </summary>
    /// <param name="filePath">Chemin du fichier de sortie</param>
    public static void ExportCommandesToXml(string filePath)
    {
        var commandes = GetCommandesFromDatabase();
        XmlSerializer convertisseurXml = new XmlSerializer(typeof(List<CommandeData>));
        using (TextWriter writer = new StreamWriter(filePath))
        {
            convertisseurXml.Serialize(writer, commandes);
        }
        Console.WriteLine($"Données des commandes exportées avec succès vers {filePath}");
    }

    /// <summary>
    /// Exporte les données du graphe du métro au format JSON
    /// </summary>
    /// <param name="filePath">Chemin du fichier de sortie</param>
    /// <param name="excelPath">Chemin du fichier Excel contenant les données du métro</param>
    public static void ExportMetroGraphToJson(string filePath, string excelPath)
    {
        var noeuds = Program.ChargerNoeuds(excelPath);
        var stationsData = new List<StationMetroData>();

        foreach (var noeud in noeuds.Values)
        {
            var stationData = new StationMetroData
            {
                Id = noeud.Id,
                Libelle = noeud.Libelle,
                LibelleLigne = noeud.LibelleLigne,
                Longitude = noeud.Longitude,
                Latitude = noeud.Latitude,
                Commune = noeud.Commune,
                CodeInsee = noeud.CodeInsee,
                TempsChangement = noeud.TempsChangement
            };
            stationsData.Add(stationData);
        }

        string donneesJson = JsonSerializer.Serialize(stationsData, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, donneesJson);
        Console.WriteLine($"Données du graphe du métro exportées avec succès vers {filePath}");
    }

    /// <summary>
    /// Exporte les données du graphe du métro au format XML
    /// </summary>
    /// <param name="filePath">Chemin du fichier de sortie</param>
    /// <param name="excelPath">Chemin du fichier Excel contenant les données du métro</param>
    public static void ExportMetroGraphToXml(string filePath, string excelPath)
    {
        var noeuds = Program.ChargerNoeuds(excelPath);
        var stationsData = new List<StationMetroData>();

        foreach (var noeud in noeuds.Values)
        {
            var stationData = new StationMetroData
            {
                Id = noeud.Id,
                Libelle = noeud.Libelle,
                LibelleLigne = noeud.LibelleLigne,
                Longitude = noeud.Longitude,
                Latitude = noeud.Latitude,
                Commune = noeud.Commune,
                CodeInsee = noeud.CodeInsee,
                TempsChangement = noeud.TempsChangement
            };
            stationsData.Add(stationData);
        }

        XmlSerializer convertisseurXml = new XmlSerializer(typeof(List<StationMetroData>));
        using (TextWriter writer = new StreamWriter(filePath))
        {
            convertisseurXml.Serialize(writer, stationsData);
        }
        Console.WriteLine($"Données du graphe du métro exportées avec succès vers {filePath}");
    }

    // Méthodes privées pour récupérer les données de la base de données

    private static List<ClientData> GetClientsFromDatabase()
    {
        var clients = new List<ClientData>();
        using (MySqlConnection connection = new MySqlConnection(Program.connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM Client";
            MySqlCommand command = new MySqlCommand(query, connection);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var client = new ClientData
                    {
                        IdClient = Convert.ToInt32(reader["idClient"]),
                        Nom = reader["nom"].ToString(),
                        Prenom = reader["prenom"].ToString(),
                        Email = reader["email"].ToString(),
                        Rue = reader["rue"].ToString(),
                        NumMaison = reader["numMaison"].ToString(),
                        CodePostal = reader["codePostal"].ToString(),
                        NumTel = reader["numTel"].ToString(),
                        Ville = reader["ville"].ToString(),
                        TotalCommande = Convert.ToInt32(reader["totalCommande"]),
                        MetroProche = reader["metroProche"].ToString()
                    };
                    clients.Add(client);
                }
            }
        }
        return clients;
    }

    private static List<CuisinierData> GetCuisiniersFromDatabase()
    {
        var cuisiniers = new List<CuisinierData>();
        using (MySqlConnection connection = new MySqlConnection(Program.connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM Cuisinier";
            MySqlCommand command = new MySqlCommand(query, connection);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var cuisinier = new CuisinierData
                    {
                        IdCuisinier = Convert.ToInt32(reader["idCuisinier"]),
                        Nom = reader["nom"].ToString(),
                        Prenom = reader["prenom"].ToString(),
                        Email = reader["email"].ToString(),
                        Rue = reader["rue"].ToString(),
                        NumMaison = reader["numMaison"].ToString(),
                        CodePostal = reader["codePostal"].ToString(),
                        NumTel = reader["numTel"].ToString(),
                        Ville = reader["ville"].ToString(),
                        TotalCommande = Convert.ToInt32(reader["totalCommande"]),
                        MetroProche = reader["metroProche"].ToString()
                    };
                    cuisiniers.Add(cuisinier);
                }
            }
        }
        return cuisiniers;
    }

    private static List<PlatData> GetPlatsFromDatabase()
    {
        var plats = new List<PlatData>();
        using (MySqlConnection connection = new MySqlConnection(Program.connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM Plat";
            MySqlCommand command = new MySqlCommand(query, connection);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var plat = new PlatData
                    {
                        IdPlat = Convert.ToInt32(reader["idPlat"]),
                        NomPlat = reader["nomPlat"].ToString(),
                        Regime = reader["regime"].ToString(),
                        Prix = Convert.ToDouble(reader["prix"]),
                        Nationalite = reader["nationalite"].ToString(),
                        DateFabrication = reader["dateFabrication"].ToString(),
                        DatePeremption = reader["datePeremption"].ToString(),
                        IdCuisinier = Convert.ToInt32(reader["idCuisinier"])
                    };
                    plats.Add(plat);
                }
            }
        }
        return plats;
    }

    private static List<CommandeData> GetCommandesFromDatabase()
    {
        var commandes = new List<CommandeData>();
        using (MySqlConnection connection = new MySqlConnection(Program.connectionString))
        {
            connection.Open();
            string query = "SELECT * FROM Commande";
            MySqlCommand command = new MySqlCommand(query, connection);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var commande = new CommandeData
                    {
                        IdCommande = Convert.ToInt32(reader["idCommande"]),
                        Nom = reader["nom"].ToString(),
                        Prix = Convert.ToDouble(reader["prix"]),
                        TempsPreparation = Convert.ToInt32(reader["tempsPreparation"]),
                        Statut = reader["statut"].ToString(),
                        Date = reader["date"].ToString(),
                        IdClient = Convert.ToInt32(reader["idClient"]),
                        Commentaire = reader["commentaire"].ToString(),
                        IdCuisinier = Convert.ToInt32(reader["idCuisinier"])
                    };
                    commandes.Add(commande);
                }
            }
        }
        return commandes;
    }
}

/// <summary>
/// Classe de données pour la sérialisation des clients
/// </summary>
[Serializable]
public class ClientData
{
    public int IdClient { get; set; }
    public string Nom { get; set; }
    public string Prenom { get; set; }
    public string Email { get; set; }
    public string Rue { get; set; }
    public string NumMaison { get; set; }
    public string CodePostal { get; set; }
    public string NumTel { get; set; }
    public string Ville { get; set; }
    public int TotalCommande { get; set; }
    public string MetroProche { get; set; }
}

/// <summary>
/// Classe de données pour la sérialisation des cuisiniers
/// </summary>
[Serializable]
public class CuisinierData
{
    public int IdCuisinier { get; set; }
    public string Nom { get; set; }
    public string Prenom { get; set; }
    public string Email { get; set; }
    public string Rue { get; set; }
    public string NumMaison { get; set; }
    public string CodePostal { get; set; }
    public string NumTel { get; set; }
    public string Ville { get; set; }
    public int TotalCommande { get; set; }
    public string MetroProche { get; set; }
}

/// <summary>
/// Classe de données pour la sérialisation des plats
/// </summary>
[Serializable]
public class PlatData
{
    public int IdPlat { get; set; }
    public string NomPlat { get; set; }
    public string Regime { get; set; }
    public double Prix { get; set; }
    public string Nationalite { get; set; }
    public string DateFabrication { get; set; }
    public string DatePeremption { get; set; }
    public int IdCuisinier { get; set; }
}

/// <summary>
/// Classe de données pour la sérialisation des commandes
/// </summary>
[Serializable]
public class CommandeData
{
    public int IdCommande { get; set; }
    public string Nom { get; set; }
    public double Prix { get; set; }
    public int TempsPreparation { get; set; }
    public string Statut { get; set; }
    public string Date { get; set; }
    public int IdClient { get; set; }
    public string Commentaire { get; set; }
    public int IdCuisinier { get; set; }
}

/// <summary>
/// Classe de données pour la sérialisation des stations de métro
/// </summary>
[Serializable]
public class StationMetroData
{
    public int Id { get; set; }
    public string Libelle { get; set; }
    public string LibelleLigne { get; set; }
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public string Commune { get; set; }
    public string CodeInsee { get; set; }
    public double TempsChangement { get; set; }
}
