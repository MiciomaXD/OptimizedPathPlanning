using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public static class NavMeshUtils
{
    public static void BakeNavMesh()
    {
        NavMeshSurface nvs = GameObject.FindFirstObjectByType<NavMeshSurface>();
        if (nvs != null)
            nvs.BuildNavMesh();
        else
            Debug.LogError("There is no NavMeshSurface component in the scene.");
    }

    public static Mesh CreateMeshFromTriangulation(NavMeshTriangulation navMeshData)
    {
        Mesh mesh = new Mesh();
        mesh.vertices = navMeshData.vertices;
        mesh.triangles = navMeshData.indices;

        mesh.RecalculateTangents();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

    public static void VisualizeMesh(Mesh m, Material mat, bool visualizeVertices = false)
    {
        if (visualizeVertices)
        {
            foreach (var v in m.vertices)
            {
                Debug.DrawRay(v, Vector3.up * 2, Color.red, Mathf.Infinity);
            }
        }

        GameObject navMeshObject = new GameObject("NavMeshVisualizer");
        navMeshObject.AddComponent<MeshFilter>().mesh = m;
        navMeshObject.AddComponent<MeshRenderer>();
        navMeshObject.GetComponent<MeshRenderer>().material = mat;
        navMeshObject.transform.position += new Vector3(0, 0.01f, 0);
    }
}
