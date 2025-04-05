    /// <summary>
    /// Classe représentant un lien entre deux noeuds dans un graphe
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Lien<T>
    {    
        /// <summary>
        /// identifiant du premier nœud    
        /// </summary>
        public Noeud<T> Source { get; }

        /// <summary>
        /// identifiant du deuxième nœud.
        /// </summary>
        public Noeud<T> Destination { get; }

        /// <summary>
        /// poids du lien entre les deux nœuds
        /// </summary>
        public double Poids { get; }

        /// <summary>
        /// constructeur qui initialise les deux noeuds et le poids du lien
        /// </summary>
        /// <param name="noeud1"></param>
        /// <param name="noeud2"></param>
        /// <param name="poids"></param>
        public Lien(Noeud<T> source, Noeud<T> destination, double poids)
        {
            Source = source;
            Destination = destination;
            Poids = poids;
        }
    }
