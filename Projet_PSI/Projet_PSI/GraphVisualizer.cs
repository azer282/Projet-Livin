using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Projet_PSI
{
    public class GraphVisualizer : SKElement
    {
        private Graphe<int> graphe;
        private Dictionary<int, SKPoint> nodePositions;

        public GraphVisualizer(Graphe<int> graphe)
        {
            this.graphe = graphe;
            this.PaintSurface += OnPaintSurface;
            GenerateNodePositions();
        }

        private void GenerateNodePositions()
        {
            nodePositions = new Dictionary<int, SKPoint>();
            Random rand = new Random();
            int canvasWidth = 600;
            int canvasHeight = 400;

            foreach (var noeud in graphe.Noeuds)
            {
                float x = rand.Next(50, canvasWidth - 50);
                float y = rand.Next(50, canvasHeight - 50);
                nodePositions[noeud.identite] = new SKPoint(x, y);
            }
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            canvas.Clear(SKColors.White);

            using (var edgePaint = new SKPaint { Color = SKColors.Black, StrokeWidth = 3 })
            using (var nodePaint = new SKPaint { Color = SKColors.Blue, IsAntialias = true })
            using (var textPaint = new SKPaint { Color = SKColors.Black, TextSize = 20, IsAntialias = true })
            {
                // Dessiner les arêtes
                foreach (var noeud in graphe.Noeuds)
                {
                    foreach (var voisin in noeud.voisins)
                    {
                        SKPoint start = nodePositions[noeud.identite];
                        SKPoint end = nodePositions[voisin.Item1.identite];
                        canvas.DrawLine(start, end, edgePaint);
                    }
                }

                // Dessiner les nœuds
                foreach (var noeud in graphe.Noeuds)
                {
                    SKPoint position = nodePositions[noeud.identite];
                    canvas.DrawCircle(position, 20, nodePaint);
                    canvas.DrawText(noeud.identite.ToString(), position.X - 10, position.Y - 30, textPaint);
                }
            }
        }
    }

    public class GraphWindow : Window
    {
        public GraphWindow(Graphe<int> graphe)
        {
            Title = "Visualisation du Graphe";
            Width = 650;
            Height = 450;

            var skElement = new GraphVisualizer(graphe);
            Content = skElement;
        }
    }
}
