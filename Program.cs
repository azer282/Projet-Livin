// File: Program.cs
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Projet_PSI; // Namespace for Graphe, Noeud

namespace MetroGraphConsoleRenderer
{
    internal class Program
    {
        // Define BOTH CSV file names
        private const string ArcCsvFileName = "MetroParisArc.csv";
        private const string CoordCsvFileName = "MetroParis(Noeuds).csv"; // <-- ADD THIS

        private const string OutputImageName = "MetroGraphGeoOutput.png"; // New output name
        private const int ImageWidth = 2000;
        private const int ImageHeight = 1600;

        static void Main(string[] args)
        {
            Console.WriteLine("Loading Metro Graph Data...");
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Define paths for BOTH files
            string arcFilePath = Path.Combine(baseDirectory, ArcCsvFileName);
            string coordFilePath = Path.Combine(baseDirectory, CoordCsvFileName); // <-- ADD THIS
            string outputImagePath = Path.Combine(baseDirectory, OutputImageName);

            Console.WriteLine($"Arc CSV Path: {arcFilePath}");
            Console.WriteLine($"Coord CSV Path: {coordFilePath}"); // <-- ADD THIS
            Console.WriteLine($"Output Image Path: {outputImagePath}");

            // Call the combined loading method
            Graphe<int> metroGraphe = LoadGraph(arcFilePath, coordFilePath); // Pass both paths

            if (metroGraphe != null && metroGraphe.Noeuds != null && metroGraphe.Noeuds.Count > 0)
            {
                Console.WriteLine($"Graph loaded with {metroGraphe.Noeuds.Count} nodes.");
                // metroGraphe.ImplementerMatrice(); // Optional

                Console.WriteLine($"Attempting to draw graph using coordinates to {outputImagePath}...");
                try
                {
                    // Call the renderer (it now uses coordinates internally)
                    GraphRenderer.DrawGraphToFile(metroGraphe, ImageWidth, ImageHeight, outputImagePath);
                }
                catch (Exception drawEx) { Console.WriteLine($"Error during drawing: {drawEx}"); }
            }
            else
            {
                Console.WriteLine("Failed to load graph or graph is empty. Cannot draw geographic map.");
                GraphRenderer.DrawGraphToFile(null, ImageWidth, ImageHeight, outputImagePath); // Draw blank
            }

            Console.WriteLine("\nProcessing finished. Press Enter to exit.");
            Console.ReadLine();
        }

        // Updated helper method to load the graph using Graphe.ChargerGrapheComplet
        static Graphe<int> LoadGraph(string arcPath, string coordPath) // Takes both paths
        {
            // Basic check if arc file exists (ChargerGrapheComplet does more)
            if (!File.Exists(arcPath))
            {
                Console.WriteLine($"ERROR: Cannot find the essential arc data file: {arcPath}");
                return null;
            }
            try
            {
                // Estimate size (less critical now, but can help initial capacity)
                var arcLines = File.ReadAllLines(arcPath);
                int estimatedSize = arcLines.Length > 0 ? arcLines.Length + 10 : 100;

                Graphe<int> graphe = new Graphe<int>(estimatedSize);

                // *** CALL THE NEW METHOD in Graphe class ***
                graphe.ChargerGrapheComplet(arcPath, coordPath);

                if (graphe.Noeuds == null || graphe.Noeuds.Count == 0)
                {
                    Console.WriteLine("Warning: Graph has no nodes after loading.");
                    return null;
                }
                return graphe;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR loading/parsing graph data:");
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
    }
}