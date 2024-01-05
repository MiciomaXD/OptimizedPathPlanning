using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class NavMeshOneShotVisualizer : MonoBehaviour
{
    public Material visualizerMat;
    public bool visualizeVertices;

    //public UnityEvent finishedVisualization;

    void Start()
    {
        //Visualize();
    }

    public GameObject Visualize()
    {

        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();
        Debug.Log(string.Format("NavMesh has {0} vertices and {1} triangles.", navMeshData.vertices.Length, navMeshData.indices.Length / 3));

        Mesh navMeshMesh = NavMeshUtils.CreateMeshFromTriangulation(navMeshData);

        return NavMeshUtils.VisualizeMesh(navMeshMesh, visualizerMat, visualizeVertices: visualizeVertices);
    }
}
