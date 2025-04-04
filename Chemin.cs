using System;
using System.Collections.Generic;
using System.Linq;

public static class Dijkstra<T>
{
    public static List<Noeud<T>> Dijsktra(Graphe<T> graphe, Noeud<T> depart, Noeud<T> arrivee)
    {
        // Initialisation
        var distances = new Dictionary<Noeud<T>, double>();
        var precedents = new Dictionary<Noeud<T>, Noeud<T>>();
        var aVisiter = new List<Noeud<T>>();

        foreach (var noeud in graphe.Noeuds)
        {
            distances[noeud] = noeud == depart ? 0 : double.MaxValue;
            precedents[noeud] = null;
            aVisiter.Add(noeud);
        }

        while (aVisiter.Count > 0)
        {
            // Trouver le nœud non visité le plus proche
            var courant = aVisiter.OrderBy(n => distances[n]).First();

            // Condition d'arrêt
            if (courant == arrivee || distances[courant] == double.MaxValue)
                break;

            aVisiter.Remove(courant);

            // Explorer les voisins
            foreach (var lien in courant.Liens)
            {
                var voisin = lien.Destination;
                if (!aVisiter.Contains(voisin)) continue;

                double distanceAlternative = distances[courant] + lien.Poids;
                if (distanceAlternative < distances[voisin])
                {
                    distances[voisin] = distanceAlternative;
                    precedents[voisin] = courant;
                }
            }
        }

        // Reconstruction du chemin
        return ReconstruireChemin(precedents, depart, arrivee);
    }

    private static List<Noeud<T>> ReconstruireChemin(Dictionary<Noeud<T>, Noeud<T>> precedents,
                                                    Noeud<T> depart, Noeud<T> arrivee)
    {
        var chemin = new List<Noeud<T>>();
        var courant = arrivee;

        // Remonter le chemin jusqu'au départ
        while (courant != null && !courant.Equals(depart))
        {
            chemin.Insert(0, courant);
            courant = precedents.GetValueOrDefault(courant);
        }

        // Vérifier si on a bien trouvé un chemin complet
        if (courant != null && courant.Equals(depart))
        {
            chemin.Insert(0, depart);
            return chemin;
        }

        return new List<Noeud<T>>(); // Aucun chemin trouvé
    }
    public static Dictionary<Noeud<T>, double> BellmanFord(Graphe<T> graphe, Noeud<T> source)
    {
        var distances = new Dictionary<Noeud<T>, double>();
        foreach (var node in graphe.Noeuds)
            distances[node] = (node == source) ? 0 : double.MaxValue;

        for (int i = 0; i < graphe.Noeuds.Count - 1; i++)
        {
            bool modifie = false;
            foreach (var lien in graphe.Liens)
            {
                if (distances[lien.Source] != double.MaxValue &&
                    distances[lien.Source] + lien.Poids < distances[lien.Destination])
                {
                    distances[lien.Destination] = distances[lien.Source] + lien.Poids;
                    modifie = true;
                }
            }
            if (!modifie) break; // Optimisation
        }

        return distances;
    }
    public static Dictionary<Noeud<T>, Dictionary<Noeud<T>, double>> FloydMarshall(Graphe<T> graphe)
    {
        var dist = new Dictionary<Noeud<T>, Dictionary<Noeud<T>, double>>();

        // Initialisation
        foreach (var u in graphe.Noeuds)
        {
            dist[u] = new Dictionary<Noeud<T>, double>();
            foreach (var v in graphe.Noeuds)
                dist[u][v] = (u == v) ? 0 : double.MaxValue;
        }

        foreach (var lien in graphe.Liens)
            dist[lien.Source][lien.Destination] = lien.Poids;

        // Algorithme
        foreach (var k in graphe.Noeuds)
            foreach (var i in graphe.Noeuds)
                foreach (var j in graphe.Noeuds)
                    if (dist[i][k] != double.MaxValue && dist[k][j] != double.MaxValue)
                        dist[i][j] = Math.Min(dist[i][j], dist[i][k] + dist[k][j]);

        return dist;
    }
}
