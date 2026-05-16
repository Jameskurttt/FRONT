using UnityEngine;
using UnityEngine.AI;

public class EnemyMeleeAttack : MonoBehaviour
{
    [Header("References")]
    public EnemyChase enemyChase;
    public Animator animator;
    public NavMeshAgent agent;

    [Header("Attack Settings")]
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public int damage = 10;

    [Header("Animator")]
    public string attackTriggerName = "Attack";

    private float nextAttackTime;

    private void Awake()
    {
        if (enemyChase == null)
            enemyChase = GetComponent<EnemyChase>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (enemyChase == null || animator == null)
            return;

        Transform target = enemyChase.GetTarget();

        if (target == null)
            return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= attackRange && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;

            if (agent != null)
            {
                agent.isStopped = true;
                agent.ResetPath();
            }

            enemyChase.FaceTarget(target.position);

            animator.SetTrigger(attackTriggerName);

            PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
        }
        else if (distance > attackRange)
        {
            if (agent != null)
            {
                agent.isStopped = false;
            }
        }
    }
}