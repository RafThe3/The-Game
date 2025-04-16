using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Android;
using UnityEngine.Rendering;

public class NPC : MonoBehaviour
{
    [SerializeField] private bool allowMovement = true;
    [SerializeField] private Transform[] targets;
    [SerializeField] private float stoppingDistance = 1, stopDuration = 0, interactionDistance = 1;

    private NavMeshAgent agent;
    private int targetPoint = 0;
    private float agentSpeed;
    private Transform player;
    private bool isPatrolling;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindWithTag("Player").transform;
    }

    private void Start()
    {
        agentSpeed = agent.speed;
    }

    private void Update()
    {
        if (allowMovement)
        {
            StartCoroutine(Patrol());
        }
    }

    private IEnumerator Patrol()
    {
        isPatrolling = true;

        Vector3 targetPosition = targets[targetPoint].position;
        Vector3 agentPosition = transform.position - targetPosition;
        bool isNearTarget = agentPosition.magnitude < stoppingDistance;

        Vector3 playerPosition = player.position - transform.position;
        bool isPlayerNear = playerPosition.magnitude < interactionDistance;

        agent.destination = targetPosition;

        if (isNearTarget && targetPoint <= targets.Length - 1)
        {
            targetPoint++;

            agent.speed = 0;
            yield return new WaitForSeconds(stopDuration);
            agent.speed = agentSpeed;
        }

        /*
        if (isPatrolling)
        {
            if (isPlayerNear)
            {
                transform.LookAt(Camera.main.transform, Vector3.up);
                agent.speed = 0;
            }
            else
            {
                if (!isNearTarget)
                {
                    agent.speed = agentSpeed;
                }
            }
        }
        */

        if (targetPoint.Equals(targets.Length))
        {
            targetPoint = 0;
        }
    }

    private void OnDrawGizmos()
    {
        foreach (Transform target in targets)
        {
            Gizmos.DrawWireSphere(target.position, stoppingDistance);
        }

        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}
