using System.Collections.Generic;
using UnityEngine;

public class SwordDamage : MonoBehaviour
{
    private Weapon weapon;
    private PlayerHealth playerStats;

    private List<EnemyHealth> hitEnemies = new List<EnemyHealth>();

    private void Awake()
    {
        weapon = GetComponentInParent<Weapon>();

        if (weapon != null)
            playerStats = weapon.GetComponentInParent<PlayerHealth>();

        if (playerStats == null)
            playerStats = FindObjectOfType<PlayerHealth>();
    }

    public void StartDamage()
    {
        hitEnemies.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (weapon == null)
            return;

        if (!weapon.IsEquipped())
            return;

        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();

        if (enemy == null)
            return;

        if (hitEnemies.Contains(enemy))
            return;

        hitEnemies.Add(enemy);

        int finalDamage = GetFinalDamage();

        enemy.TakeDamage(finalDamage);

        Debug.Log("Sword hit: " + enemy.name + " | Final Damage: " + finalDamage);
    }

    private int GetFinalDamage()
    {
        if (playerStats != null)
            return Mathf.RoundToInt(playerStats.GetTotalPhysicalAttack());

        if (weapon != null)
            return weapon.GetWeaponDamage();

        return 0;
    }
}