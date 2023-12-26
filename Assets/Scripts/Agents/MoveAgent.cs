using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveAgent : MonoBehaviour
{
    public Transform dest;
    NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        agent=GetComponent<NavMeshAgent>();

        agent.SetDestination(dest.position);
    }

    // Update is called once per frame
    void Update()
    {
    }
}
