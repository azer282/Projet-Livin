using System;
using System.Drawing;
using System.Drawing.Imaging;

public class GraphVisualizer
{
    public static void GenerateGraphImage(Graphe<int> graphe, string filePath)
    {
        int width = 800, height = 600;
        Random rand = new Random();

        using (Bitmap bitmap = new Bitmap(width, height))
        using (Graphics g = Graphics.FromImage(bitmap))
        using (Pen edgePen = new Pen(Color.Black, 2))
        using (Brush nodeBrush = new SolidBrush(Color.Blue))
        {
            g.Clear(Color.White);

            // Générer des positions aléatoires pour les nœuds
            var nodePositions = new Dictionary<int, Point>();
            foreach (var noeud in graphe.Noeuds)
            {
                nodePositions[noeud.Id] = new Point(rand.Next(50, width - 50), rand.Next(50, height - 50));
            }

            // Dessiner les arêtes
            foreach (var lien in graphe.Liens)
            {
                Point p1 = nodePositions[lien.Source.Id];
                Point p2 = nodePositions[lien.Destination.Id];
                g.DrawLine(edgePen, p1, p2);
            }

            // Dessiner les nœuds
            foreach (var noeud in graphe.Noeuds)
            {
                Point p = nodePositions[noeud.Id];
                g.FillEllipse(nodeBrush, p.X - 10, p.Y - 10, 20, 20);
                g.DrawString(noeud.Id.ToString(), new Font("Arial", 12), Brushes.Black, p.X + 5, p.Y + 5);
            }

            bitmap.Save(filePath, ImageFormat.Png);
        }
    }
}

