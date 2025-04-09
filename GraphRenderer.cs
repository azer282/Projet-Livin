// File: GraphRenderer.cs
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Globalization; // Needed if parsing numbers here, but not currently
using Projet_PSI; // Or your specific namespace

public static class GraphRenderer // public static class
{
    private const float NodeRadius = 5f;
    private const float Margin = 30f;

    /// <summary>
    /// Draws the graph to a bitmap file using geographical coordinates from nodes.
    /// </summary>
    public static void DrawGraphToFile(Graphe<int> graph, int width, int height, string outputPath) // Takes Graphe<int>
    {
        // Handle null or empty graph by creating a blank image
        if (graph == null || graph.Noeuds == null || graph.Noeuds.Count == 0)
        {
            Console.WriteLine("Graph is null or empty. Creating blank image.");
            using (Bitmap blankBitmap = new Bitmap(width, height))
            using (Graphics blankG = Graphics.FromImage(blankBitmap))
            using (Font messageFont = new Font("Arial", 12)) // Use using for Font
            using (Brush messageBrush = new SolidBrush(Color.DarkGray)) // Use using for Brush
            {
                blankG.Clear(Color.WhiteSmoke);
                string message = "Graph data not available or empty.";
                SizeF messageSize = blankG.MeasureString(message, messageFont);
                PointF messagePos = new PointF((width - messageSize.Width) / 2f, (height - messageSize.Height) / 2f);
                // FIX CS1501: Ensure DrawString arguments are correct
                blankG.DrawString(message, messageFont, messageBrush, messagePos); // Check again!
                try { blankBitmap.Save(outputPath, ImageFormat.Png); } catch (Exception ex) { Console.WriteLine($"Error saving blank image: {ex.Message}"); }
            }
            return;
        }

        // --- Coordinate Mapping ---
        Console.WriteLine("Mapping coordinates to image pixels...");
        // FIX CS1061: Use node.HasValidCoordinates
        var nodesWithCoords = graph.Noeuds.Where(n => n != null && n.HasValidCoordinates).ToList();

        // FIX CS0019: Check for null before Count
        if (nodesWithCoords == null || nodesWithCoords.Count == 0)
        {
            Console.WriteLine("AVERTISSEMENT: Aucun noeud n'a de coordonnées valides. Impossible de dessiner la carte géographique.");
            // Draw blank image with specific message
            using (Bitmap blankBitmap = new Bitmap(width, height))
            using (Graphics blankG = Graphics.FromImage(blankBitmap))
            using (Font messageFont = new Font("Arial", 12))
            using (Brush messageBrush = new SolidBrush(Color.DarkGray))
            {
                blankG.Clear(Color.WhiteSmoke);
                string message = "Aucune coordonnée valide trouvée pour les noeuds.";
                SizeF messageSize = blankG.MeasureString(message, messageFont);
                PointF messagePos = new PointF((width - messageSize.Width) / 2f, (height - messageSize.Height) / 2f);
                blankG.DrawString(message, messageFont, messageBrush, messagePos); // Use correct DrawString
                try { blankBitmap.Save(outputPath, ImageFormat.Png); } catch (Exception ex) { Console.WriteLine($"Error saving no-coord image: {ex.Message}"); }
            }
            return;
        }

        // Find coordinate bounds
        double minLon = nodesWithCoords.Min(n => n.Longitude); double maxLon = nodesWithCoords.Max(n => n.Longitude);
        double minLat = nodesWithCoords.Min(n => n.Latitude); double maxLat = nodesWithCoords.Max(n => n.Latitude);
        double lonRange = maxLon - minLon; double latRange = maxLat - minLat;
        float effectiveWidth = width - 2 * Margin; float effectiveHeight = height - 2 * Margin;
        bool zeroLonRange = lonRange < 1e-9; bool zeroLatRange = latRange < 1e-9;

        // Create pixel positions dictionary
        var nodePositions = new Dictionary<int, PointF>();
        foreach (var node in nodesWithCoords)
        {
            double relX = zeroLonRange ? 0.5 : (node.Longitude - minLon) / lonRange;
            double relY = zeroLatRange ? 0.5 : (node.Latitude - minLat) / latRange;
            float pixelX = Margin + (float)relX * effectiveWidth;
            float pixelY = Margin + (1.0f - (float)relY) * effectiveHeight; // Invert Y
            if (!nodePositions.ContainsKey(node.identite)) { nodePositions.Add(node.identite, new PointF(pixelX, pixelY)); }
        }
        Console.WriteLine($" -> Min/Max Lon: {minLon:F5} / {maxLon:F5}, Min/Max Lat: {minLat:F5} / {maxLat:F5}");
        Console.WriteLine($" -> Mappé {nodePositions.Count} noeuds sur l'image.");

        // --- Drawing ---
        using (Bitmap bitmap = new Bitmap(width, height))
        using (Graphics g = Graphics.FromImage(bitmap))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias; g.Clear(Color.WhiteSmoke);

            using (Pen edgePen = new Pen(Color.LightSlateGray, 0.8f))
            using (Pen nodeBorderPen = new Pen(Color.DarkSlateBlue, 1.0f))
            using (Brush nodeBrush = new SolidBrush(Color.SkyBlue))
            using (Brush textBrush = new SolidBrush(Color.Black))
            using (Font nodeFont = new Font("Arial", 6f))
            {
                // --- Draw Edges ---
                var drawnEdges = new HashSet<Tuple<int, int>>();
                foreach (Noeud<int> node in graph.Noeuds) // Iterate all nodes for connections
                {
                    if (node == null || !nodePositions.TryGetValue(node.identite, out PointF pos1)) continue;
                    if (node.voisins != null)
                    {
                        foreach ((Noeud<int> voisin, _) in node.voisins)
                        {
                            if (voisin == null || !nodePositions.TryGetValue(voisin.identite, out PointF pos2)) continue;
                            int id1 = Math.Min(node.identite, voisin.identite); int id2 = Math.Max(node.identite, voisin.identite);
                            if (drawnEdges.Add(Tuple.Create(id1, id2))) { g.DrawLine(edgePen, pos1, pos2); }
                        }
                    }
                }
                // --- Draw Nodes ---
                foreach (var kvp in nodePositions) // Iterate mapped positions
                {
                    int nodeId = kvp.Key; PointF pos = kvp.Value;
                    // FIX CS0246: Use int directly, remove T logic
                    Noeud<int> node = graph.Noeuds.FirstOrDefault(n => n != null && n.identite == nodeId);
                    if (node == null) continue;

                    RectangleF nodeRect = new RectangleF(pos.X - NodeRadius, pos.Y - NodeRadius, 2 * NodeRadius, 2 * NodeRadius);
                    g.FillEllipse(nodeBrush, nodeRect); g.DrawEllipse(nodeBorderPen, nodeRect);
                    string label = node.Nom ?? node.identite.ToString();
                    SizeF textSize = g.MeasureString(label, nodeFont);
                    PointF textPos = new PointF(pos.X + NodeRadius, pos.Y - textSize.Height / 2f);
                    g.DrawString(label, nodeFont, textBrush, textPos); // Standard DrawString
                }
            } // Dispose pens/brushes
            // Save bitmap
            try
            {
                string outputDir = Path.GetDirectoryName(outputPath);
                if (!string.IsNullOrEmpty(outputDir) && !Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);
                bitmap.Save(outputPath, ImageFormat.Png);
                Console.WriteLine($"Graph image saved successfully to: {outputPath}");
            }
            catch (Exception ex) { Console.WriteLine($"Error saving image: {ex.Message}"); }
        } // Dispose bitmap/graphics
    } // End DrawGraphToFile
} // End class