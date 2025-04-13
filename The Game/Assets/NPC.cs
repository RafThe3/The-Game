using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.Android;
using UnityEngine.Rendering;

public class NPC : MonoBehaviour
{
    [SerializeField] private Transform[] targets;
    [SerializeField] private float stoppingDistance = 1, stopDuration = 0;

    private NavMeshAgent agent;
    private int targetPoint = 0;
    private float agentSpeed;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        agentSpeed = agent.speed;
    }

    private void Update()
    {
        StartCoroutine(Patrol());
    }

    private IEnumerator Patrol()
    {
        Vector3 targetPosition = targets[targetPoint].position;
        Vector3 agentPosition = transform.position - targetPosition;
        bool isNearTarget = agentPosition.magnitude < stoppingDistance;

        agent.destination = targetPosition;

        if (isNearTarget && targetPoint <= targets.Length - 1)
        {
            targetPoint++;

            agent.speed = 0;
            yield return new WaitForSeconds(stopDuration);
            agent.speed = agentSpeed;
        }

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
    }
}
