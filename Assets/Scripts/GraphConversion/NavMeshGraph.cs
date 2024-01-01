using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor.Animations;
using UnityEngine;

public class NavMeshGraph
{
    Dictionary<int, NavMeshPolygon> nodes;
    Dictionary<int, PolygonEdge> edges;
    Dictionary<(int, int), int> connectivity;

    public NavMeshGraph()
    {
        this.nodes = new();
        this.edges = new();
        this.connectivity = new();
    }

    public bool[,] ToAdjMat()
    {

        bool[,] mat = new bool[nodes.Count, nodes.Count];
        foreach (var e in connectivity.Keys)
        {
            mat[e.Item1, e.Item2] = true;
        }

        return mat;
    }

    public void AddNode(NavMeshPolygon n)
    {
        nodes.Add(n.Id, n);
    }

    public void AddEdge(int from, int to, PolygonEdge e = null)
    {
        edges.Add(e.Id, e);
        connectivity.Add((from, to), e.Id);
    }

    public void Mesh2Graph(Mesh m)
    {
        nodes.Clear();
        connectivity.Clear();
        edges.Clear();

        int[] triss = m.triangles;
        Vector3[] verts = m.vertices;
        for (int i = 0; i < triss.Length - 3; i++)
        {
            int v1_i = triss[i + 0];
            int v2_i = triss[i + 1];
            int v3_i = triss[i + 2];

            NavMeshPolygon r = new(verts[v1_i], verts[v2_i], verts[v3_i]);
            nodes.Add(r.Id, r);

            foreach (var kvp in nodes)
            {
                if (r.IsAdjacent(kvp.Value))
                {
                    //bidirectional
                    AddEdge(r.Id, kvp.Key);
                    AddEdge(kvp.Key, r.Id);
                }

            }
        }
    }
}

public class NavMeshPolygon
{
    public NavMeshPolygon(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        V1 = v1;
        V2 = v2;
        V3 = v3;

        Id = GetID();
    }

    public int GetID()
    {
        Id = 0;
        Id ^= V1.GetHashCode();
        Id ^= V2.GetHashCode();
        Id ^= V3.GetHashCode();

        return Id;
    }

    public bool IsAdjacent(NavMeshPolygon other)
    {
        return V1 == other.V1 ||
            V1 == other.V2 ||
            V1 == other.V3 ||
            V1 == other.V1 ||
            V2 == other.V1 ||
            V2 == other.V2 ||
            V2 == other.V3 ||
            V3 == other.V1 ||
            V3 == other.V2 ||
            V3 == other.V3;
    }

    public Vector3 V1 { get; set; }
    public Vector3 V2 { get; set; }
    public Vector3 V3 { get; set; }
    public int Id { get; set; }
}

public class PolygonEdge
{
    public float Weight { get; set; }
    public int Id { get; set; }

    public PolygonEdge(float weight)
    {
        this.Weight = weight;

        Id = GetID();
    }

    public int GetID()
    {
        Id = 0;
        Id ^= Weight.GetHashCode();

        return Id;
    }
}