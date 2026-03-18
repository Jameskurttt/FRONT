using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{
    public List<GameObject> enemyPrefabs;
    public Transform[] spawnPoints;
    public int totalWaves = 3;
    private int currentWave = 0;

    public IEnumerator SpawnWaves()
    {
        for (int i = 0; i < totalWaves; i++)
        {
            currentWave++;
            Debug.Log("Starting Wave: " + currentWave);

            // Spawn enemies
            SpawnEnemyGroup(currentWave * 2); // Example: 2, 4, 6 enemies

            // Wait until all enemies in this wave are dead
            yield return new WaitUntil(() => GameObject.FindGameObjectsWithTag("Enemy").Length == 0);

            Debug.Log("Wave " + currentWave + " Cleared");
            yield return new WaitForSeconds(2f); // Short break
        }
        Debug.Log("All Waves Cleared! Exit Open.");
    }

    void SpawnEnemyGroup(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(enemyPrefabs[0], spawnPoint.position, Quaternion.identity);
        }
    }
}
