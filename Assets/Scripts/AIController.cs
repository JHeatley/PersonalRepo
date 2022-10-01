using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum BehaviorStates { Void, Travel, Follow, Attack, FollowAndAttack }
public class AIController : MonoBehaviour
{
    GameObject player;

    public GameObject[] wayPoints;
    NavMeshAgent agent;

    public BehaviorStates state;

    public float distanceRadius;
    int startingPath = 0;
    int pathLength = 0;

    public GameObject shootingPos;
    public GameObject bullet;
    public float timeToFire;
    public float timeToFireStartTime;

    // Start is called before the first frame update
    void Start()
    {
        timeToFire = timeToFireStartTime;
        state = BehaviorStates.Void;
        startingPath = 0;
        pathLength = wayPoints.Length;
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (startingPath == 0)
            state = BehaviorStates.Void;


        if (startingPath < pathLength)
        {
            state = BehaviorStates.Travel;
            if (Vector3.Distance(wayPoints[startingPath].transform.position, gameObject.transform.position) < 1)
            {
                if (startingPath == pathLength - 1)
                {
                    startingPath = 0;
                }
                else
                {
                    startingPath++;
                }
            }
        }
        if (state == BehaviorStates.Travel)
            agent.SetDestination(wayPoints[startingPath].transform.position);

        if (Vector3.Distance(transform.position, player.transform.position) < distanceRadius)
        {
            state = BehaviorStates.FollowAndAttack;
        }

        if (state == BehaviorStates.Follow)
            agent.SetDestination(player.transform.position);

        else if (state == BehaviorStates.FollowAndAttack)
        {
            agent.SetDestination(player.transform.position);

            timeToFire -= Time.deltaTime;

            if (timeToFire <= 0)
                timeToFire = 0;

            if (timeToFire == 0)
            {
                Instantiate(bullet.transform, shootingPos.transform.position, Quaternion.identity);
                timeToFire = timeToFireStartTime;
            }
        }
        else
        {
            state = BehaviorStates.Travel;
        }
    }
}