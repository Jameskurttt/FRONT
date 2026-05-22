using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [System.Serializable]
    public class EnemyLootItem
    {
        public string itemName;
        public ItemDropData itemData;

        [Range(0f, 100f)]
        public float dropChance = 50f;

        [Min(1)]
        public int minDropAmount = 1;

        [Min(1)]
        public int maxDropAmount = 1;
    }

    [Header("Enemy Stats")]
    public int maxHealth = 100;
    public int expReward = 3;
    public int goldReward = 10;

    [Header("Animation")]
    public Animator enemyAnimator;
    public float deathDelay = 2f;

    [Header("Loot Drop")]
    public GameObject lootDropPrefab;
    public EnemyLootItem[] possibleDrops;

    private EnemyHitFlash hitFlash;

    public delegate void MonsterDefeated(int exp);
    public static event MonsterDefeated OnMonsterDefeated;

    private int currentHealth;
    private bool isDead = false;

    private void Start()
    {
        currentHealth = maxHealth;
        hitFlash = GetComponent<EnemyHitFlash>();

        if (enemyAnimator == null)
        {
            enemyAnimator = GetComponentInChildren<Animator>();
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;

        if (hitFlash != null)
        {
            hitFlash.FlashRed();
        }

        Debug.Log($"{gameObject.name} took {damage} damage. Health left: {currentHealth}");

        if (currentHealth <= 0)
        {
            StartCoroutine(Die());
        }
    }

    private IEnumerator Die()
    {
        if (isDead) yield break;
        isDead = true;

        Debug.Log($"{gameObject.name} died");

        // 1. STOP NAVMESH
        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            agent.isStopped = true;
            agent.ResetPath();
            agent.enabled = false;
        }

        // 2. STOP ALL AI BEHAVIOUR
        EnemyChase chase = GetComponent<EnemyChase>();
        if (chase != null) chase.enabled = false;

        EnemyMeleeAttack attack = GetComponent<EnemyMeleeAttack>();
        if (attack != null) attack.enabled = false;

        // FIX: Added explicit shutdown for the shooter script component
        EnemyShooter shooter = GetComponent<EnemyShooter>();
        if (shooter != null) shooter.enabled = false;

        // 3. DISABLE COLLIDER
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        // 4. ANIMATION STATE MACHINE OVERRIDES
        if (enemyAnimator != null)
        {
            enemyAnimator.ResetTrigger("Attack");
            enemyAnimator.SetBool("isRunning", false);

            // Force crossfade directly into the exact string name of your death node
            enemyAnimator.CrossFade("Goblin Archer Rig|GA_Death", 0.1f);
            enemyAnimator.SetTrigger("Die");
        }

        // 5. REWARDS AND LOOT
        GiveDefeatRewards(expReward, 1);

        if (GoldManager.instance != null)
        {
            GoldManager.instance.AddGold(goldReward);
        }

        TryDropLoot();

        yield return new WaitForSeconds(deathDelay);

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

        for (int i = 0; i < possibleDrops.Length; i++)
        {
            EnemyLootItem loot = possibleDrops[i];

            if (loot == null || loot.itemData == null)
                continue;

            float roll = Random.Range(0f, 100f);

            if (roll > loot.dropChance)
                continue;

            int minAmount = Mathf.Max(1, loot.minDropAmount);
            int maxAmount = Mathf.Max(minAmount, loot.maxDropAmount);

            int amountToDrop = Random.Range(minAmount, maxAmount + 1);

            for (int j = 0; j < amountToDrop; j++)
            {
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
                    drop.Setup(loot.itemData);
                }
            }

            Debug.Log("Dropped: " + loot.itemName + " x" + amountToDrop);
        }
    }
}