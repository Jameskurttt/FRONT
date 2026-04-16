using UnityEngine;

public class BossHealth : MonoBehaviour
{
    [Header("Boss Info")]
    public string bossName = "Demon King";
    public int maxHealth = 500;

    [Header("Rewards")]
    public int expReward = 50;
    public int goldReward = 100;
    public int killCountValue = 1;

    [HideInInspector] public BossHealthUI bossUI;

    private int currentHealth;
    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;

        if (bossUI != null)
        {
            bossUI.Setup(maxHealth, bossName);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (currentHealth < 0)
            currentHealth = 0;

        if (bossUI != null)
        {
            bossUI.UpdateHealth(currentHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        EnemyHealth.GiveDefeatRewards(expReward, killCountValue);

        if (GoldManager.instance != null)
        {
            GoldManager.instance.AddGold(goldReward);
        }

        if (bossUI != null)
        {
            bossUI.Hide();
        }

        Debug.Log($"Boss defeated! +{expReward} EXP, +{goldReward} GOLD, +{killCountValue} KILL");

        Destroy(gameObject);
    }
}