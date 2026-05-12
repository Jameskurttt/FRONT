using UnityEngine;
using System.Collections.Generic;

public class SwordDamage : MonoBehaviour
{
    [Header("References")]
    public PlayerHealth playerStats;

    [Header("Sword Hitbox")]
    public Collider swordCollider;

    private bool canDealDamage = false;

    private List<EnemyHealth> hitEnemies = new List<EnemyHealth>();

    void Start()
    {
        if (playerStats == null)
            playerStats = GetComponentInParent<PlayerHealth>();

        if (swordCollider != null)
        {
            swordCollider.isTrigger = true;
            swordCollider.enabled = false;
        }
    }

    public void EnableSwordHitbox()
    {
        Debug.Log("HITBOX ON");

        canDealDamage = true;

        hitEnemies.Clear();

        if (swordCollider != null)
            swordCollider.enabled = true;
    }

    public void DisableSwordHitbox()
    {
        Debug.Log("HITBOX OFF");

        canDealDamage = false;

        if (swordCollider != null)
            swordCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canDealDamage)
            return;

        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();

        if (enemy != null && !hitEnemies.Contains(enemy))
        {
            hitEnemies.Add(enemy);

            int finalDamage = 10;

            if (playerStats != null)
                finalDamage = Mathf.RoundToInt(playerStats.GetTotalPhysicalAttack());

            Debug.Log("Hit Enemy: " + enemy.name);

            enemy.TakeDamage(finalDamage);
        }
    }
}