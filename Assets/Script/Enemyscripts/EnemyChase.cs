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
    public float detectionRange = 999f;
    public float stoppingDistance = 1.5f;
    public float rotationSpeed = 8f;

    [Header("Animation")]
    public string runBoolName = "IsRunning";

    private void Awake()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (agent != null)
        {
            agent.updateRotation = false;
            agent.stoppingDistance = stoppingDistance;
            agent.isStopped = false;
        }

        if (animator != null)
            animator.applyRootMotion = false;
    }

    private void Start()
    {
        FindPlayer();
    }

    private void Update()
    {
        if (agent == null)
            return;

        if (!agent.enabled || !agent.isOnNavMesh)
            return;

        if (player == null)
        {
            FindPlayer();
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > detectionRange)
        {
            StopEnemy();
            return;
        }

        if (distance <= stoppingDistance)
        {
            StopEnemy();
            FaceTarget(player.position);
            return;
        }

        ChasePlayer();
    }

    private void ChasePlayer()
    {
        agent.isStopped = false;
        agent.stoppingDistance = stoppingDistance;
        agent.SetDestination(player.position);

        SetRunning(true);

        Vector3 direction = agent.desiredVelocity;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private void StopEnemy()
    {
        agent.isStopped = true;
        SetRunning(false);
    }

    private void SetRunning(bool value)
    {
        if (animator == null)
            return;

        foreach (AnimatorControllerParameter parameter in animator.parameters)
        {
            if (parameter.name == runBoolName)
            {
                animator.SetBool(runBoolName, value);
                return;
            }
        }
    }

    private void FindPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);

        if (playerObject != null)
        {
            player = playerObject.transform;
            Debug.Log(gameObject.name + " found player: " + player.name);
        }
        else
        {
            Debug.LogError(gameObject.name + " could not find Player. Set Player Tag = Player.");
        }
    }

    public Transform GetTarget()
    {
        if (player == null)
            return null;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= detectionRange)
            return player;

        return null;
    }

    public void FaceTarget(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude <= 0.01f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }
}