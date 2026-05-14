using UnityEngine;
using UnityEngine.AI;

public class EnemyChase : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;

    [Header("Player")]
    public string playerTag = "Player";
    public Transform player;

    [Header("Chase Settings")]
    public float detectionRange = 20f;
    public float stoppingDistance = 2f;
    public float rotationSpeed = 8f;

    private void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (agent != null)
        {
            agent.stoppingDistance = stoppingDistance;
            agent.updateRotation = false;
        }
        else
        {
            Debug.LogError($"{gameObject.name} has no NavMeshAgent.");
        }
    }

    private void Start()
    {
        FindPlayer();
    }

    private void Update()
    {
        if (agent == null)
            return;

        if (player == null)
        {
            FindPlayer();
            return;
        }

        // Make sure stopping distance always matches inspector value
        agent.stoppingDistance = stoppingDistance;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Only chase when player is close enough
        if (distanceToPlayer > detectionRange)
        {
            if (!agent.isStopped)
            {
                agent.isStopped = true;
                agent.ResetPath();
            }
            return;
        }

        // Chase player
        if (distanceToPlayer > stoppingDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);

            Vector3 moveDirection = agent.desiredVelocity;
            moveDirection.y = 0f;

            if (moveDirection.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }
        }
        else
        {
            agent.isStopped = true;
            FaceTarget(player.position);
        }
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    public Transform GetTarget()
    {
        if (player == null)
            return null;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= detectionRange)
            return player;

        return null;
    }

    public void FaceTarget(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}