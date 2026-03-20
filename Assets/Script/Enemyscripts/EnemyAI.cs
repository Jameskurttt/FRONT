using UnityEngine;
using UnityEngine.AI;

public class EnemyChasePro : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;

    [Header("Chase Settings")]
    public float chaseRange = 20f;
    public float stoppingDistance = 2f;
    public float rotationSpeed = 8f;

    private Transform player;
    private bool isChasing;

    void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        agent.stoppingDistance = stoppingDistance;
        agent.updateRotation = false; // we rotate manually
    }

    void Update()
    {
        if (!isChasing || player == null)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Only move if player is farther than stopping distance
        if (distanceToPlayer > stoppingDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else
        {
            agent.isStopped = true;
        }

        FacePlayer();
    }

    void FacePlayer()
    {
        if (player == null) return;

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f) return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    public void StartChasing(Transform target)
    {
        player = target;
        isChasing = true;
    }

    public void StopChasing()
    {
        isChasing = false;
        player = null;
        agent.isStopped = true;
        agent.ResetPath();
    }
}