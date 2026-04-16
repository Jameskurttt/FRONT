using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class EnemySpawnData
    {
        public GameObject enemyPrefab;
        [Range(1, 100)]
        public int spawnWeight = 10;
    }

    [Header("Normal Enemy Variants")]
    public EnemySpawnData[] enemyVariants;
    public Transform[] spawnPoints;

    [Header("Boss Settings")]
    public GameObject bossPrefab;
    public Transform bossSpawnPoint;
    public int bossWave = 25;

    [Header("Wave Settings")]
    public int startingEnemiesPerWave = 3;
    public int enemyIncreasePerWave = 1;
    public int totalWaves = 25;
    public float delayBetweenSpawns = 0.5f;
    public float delayBetweenWaves = 2f;

    [Header("Spawn Settings")]
    public float spawnRadius = 3f;
    public float minDistanceBetweenEnemies = 1.5f;
    public int maxSpawnAttempts = 10;

    [Header("Arena")]
    public GameObject arenaWall;

    [Header("Chest Reward")]
    public GameObject chestPrefab;
    public Transform chestSpawnPoint;
    public int chestEveryXWaves = 5;

    [Header("Wave UI")]
    public TMP_Text waveText;
    public float bigWaveTextDuration = 2f;
    public float shrinkDuration = 0.4f;
    public Vector3 bigWaveScale = new Vector3(2.5f, 2.5f, 2.5f);
    public Vector3 smallWaveScale = Vector3.one;

    private bool hasStarted = false;
    private int currentWave = 0;

    private readonly List<GameObject> aliveEnemies = new List<GameObject>();

    private void Start()
    {
        if (waveText != null)
        {
            waveText.gameObject.SetActive(false);
            waveText.transform.localScale = smallWaveScale;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasStarted) return;

        if (other.CompareTag("Player"))
        {
            hasStarted = true;

            Debug.Log("Arena started.");

            if (arenaWall != null)
                arenaWall.SetActive(true);

            if (waveText != null)
                waveText.gameObject.SetActive(true);

            StartCoroutine(StartWaves());
        }
    }

    private IEnumerator StartWaves()
    {
        while (currentWave < totalWaves)
        {
            currentWave++;

            // =========================
            // BOSS WAVE
            // =========================
            if (currentWave == bossWave)
            {
                Debug.Log("Boss wave started.");

                yield return StartCoroutine(ShowWaveText("BOSS!"));

                SpawnBoss();

                // Arena stays closed until boss dies
                yield return new WaitUntil(() => AreAllEnemiesDead());

                Debug.Log("Boss defeated.");
            }
            else
            {
                int enemiesThisWave = startingEnemiesPerWave + ((currentWave - 1) * enemyIncreasePerWave);

                Debug.Log($"Wave {currentWave} starting. Enemies: {enemiesThisWave}");

                yield return StartCoroutine(ShowWaveText($"WAVE {currentWave}"));

                for (int i = 0; i < enemiesThisWave; i++)
                {
                    SpawnEnemy();
                    yield return new WaitForSeconds(delayBetweenSpawns);
                }

                yield return new WaitUntil(() => AreAllEnemiesDead());

                Debug.Log($"Wave {currentWave} cleared.");

                if (ShouldSpawnChest())
                    SpawnChest();
            }

            if (currentWave < totalWaves)
                yield return new WaitForSeconds(delayBetweenWaves);
        }

        Debug.Log("All waves finished.");

        if (arenaWall != null)
            arenaWall.SetActive(false);

        if (waveText != null)
            waveText.gameObject.SetActive(false);
    }

    private IEnumerator ShowWaveText(string textToShow)
    {
        if (waveText == null)
            yield break;

        waveText.text = textToShow;
        waveText.gameObject.SetActive(true);
        waveText.transform.localScale = bigWaveScale;

        yield return new WaitForSeconds(bigWaveTextDuration);

        float timer = 0f;

        while (timer < shrinkDuration)
        {
            timer += Time.deltaTime;
            float t = timer / shrinkDuration;
            waveText.transform.localScale = Vector3.Lerp(bigWaveScale, smallWaveScale, t);
            yield return null;
        }

        waveText.transform.localScale = smallWaveScale;
    }

    private void SpawnEnemy()
    {
        if (enemyVariants == null || enemyVariants.Length == 0)
        {
            Debug.LogWarning("No normal enemy variants assigned.");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points assigned.");
            return;
        }

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject selectedEnemyPrefab = GetWeightedRandomEnemy();

        if (selectedEnemyPrefab == null)
        {
            Debug.LogWarning("No valid enemy prefab found.");
            return;
        }

        // Try random positions around the spawn point
        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 randomPosition = spawnPoint.position + new Vector3(randomCircle.x, 0f, randomCircle.y);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPosition, out hit, 3f, NavMesh.AllAreas))
            {
                if (IsTooCloseToOtherEnemies(hit.position))
                    continue;

                GameObject spawnedEnemy = Instantiate(selectedEnemyPrefab, hit.position, spawnPoint.rotation);
                aliveEnemies.Add(spawnedEnemy);

                Debug.Log($"Spawned enemy: {selectedEnemyPrefab.name} at {hit.position}");
                return;
            }
        }

        // Fallback: try exact spawn point
        NavMeshHit fallbackHit;
        if (NavMesh.SamplePosition(spawnPoint.position, out fallbackHit, 5f, NavMesh.AllAreas))
        {
            GameObject fallbackEnemy = Instantiate(selectedEnemyPrefab, fallbackHit.position, spawnPoint.rotation);
            aliveEnemies.Add(fallbackEnemy);

            Debug.LogWarning($"Used fallback spawn for {selectedEnemyPrefab.name} at {fallbackHit.position}");
            return;
        }

        Debug.LogWarning("Could not find a good position to spawn a normal enemy.");
    }

    private void SpawnBoss()
    {
        if (bossPrefab == null)
        {
            Debug.LogWarning("Boss prefab is missing.");
            return;
        }

        Vector3 spawnPosition;

        if (bossSpawnPoint != null)
        {
            spawnPosition = bossSpawnPoint.position;
        }
        else if (spawnPoints != null && spawnPoints.Length > 0)
        {
            spawnPosition = spawnPoints[Random.Range(0, spawnPoints.Length)].position;
        }
        else
        {
            spawnPosition = transform.position;
        }

        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawnPosition, out hit, 3f, NavMesh.AllAreas))
        {
            spawnPosition = hit.position;
        }

        GameObject spawnedBoss = Instantiate(bossPrefab, spawnPosition, Quaternion.identity);
        aliveEnemies.Add(spawnedBoss);

        BossHealth bossHealth = spawnedBoss.GetComponent<BossHealth>();
        if (bossHealth != null)
        {
            BossHealthUI bossUI = FindFirstObjectByType<BossHealthUI>();
            if (bossUI != null)
            {
                bossHealth.bossUI = bossUI;
                bossUI.Setup(bossHealth.maxHealth, bossHealth.bossName);
            }
            else
            {
                Debug.LogWarning("No BossHealthUI found in the scene.");
            }
        }
        else
        {
            Debug.LogWarning("Spawned boss has no BossHealth script.");
        }

        Debug.Log("Boss spawned at: " + spawnPosition);
    }

    private GameObject GetWeightedRandomEnemy()
    {
        int totalWeight = 0;

        foreach (EnemySpawnData enemyData in enemyVariants)
        {
            if (enemyData != null && enemyData.enemyPrefab != null && enemyData.spawnWeight > 0)
            {
                totalWeight += enemyData.spawnWeight;
            }
        }

        if (totalWeight <= 0)
            return null;

        int randomValue = Random.Range(0, totalWeight);

        foreach (EnemySpawnData enemyData in enemyVariants)
        {
            if (enemyData == null || enemyData.enemyPrefab == null || enemyData.spawnWeight <= 0)
                continue;

            if (randomValue < enemyData.spawnWeight)
                return enemyData.enemyPrefab;

            randomValue -= enemyData.spawnWeight;
        }

        return null;
    }

    private bool IsTooCloseToOtherEnemies(Vector3 spawnPosition)
    {
        aliveEnemies.RemoveAll(enemy => enemy == null);

        foreach (GameObject enemy in aliveEnemies)
        {
            if (enemy == null) continue;

            float distance = Vector3.Distance(spawnPosition, enemy.transform.position);
            if (distance < minDistanceBetweenEnemies)
                return true;
        }

        return false;
    }

    private void SpawnChest()
    {
        if (chestPrefab == null)
            return;

        Vector3 spawnPos = chestSpawnPoint != null ? chestSpawnPoint.position : transform.position;
        Instantiate(chestPrefab, spawnPos, Quaternion.identity);

        Debug.Log("Chest spawned.");
    }

    private bool ShouldSpawnChest()
    {
        if (currentWave == bossWave)
            return false;

        return chestPrefab != null &&
               chestEveryXWaves > 0 &&
               currentWave % chestEveryXWaves == 0;
    }

    private bool AreAllEnemiesDead()
    {
        aliveEnemies.RemoveAll(enemy => enemy == null);
        return aliveEnemies.Count == 0;
    }
}