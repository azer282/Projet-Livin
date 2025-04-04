using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Projet_PSI
{
    internal class Noeud<T>
    {
        public T identite;
        public List<(Noeud<T>,int t)> voisins;
        public Noeud(T identite)
        {
            this.identite = identite;
            this.voisins = new List<(Noeud<T>,int t)>();
        }
        public T Identite
        {
            get; 
        }
        public List<Noeud<T>> Voisins
        {
            get; 
        }
    }
}

