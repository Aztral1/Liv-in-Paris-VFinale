using System.Collections.Generic;

public class Noeud<T>
{
    public int Id { get; }
    public T Libelle { get; }
    public string LibelleLigne { get; }
    public double Longitude { get; }
    public double Latitude { get; }
    public string Commune { get; }
    public string CodeInsee { get; }
    public List<Lien<T>> Liens { get; } = new List<Lien<T>>();
    public double TempsChangement { get; set; }
    public Noeud(int id, T libelle, string libelleLigne, double longitude, double latitude, string commune, string codeInsee, double tempschangement)
    {
        Random r = new Random();
        Id = id;
        Libelle = libelle;
        LibelleLigne = libelleLigne;
        Longitude = longitude;
        Latitude = latitude;
        Commune = commune;
        CodeInsee = codeInsee;
        TempsChangement = tempschangement;
    }

    public void AjouterLien(Noeud<T> destination, double poids, bool bidirectionnel = false)
    {
        Liens.Add(new Lien<T>(this, destination, poids));
        if (bidirectionnel)
        {
            destination.Liens.Add(new Lien<T>(destination, this, poids));
        }
    }
}
