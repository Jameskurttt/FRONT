using System.Collections.Generic;
using UnityEngine;

public class ChestInteractable : MonoBehaviour
{
    [System.Serializable]
    public class LootItem
    {
        public string itemName = "New Item";
        public ItemDropData itemData;

        [Range(0f, 100f)]
        public float dropChance = 50f;

        [Min(1)]
        public int minDropAmount = 1;

        [Min(1)]
        public int maxDropAmount = 1;
    }

    [Header("Chest UI")]
    public string interactMessage = "Press E to Interact";

    [TextArea]
    public string chestDescription = "Open chest to get random loot.";

    [Header("Chest State")]
    public bool hasOpened = false;
    public bool destroyChestAfterOpen = true;

    [Header("Loot Drop Prefab")]
    public GameObject lootDropPrefab;

    [Header("Drop Position")]
    public Transform rewardSpawnPoint;
    public float rewardHeightOffset = 0.5f;
    public float horizontalSpread = 1.2f;

    [Header("Loot Table")]
    public List<LootItem> possibleLoot = new List<LootItem>();

    [Header("Gold Reward")]
    public bool giveGoldDirectly = true;

    [Range(0f, 100f)]
    public float goldDropChance = 100f;

    public int minGold = 5;
    public int maxGold = 20;

    [Header("Drop Rules")]
    public bool dropAllSuccessfulItems = true;
    public int minimumItemsToDrop = 1;
    public int maximumItemsToDrop = 3;

    public void Interact()
    {
        if (hasOpened)
            return;

        hasOpened = true;

        OpenChest();

        Debug.Log("Chest opened.");

        if (destroyChestAfterOpen)
            Destroy(gameObject);
    }

    private void OpenChest()
    {
        DropLootItems();
        GiveGoldToPlayer();
    }

    private void DropLootItems()
    {
        List<LootItem> successfulLoot = RollLootTable();

        if (successfulLoot.Count == 0)
        {
            Debug.Log("No item loot dropped from this chest.");
            return;
        }

        if (!dropAllSuccessfulItems)
        {
            int randomItemCount = Random.Range(minimumItemsToDrop, maximumItemsToDrop + 1);
            successfulLoot = GetRandomSelection(successfulLoot, randomItemCount);
        }

        for (int i = 0; i < successfulLoot.Count; i++)
        {
            LootItem loot = successfulLoot[i];

            if (loot == null || loot.itemData == null)
                continue;

            int amountToSpawn = Random.Range(loot.minDropAmount, loot.maxDropAmount + 1);

            for (int j = 0; j < amountToSpawn; j++)
            {
                SpawnLootDrop(loot.itemData);
                Debug.Log("Dropped item: " + loot.itemName);
            }
        }
    }

    private void SpawnLootDrop(ItemDropData itemData)
    {
        if (lootDropPrefab == null)
        {
            Debug.LogWarning("Loot drop prefab is missing on chest.");
            return;
        }

        if (itemData == null)
        {
            Debug.LogWarning("ItemDropData is missing on chest loot.");
            return;
        }

        Vector3 spawnPos = GetSpawnPosition();
        GameObject dropObject = Instantiate(lootDropPrefab, spawnPos, Quaternion.identity);

        WorldLootDrop lootDrop = dropObject.GetComponent<WorldLootDrop>();
        if (lootDrop != null)
        {
            lootDrop.Setup(itemData);
        }
        else
        {
            Debug.LogWarning("WorldLootDrop component was not found on lootDropPrefab.");
        }
    }

    private void GiveGoldToPlayer()
    {
        if (!giveGoldDirectly)
            return;

        float goldRoll = Random.Range(0f, 100f);

        if (goldRoll > goldDropChance)
        {
            Debug.Log("No gold rewarded from this chest.");
            return;
        }

        int goldAmount = Random.Range(minGold, maxGold + 1);

        if (GoldManager.instance != null)
        {
            GoldManager.instance.AddGold(goldAmount);
            Debug.Log("Player received gold: " + goldAmount);
        }
        else
        {
            Debug.LogWarning("GoldManager instance not found.");
        }
    }

    private List<LootItem> RollLootTable()
    {
        List<LootItem> droppedLoot = new List<LootItem>();

        for (int i = 0; i < possibleLoot.Count; i++)
        {
            LootItem loot = possibleLoot[i];

            if (loot == null || loot.itemData == null)
                continue;

            float roll = Random.Range(0f, 100f);

            if (roll <= loot.dropChance)
            {
                droppedLoot.Add(loot);
            }
        }

        return droppedLoot;
    }

    private Vector3 GetSpawnPosition()
    {
        Vector3 basePosition;

        if (rewardSpawnPoint != null)
            basePosition = rewardSpawnPoint.position;
        else
            basePosition = transform.position + Vector3.up * rewardHeightOffset;

        Vector2 randomCircle = Random.insideUnitCircle * horizontalSpread;
        Vector3 finalPosition = basePosition + new Vector3(randomCircle.x, 0f, randomCircle.y);

        return finalPosition;
    }

    private List<LootItem> GetRandomSelection(List<LootItem> sourceList, int count)
    {
        List<LootItem> tempList = new List<LootItem>(sourceList);
        List<LootItem> result = new List<LootItem>();

        count = Mathf.Min(count, tempList.Count);

        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, tempList.Count);
            result.Add(tempList[randomIndex]);
            tempList.RemoveAt(randomIndex);
        }

        return result;
    }
}