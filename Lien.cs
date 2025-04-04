    /// <summary>
    /// Classe représentant un lien entre deux noeuds dans un graphe
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Lien<T>
    {    
        /// <summary>
        /// identifiant du premier nœud    
        /// </summary>
        public T Noeud1 { get; set; }

        /// <summary>
        /// identifiant du deuxième nœud.
        /// </summary>
        public T Noeud2 { get; set; }

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
        public Lien(T noeud1, T noeud2, double poids)
        {
            Noeud1 = noeud1;
            Noeud2 = noeud2;
            Poids = poids;
        }
    }
