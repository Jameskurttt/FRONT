using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class GoblinKingAttack : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent agent;
    public Animator animator;
    public Transform player;

    [Header("Settings")]
    public float detectionRange = 15f;
    public float attackRange = 3f;
    public float attackCooldown = 2f;
    public int damage = 25;

    private bool isAttacking;
    private float nextAttackTime;

    private void Start()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        GameObject target = GameObject.FindGameObjectWithTag("Player");

        if (target != null)
            player = target.transform;
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // OUTSIDE DETECTION RANGE
        if (distance > detectionRange)
        {
            agent.isStopped = true;
            animator.SetBool("isRunning", false);
            return;
        }

        // ATTACK RANGE
        if (distance <= attackRange)
        {
            agent.isStopped = true;
            animator.SetBool("isRunning", false);

            FacePlayer();

            if (Time.time >= nextAttackTime && !isAttacking)
            {
                StartCoroutine(Attack());
            }
        }
        else
        {
            // CHASE PLAYER
            agent.isStopped = false;
            agent.SetDestination(player.position);

            animator.SetBool("isRunning", true);
        }
    }

    private IEnumerator Attack()
    {
        isAttacking = true;

        nextAttackTime = Time.time + attackCooldown;

        animator.SetTrigger("Attack");

        // wait until hit frame
        yield return new WaitForSeconds(0.8f);

        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            if (distance <= attackRange)
            {
                PlayerHealth ph = player.GetComponent<PlayerHealth>();

                if (ph != null)
                {
                    ph.TakeDamage(damage);
                }
            }
        }

        // wait before allowing another attack
        yield return new WaitForSeconds(0.5f);

        isAttacking = false;
    }

    private void FacePlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                lookRotation,
                10f * Time.deltaTime
            );
        }
    }
}