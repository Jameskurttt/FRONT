using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    public GameObject enemyPrefab;
    public Transform[] spawnPoints;

    [Header("Wave Settings")]
    public int enemiesPerWave = 3;
    public int totalWaves = 2;
    public float spawnDelay = 1f;
    public float timeBetweenWaves = 3f;

    [Header("Arena")]
    public GameObject arenaWall;

    private bool started = false;
    private int currentWave = 0;

    // keeps track of enemies that are still alive
    private List<GameObject> aliveEnemies = new List<GameObject>();

    private void OnTriggerEnter(Collider other)
    {
        if (!started && other.CompareTag("Player"))
        {
            started = true;

            if (arenaWall != null)
                arenaWall.SetActive(true);

            StartCoroutine(SpawnWaves());
        }
    }

    IEnumerator SpawnWaves()
    {
        while (currentWave < totalWaves)
        {
            currentWave++;
            Debug.Log("Wave " + currentWave + " started");

            // Spawn all enemies for this wave
            for (int i = 0; i < enemiesPerWave; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(spawnDelay);
            }

            // Wait until all spawned enemies are dead
            yield return new WaitUntil(() => AllEnemiesDead());

            Debug.Log("Wave " + currentWave + " cleared");

            // small pause before next wave
            if (currentWave < totalWaves)
                yield return new WaitForSeconds(timeBetweenWaves);
        }

        Debug.Log("All waves finished!");

        if (arenaWall != null)
            arenaWall.SetActive(false);
    }

    void SpawnEnemy()
    {
        if (spawnPoints.Length == 0 || enemyPrefab == null)
        {
            Debug.LogWarning("Spawner is missing spawn points or enemy prefab.");
            return;
        }

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        aliveEnemies.Add(enemy);
    }

    bool AllEnemiesDead()
    {
        // remove destroyed enemies from the list
        aliveEnemies.RemoveAll(enemy => enemy == null);

        return aliveEnemies.Count == 0;
    }
}