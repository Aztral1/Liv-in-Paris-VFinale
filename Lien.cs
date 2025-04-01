using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// Représente un lien entre deux nœuds dans un graphe.
namespace LivinParisVfinale
{
    public class Lien<T>
    {
        /// Identifiant du premier nœud    
        public T Noeud1 { get; set; }
        /// Identifiant du deuxième nœud.
        public T Noeud2 { get; set; }
        
        public Lien(T noeud1, T noeud2)
        {
            Noeud1 = noeud1;
            Noeud2 = noeud2;
        }
    }
}
