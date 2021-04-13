using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolEnemy : BaseEnemy
{
    public GameObject[] points;
    private int destPoint = 0;
    private NavMeshAgent navAgent;

    // Start is called before the first frame update
    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();
        points = GameObject.FindGameObjectsWithTag("NavPoint");
        navAgent.autoBraking = false;
        GoToNextPoint();
    }

    void GoToNextPoint()
    {
        if (points.Length == 0)
            return;

        navAgent.destination = points[destPoint].transform.position;
        destPoint = (destPoint + 1) % points.Length;
    }

    private void Update()
    {
        if (!navAgent.pathPending && navAgent.remainingDistance < 0.5f)
            GoToNextPoint();
    }
}
