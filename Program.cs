using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
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

            string choix = Console.ReadLine();

            if (choix == "2") break;

            if (choix == "1")
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
    }


    
    /// conversion degrés -> radians
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
