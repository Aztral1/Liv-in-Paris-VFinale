using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivinParisVfinale
{   /// Représente un nœud dans un graphe.
    public class Noeud<T>
    {
        /// Identifiant unique du nœud.
        public T IdNoeud { get; set; }
        /// Liste des nœuds adjacents.
        public List<Noeud<T>> Adjacents { get; set; } = new List<Noeud<T>>();
        /// Constructeur pour créer un nœud avec un identifiant donné.
        public Noeud(T idnoeud)
        {
            IdNoeud = idnoeud;
        }
    }
}
