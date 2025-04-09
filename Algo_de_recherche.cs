using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Projet_PSI
{
    internal class Algo_de_recherche
    {
        private int[,] matrice;
        private int depart;
        private int arrivee;
        private int tailleMat; 

        public Algo_de_recherche(int[,] matrice, int depart, int arrivee, int tailleMat)
        {
            this.matrice = matrice;
            this.depart = depart;
            this.arrivee = arrivee-1;
            this.tailleMat = tailleMat; 
        }

        public void Dijkstra()
        {

            bool[] visite = new bool[this.matrice.GetLength(0)];
            int[] dist = new int[this.matrice.GetLength(0)];
            int[] pred = new int[this.matrice.GetLength(0)];

            

            for ( int i = 0; i < tailleMat; i++)
            {
                pred[i] = -1;
                dist[i] = 999999999; 
            }
            dist[depart] = 0;

            for (int i = 0; i < tailleMat - 1; i++)                                              // tailleMat - 1 car Dijkstra se déroule en n-1 étapes
            {
                int u = PPDistance(visite, dist);
                visite[u] = true; 

                for ( int j = 0; j < tailleMat; j++)
                {
                    if (!visite[j] && matrice[u,j]>0 && dist[u] != 999999999 && dist[u] + matrice[u,j] < dist[j])
                    {
                        dist[j] = dist[u] + matrice[u, j];
                        pred[j] = u;  
                    }
                }
            
            }
            ResultatRecherche(dist);
            AfficherChemin(pred);
        }


        public int PPDistance(bool[] visite, int[] dist)                          // méthode pour avoir le point le plus proche 
        {
            int distMin = 999999999;
            int index = -1;

            for (int i = 0; i < tailleMat - 1; i++)
            {
                if (!visite[i] && dist[i] <= distMin)
                {
                    distMin = dist[i];
                    index = i; 
                }
            }
            return index; 
        }

        public void BellemanFord()
        {
            int[] dist = new int[this.matrice.GetLength(0)];
            int[] pred = new int[this.matrice.GetLength(0)];


            for (int i = 0; i < tailleMat; i++)
            {
                pred[i] = -1;
                dist[i] = 999999999;
            }
            dist[depart] = 0;

            for ( int i = 0; i < tailleMat - 1; i++)     //comme Dijkstra, tailleMat-1 car on a besoin de n-1 étapes
            {
                for ( int j = 0; j < tailleMat; j++)
                {
                    for (int k = 0; k < tailleMat;k++)
                    {
                        if (matrice[j,k] != 0 && dist[j] != 999999999 && dist[j] + matrice[j,k] < dist[k])
                        {
                            dist[k] = dist[j] + matrice[j, k]; 
                            pred[k] = j; 
                        }
                    }
                }
            }
            for ( int i = 0; i< tailleMat; i++)                          // detecte si il y a un cycle de point négatif
            {
                for( int j =0; j < tailleMat; j++)
                {
                    if(matrice[i,j] != 0 && dist[i] != 999999999 && dist[i] + matrice[i,j] < dist[j])
                    {
                        return; 
                    }
                }
            }
            ResultatRecherche(dist);
            AfficherChemin(pred);

        }


        public void FloydWarshall()
        {
            int n = tailleMat;
            int[,] dist = new int[n, n];
            int[,] pred = new int[n, n];

            
            for (int i = 0; i < n; i++)                         // initialisation dist et pred
            {
                for (int j = 0; j < n; j++)
                {
                    if (i == j)
                    {
                        dist[i, j] = 0;
                        pred[i, j] = -1;
                    }
                    else if (matrice[i, j] != 0)
                    {
                        dist[i, j] = matrice[i, j];
                        pred[i, j] = i;
                    }
                    else
                    {
                        dist[i, j] = int.MaxValue / 2;             // pas de depassement
                        pred[i, j] = -1;
                    }
                }
            }

            
            for (int k = 0; k < n; k++)                                //floyd warshall
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (dist[i, k] + dist[k, j] < dist[i, j])
                        {
                            dist[i, j] = dist[i, k] + dist[k, j];
                            pred[i, j] = pred[k, j];
                        }
                    }
                }
            }

            
            int[] distFinale = new int[n];
            int[] predFinal = new int[n];
            for (int i = 0; i < n; i++)
            {
                distFinale[i] = 999999999;
                predFinal[i] = -1;
            }
            for (int i = 0; i < n; i++)
            {
                distFinale[i] = dist[depart, i];
                predFinal[i] = pred[depart, i];
            }

            ResultatRecherche(distFinale);
            AfficherChemin(predFinal);
        }




        public void ResultatRecherche(int[] dist)
        {
            Console.WriteLine("Le plus petit chemin entre le point de départ : " + depart + " et le point d'arrivé : " + (arrivee+1) + " est de : " + dist[arrivee]);
            
        }

        public void AfficherChemin(int[] pred)
        {
            Stack<int> chemin = new Stack<int>();
            int actuel = arrivee+1;

            while (actuel!= -1)
            {
                chemin.Push(actuel);
                actuel = pred[actuel];
            }

            Console.WriteLine("Chemin le plus court : " + string.Join(" | ", chemin) );
        }

        

    }
}
