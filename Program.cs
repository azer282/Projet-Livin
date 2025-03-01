using System;
using System.IO;

namespace Projet_PSI
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string fichier = @"C:\Users\Marin.Monjalous\Documents\LeSoc-karate.mtx";
            Graphe graphe = ImporterGraphe(fichier);

            if (graphe == null)
            {
                Console.WriteLine("erreur de chargement du graphe");
                return;
            }

            graphe.AfficherListe();                                    // affiche la liste d'adjacence
            graphe.ImplementerMatrice();                              //fais la matrice
            graphe.AfficherMatrice();                                  //affiche la matrice d'adjacence

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

        }


        static Graphe ImporterGraphe(string chemin)
        {
            if (!File.Exists(chemin))                                        //virifie si il peut acceder au fichier
            {
                Console.WriteLine($"ne trouve pas le fichier {chemin}");
                return null;
            }

            string[] lignes = File.ReadAllLines(chemin);
            if (lignes.Length==0)                                   //regarde toutes les lignes et test si elles sont vides
            {
                Console.WriteLine("fichir est vide");
                return null;
            }

            int nbnoeuds;
            if (!int.TryParse(lignes[0].Split()[0], out nbnoeuds) || nbnoeuds <= 0)
            {
                Console.WriteLine("nb de noeud pas valide");
                return null;
            }

            Graphe graphe = new Graphe(nbnoeuds);
            graphe.ChargerListeDeNoeuds(chemin);
            return graphe;
        }




        

    }
}


