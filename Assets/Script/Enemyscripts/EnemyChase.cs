using UnityEngine;
using UnityEngine.AI;

public class EnemyChase : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    public Animator animator;

    [Header("Player")]
    public string playerTag = "Player";
    public Transform player;

    [Header("Chase Settings")]
    public float detectionRange = 10f;
    public float stoppingDistance = 1.5f;

    [Header("Rotation")]
    public float rotationSpeed = 8f;

    [Header("Animation")]
    public string runBoolName = "isRunning";

    private bool isRunning = false;

    private void Awake()
    {
        // Auto assign references
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        // Setup agent
        agent.stoppingDistance = stoppingDistance;

        // We rotate manually
        agent.updateRotation = false;
    }

    private void Start()
    {
        FindPlayer();
    }

    private void Update()
    {
        // Safety checks
        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
            return;

        // Find player again if missing
        if (player == null)
        {
            FindPlayer();
            return;
        }

        // Rotate toward player
        RotateToPlayer();

        // Distance to player
        float distance = Vector3.Distance(transform.position, player.position);

        // Outside detection range
        if (distance > detectionRange)
        {
            StopMoving();
            return;
        }

        // Chase player
        if (distance > stoppingDistance)
        {
            MoveToPlayer();
        }
        else
        {
            StopMoving();
        }
    }

    private void MoveToPlayer()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);

        if (!isRunning)
        {
            SetRunning(true);
            isRunning = true;
        }
    }

    private void StopMoving()
    {
        agent.isStopped = true;
        agent.ResetPath();

        if (isRunning)
        {
            SetRunning(false);
            isRunning = false;
        }
    }

    private void RotateToPlayer()
    {
        if (player == null)
            return;

        Vector3 direction = player.position - transform.position;

        // Ignore vertical tilt
        direction.y = 0f;

        // Prevent errors
        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion lookRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            lookRotation,
            Time.deltaTime * rotationSpeed
        );
    }

    private void SetRunning(bool value)
    {
        if (animator == null)
            return;

        animator.SetBool(runBoolName, value);

    }

    private void FindPlayer()
    {
        GameObject obj = GameObject.FindGameObjectWithTag(playerTag);

        if (obj != null)
            player = obj.transform;
    }

    public Transform GetTarget()
    {
        return player;
    }
}