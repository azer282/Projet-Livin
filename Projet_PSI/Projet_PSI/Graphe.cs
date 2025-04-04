using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Projet_PSI
{
    internal class Graphe<T>
    {
        public List<Noeud<int>> noeuds;
        public int[,] matrice;

        public Graphe(int taille)
        {
            this.noeuds = new List<Noeud<int>>();
            this.matrice = new int[taille, taille];
        }

        public List<Noeud<int>> Noeuds
        {
            get {return noeuds;}
        }

        public int[,] Matrice
        {
            get { return matrice; }
            set { matrice = value; }
        }

        public void AjouterNoeud(Noeud<int> noeud)
        {
            this.noeuds.Add(noeud);
        }

        public void ImplementerMatrice()
        {
            for (int i= 0; i <noeuds.Count; i++)
            {
                foreach ((Noeud<int> voisin, int t) in noeuds[i].voisins)
                {
                    int j= noeuds.IndexOf(voisin);
                    if (j!= -1)
                    {
                        matrice[i,j]= t;
                    }
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
            foreach (Noeud<int> element in noeuds)
            {
                Console.Write(element.identite + " : [");
                foreach ((Noeud<int> voisin,int t) in element.voisins)
                {
                    Console.Write(voisin.identite + " ");
                }
                Console.WriteLine("]");
            }
        }

        public void ChargerListeDeNoeuds(string cheminfichier)
        {
            string[] Tablignes =File.ReadAllLines(cheminfichier);
            int nNoeuds =Tablignes.Length-1;
            
            List<Noeud<int>> liste = new List<Noeud<int>>();

            for (int i = 0; i < nNoeuds; i++)
            {
                liste.Add(new Noeud<int>(i));
            }

            foreach (string ligne in Tablignes.Skip(1))
            {
                
                string[] parties =ligne.Split(';');
                if (Convert.ToInt32(parties[3]) != 0)
                {
                    int t2 = int.Parse(parties[4]);
                    int partie1 = int.Parse(parties[0]) - 1; // actuel
                    int partie2 = int.Parse(parties[3]) - 1; // successeur

                    int sens_unique = int.Parse(parties[6]);                   //cas sens unique
                    if( sens_unique != 1 )
                    {
                        liste[partie1].voisins.Add((liste[partie2], t2));
                        liste[partie2].voisins.Add((liste[partie1], t2));
                    }
                    else
                    {
                        liste[partie1].voisins.Add((liste[partie2], t2));
                    }
                }
                
            }
            
            string[,] MatNomId = new string[nNoeuds, 2];
            string[] partie;
            int compteur = 0; 
            foreach (string ligne in Tablignes.Skip(1))                          
            {
                partie = ligne.Split(';');
                MatNomId[compteur, 0] = partie[0];
                MatNomId[compteur,1] = partie[1];
                compteur++;     
            }
            
            for(int i = 0; i < MatNomId.GetLength(0); i++)
            {
                string NomActu = MatNomId[i, 1]; 
                for (int j = 0;  j < MatNomId.GetLength(0);j++)
                {
                    if(NomActu == MatNomId[j,1]&& i !=j )
                    {
                        string[] partieTab = Tablignes[i+1].Split(';');
                        int PoidsChangement = Convert.ToInt32(partieTab[5]); 
                        liste[i].voisins.Add((liste[Convert.ToInt32(MatNomId[j, 0])-1],PoidsChangement));           
                    }
                }
            } 

            noeuds = liste;

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
            Console.Write(noeuds[index].identite + " ");

            foreach ((Noeud<int> voisin, int t) in noeuds[index].voisins)
            {
                int voisinIndex = noeuds.IndexOf(voisin);
                if ( voisinIndex !=-1)
                {
                    DFSRecursif(voisinIndex, visites);
                }
                
            }
        }

        public void ParcoursLargeur(int depart)
        {
            if (depart - 1< 0 || depart - 1>= noeuds.Count)
            {
                Console.WriteLine("départ non vazlide");
                return;
            }
            List<int> visites = new List<int>();
            Queue<int> file = new Queue<int>();

            file.Enqueue(depart- 1);
            visites.Add(depart-1);

            Console.WriteLine("Parcours en largeur:");
            while (file.Count> 0)
            {
                int noeudActuel = file.Dequeue();
                Console.Write(noeuds[noeudActuel].identite + " ");

                foreach ((Noeud<int> voisin, int t) in noeuds[noeudActuel].voisins)
                {
                    int voisinIndex = noeuds.IndexOf(voisin);
                    if (voisinIndex != -1 && !visites.Contains(voisinIndex))
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

            foreach ((Noeud<int> voisin, int t) in noeuds[index].voisins)
            {
                int voisinIndex = noeuds.IndexOf(voisin);

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


        /*string[] parties = ligne.Split(';');
                string[] partiesBis = ligne.Split(';');
                foreach (string ligneBis in Tablignes.Skip(1))
                {
                    if (parties[2] == partiesBis[2])
                    {
                        liste[]
                    }
                }
        */
    }


}

