using System;
using System.Collections.Generic;
using UnityEngine;

public class AStar
{
    const float SQRT2 = 1.41421356f; //molto più lesto lesto

    //TODO check correctness wiht graph implementation, compiles correctly
    public List<int> FindShortestPath(NavMeshGraph g, int start, int end, Func<NavMeshGraph, int, int, float> heuristic, bool returnFullPath = false)
    {
        MinPriorityQueue<int> boundaryNodes = new (); //open set
        boundaryNodes.Equeue(start, 0.0f);

        var distances = new Dictionary<int, float> { { start, 0.0f } };

        var cameFrom = new Dictionary<int, int> { { start, -1 } };

        while (boundaryNodes.IsEmpty())
        {
            (int currentNode, float currentCost) = boundaryNodes.Pop();

            if (currentNode == end)
            {
                if (returnFullPath)
                {
                    List<int> path = new List<int> { end };
                    while (cameFrom[end] != -1)
                    {
                        end = cameFrom[end];
                        path.Insert(0, end);
                    }
                    return path;
                }
                else
                {
                    return new List<int> { end };
                }
            }

            var neighbours = g.GetNeighbours(currentNode);
            foreach (var neighbor in neighbours)
            {
                var proposedDistance = distances[currentNode] + g.GetEdge(currentNode, neighbor).Weight;

                if (!distances.ContainsKey(neighbor) || distances[neighbor] > proposedDistance)
                {
                    distances[neighbor] = proposedDistance;
                    var priority = proposedDistance + heuristic(g, neighbor, end);
                    boundaryNodes.Equeue(neighbor, priority);
                    cameFrom[neighbor] = currentNode;
                }
            }
        }

        return new List<int>();
    }

    // Define heuristic functions
    //TODO vale la pena renderle 3d secondo me, ma vediamo
    public float ManhattanDistance(NavMeshGraph graph, int node, int target)
    {
        Vector3 n1 = graph.GetNode(node).GetBarycenter();
        Vector3 n2 = graph.GetNode(target).GetBarycenter();

        return Mathf.Abs(n1.x - n2.x) + Mathf.Abs(n1.z - n2.z);
    }

    public float EuclideanDistance(NavMeshGraph graph, int node, int target)
    {
        Vector3 n1 = graph.GetNode(node).GetBarycenter();
        Vector3 n2 = graph.GetNode(target).GetBarycenter();

        return Mathf.Sqrt(Mathf.Pow(n1.x - n2.x, 2) + Mathf.Pow(n1.y - n2.y, 2));
    }

    public float ChebyshevDistance(NavMeshGraph graph, int node, int target)
    {
        Vector3 n1 = graph.GetNode(node).GetBarycenter();
        Vector3 n2 = graph.GetNode(target).GetBarycenter();

        return Mathf.Max(Mathf.Abs(n1.x - n2.x), Mathf.Abs(n1.y - n2.y));
    }

    public float MinkowskiDistance(NavMeshGraph graph, int node, int target, float p = 3)
    {
        Vector3 n1 = graph.GetNode(node).GetBarycenter();
        Vector3 n2 = graph.GetNode(target).GetBarycenter();

        return Mathf.Pow(Mathf.Pow(Mathf.Abs(n1.x - n2.x), p) + Mathf.Pow(Mathf.Abs(n1.y - n2.y), p), 1 / p);
    }

    public float DiagonalManhattanDistance(NavMeshGraph graph, int node, int target)
    {
        Vector3 n1 = graph.GetNode(node).GetBarycenter();
        Vector3 n2 = graph.GetNode(target).GetBarycenter();

        var dx = Mathf.Abs(n1.x - n2.x);
        var dy = Mathf.Abs(n1.y - n2.y);
        return Mathf.Max(dx, dy) + (SQRT2 - 1) * Mathf.Min(dx, dy);
    }
}

