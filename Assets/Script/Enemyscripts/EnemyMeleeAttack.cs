using UnityEngine;
using UnityEngine.AI;

public class EnemyMeleeAttack : MonoBehaviour
{
    [Header("References")]
    public EnemyChase enemyChase;
    public Animator animator;

    [Header("Attack Settings")]
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public int damage = 10;

    [Header("Animator")]
    public string attackBoolName = "isAttacking";

    private float nextAttackTime;

    private void Awake()
    {
        if (enemyChase == null) enemyChase = GetComponent<EnemyChase>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (enemyChase == null) return;

        Transform target = enemyChase.GetTarget();
        if (target == null)
        {
            SetAttack(false);
            return;
        }

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > attackRange)
        {
            SetAttack(false);
            return;
        }

        // Face target ONLY during attack (safe since NavMesh rotates)
        Vector3 dir = target.position - transform.position;
        dir.y = 0;

        if (dir != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(dir),
                10f * Time.deltaTime
            );
        }

        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;

            SetAttack(true);

            PlayerHealth ph = target.GetComponent<PlayerHealth>();
            if (ph != null)
            {
                ph.TakeDamage(damage);
            }
        }
    }

    private void SetAttack(bool value)
    {
        if (animator != null)
            animator.SetBool(attackBoolName, value);
    }
}