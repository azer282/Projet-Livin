
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Projet_PSI
{
    public class Noeud<T> where T : IEquatable<T>
    {
        public T identite;
        public string Nom { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public List<(Noeud<T> voisin, int t)> voisins;

        public Noeud(T identite, string nom = null)
        {
            this.identite = identite;
            this.Nom = string.IsNullOrWhiteSpace(nom) ? identite?.ToString() : nom.Trim();
            if (string.IsNullOrWhiteSpace(this.Nom))
            {
                this.Nom = "Unnamed Node";
            }
            this.Longitude = double.NaN;
            this.Latitude = double.NaN;
            this.voisins = new List<(Noeud<T>, int t)>();
        }

        public bool HasValidCoordinates => !double.IsNaN(this.Longitude) && !double.IsNaN(this.Latitude);

        public override bool Equals(object obj)
        {
            return obj is Noeud<T> otherNode &&
                   EqualityComparer<T>.Default.Equals(this.identite, otherNode.identite);
        }

        public override int GetHashCode()
        {
            return EqualityComparer<T>.Default.GetHashCode(this.identite);
        }

        public static bool operator ==(Noeud<T> left, Noeud<T> right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }
            return left.Equals(right);
        }

        public static bool operator !=(Noeud<T> left, Noeud<T> right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            string coordStr = HasValidCoordinates ? $" ({this.Longitude:F6}, {this.Latitude:F6})" : " (No Coords)";
            return $"[{this.identite}] {this.Nom}{coordStr}";
        }
    }
}