using System;
using System.Collections.Generic;
using System.Linq;
using SkiaSharp;

public class Graphe<T> 
{
    public List<Noeud<T>> Noeuds { get; } = new List<Noeud<T>>();
    public List<Lien<T>> Liens { get; } = new List<Lien<T>>();

    public void AjouterNoeud(Noeud<T> noeud) => Noeuds.Add(noeud);

    public void AjouterLien(Noeud<T> source, Noeud<T> destination, double poids, bool bidirectionnel = true)
    {
        var lien = new Lien<T>(source, destination, poids);
        Liens.Add(lien);
        source.AjouterLien(destination, poids, bidirectionnel);
    }

    
    public void AfficherGraphe(string nomFichier, List<Noeud<T>> chemin = null)
    {
        const int width = 2000, height = 2000, marge = 50;
        double minLon = Noeuds.Min(n => n.Longitude);
        double maxLon = Noeuds.Max(n => n.Longitude);
        double minLat = Noeuds.Min(n => n.Latitude);
        double maxLat = Noeuds.Max(n => n.Latitude);

        using (var surface = SKSurface.Create(new SKImageInfo(width, height)))
        {
            var canvas = surface.Canvas;
            canvas.Clear(SKColors.White);

            // Styles prédéfinis
            var paintLienNormal = new SKPaint { Color = SKColors.LightGray, StrokeWidth = 3 };
            var paintLienChemin = new SKPaint { Color = SKColors.Blue, StrokeWidth = 6 };
            var paintNoeud = new SKPaint { Color = SKColors.Red, Style = SKPaintStyle.Fill };
            var paintContour = new SKPaint { Color = SKColors.Black, Style = SKPaintStyle.Stroke, StrokeWidth = 2 };
            var paintTexte = new SKPaint { Color = SKColors.Black, TextSize = 16, TextAlign = SKTextAlign.Center };

            // Dessin des liens
            foreach (var lien in Liens)
            {
                float x1 = marge + (float)((lien.Source.Longitude - minLon) / (maxLon - minLon) * (width - 2 * marge));
                float y1 = marge + (float)((maxLat - lien.Source.Latitude) / (maxLat - minLat) * (height - 2 * marge));
                float x2 = marge + (float)((lien.Destination.Longitude - minLon) / (maxLon - minLon) * (width - 2 * marge));
                float y2 = marge + (float)((maxLat - lien.Destination.Latitude) / (maxLat - minLat) * (height - 2 * marge));

                bool estDansChemin = chemin != null && chemin.Contains(lien.Source) && chemin.Contains(lien.Destination)
                    && Math.Abs(chemin.IndexOf(lien.Source) - chemin.IndexOf(lien.Destination)) == 1;

                canvas.DrawLine(x1, y1, x2, y2, estDansChemin ? paintLienChemin : paintLienNormal);
            }

            // Dessin des nœuds
            foreach (var noeud in Noeuds)
            {
                float x = marge + (float)((noeud.Longitude - minLon) / (maxLon - minLon) * (width - 2 * marge));
                float y = marge + (float)((maxLat - noeud.Latitude) / (maxLat - minLat) * (height - 2 * marge));

                bool estDansChemin = chemin != null && chemin.Contains(noeud);

                if (estDansChemin)
                {
                    canvas.DrawCircle(x, y, 10, paintNoeud);
                    canvas.DrawCircle(x, y, 10, paintContour);
                }
                else
                {
                    canvas.DrawCircle(x, y, 8, paintNoeud);
                }

                canvas.DrawText(noeud.Libelle.ToString(), x, y - 15, paintTexte);
            }

            using (var image = surface.Snapshot())
            using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
            using (var stream = File.OpenWrite(nomFichier))
            {
                data.SaveTo(stream);
            }
        }
    }
}
