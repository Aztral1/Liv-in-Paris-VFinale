using System;
using System.Collections.Generic;
using System.Linq;

public static class Chemin<T>
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
            bool updated = false;
            foreach (var lien in graphe.Liens)
            {
                if (distances[lien.Source] != double.MaxValue &&
                    distances[lien.Source] + lien.Poids < distances[lien.Destination])
                {
                    distances[lien.Destination] = distances[lien.Source] + lien.Poids;
                    updated = true;
                }
            }
            if (!updated) break; // Optimisation
        }

        return distances;
    }
    public static List<Noeud<string>> ReconstruireCheminBellmanFord(
    Dictionary<Noeud<string>, double> distances,
    Graphe<string> graphe,
    Noeud<string> depart,
    Noeud<string> arrivee)
    {
        double INF = double.MaxValue / 2;

        if (distances[arrivee] >= INF)
        {
            return new List<Noeud<string>>();
        }

        var chemin = new List<Noeud<string>> { arrivee };
        var courant = arrivee;
        int limite = graphe.Noeuds.Count;

        while (courant != depart && limite-- > 0)
        {
            var precedent = graphe.Noeuds
                .Where(n => n.Liens.Any(l => l.Destination == courant))
                .OrderBy(n => distances[n] + n.Liens.First(l => l.Destination == courant).Poids)
                .FirstOrDefault();

            if (precedent == null || distances[precedent] >= INF)
            {
                return new List<Noeud<string>>();
            }

            chemin.Insert(0, precedent);
            courant = precedent;
        }

        if (limite <= 0 || courant != depart)
        {
            return new List<Noeud<string>>();
        }

        return chemin;
    }
    public static (Dictionary<(Noeud<T>, Noeud<T>), double> distances,
                  Dictionary<(Noeud<T>, Noeud<T>), Noeud<T>> predecesseurs)
        FloydWarshall(Graphe<T> graphe)
    {
        var distances = new Dictionary<(Noeud<T>, Noeud<T>), double>();
        var predecesseurs = new Dictionary<(Noeud<T>, Noeud<T>), Noeud<T>>();

        // Initialisation
        foreach (var u in graphe.Noeuds)
        {
            foreach (var v in graphe.Noeuds)
            {
                if (u == v)
                {
                    distances[(u, v)] = 0;
                }
                else
                {
                    // Vérifie s'il y a un lien direct
                    var lienDirect = u.Liens.FirstOrDefault(l => l.Destination == v);
                    if (lienDirect != null)
                    {
                        distances[(u, v)] = lienDirect.Poids;
                        predecesseurs[(u, v)] = u;
                    }
                    else
                    {
                        distances[(u, v)] = double.MaxValue;
                    }
                }
            }
        }

        // Algorithme principal
        foreach (var k in graphe.Noeuds)
        {
            foreach (var i in graphe.Noeuds)
            {
                foreach (var j in graphe.Noeuds)
                {
                    // Prend en compte le temps de changement si on change de ligne
                    double changement = (i != k && k != j && i.Lignes.Intersect(k.Lignes).Count() == 0)
                        ? k.TempsChangement
                        : 0;

                    if (distances[(i, k)] + distances[(k, j)] + changement < distances[(i, j)])
                    {
                        distances[(i, j)] = distances[(i, k)] + distances[(k, j)] + changement;
                        predecesseurs[(i, j)] = predecesseurs[(k, j)];
                    }
                }
            }
        }

        return (distances, predecesseurs);
    }

    public static List<Noeud<T>> ReconstruireCheminFloydWarshall(
        Dictionary<(Noeud<T>, Noeud<T>), Noeud<T>> predecesseurs,
        Noeud<T> depart, Noeud<T> arrivee)
    {
        var chemin = new List<Noeud<T>>();

        if (predecesseurs.TryGetValue((depart, arrivee), out var precedent))
        {
            if (precedent == null)
            {
                return new List<Noeud<T>>(); // Pas de chemin
            }

            var courant = arrivee;
            while (courant != depart)
            {
                chemin.Insert(0, courant);
                courant = predecesseurs[(depart, courant)];
            }
            chemin.Insert(0, depart);
        }

        return chemin;
    }
}   
