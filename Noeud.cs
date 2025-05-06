using System.Collections.Generic;

public class Noeud<T>
{
    /// <summary>
    /// identifiant unique du noeud
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// libellé associé au nom (station de métro)
    /// </summary>
    public T Libelle { get; }

    /// <summary>
    /// nom de la ligne 
    /// </summary>
    public string LibelleLigne { get; }

    /// <summary>
    /// coordonnée géographique en longitude du noeud
    /// </summary>
    public double Longitude { get; }

    /// <summary>
    /// coordonnée géographique en latitude du nœud
    /// </summary>
    public double Latitude { get; }

    /// <summary>
    /// nom de la commune où se situe le noeud
    /// </summary>
    public string Commune { get; }

    /// <summary>
    /// code INSEE de la commune
    /// </summary>
    public string CodeInsee { get; }

    /// <summary>
    /// liste des liens partant du noeud 
    /// </summary>
    public List<Lien<T>> Liens { get; } = new List<Lien<T>>();

    /// <summary>
    /// temps de changemebnt à partir de ce noeud
    /// </summary>
    public double TempsChangement { get; set; }

    // Ajouter cette propriété à la classe Noeud pour accéder aux lignes
    public List<string> Lignes
    {
        get
        {
            return new List<string> { LibelleLigne };
        }
    }

    /// <summary>
    /// constructeur qui initialise toutes les propriétés d’un noeud.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="libelle"></param>
    /// <param name="libelleLigne"></param>
    /// <param name="longitude"></param>
    /// <param name="latitude"></param>
    /// <param name="commune"></param>
    /// <param name="codeInsee"></param>
    /// <param name="tempschangement"></param>
    public Noeud(int id, T libelle, string libelleLigne, double longitude, double latitude, string commune, string codeInsee, double tempschangement)
    {

        Id = id;
        Libelle = libelle;
        LibelleLigne = libelleLigne;
        Longitude = longitude;
        Latitude = latitude;
        Commune = commune;
        CodeInsee = codeInsee;
        TempsChangement = tempschangement;
    }

    /// <summary>
    /// ajoute un lien sortant vers un autre noeud
    /// </summary>
    /// <param name="destination"></param>
    /// <param name="poids"></param>
    /// <param name="bidirectionnel"></param>
    public void AjouterLien(Noeud<T> destination, double poids, bool bidirectionnel = false)
    {
        Liens.Add(new Lien<T>(this, destination, poids));
        if (bidirectionnel)
        {
            destination.Liens.Add(new Lien<T>(destination, this, poids));
        }
    }
}
