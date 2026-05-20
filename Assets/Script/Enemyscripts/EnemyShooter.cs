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

    [Header("Rotation")]
    public float rotationSpeed = 5f;

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

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= shootRange)
        {
            if (agent != null && agent.enabled)
            {
                agent.isStopped = true;
                agent.ResetPath();
            }

            RotateToward(player);

            shootTimer -= Time.deltaTime;

            if (shootTimer <= 0f)
            {
                shootTimer = shootCooldown;
                Shoot(player);
            }
        }
        else
        {
            if (agent != null && agent.enabled)
                agent.isStopped = false;
        }
    }

    private void RotateToward(Transform target)
    {
        Vector3 direction = target.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * 180f * Time.deltaTime
        );
    }

    private void Shoot(Transform player)
    {
        if (bulletPrefab == null || firePoint == null)
            return;

        if (animator != null)
        {
            animator.ResetTrigger(attackTriggerName);
            animator.SetTrigger(attackTriggerName);
        }

        Vector3 targetPos = player.position + Vector3.up * 0.4f;
        Vector3 direction = (targetPos - firePoint.position).normalized;

        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.LookRotation(direction)
        );

        Rigidbody rb = bullet.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.AddForce(direction * bulletSpeed, ForceMode.VelocityChange);
        }
    }
}