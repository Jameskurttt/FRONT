using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimationController : MonoBehaviour
{
    public Animator animator;
    public NavMeshAgent agent;

    private bool isDead;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (agent == null)
            agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        if (animator == null)
            Debug.LogError("Animator not found in children!");

        if (agent == null)
            Debug.LogError("NavMeshAgent not found!");
    }

    public void PlayAttack()
    {
        if (isDead || animator == null) return;

        animator.ResetTrigger("Death");
        animator.SetTrigger("Attack");
    }

    public void PlayDeath()
    {
        if (isDead || animator == null) return;

        isDead = true;

        animator.ResetTrigger("Attack");
        animator.SetTrigger("Death");

        if (agent != null)
            agent.isStopped = true;
    }
}