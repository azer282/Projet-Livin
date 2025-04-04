using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet_PSI
{
    internal class Lien<T>
    {
        public Noeud<T> noeud1;
        public Noeud<T> noeud2; 

        public Lien(Noeud<T> noeud1 , Noeud<T> noeud2)
        {
            this.noeud1 = noeud1;
            this.noeud2 = noeud2; 
        }

        public Noeud<T> Noeud1
        {
            get { return noeud1; }
            set { noeud1 = value; }
        }
        public Noeud<T> Noeud2
        {
            get { return noeud2; }
            set { noeud2 = value; }
        }
    }
}
