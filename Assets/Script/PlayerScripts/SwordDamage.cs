using UnityEngine;

public class SwordDamage : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRadius = 2f;
    public int damage = 25;
    public LayerMask enemyLayer;

    [Header("Attack Timing")]
    public float attackCooldown = 0.5f;

    private float cooldownTimer;

    void Update()
    {
        cooldownTimer -= Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && cooldownTimer <= 0f)
        {
            Attack();
            cooldownTimer = attackCooldown;
        }
    }

    void Attack()
    {
       

        Collider[] hits = Physics.OverlapSphere(transform.position, attackRadius, enemyLayer);

        foreach (Collider hit in hits)
        {
            EnemyHealth enemy = hit.GetComponentInParent<EnemyHealth>();

            if (enemy != null)
            {
                Debug.Log("Hit: " + hit.name);
                enemy.TakeDamage(damage);
            }
        }
    }

    // Visualize attack range
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}