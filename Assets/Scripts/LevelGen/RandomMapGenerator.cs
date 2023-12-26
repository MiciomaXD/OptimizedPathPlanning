using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class RandomMapGenerator : MonoBehaviour
{
    [SerializeField]
    MeshRenderer terrainMR;
    [SerializeField]
    Transform parentOfObstacles;

    public int minObstacles, maxObstacles, seed = 9516;

    public List<GameObject> obstacles;

    public float minScale = 1f, maxScale = 1f;

    Bounds mapBounds;
    Vector3 center;
    float minX, maxX, minY, maxY, minZ, maxZ;
    System.Random rng;

    // Start is called before the first frame update
    void Start()
    {
        rng = new System.Random(seed);

        mapBounds = terrainMR.bounds;

        center = mapBounds.center;
        minX = mapBounds.min.x;
        minY = mapBounds.min.y;
        minZ = mapBounds.min.z;

        maxX = mapBounds.max.x;
        maxY = mapBounds.max.y;
        maxZ = mapBounds.max.z;

        StartCoroutine(DoCreateMap(5f));
    }

    IEnumerator DoCreateMap(float wait)
    {
        while (true)
        {
            CreateMap();
            yield return new WaitForSeconds(wait);
        }
    }

    private void CreateMap()
    {
        DestroyObstacles();

        int effectiveObstacleNumber = (int)(minObstacles + (float)rng.NextDouble() * (maxObstacles - minObstacles));

        for (int i = 0; i < effectiveObstacleNumber; i++)
        {
            AttemptToPlaceObstacle();
        }

        NavMeshUtils.BakeNavMesh();
    }

    private void DestroyObstacles()
    {
        while (parentOfObstacles.childCount > 0)
        {
            //serve immediate altrimenti il baking di mesh è solo parzialmente aggiornato
            GameObject.DestroyImmediate(parentOfObstacles.GetChild(0).gameObject);
        }
    }

    private void AttemptToPlaceObstacle()
    {
        float x = minX + (float)rng.NextDouble() * (maxX - minX);
        float z = minZ + (float)rng.NextDouble() * (maxZ - minZ);


        Ray r = new(new Vector3(x, maxY, z), Vector3.down);
        LayerMask mask = LayerMask.GetMask("Terrain");

        RaycastHit hit;
        if (Physics.Raycast(r, out hit, maxY - minY, mask))
        {
            SpawnObstacle(hit.point);
        }
    }

    private void SpawnObstacle(Vector3 point)
    {
        int chosenObstacle = (int)(rng.NextDouble() * obstacles.Count);

        GameObject obj = GameObject.Instantiate(obstacles[chosenObstacle]);
        obj.transform.position = point;
        obj.transform.localScale = new(minScale + (float)rng.NextDouble() * (maxScale - minScale), minScale + (float)rng.NextDouble() * (maxScale - minScale), minScale + (float)rng.NextDouble() * (maxScale - minScale));
        obj.transform.parent = parentOfObstacles;

        //random y rot
        obj.transform.rotation = Quaternion.AngleAxis((float)rng.NextDouble() * 360, Vector3.up);
    }
}
