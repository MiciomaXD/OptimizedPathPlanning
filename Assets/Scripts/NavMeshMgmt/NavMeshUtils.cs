using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public static class NavMeshUtils
{
    public static void BakeNavMesh(bool update = true)
    {
        NavMeshSurface nvs = GameObject.FindFirstObjectByType<NavMeshSurface>();
        if (nvs != null)
        {
            /*if (update)
                nvs.UpdateNavMesh(nvs.navMeshData);
            else
                nvs.BuildNavMesh();*/

            nvs.BuildNavMesh();
        }
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

    public static GameObject VisualizeMesh(Mesh m, Material mat, bool visualizeVertices = false)
    {
        if (visualizeVertices)
        {
            Vector3[] verts = m.vertices;
            foreach (var v in verts)
            {
                Debug.DrawRay(v, Vector3.up * 2, Color.red, Mathf.Infinity);
            }
        }

        GameObject navMeshObject = GameObject.Find("NavMeshVisualizer");
        if (navMeshObject != null)
            GameObject.Destroy(navMeshObject);

        navMeshObject = new GameObject("NavMeshVisualizer");
        navMeshObject.AddComponent<MeshFilter>().mesh = m;
        navMeshObject.AddComponent<MeshRenderer>();
        navMeshObject.GetComponent<MeshRenderer>().material = mat;
        navMeshObject.transform.position += new Vector3(0, 0.01f, 0);

        return navMeshObject;
    }
}
