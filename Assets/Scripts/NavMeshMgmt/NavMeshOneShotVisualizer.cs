using UnityEngine;
using UnityEngine.AI;

public class NavMeshOneShotVisualizer : MonoBehaviour
{
    public Material visualizerMat;

    void Start()
    {
        NavMeshUtils.BakeNavMesh();

        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();

        Mesh navMeshMesh = NavMeshUtils.CreateMeshFromTriangulation(navMeshData);

        NavMeshUtils.VisualizeMesh(navMeshMesh, visualizerMat, true);
    }
}
