using System.Collections.Generic;
using UnityEngine;

public class SwordDamage : MonoBehaviour
{
    private Weapon weapon;
    private PlayerHealth playerStats;
    private List<GameObject> hitTargets = new List<GameObject>();

    private void Awake()
    {
        weapon = GetComponentInParent<Weapon>();

        if (weapon != null)
            playerStats = weapon.GetComponentInParent<PlayerHealth>();

        if (playerStats == null)
            playerStats = FindAnyObjectByType<PlayerHealth>();
    }

    public void StartDamage()
    {
        hitTargets.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (weapon == null)
            return;

        if (!weapon.IsEquipped())
            return;

        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();
        BossHealth boss = other.GetComponentInParent<BossHealth>();

        if (enemy == null && boss == null)
            return;

        GameObject targetObject = enemy != null ? enemy.gameObject : boss.gameObject;

        if (hitTargets.Contains(targetObject))
            return;

        hitTargets.Add(targetObject);

        int finalDamage = GetFinalDamage();

        if (enemy != null)
        {
            enemy.TakeDamage(finalDamage);
            Debug.Log("Sword hit: " + enemy.name + " | Final Damage: " + finalDamage);
        }
        else if (boss != null)
        {
            boss.TakeDamage(finalDamage);
            Debug.Log("Sword hit Boss: " + boss.bossName + " | Final Damage: " + finalDamage);
        }

        PlayerMovement playerMovement = weapon.GetComponentInParent<PlayerMovement>();

        if (playerMovement != null)
        {
            Animator animator = playerMovement.animator;

            if (animator != null)
            {
               
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(1);

                if (stateInfo.IsName("SWORD_ATTACK1"))
                {
                    playerMovement.PlaySwordCombo1HitSound();
                }
                else if (stateInfo.IsName("SWORD_ATTACK2"))
                {
                    playerMovement.PlaySwordCombo2HitSound();
                }
                else if (stateInfo.IsName("SWORD_ATTACK3"))
                {
                    playerMovement.PlaySwordCombo3HitSound();
                }
            }
        }
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