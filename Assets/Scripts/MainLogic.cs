using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class MainLogic : MonoBehaviour
{
    public RandomMapGenerator randomMapGenerator;
    public NavMeshOneShotVisualizer navMeshOneShotVisualizer;

    public Material edgeVis;

    public UnityEvent onMapCreation;

    // Start is called before the first frame update
    void Start()
    {
        CreateOneInstanceGraph()
            .VisualizeGraphInScene(edgeVis);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public NavMeshGraph CreateOneInstanceGraph()
    {
        randomMapGenerator.CreateMap();
        Debug.Log("Map Created");

        GameObject navVisuals = navMeshOneShotVisualizer.Visualize();
        Debug.Log("Visualized NavMesh");

        NavMeshGraph g = new(navVisuals.GetComponent<MeshFilter>().mesh);

        return g;
    }
}
