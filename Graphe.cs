// File: Graphe.cs

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Globalization; // Required for CultureInfo

// Ensure this namespace matches your Noeud class and project settings
namespace Projet_PSI
{
    /// <summary>
    /// Represents a graph structure, specifically tailored for the Metro Paris data.
    /// Uses a list of nodes (stations) and an adjacency matrix.
    /// Made public for accessibility from other parts of the application like Program.cs and GraphRenderer.cs.
    /// </summary>
    /// <typeparam name="T">The type of the node identifier (expected to be int for this project).</typeparam>
    public class Graphe<T> where T : IEquatable<T> // Added public and constraint
    {
        // --- Fields ---
        private List<Noeud<T>> _noeuds; // Private list of nodes
        public int[,] matrice;          // Adjacency matrix (kept public)
        private Dictionary<T, Noeud<T>> noeudMap; // For fast lookup by ID

        // --- Properties ---
        public List<Noeud<T>> Noeuds => _noeuds; // Public read-only access to node list
        public int[,] Matrice { get { return matrice; } set { matrice = value; } } // Public access to matrix

        // --- Constructor ---
        public Graphe(int taille)
        {
            taille = Math.Max(0, taille); // Ensure non-negative size
            this._noeuds = new List<Noeud<T>>(taille);
            this.matrice = new int[taille, taille];
            this.noeudMap = new Dictionary<T, Noeud<T>>();
        }

        // --- Loading Method ---
        /// <summary>
        /// Loads graph data from TWO CSV files: one for arcs/connections and one for coordinates.
        /// </summary>
        /// <param name="arcFilePath">Path to the CSV containing connections (MetroParisArc.csv format).</param>
        /// <param name="coordFilePath">Path to the CSV containing coordinates (MetroParis(Noeuds).csv format).</param>
        public void ChargerGrapheComplet(string arcFilePath, string coordFilePath) // PUBLIC method
        {
            if (typeof(T) != typeof(int)) { Console.WriteLine("ERREUR: ChargerGrapheComplet expects <int>."); return; }
            if (!File.Exists(arcFilePath)) { Console.WriteLine($"ERREUR : Fichier Arc non trouvé: {arcFilePath}"); return; }
            bool coordFileExists = File.Exists(coordFilePath);
            if (!coordFileExists) { Console.WriteLine($"AVERTISSEMENT : Fichier Coordonnées non trouvé: {coordFilePath}."); }

            var tempNodeMapInt = new Dictionary<int, Noeud<int>>(); // Specific to int for loading

            // --- Pass 1: Read Arcs File (MetroParisArc.csv) ---
            Console.WriteLine($"Lecture du fichier Arc: {Path.GetFileName(arcFilePath)}...");
            string[] arcLines = File.ReadAllLines(arcFilePath);
            foreach (string ligne in arcLines.Skip(1))
            {
                string[] parties = ligne.Split(';');
                if (parties.Length < 7) continue;
                if (int.TryParse(parties[0], out int stationId) && stationId > 0)
                {
                    if (!tempNodeMapInt.ContainsKey(stationId)) { tempNodeMapInt.Add(stationId, new Noeud<int>(stationId, parties[1]?.Trim())); }
                    else { tempNodeMapInt[stationId].Nom = parties[1]?.Trim() ?? tempNodeMapInt[stationId].Nom; }
                    if (int.TryParse(parties[2], out int precedentId) && precedentId > 0 && !tempNodeMapInt.ContainsKey(precedentId)) { tempNodeMapInt.Add(precedentId, new Noeud<int>(precedentId, $"Station {precedentId}")); }
                    if (int.TryParse(parties[3], out int suivantId) && suivantId > 0 && !tempNodeMapInt.ContainsKey(suivantId)) { tempNodeMapInt.Add(suivantId, new Noeud<int>(suivantId, $"Station {suivantId}")); }
                }
            }
            Console.WriteLine($" -> Trouvé {tempNodeMapInt.Count} noeuds potentiels dans le fichier Arc.");

            // --- Pass 2: Read Coordinates File (MetroParis(Noeuds).csv) ---
            if (coordFileExists)
            {
                Console.WriteLine($"Lecture du fichier Coordonnées: {Path.GetFileName(coordFilePath)}...");
                string[] coordLines = File.ReadAllLines(coordFilePath);
                int coordsLoaded = 0;
                foreach (string ligne in coordLines.Skip(1))
                {
                    string[] parties = ligne.Split(';');
                    if (parties.Length < 5) continue;
                    if (int.TryParse(parties[0], out int stationId) && stationId > 0)
                    {
                        // *** USE CultureInfo.InvariantCulture for reliable decimal parsing ***
                        if (double.TryParse(parties[3], NumberStyles.Any, CultureInfo.InvariantCulture, out double longitude) &&
                            double.TryParse(parties[4], NumberStyles.Any, CultureInfo.InvariantCulture, out double latitude))
                        {
                            if (tempNodeMapInt.TryGetValue(stationId, out Noeud<int> nodeToUpdate))
                            {
                                // *** Assign to Longitude/Latitude properties (CS1061 Fix) ***
                                nodeToUpdate.Longitude = longitude;
                                nodeToUpdate.Latitude = latitude;
                                // Update name from coord file if different
                                string coordStationName = parties[2]?.Trim();
                                if (!string.IsNullOrWhiteSpace(coordStationName) && nodeToUpdate.Nom != coordStationName) { nodeToUpdate.Nom = coordStationName; }
                                coordsLoaded++;
                            }
                        }
                    }
                }
                Console.WriteLine($" -> Chargé {coordsLoaded} coordonnées.");
            }

            // --- Finalize Node List and Map ---
            this._noeuds.Clear(); this.noeudMap.Clear();
            foreach (var node in tempNodeMapInt.Values.OrderBy(n => n.identite))
            {
                var nodeT = node as Noeud<T>; // Cast once
                if (nodeT != null) // Ensure cast worked (should if T is int)
                {
                    this._noeuds.Add(nodeT);
                    // Use nodeT.identite (which is type T) as the key
                    this.noeudMap.Add(nodeT.identite, nodeT);
                }
            }

            // --- Add edges (voisins) using finalized nodes ---
            Console.WriteLine("Ajout des connexions (voisins)...");
            foreach (string ligne in arcLines.Skip(1))
            {
                string[] parties = ligne.Split(';');
                if (parties.Length < 7 || !int.TryParse(parties[0], out int actuelId) || actuelId <= 0) continue;
                T actuelIdT = (T)(object)actuelId; // Cast ID to T for lookup
                if (!noeudMap.TryGetValue(actuelIdT, out Noeud<T> nodeActuel)) continue;

                if (int.TryParse(parties[3], out int successeurId) && successeurId > 0)
                {
                    T successeurIdT = (T)(object)successeurId; // Cast ID to T
                    if (noeudMap.TryGetValue(successeurIdT, out Noeud<T> nodeSuccesseur))
                    {
                        if (int.TryParse(parties[4], out int temps) && temps >= 0)
                        {
                            bool sensUnique = (int.TryParse(parties[6], out int suVal) && suVal == 1);
                            if (!nodeActuel.voisins.Any(v => v.voisin == nodeSuccesseur)) nodeActuel.voisins.Add((nodeSuccesseur, temps));
                            if (!sensUnique && !nodeSuccesseur.voisins.Any(v => v.voisin == nodeActuel)) nodeSuccesseur.voisins.Add((nodeActuel, temps));
                        }
                    }
                }
            }
            // --- Add transfer edges ---
            var nodesByNameFinal = _noeuds
                                    .Where(n => n != null && !string.IsNullOrWhiteSpace(n.Nom))
                                    .GroupBy(n => n.Nom)
                                    .Where(g => g.Count() > 1)
                                    .ToDictionary(g => g.Key, g => g.ToList());

            foreach (string ligne in arcLines.Skip(1))
            {
                string[] parties = ligne.Split(';');
                if (parties.Length < 7 || !int.TryParse(parties[0], out int actuelId) || actuelId <= 0) continue;
                T actuelIdT = (T)(object)actuelId; // Cast ID to T
                if (!noeudMap.TryGetValue(actuelIdT, out Noeud<T> nodeActuel) || string.IsNullOrWhiteSpace(nodeActuel.Nom)) continue;

                if (nodesByNameFinal.TryGetValue(nodeActuel.Nom, out List<Noeud<T>> transferGroup))
                {
                    if (int.TryParse(parties[5], out int tempsChangement) && tempsChangement > 0)
                    {
                        foreach (Noeud<T> otherNodeInGroup in transferGroup)
                        {
                            if (!EqualityComparer<T>.Default.Equals(otherNodeInGroup.identite, nodeActuel.identite)) // Compare using T
                            {
                                if (!nodeActuel.voisins.Any(v => v.voisin == otherNodeInGroup)) nodeActuel.voisins.Add((otherNodeInGroup, tempsChangement));
                                if (!otherNodeInGroup.voisins.Any(v => v.voisin == nodeActuel)) otherNodeInGroup.voisins.Add((nodeActuel, tempsChangement));
                            }
                        }
                    }
                }
            }

            // Resize matrix
            int actualNodeCount = this._noeuds.Count;
            Console.WriteLine($"Chargement terminé. Nombre final de noeuds : {actualNodeCount}");
            this.matrice = new int[actualNodeCount, actualNodeCount];
            this.Matrice = this.matrice;
        }

        // --- Other Public Methods ---
        public void ImplementerMatrice()
        {
            int n = _noeuds.Count;
            if (matrice.GetLength(0) != n || matrice.GetLength(1) != n) { matrice = new int[n, n]; Matrice = matrice; }
            else { Array.Clear(matrice, 0, matrice.Length); }

            Dictionary<Noeud<T>, int> nodeToIndex = new Dictionary<Noeud<T>, int>();
            for (int i = 0; i < n; i++) { if (_noeuds[i] != null) { nodeToIndex[_noeuds[i]] = i; } }

            for (int i = 0; i < n; i++)
            {
                Noeud<T> currentNode = _noeuds[i];
                if (currentNode == null || currentNode.voisins == null) continue;
                foreach ((Noeud<T> voisin, int t) in currentNode.voisins)
                {
                    if (voisin != null && nodeToIndex.TryGetValue(voisin, out int j)) { matrice[i, j] = t; }
                }
            }
            Console.WriteLine("Implémentation de la matrice d'adjacence terminée.");
        }

        public void AfficherMatrice()
        {
            Console.WriteLine("\nMatrice d'adjacence :");
            int count = _noeuds.Count;
            for (int i = 0; i < count; i++) { for (int j = 0; j < count; j++) { Console.Write($"{matrice[i, j],4} "); } Console.WriteLine(); }
        }

        public void AfficherListe()
        {
            Console.WriteLine("\nListe d'adjacence :");
            if (_noeuds == null || _noeuds.Count == 0) { Console.WriteLine("Graphe vide."); return; }
            foreach (Noeud<T> element in _noeuds)
            {
                if (element == null) continue;
                Console.Write($"[{element.identite}] {element.Nom} : [");
                if (element.voisins != null)
                {
                    string neighbors = string.Join(", ", element.voisins.Where(v => v.voisin != null).Select(v => $"{v.voisin.Nom}({v.voisin.identite}):{v.t}"));
                    Console.Write(neighbors);
                }
                Console.WriteLine("]");
            }
        }

        // --- Traversal and Analysis Methods ---
        public void ParcoursProfondeur(T departId)
        {
            if (!noeudMap.TryGetValue(departId, out Noeud<T> startNode)) { Console.WriteLine($"Parcours en profondeur: Noeud départ ID {departId} non trouvé."); return; }
            HashSet<Noeud<T>> visites = new HashSet<Noeud<T>>();
            Console.WriteLine("\nParcours en profondeur:"); DFSRecursif(startNode, visites); Console.WriteLine();
        }

        private void DFSRecursif(Noeud<T> courant, HashSet<Noeud<T>> visites)
        {
            if (courant == null || !visites.Add(courant)) return;
            Console.Write($"[{courant.identite}]{courant.Nom} -> ");
            if (courant.voisins != null) { foreach ((Noeud<T> voisin, _) in courant.voisins) { DFSRecursif(voisin, visites); } }
        }

        public void ParcoursLargeur(T departId)
        {
            if (!noeudMap.TryGetValue(departId, out Noeud<T> startNode)) { Console.WriteLine($"Parcours en largeur: Noeud départ ID {departId} non trouvé."); return; }
            HashSet<Noeud<T>> visites = new HashSet<Noeud<T>>(); Queue<Noeud<T>> file = new Queue<Noeud<T>>();
            file.Enqueue(startNode); visites.Add(startNode);
            Console.WriteLine("\nParcours en largeur:");
            while (file.Count > 0)
            {
                Noeud<T> noeudActuel = file.Dequeue(); Console.Write($"[{noeudActuel.identite}]{noeudActuel.Nom} -> ");
                if (noeudActuel.voisins != null) { foreach ((Noeud<T> voisin, _) in noeudActuel.voisins) { if (voisin != null && visites.Add(voisin)) { file.Enqueue(voisin); } } }
            }
            Console.WriteLine();
        }

        // FIX CS0161
        public bool GrapheConnexe()
        {
            if (_noeuds == null) return true; // Treat null list as connected/vacuously true
            if (_noeuds.Count <= 1) return true;
            HashSet<Noeud<T>> visites = new HashSet<Noeud<T>>();
            Noeud<T> startNode = _noeuds.FirstOrDefault(n => n != null);
            if (startNode == null) return true; // All nodes were null?
            DFSRecursif(startNode, visites);
            int nonNullNodeCount = _noeuds.Count(n => n != null);
            return visites.Count == nonNullNodeCount; // Return comparison result
        }

        // FIX CS0161
        public bool ContientCycle()
        {
            if (_noeuds == null || _noeuds.Count == 0) return false;
            HashSet<Noeud<T>> visited = new HashSet<Noeud<T>>(); HashSet<Noeud<T>> recursionStack = new HashSet<Noeud<T>>();
            foreach (Noeud<T> node in _noeuds)
            {
                if (node == null) continue;
                if (!visited.Contains(node)) { if (ContientCycleUtil(node, visited, recursionStack)) return true; } // Return true if found
            }
            return false; // Explicitly return false if loop completes
        }

        // FIX CS0161
        private bool ContientCycleUtil(Noeud<T> node, HashSet<Noeud<T>> visited, HashSet<Noeud<T>> recursionStack)
        {
            if (node == null) return false;
            if (!visited.Add(node)) { return recursionStack.Contains(node); } // If already visited, check stack
            recursionStack.Add(node); // Add to stack since it's newly visited on this path
            if (node.voisins != null)
            {
                foreach ((Noeud<T> voisin, _) in node.voisins)
                {
                    if (voisin == null) continue;
                    if (ContientCycleUtil(voisin, visited, recursionStack)) return true; // Return true if cycle found deeper
                }
            }
            recursionStack.Remove(node); // Remove from stack when backtracking
            return false; // No cycle found along this specific DFS path
        }

    } // End Graphe class
} // End namespace