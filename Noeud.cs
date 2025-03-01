using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Projet_PSI
{
    internal class Noeud
    {
        public int identite;
        public List<Noeud> voisins;
        public Noeud(int identite)
        {
            this.identite = identite;
            this.voisins = new List<Noeud>();
        }
        public int Identite
        {
            get { return identite; }
        }
        public List<Noeud> Voisins
        {
            get { return voisins; }
        }
    }
}

