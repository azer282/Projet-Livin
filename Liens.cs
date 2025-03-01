using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet_PSI
{
    internal class Lien
    {
        public Noeud noeud1;
        public Noeud noeud2; 

        public Lien(Noeud noeud1 , Noeud noeud2)
        {
            this.noeud1 = noeud1;
            this.noeud2 = noeud2; 
        }

        public Noeud Noeud1
        {
            get { return noeud1; }
            set { noeud1 = value; }
        }
        public Noeud Noeud2
        {
            get { return noeud2; }
            set { noeud2 = value; }
        }
    }
}
