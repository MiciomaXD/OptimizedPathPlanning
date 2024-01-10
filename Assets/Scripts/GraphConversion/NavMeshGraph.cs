using System;
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
        nodes = new();
        edges = new();
        connectivity = new();
    }

    public NavMeshGraph(Mesh m)
    {
        nodes = new();
        connectivity = new();
        edges = new();

        int[] triss = m.triangles;
        Vector3[] verts = m.vertices;
        for (int i = 0; i < triss.Length; i += 3)
        {
            int v1_ix = triss[i];
            int v2_ix = triss[i + 1];
            int v3_ix = triss[i + 2];

            NavMeshPolygon r = new(verts[v1_ix], verts[v2_ix], verts[v3_ix]);

            foreach (var kvp in nodes)
            {
                if (r.IsAdjacent(kvp.Value))
                {
                    //bidirectional
                    PolygonEdge e = new(r.Id, kvp.Key, directional: false);
                    AddEdge(r.Id, kvp.Key, e);

                }
            }

            AddNode(r);
        }
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

    public void AddEdge(int from, int to, PolygonEdge e)
    {
        edges.Add(e.Id, e);

        connectivity.Add((from, to), e.Id);
        if (!e.Directional)
        {
            connectivity.Add((to, from), e.Id);
        }
    }

    /// <summary>
    /// Return ids of all nodes reachable from current (in case of directional graphs only children are returned) 
    /// </summary>
    /// <param name="current"></param>
    /// <returns></returns>
    public List<int> GetNeighbours(int current)
    {
        return connectivity.Where(x => x.Key.Item1 == current)
            .Select(x => x.Key.Item2).ToList();
    }

    public PolygonEdge GetEdge(int from, int to)
    {
        int edgeId = connectivity[(from, to)];
        return edges[edgeId];
    }

    public NavMeshPolygon GetNode(int nodeId)
    {
        return nodes[nodeId];
    }

    public void VisualizeGraphInScene(Material edgeMat)
    {
        GameObject gRoot = new GameObject("GraphVisRoot");

        var barycenters = nodes.Values
            .Select(x => new { name = x.Id, barycenter = x.GetBarycenter() });

        Vector3 centerOfGraph = barycenters
            .Aggregate(Vector3.zero, (acc, next) => acc += next.barycenter, summation => summation / nodes.Values.Count);
        gRoot.transform.position = centerOfGraph;

        GameObject[] nodesVisGOs = barycenters
            .Select(x =>
                {
                    GameObject go = new GameObject(x.name.ToString());
                    go.transform.position = x.barycenter;
                    go.transform.parent = gRoot.transform;
                    return go;
                }
            ).ToArray();

        //draw edges
        foreach (var kvp in connectivity)
        {
            (int, int) couple = kvp.Key;

            Transform parent = nodesVisGOs.Where(x => x.name == couple.Item1.ToString())
                .Select(x => x.transform).First();

            Transform otherGO = nodesVisGOs.Where(x => x.name == couple.Item2.ToString())
                .Select(x => x.transform).First();

            GameObject childLR = new GameObject(parent.name + "_" + otherGO.name);
            childLR.transform.parent = parent;

            LineRenderer lr = childLR.AddComponent<LineRenderer>();
            lr.material = edgeMat;
            lr.widthCurve = AnimationCurve.Constant(0, 1, 0.01f);
            lr.positionCount = 2;
            lr.SetPositions(new Vector3[2] { parent.transform.position + new Vector3(0f, 0.5f, 0f),
                otherGO.transform.position + new Vector3(0f, 0.5f, 0f) });

        }

    }
}

public class NavMeshPolygon
{
    public NavMeshPolygon(Vector3 v1, Vector3 v2, Vector3 v3, float cost = 1f)
    {
        V1 = v1;
        V2 = v2;
        V3 = v3;
        Cost = cost;

        Id = GetID();
    }

    public int GetID()
    {
        return HashCode.Combine(V1, V2, V3, Cost);
    }

    public Vector3 GetBarycenter()
    {
        return (V1 + V2 + V3) / 3f;
    }

    public bool IsAdjacent(NavMeshPolygon other)
    {
        /*
        if (this.Equals(other))
            return false;*/

        Vector3[] t1 = new Vector3[3] { V1, V2, V3 };
        Vector3[] t2 = new Vector3[3] { other.V1, other.V2, other.V3 };
        /*return (from v1 in t1
                from v2 in t2
                where v1 == v2
                select v1).Count() == 2;*/
        //in maniera lambda - migliore - viene
        return t1.SelectMany(v1 => t2, (v1,v2) => (v1,v2))
            .Where(x => x.v1 == x.v2).Count()==2; //posso evitare anche il check di uguaglianza a
                                                  //se stesso perchè ==2 (una regione controntata con
                                                  //se stessa ha tutti e tre i vertici uguali e non
                                                  //deve essere adiacente a se stessa)

    }

    public override bool Equals(object obj)
    {
        return obj is NavMeshPolygon polygon &&
               V1.Equals(polygon.V1) &&
               V2.Equals(polygon.V2) &&
               V3.Equals(polygon.V3) &&
               Id == polygon.Id &&
               Cost == polygon.Cost;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(V1, V2, V3, Id, Cost);
    }

    public Vector3 V1 { get; set; }
    public Vector3 V2 { get; set; }
    public Vector3 V3 { get; set; }
    public int Id { get; set; }
    public float Cost { get; set; }
}

public class PolygonEdge
{
    public float Weight { get; set; }
    //(int, int) DirectionalId { get; set; }
    public int Id { get; set; }
    public bool Directional { get; set; }

    public PolygonEdge(int from, int to, float weight = 1f, bool directional = false)
    {
        Weight = weight;
        Directional = directional;
        Id = Directional ? ComputeDirectionalId(from, to) : ComputeId(from, to);
    }

    int ComputeId(int from, int to)
    {
        return HashCode.Combine(Weight, from, to, Directional);
    }

    int ComputeDirectionalId(int from, int to)
    {

        return HashCode.Combine(Weight, from.ToString() + to.ToString(), Directional);
    }

    public override bool Equals(object obj)
    {
        return obj is PolygonEdge edge &&
               Weight == edge.Weight &&
               Id == edge.Id &&
               Directional == edge.Directional;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Weight, Id, Directional);
    }
}