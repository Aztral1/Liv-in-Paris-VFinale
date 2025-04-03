
    public class Lien<T>
    {    
        /// Identifiant du premier nœud    
        public T Noeud1 { get; set; }
        /// Identifiant du deuxième nœud.
        public T Noeud2 { get; set; }
        
        public double Poids { get; }
        public Lien(T noeud1, T noeud2, double poids)
        {
            Noeud1 = noeud1;
            Noeud2 = noeud2;
            Poids = poids;
        }
    }

