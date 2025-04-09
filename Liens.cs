// Fichier : Liens.cs (ou Liens (1).cs)

using System;
// Ajoutez d'autres using si nécessaire (System.Collections.Generic, etc.)

// Assurez-vous que le namespace correspond aux autres fichiers
namespace Projet_PSI
{
    // AJOUTER "public" ET LA CONTRAINTE "where T : IEquatable<T>"
    public class Lien<T> where T : IEquatable<T>
    {
        // Les champs peuvent rester publics ou devenir privés avec des propriétés
        public Noeud<T> noeud1;
        public Noeud<T> noeud2;

        // Constructeur
        public Lien(Noeud<T> noeud1, Noeud<T> noeud2)
        {
            // Valider que les nœuds ne sont pas null si nécessaire
            this.noeud1 = noeud1 ?? throw new ArgumentNullException(nameof(noeud1));
            this.noeud2 = noeud2 ?? throw new ArgumentNullException(nameof(noeud2));
        }

        // Propriétés publiques (bonnes pratiques)
        public Noeud<T> Noeud1
        {
            get { return noeud1; }
            // Optionnel: set { noeud1 = value; }
        }
        public Noeud<T> Noeud2
        {
            get { return noeud2; }
            // Optionnel: set { noeud2 = value; }
        }

        // Optionnel: Surcharger Equals, GetHashCode, ToString si nécessaire pour les liens
        public override string ToString()
        {
            return $"Lien: ({noeud1?.ToString() ?? "null"}) <-> ({noeud2?.ToString() ?? "null"})";
        }
    }
}