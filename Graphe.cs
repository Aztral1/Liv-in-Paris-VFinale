using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivinParisVfinale
{
    public class Graphe<T>
    {
        /// Représente un graphe composé de nœuds et de liens génériques.
        private Dictionary<T, Noeud<T>> noeuds = new Dictionary<T, Noeud<T>>();
        private int[,] Matriceadjacence;

        /// Ajoute un nœud au graphe.
        public void AjouteNoeud(T id)
        {
            if (!noeuds.ContainsKey(id))
                noeuds[id] = new Noeud<T>(id);
        }

        /// Ajoute un lien entre deux nœuds.
        public void AjouteLien(T id1, T id2)
        {
            if (noeuds.ContainsKey(id1) && noeuds.ContainsKey(id2))
            {
                noeuds[id1].Adjacents.Add(noeuds[id2]);
                noeuds[id2].Adjacents.Add(noeuds[id1]);
            }
        }

        /// Construit la matrice d'adjacence du graphe.
        public void Matrice_Adjacence()
        {
            int size = noeuds.Count;
            Matriceadjacence = new int[size, size];
            var keys = noeuds.Keys.ToList();

            for (int i = 0; i < size; i++)
            {
                var noeud = noeuds[keys[i]];
                foreach (var adjacent in noeud.Adjacents)
                {
                    int j = keys.IndexOf(adjacent.IdNoeud);
                    Matriceadjacence[i, j] = 1;
                    Matriceadjacence[j, i] = 1;
                }
            }
        }

        /// Parcours en largeur (BFS) à partir d'un nœud donné.
        public void BFS(T start)
        {
            HashSet<T> parcouru = new HashSet<T>();
            Queue<T> queue = new Queue<T>();
            queue.Enqueue(start);
            parcouru.Add(start);

            while (queue.Count > 0)
            {
                T nodeId = queue.Dequeue();
                Console.Write(nodeId + " ");
                foreach (var adjacent in noeuds[nodeId].Adjacents)
                {
                    if (!parcouru.Contains(adjacent.IdNoeud))
                    {
                        parcouru.Add(adjacent.IdNoeud);
                        queue.Enqueue(adjacent.IdNoeud);
                    }
                }
            }
            Console.WriteLine();
        }

        /// Parcours en profondeur (DFS) à partir d'un nœud donné.
        public void DFS(T start)
        {
            HashSet<T> parcouru = new HashSet<T>();
            Stack<T> stack = new Stack<T>();
            stack.Push(start);

            while (stack.Count > 0)
            {
                T Idnoeud = stack.Pop();
                if (!parcouru.Contains(Idnoeud))
                {
                    Console.Write(Idnoeud + " ");
                    parcouru.Add(Idnoeud);
                    foreach (var adjacent in noeuds[Idnoeud].Adjacents)
                    {
                        stack.Push(adjacent.IdNoeud);
                    }
                }
            }
            Console.WriteLine();
        }

        /// Lit un fichier pour construire le graphe.
        public void ReadFile(string cheminfichier)
        {
            using (StreamReader sr = new StreamReader(cheminfichier))
            {
                string ligne;
                while ((ligne = sr.ReadLine()) != null)
                {
                    if (ligne.StartsWith("%") || ligne.StartsWith("%%"))
                        continue;

                    string[] parties = ligne.Split(' '); 
                    if (parties.Length == 2)
                    {
                        T noeud1 = (T)Convert.ChangeType(parties[0], typeof(T));
                        T noeud2 = (T)Convert.ChangeType(parties[1], typeof(T));

                        AjouteNoeud(noeud1);
                        AjouteNoeud(noeud2);
                        AjouteLien(noeud1, noeud2);
                    }
                }
            }
        }

        /// Vérifie si le graphe est connexe.
        public bool EstConnecté()
        {
            if (noeuds.Count == 0) return false;

            HashSet<T> parcouru = new HashSet<T>();
            Queue<T> queue = new Queue<T>();

            T Depart = noeuds.Keys.First();
            queue.Enqueue(Depart);
            parcouru.Add(Depart);

            while (queue.Count > 0)
            {
                T Idnoeud = queue.Dequeue();
                foreach (var adjacent in noeuds[Idnoeud].Adjacents)
                {
                    if (!parcouru.Contains(adjacent.IdNoeud))
                    {
                        parcouru.Add(adjacent.IdNoeud);
                        queue.Enqueue(adjacent.IdNoeud);
                    }
                }
            }

            return parcouru.Count == noeuds.Count;
        }

        /// Vérifie si le graphe contient un cycle.
        public bool Cycleoupas()
        {
            HashSet<T> parcouru = new HashSet<T>();

            foreach (var noeud in noeuds.Keys)
            {
                if (!parcouru.Contains(noeud) && CycleDFS(noeud, parcouru, null))
                {
                    return true;
                }
            }
            return false;
        }

        private bool CycleDFS(T debut, HashSet<T> parcouru, T parent)
        {
            parcouru.Add(debut);

            foreach (var adjacent in noeuds[debut].Adjacents)
            {
                if (!parcouru.Contains(adjacent.IdNoeud))
                {
                    if (CycleDFS(adjacent.IdNoeud, parcouru, debut))
                        return true;
                }
                else if (!EqualityComparer<T>.Default.Equals(adjacent.IdNoeud, parent))
                {
                    return true;
                }
            }
            return false;
        }

        /// Retourne l'ordre du graphe (nombre de nœuds).
        public int Ordre()
        {
            return noeuds.Count;
        }

        /// Retourne la taille du graphe (nombre d'arêtes).
        public int Taille()
        {
            int count = 0;
            foreach (var node in noeuds.Values)
            {
                count += node.Adjacents.Count;
            }
            return count / 2; 
        }

        /// Affiche la matrice d'adjacence du graphe.
        public void AfficherMatriceAdjacence()
        {
            if (Matriceadjacence == null)
            {
                Console.WriteLine("La matrice d'adjacence n'a pas été initialisée.");
                return;
            }

            int size = Matriceadjacence.GetLength(0);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Console.Write(Matriceadjacence[i, j] + " ");
                }
                Console.WriteLine();
            }
        }

        public void DessinerGrapheAvecSkiaSharp(string cheminImage)
        {
            int largeur = 1000;
            int hauteur = 1000;

            using (var surface = SKSurface.Create(new SKImageInfo(largeur, hauteur)))
            {
                var canvas = surface.Canvas;
                canvas.Clear(SKColors.White);

                var paintNoeud = new SKPaint
                {
                    Color = SKColors.LightBlue,
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill
                };

                var paintTexte = new SKPaint
                {
                    Color = SKColors.Black,
                    TextSize = 24,
                    IsAntialias = true,
                    TextAlign = SKTextAlign.Center
                };

                var paintLien = new SKPaint
                {
                    Color = SKColors.Black,
                    StrokeWidth = 2,
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke
                };

                int centreX = largeur / 2;
                int centreY = hauteur / 2;
                int rayon = 400;

                int nombreNoeuds = noeuds.Count;
                double angleIncrement = 2 * Math.PI / nombreNoeuds;

                Dictionary<T, SKPoint> positionsNoeuds = new Dictionary<T, SKPoint>();

                int index = 0;
                foreach (var noeud in noeuds.Values)
                {
                    double angle = angleIncrement * index;
                    int x = (int)(centreX + rayon * Math.Cos(angle));
                    int y = (int)(centreY + rayon * Math.Sin(angle));

                    positionsNoeuds[noeud.IdNoeud] = new SKPoint(x, y);

                    index++;
                }

                foreach (var noeud in noeuds.Values)
                {
                    var positionNoeud = positionsNoeuds[noeud.IdNoeud];

                    foreach (var adjacent in noeud.Adjacents)
                    {
                        var positionAdjacent = positionsNoeuds[adjacent.IdNoeud];
                        canvas.DrawLine(positionNoeud, positionAdjacent, paintLien);
                    }
                }

                foreach (var noeud in noeuds.Values)
                {
                    var position = positionsNoeuds[noeud.IdNoeud];

                    canvas.DrawCircle(position, 30, paintNoeud);
                    canvas.DrawText(noeud.IdNoeud.ToString(), position.X, position.Y + 8, paintTexte);
                }

                using (var image = surface.Snapshot())
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                using (var stream = File.OpenWrite(cheminImage))
                {
                    data.SaveTo(stream);
                }
            }

            Process.Start(new ProcessStartInfo(cheminImage) { UseShellExecute = true });
            Console.WriteLine($"Graphe dessiné et enregistré dans {cheminImage}");
        }
    }
}




