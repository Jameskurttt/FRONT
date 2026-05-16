using UnityEngine;
using UnityEngine.AI;

public class EnemyShooter : MonoBehaviour
{
    [Header("References")]
    public EnemyChase chaseScript;
    public NavMeshAgent agent;
    public Animator animator;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("Range")]
    public float shootRange = 10f;
    public float shootCooldown = 1.5f;
    public float bulletSpeed = 20f;

    [Header("Animator")]
    public string attackTriggerName = "Attack";

    private float shootTimer;

    private void Awake()
    {
        if (chaseScript == null)
            chaseScript = GetComponent<EnemyChase>();

        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (chaseScript == null)
            return;

        Transform player = chaseScript.GetTarget();

        if (player == null)
            return;

        float distanceToPlayer =
            Vector3.Distance(transform.position, player.position);

        // Stop and shoot
        if (distanceToPlayer <= shootRange)
        {
            if (agent != null)
            {
                agent.isStopped = true;
                agent.ResetPath();
            }

            chaseScript.FaceTarget(player.position);
        }
        else
        {
            if (agent != null)
                agent.isStopped = false;
        }

        shootTimer -= Time.deltaTime;

        if (distanceToPlayer <= shootRange && shootTimer <= 0f)
        {
            shootTimer = shootCooldown;

            Shoot(player);
        }
    }

    private void Shoot(Transform player)
    {
        if (bulletPrefab == null || firePoint == null)
            return;

        // Play attack animation
        if (animator != null)
        {
            animator.ResetTrigger(attackTriggerName);
            animator.SetTrigger(attackTriggerName);
        }

        // Aim lower so arrow does not shoot upward
        Vector3 targetPos = player.position + Vector3.up * 0.4f;

        Vector3 direction =
            (targetPos - firePoint.position).normalized;

        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.LookRotation(direction)
        );

        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.isKinematic = false;

            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            rb.AddForce(
                direction * bulletSpeed,
                ForceMode.VelocityChange
            );
        }
    }
}