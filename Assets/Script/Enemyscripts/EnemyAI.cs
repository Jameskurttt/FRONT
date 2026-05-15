using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    public Transform player;

    [Header("Patrol Settings")]
    public float patrolRadius = 10f;
    public float waitTime = 2f;

    [Header("Chase Settings")]
    public float rotationSpeed = 5f;

    private Vector3 patrolPoint;
    private bool hasPatrolPoint;
    private float waitTimer;

    private bool isChasing = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    // ---------------- PATROL ----------------
    void Patrol()
    {
        if (!hasPatrolPoint)
        {
            SetRandomPatrolPoint();
        }

        if (hasPatrolPoint)
        {
            agent.SetDestination(patrolPoint);

            float distance = Vector3.Distance(transform.position, patrolPoint);

            if (distance < 1.5f)
            {
                waitTimer += Time.deltaTime;

                if (waitTimer >= waitTime)
                {
                    hasPatrolPoint = false;
                    waitTimer = 0f;
                }
            }
        }
    }

    void SetRandomPatrolPoint()
    {
        float randomZ = Random.Range(-patrolRadius, patrolRadius);
        float randomX = Random.Range(-patrolRadius, patrolRadius);

        patrolPoint = new Vector3(
            transform.position.x + randomX,
            transform.position.y,
            transform.position.z + randomZ
        );

        if (NavMesh.SamplePosition(patrolPoint, out NavMeshHit hit, 2f, NavMesh.AllAreas))
        {
            patrolPoint = hit.position;
            hasPatrolPoint = true;
        }
    }

    // ---------------- CHASE ----------------
    void ChasePlayer()
    {
        if (player == null) return;

        agent.SetDestination(player.position);

     
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        }
    }

    // ---------------- TRIGGER ----------------
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
            isChasing = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isChasing = false;
            hasPatrolPoint = false;
        }
    }
}