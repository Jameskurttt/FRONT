using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Enemy Stats")]
    public int maxHealth = 100;
    public int expReward = 3;
    public int goldReward = 10;

    [Header("Loot Drop")]
    public GameObject lootDropPrefab;
    public ItemDropData[] possibleDrops;
    [Range(0f, 100f)] public float dropChance = 50f;

    public delegate void MonsterDefeated(int exp);
    public static event MonsterDefeated OnMonsterDefeated;

    private int currentHealth;
    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Health left: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"{gameObject.name} died");

        GiveDefeatRewards(expReward, 1);

        if (GoldManager.instance != null)
        {
            GoldManager.instance.AddGold(goldReward);
        }

        TryDropLoot();

        Destroy(gameObject);
    }

    public static void GiveDefeatRewards(int exp, int killCount = 1)
    {
        OnMonsterDefeated?.Invoke(exp);

        if (KillCounterManager.Instance != null)
        {
            for (int i = 0; i < killCount; i++)
            {
                KillCounterManager.Instance.AddKill();
            }
        }
    }

    private void TryDropLoot()
    {
        if (lootDropPrefab == null || possibleDrops == null || possibleDrops.Length == 0)
            return;

        float roll = Random.Range(0f, 100f);

        if (roll > dropChance)
            return;

        ItemDropData randomItem = possibleDrops[Random.Range(0, possibleDrops.Length)];

        Vector3 offset = new Vector3(
            Random.Range(-0.5f, 0.5f),
            0f,
            Random.Range(-0.5f, 0.5f)
        );

        Vector3 spawnPos = transform.position + offset;

        GameObject dropObj = Instantiate(lootDropPrefab, spawnPos, Quaternion.identity);

        WorldLootDrop drop = dropObj.GetComponent<WorldLootDrop>();
        if (drop != null)
        {
            drop.Setup(randomItem);
        }
    }
}