using UnityEngine;

public class StarterDropSpawner : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Loot")]
    public GameObject lootDropPrefab;
    public ItemDropData swordItem;
    public ItemDropData bowItem;

    [Header("Spawn Settings")]
    public float forwardDistance = 2.5f;
    public float sideSpacing = 1.2f;
    public float heightOffset = 0.5f;

    [Header("Options")]
    public bool spawnOnStart = true;

    private void Start()
    {
        if (spawnOnStart)
        {
            SpawnStarterItems();
        }
    }

    public void SpawnStarterItems()
    {
        if (player == null || lootDropPrefab == null)
        {
            Debug.LogWarning("StarterDropSpawner: Missing player or lootDropPrefab.");
            return;
        }

        Vector3 basePosition = player.position
                             + player.forward * forwardDistance
                             + Vector3.up * heightOffset;

        // Left = sword
        SpawnItem(swordItem, basePosition - player.right * sideSpacing);

        // Right = bow
        SpawnItem(bowItem, basePosition + player.right * sideSpacing);

        Debug.Log("Starter items spawned.");
    }

    private void SpawnItem(ItemDropData itemData, Vector3 position)
    {
        if (itemData == null)
        {
            Debug.LogWarning("StarterDropSpawner: Missing item data.");
            return;
        }

        GameObject drop = Instantiate(lootDropPrefab, position, Quaternion.identity);

        WorldLootDrop loot = drop.GetComponent<WorldLootDrop>();
        if (loot != null)
        {
            loot.Setup(itemData);
        }
    }
}