//VISUALISATION DU GRAPH
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

public class Noeud
{
    public int Id { get; set; }
    public PointF Position { get; set; }

    public Noeud(int id, float x, float y)
    {
        Id = id;
        Position = new PointF(x, y);
    }
}

public class Lien
{
    public Noeud Start { get; set; }
    public Noeud End { get; set; }

    public Lien(Noeud start, Noeud end)
    {
        Start = start;
        End = end;
    }
}

public class Graphe
{
    public Dictionary<int, Noeud> Noeuds { get; set; }
    public List<Lien> Liens { get; set; }

    public Graphe()
    {
        Noeuds = new Dictionary<int, Noeud>();
        Liens = new List<Lien>();
    }

    public void LoadFromMtxFile(string filePath)
    {
        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith("%")) continue; // Ignore comments
                var parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                int startId = int.Parse(parts[0]);
                int endId = int.Parse(parts[1]);

                if (!Noeuds.ContainsKey(startId))
                    Noeuds[startId] = new Noeud(startId, 0, 0);
                if (!Noeuds.ContainsKey(endId))
                    Noeuds[endId] = new Noeud(endId, 0, 0);

                Liens.Add(new Lien(Noeuds[startId], Noeuds[endId]));
            }
        }
    }

    public void SetNodePositions(int width, int height)
    {
        int count = Noeuds.Count;
        int centerx = width / 2;
        int centery = height / 2;
        int radius = Math.Min(width, height) / 2 - 50; // Margin from edges
        int i = 0;
        foreach (var node in Noeuds.Values)
        {
            double angle = 2 * Math.PI * i / count;
            node.Position = new PointF(
                centerx + (float)(radius * Math.Cos(angle)),
                centery + (float)(radius * Math.Sin(angle))
            );
            i++;
        }
    }
}

public class GraphVisualizer : Form
{
    private Graphe graphe;

    public GraphVisualizer(Graphe graphe)
    {
        this.graphe = graphe;
        this.Width = 800;
        this.Height = 600;
        this.Text = "Visualisation de Graphe";
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Graphics g = e.Graphics;

        foreach (var lien in graphe.Liens)
        {
            g.DrawLine(Pens.Black, lien.Start.Position, lien.End.Position);
        }

        foreach (var noeud in graphe.Noeuds.Values)
        {
            g.FillEllipse(Brushes.Blue, noeud.Position.X - 5, noeud.Position.Y - 5, 10, 10);
            g.DrawString(noeud.Id.ToString(), this.Font, Brushes.Black, noeud.Position);
        }
    }
}

public static class Program
{
    [STAThread]
    public static void Main()
    {
        Graphe graphe = new Graphe();
        graphe.LoadFromMtxFile("C:\\Users\\augus\\Desktop\\Augustin\\ESILV A2\\S2\\données\\soc-karate.mtx");
        graphe.SetNodePositions(800, 600);

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new GraphVisualizer(graphe));
    }
}
