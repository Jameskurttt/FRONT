using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    public GameObject enemyPrefab;
    public Transform[] spawnPoints;
    
   
    public int enemiesPerWave = 3;
    public int totalWaves = 2;
    public float spawnDelay = 1f;
    public float timeBetweenWaves = 3f;

   
    public GameObject arenaWall;

    bool started = false;
    int currentWave = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !started)
        {
            started = true;

            // Activate the wall so player cannot leave
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

            for (int i = 0; i < enemiesPerWave; i++)
            {
                SpawnEnemy();
                yield return new WaitForSeconds(spawnDelay);
            }

            yield return new WaitForSeconds(timeBetweenWaves);
        }

        Debug.Log("All waves finished!");

        // Disable the wall so player can leave
        if (arenaWall != null)
            arenaWall.SetActive(false);
    }

    void SpawnEnemy()
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }
} 