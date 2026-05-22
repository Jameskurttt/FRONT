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

    [Header("Combat Timings")]
    public float hitFrameDelay = 0.3f; 
    public float attackRecovery = 0.5f;

    private bool isAttacking;
    private float nextAttackTime;

    private void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponentInChildren<Animator>();

        GameObject target = GameObject.FindGameObjectWithTag("Player");
        if (target != null) player = target.transform;
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > detectionRange)
        {
            agent.isStopped = true;
            animator.SetBool("isRunning", false);
            return;
        }

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
        else if (!isAttacking)
        {
            
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

        
        yield return new WaitForSeconds(hitFrameDelay);

        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            if (distance <= attackRange)
            {
                PlayerHealth ph = player.GetComponent<PlayerHealth>();
                if (ph != null) ph.TakeDamage(damage);
            }
        }

        
        yield return new WaitForSeconds(attackRecovery);
        isAttacking = false;
    }

    private void FacePlayer()
    {
        Vector3 direction = player.position - transform.position;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion lookRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, 10f * Time.deltaTime);
        }
    }
}