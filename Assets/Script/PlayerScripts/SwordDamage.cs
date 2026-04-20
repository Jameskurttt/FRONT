using UnityEngine;

public class SwordDamage : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRadius = 2f;
    public LayerMask enemyLayer;

    [Header("Attack Timing")]
    public float attackCooldown = 0.5f;

    [Header("Player Reference")]
    public PlayerHealth playerStats;

    private float cooldownTimer;

    void Start()
    {
        if (playerStats == null)
            playerStats = GetComponentInParent<PlayerHealth>();
    }

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
        int finalDamage = 0;

        if (playerStats != null)
            finalDamage = Mathf.RoundToInt(playerStats.GetTotalPhysicalAttack());

        Collider[] hits = Physics.OverlapSphere(transform.position, attackRadius, enemyLayer);

        foreach (Collider hit in hits)
        {
            EnemyHealth enemy = hit.GetComponentInParent<EnemyHealth>();

            if (enemy != null)
            {
                Debug.Log("Hit: " + hit.name + " | Damage: " + finalDamage);
                enemy.TakeDamage(finalDamage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}