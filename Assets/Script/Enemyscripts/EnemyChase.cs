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

    [Header("Animation")]
    public string runBoolName = "isRunning"; // IMPORTANT FIXED

    private void Awake()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponentInChildren<Animator>();

        agent.stoppingDistance = stoppingDistance;

        // IMPORTANT: we control rotation manually (prevents jitter)
        agent.updateRotation = false;
    }

    private void Start()
    {
        FindPlayer();
    }

    private void Update()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
            return;

        if (player == null)
        {
            FindPlayer();
            return;
        }

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > detectionRange)
        {
            StopMoving();
            return;
        }

        if (distance <= stoppingDistance)
        {
            StopMoving();
            return;
        }

        MoveToPlayer();
    }

    private void MoveToPlayer()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);

        SetRunning(true);
    }

    private void StopMoving()
    {
        agent.isStopped = true;
        agent.ResetPath();

        SetRunning(false);
    }

    private void SetRunning(bool value)
    {
        if (animator == null) return;

        // SAFE: prevents crash if parameter missing
        if (HasParameter(runBoolName))
            animator.SetBool(runBoolName, value);

        Debug.Log("isRunning = " + value);
    }

    private bool HasParameter(string name)
    {
        foreach (AnimatorControllerParameter p in animator.parameters)
        {
            if (p.name == name)
                return true;
        }
        return false;
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