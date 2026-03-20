using UnityEngine;
using UnityEngine.AI;

public class EnemyinSpawmer : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    public Transform player;

    [Header("Ranges")]
    public float detectionRange = 10f;
    public float stoppingDistance = 2f;

    [Header("Settings")]
    public float rotationSpeed = 5f;

    private bool isChasing = false;

    void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        player = GameObject.FindGameObjectWithTag("Player").transform;

        agent.stoppingDistance = stoppingDistance;
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        // Detect player
        if (distance <= detectionRange)
        {
            isChasing = true;
        }
        else
        {
            isChasing = false;
            agent.ResetPath(); // stop moving
        }

        // Chase player
        if (isChasing)
        {
            agent.SetDestination(player.position);

            // Smoothly face player
            Vector3 direction = (player.position - transform.position).normalized;
            direction.y = 0;

            if (direction != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }

   
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}