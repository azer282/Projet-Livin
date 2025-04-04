using System;
using System.IO;

namespace Projet_PSI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string fichier = "../../MetroParisArc.csv";
            fichier = Path.GetFullPath(fichier); 
            Graphe<int> graphe = ImporterGraphe(fichier);

            if (graphe == null)
            {
                Console.WriteLine("erreur de chargement du graphe");
                return;
            }
            
            graphe.AfficherListe();                                    // affiche la liste d'adjacence
            graphe.ImplementerMatrice();                              //fais la matrice
            //graphe.AfficherMatrice();                                  //affiche la matrice d'adjacence
            Console.WriteLine("choisir pt de départ");
            int ptDepart = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("choisir pt d'arrivée");
            int ptArrivee = Convert.ToInt32(Console.ReadLine());



            Algo_de_recherche Recherche = new Algo_de_recherche(graphe.matrice, ptDepart, ptArrivee, graphe.matrice.GetLength(0));
            Console.WriteLine("Algo de recherche Dijkstra : "); 
            Recherche.Dijkstra();
            Console.WriteLine("Algo de recherche BellmanFord : "); 
            Recherche.BellemanFord();
            Console.WriteLine("Algo de recherche FloydWarshall : ");
            Recherche.FloydWarshall(); 
            /*
            int sommetDepart = 1;
            graphe.ParcoursProfondeur(sommetDepart);
            graphe.ParcoursLargeur(sommetDepart);

            
            if (graphe.GrapheConnexe())                      //test si graphe connexe
            {
                Console.WriteLine("\nLe graphe est connexe");
            }
            else
            {
                Console.WriteLine("\nLe graphe n'est pas connexe");
            }

            if (graphe.ContientCycle())                     //test si il y a un cycle 
            {
                Console.WriteLine("\nLe graphe contient un cyle");
            }
            else
            {
                Console.WriteLine("\nLe graphe n'a pas de cycle");
            }
            Console.ReadLine(); 
            */
        }


        static Graphe<int> ImporterGraphe(string chemin)
        {
            if (!File.Exists(chemin))                                        //virifie si il peut acceder au fichier
            {
                Console.WriteLine($"ne trouve pas le fichier {chemin}");
                return null;
            }

            string[] lignes = File.ReadAllLines(chemin);
            if (lignes.Length == 0)                                   //regarde toutes les lignes et test si elles sont vides
            {
                Console.WriteLine("fichir est vide");
                return null;
            }

            int nbnoeuds;
            
            if (!int.TryParse(lignes[1].Split(';')[0], out nbnoeuds) || nbnoeuds <= 0)
            {
                Console.WriteLine("nb de noeud pas valide");
                return null;
            }
            nbnoeuds = lignes.Length-1;
            Graphe<int> graphe = new Graphe<int>(nbnoeuds);
            graphe.ChargerListeDeNoeuds(chemin);
            return graphe;
        }

        




    }

}


