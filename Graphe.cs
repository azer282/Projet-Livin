using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Projet_PSI
{
    internal class Graphe
    {
        public List<Noeud> noeuds;
        public int[,] matrice;

        public Graphe(int taille)
        {
            this.noeuds = new List<Noeud>();
            this.matrice = new int[taille, taille];
        }

        public List<Noeud> Noeuds
        {
            get { return noeuds; }
        }

        public int[,] Matrice
        {
            get { return matrice; }
            set { matrice = value; }
        }

        public void AjouterNoeud(Noeud noeud)
        {
            this.noeuds.Add(noeud);
        }

        public void ImplementerMatrice()
        {
            foreach (Noeud element in this.noeuds)
            {
                int i = element.identite - 1;
                foreach (Noeud voisin in element.voisins)
                {
                    int j = voisin.identite - 1;
                    matrice[i, j] = 1;
                }
            }
        }

        public void AfficherMatrice()
        {
            Console.WriteLine("Matrice d'adjacence :");
            for (int i = 0; i < noeuds.Count; i++)
            {
                for (int j = 0; j < noeuds.Count; j++)
                {
                    Console.Write(matrice[i, j] + " ");
                }
                Console.WriteLine();
            }
        }

        public void AfficherListe()
        {
            Console.WriteLine("Liste d'adjacence :");
            foreach (Noeud element in noeuds)
            {
                Console.Write(element.identite + " : [");
                foreach (Noeud voisin in element.voisins)
                {
                    Console.Write(voisin.identite + " ");
                }
                Console.WriteLine("]");
            }
        }

        public void ChargerListeDeNoeuds(string cheminfichier)
        {
            string[] Tablignes =File.ReadAllLines(cheminfichier);
            int nNoeuds =int.Parse(Tablignes[0].Split()[0]);


            for (int i = 0; i < nNoeuds; i++)
            {
                noeuds.Add(new Noeud(i + 1));
            }
            foreach (string ligne in Tablignes.Skip(1))
            {
                string[] parties =ligne.Split();
                int partie1= int.Parse(parties[0]) -1;
                int partie2=int.Parse(parties[1])-1;

                noeuds[partie1].voisins.Add(noeuds[partie2]);
                noeuds[partie2].voisins.Add(noeuds[partie1]); 
            }


        }
        public void ParcoursProfondeur(int depart)
        {
            if (depart- 1 < 0 || depart-1 >= noeuds.Count)
            {
                Console.WriteLine("Départ pas valide");
                return;
            }
            List<int> visites = new List<int>();
            Console.WriteLine("parcours en profondeur:");
            DFSRecursif(depart - 1, visites);
            Console.WriteLine();
        }

        public void DFSRecursif(int index, List<int> visites)
        {
            if (visites.Contains(index)) return;

            visites.Add(index);
            Console.Write(index + 1 + " ");

            foreach (Noeud voisin in noeuds[index].voisins)
            {
                DFSRecursif(voisin.identite - 1,visites);
            }
        }

        public void ParcoursLargeur(int depart)
        {
            if (depart - 1 < 0 || depart - 1 >= noeuds.Count)
            {
                Console.WriteLine("départ non vazlide");
                return;
            }
            List<int> visites = new List<int>();
            Queue<int> file = new Queue<int>();

            file.Enqueue(depart - 1);
            visites.Add(depart - 1);

            Console.WriteLine("Parcours en largeur:");
            while (file.Count > 0)
            {
                int noeudActuel = file.Dequeue();
                Console.Write((noeudActuel + 1) + " ");

                foreach (Noeud voisin in noeuds[noeudActuel].voisins)
                {
                    int voisinIndex = voisin.identite - 1;
                    if (!visites.Contains(voisinIndex))
                    {
                        file.Enqueue(voisinIndex);
                        visites.Add(voisinIndex);
                    }
                }
            }
            Console.WriteLine();
        }

        public bool GrapheConnexe()
        {
            if (noeuds.Count == 0)
            {
                return false; 
            }

            List<int> visites = new List<int>();
            DFSRecursif(0, visites);
            if ( visites.Count == noeuds.Count)
            {
                return true; 
            }
            return false; 
        }

        public bool ContientCycle()
        {
            bool[] visites = new bool[noeuds.Count];
            for (int i = 0; i< noeuds.Count; i++)
            {
                if (!visites[i])
                {
                    if (ParcoursProfondeurRechercheCycle(i,-1,visites))
                    {
                        return true;
                    }
                        
                }
            }
            return false;
        }

        private bool ParcoursProfondeurRechercheCycle(int index,int parent,bool[]visites)
        {
            visites[index]= true;

            foreach (Noeud voisin in noeuds[index].voisins)
            {
                int voisinIndex = voisin.identite-1;

                if (!visites[voisinIndex])
                {
                    if (ParcoursProfondeurRechercheCycle(voisinIndex, index,visites))
                        return true;
                }
                else if (voisinIndex!=parent)
                {
                    return true;
                }
            }
            return false;
        }

    }


}

