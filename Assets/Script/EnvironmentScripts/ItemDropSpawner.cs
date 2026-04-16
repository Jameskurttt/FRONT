using UnityEngine;

public class ItemDropSpawner : MonoBehaviour
{
    [Header("Drop Prefab")]
    public GameObject worldItemDropPrefab;

    [Header("Possible Drops")]
    public ItemDropData[] possibleDrops;

    [Header("Spawn Settings")]
    public float spawnHeightOffset = 0.5f;

    public void SpawnRandomDrop(Vector3 spawnPosition)
    {
        if (worldItemDropPrefab == null)
        {
            Debug.LogWarning("World item drop prefab is missing.");
            return;
        }

        if (possibleDrops == null || possibleDrops.Length == 0)
        {
            Debug.LogWarning("No possible drops assigned.");
            return;
        }

        ItemDropData randomItem = possibleDrops[Random.Range(0, possibleDrops.Length)];

        if (randomItem == null)
        {
            Debug.LogWarning("Random item drop data is missing.");
            return;
        }

        Vector3 finalSpawnPos = spawnPosition;
        finalSpawnPos.y += spawnHeightOffset;

        GameObject newDrop = Instantiate(worldItemDropPrefab, finalSpawnPos, Quaternion.identity);

        WorldLootDrop dropScript = newDrop.GetComponent<WorldLootDrop>();
        if (dropScript != null)
        {
            dropScript.Setup(randomItem);
        }
        else
        {
            Debug.LogWarning("WorldLootDrop component was not found on the drop prefab.");
        }
    }
}