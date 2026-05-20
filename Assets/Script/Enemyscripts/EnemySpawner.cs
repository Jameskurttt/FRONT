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
        [Range(1, 100)] public int spawnWeight = 10;
    }

    [Header("Normal Enemy Variants")]
    public EnemySpawnData[] enemyVariants;
    public Transform[] spawnPoints;

    [Header("Boss Settings")]
    public GameObject bossPrefab;
    public Transform bossSpawnPoint;
    public int bossWave = 25;
    public float bossSoundDelay = 1.5f;
    public float bossMusicFadeInDuration = 2.5f;

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

    [Header("Arena Sounds")]
    public AudioSource audioSource;
    public AudioClip arenaEnterSound;
    public float arenaEnterSoundDelay = 0.5f;
    public float arenaEnterFadeInDuration = 2.0f;
    public AudioClip waveStartSound;
    public AudioClip bossWaveSound;

    [Header("Loop Gap Tuning Fix")]
    public float loopCutoffTime = 0.533f;

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

    [Header("Grinding Cooldown")]
    public float zoneResetCooldown = 30f;

    private bool hasStarted = false;
    private bool isCooldownActive = false;
    private bool playerInsideZone = false;
    private int currentWave = 0;
    private float originalAudioVolume = 1f;
    private Coroutine activeLoopCoroutine;
    private bool shouldLoopCurrentTrack = false;
    private List<GameObject> aliveEnemies = new List<GameObject>();

    private void Start()
    {
        if (waveText != null)
        {
            waveText.gameObject.SetActive(false);
            waveText.transform.localScale = smallWaveScale;
        }

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            originalAudioVolume = audioSource.volume;
        }
    }

    private void Update()
    {
        // Automatically restarts the arena if the player is still inside when the cooldown ends
        if (!hasStarted && !isCooldownActive && playerInsideZone)
        {
            ActivateArena();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideZone = true;

            if (!hasStarted && !isCooldownActive)
            {
                ActivateArena();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInsideZone = false;
        }
    }

    private void ActivateArena()
    {
        hasStarted = true;

        if (arenaWall != null) arenaWall.SetActive(true);
        if (waveText != null) waveText.gameObject.SetActive(true);

        StartCoroutine(StartWaves());
    }

    private IEnumerator StartWaves()
    {
        if (GameAudioManager.Instance != null)
        {
            GameAudioManager.Instance.StopBackgroundMusic();
        }

        if (audioSource != null && arenaEnterSound != null)
        {
            if (arenaEnterSoundDelay > 0f) yield return new WaitForSeconds(arenaEnterSoundDelay);

            audioSource.loop = false;
            shouldLoopCurrentTrack = true;
            activeLoopCoroutine = StartCoroutine(ScheduleSeamlessLoop(arenaEnterSound, arenaEnterFadeInDuration));
        }

        while (currentWave < totalWaves)
        {
            currentWave++;

            if (currentWave == bossWave)
            {
                StopCurrentLoop();
                yield return StartCoroutine(ShowWaveText("BOSS!"));

                if (bossSoundDelay > 0f) yield return new WaitForSeconds(bossSoundDelay);

                if (audioSource != null && bossWaveSound != null)
                {
                    audioSource.loop = false;
                    shouldLoopCurrentTrack = true;
                    activeLoopCoroutine = StartCoroutine(ScheduleSeamlessLoop(bossWaveSound, bossMusicFadeInDuration));
                }

                SpawnBoss();
                yield return new WaitUntil(() => AreAllEnemiesDead());
                StopCurrentLoop();
            }
            else
            {
                int enemiesThisWave = startingEnemiesPerWave + ((currentWave - 1) * enemyIncreasePerWave);

                if (audioSource != null && waveStartSound != null)
                {
                    audioSource.PlayOneShot(waveStartSound, originalAudioVolume);
                }

                yield return StartCoroutine(ShowWaveText($"WAVE {currentWave}"));

                for (int i = 0; i < enemiesThisWave; i++)
                {
                    SpawnEnemy();
                    yield return new WaitForSeconds(delayBetweenSpawns);
                }

                yield return new WaitUntil(() => AreAllEnemiesDead());

                if (ShouldSpawnChest()) SpawnChest();
            }

            if (currentWave < totalWaves) yield return new WaitForSeconds(delayBetweenWaves);
        }

        StopCurrentLoop();

        if (GameAudioManager.Instance != null)
        {
            GameAudioManager.Instance.ResumeBackgroundMusic();
        }

        if (arenaWall != null) arenaWall.SetActive(false);
        if (waveText != null) waveText.gameObject.SetActive(false);

        // Start grinding cooldown loop
        isCooldownActive = true;
        Debug.Log($"Arena cleared. Cooldown started: {zoneResetCooldown} seconds.");

        yield return new WaitForSeconds(zoneResetCooldown);

        isCooldownActive = false;
        hasStarted = false;
        currentWave = 0;
        Debug.Log("Arena reset. Ready to run again.");
    }

    private IEnumerator ScheduleSeamlessLoop(AudioClip clipToPlay, float fadeDuration)
    {
        audioSource.clip = clipToPlay;
        double nextStartTime = AudioSettings.dspTime;
        bool isFirstPlay = true;

        while (shouldLoopCurrentTrack)
        {
            audioSource.PlayScheduled(nextStartTime);

            if (isFirstPlay)
            {
                isFirstPlay = false;
                if (fadeDuration > 0f) StartCoroutine(FadeInVolumeWorker(fadeDuration));
                else audioSource.volume = originalAudioVolume;
            }

            double clipDuration = (double)clipToPlay.samples / clipToPlay.frequency;
            double calculatedDuration = clipDuration - (double)loopCutoffTime;

            if (calculatedDuration < 0.1) calculatedDuration = clipDuration;
            nextStartTime += calculatedDuration;

            yield return new WaitUntil(() => AudioSettings.dspTime >= nextStartTime - 0.1f);
        }
    }

    private IEnumerator FadeInVolumeWorker(float duration)
    {
        audioSource.volume = 0f;
        float currentTime = 0f;

        while (currentTime < duration && shouldLoopCurrentTrack)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0f, originalAudioVolume, currentTime / duration);
            yield return null;
        }

        if (shouldLoopCurrentTrack) audioSource.volume = originalAudioVolume;
    }

    private void StopCurrentLoop()
    {
        shouldLoopCurrentTrack = false;
        if (activeLoopCoroutine != null)
        {
            StopCoroutine(activeLoopCoroutine);
            activeLoopCoroutine = null;
        }
        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.volume = originalAudioVolume;
        }
    }

    private IEnumerator ShowWaveText(string textToShow)
    {
        if (waveText == null) yield break;

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
        if (enemyVariants == null || enemyVariants.Length == 0 || spawnPoints == null || spawnPoints.Length == 0) return;

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject selectedEnemyPrefab = GetWeightedRandomEnemy();
        if (selectedEnemyPrefab == null) return;

        for (int i = 0; i < maxSpawnAttempts; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * spawnRadius;
            Vector3 randomPosition = spawnPoint.position + new Vector3(randomCircle.x, 0f, randomCircle.y);
            NavMeshHit hit;

            if (NavMesh.SamplePosition(randomPosition, out hit, 3f, NavMesh.AllAreas))
            {
                if (IsTooCloseToOtherEnemies(hit.position)) continue;

                GameObject spawnedEnemy = Instantiate(selectedEnemyPrefab, hit.position, spawnPoint.rotation);
                aliveEnemies.Add(spawnedEnemy);
                return;
            }
        }

        NavMeshHit fallbackHit;
        if (NavMesh.SamplePosition(spawnPoint.position, out fallbackHit, 5f, NavMesh.AllAreas))
        {
            GameObject fallbackEnemy = Instantiate(selectedEnemyPrefab, fallbackHit.position, spawnPoint.rotation);
            aliveEnemies.Add(fallbackEnemy);
        }
    }

    private void SpawnBoss()
    {
        if (bossPrefab == null) return;

        Vector3 spawnPosition = (bossSpawnPoint != null) ? bossSpawnPoint.position :
                                ((spawnPoints != null && spawnPoints.Length > 0) ? spawnPoints[Random.Range(0, spawnPoints.Length)].position : transform.position);

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
        }
    }

    private GameObject GetWeightedRandomEnemy()
    {
        int totalWeight = 0;
        foreach (EnemySpawnData enemyData in enemyVariants)
        {
            if (enemyData != null && enemyData.enemyPrefab != null && enemyData.spawnWeight > 0)
                totalWeight += enemyData.spawnWeight;
        }

        if (totalWeight <= 0) return null;
        int randomValue = Random.Range(0, totalWeight);

        foreach (EnemySpawnData enemyData in enemyVariants)
        {
            if (enemyData == null || enemyData.enemyPrefab == null || enemyData.spawnWeight <= 0) continue;
            if (randomValue < enemyData.spawnWeight) return enemyData.enemyPrefab;
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
            if (Vector3.Distance(spawnPosition, enemy.transform.position) < minDistanceBetweenEnemies) return true;
        }
        return false;
    }

    private void SpawnChest()
    {
        if (chestPrefab == null) return;
        Vector3 spawnPos = (chestSpawnPoint != null) ? chestSpawnPoint.position : transform.position;
        Instantiate(chestPrefab, spawnPos, Quaternion.identity);
    }

    private bool ShouldSpawnChest()
    {
        if (currentWave == bossWave) return false;
        return chestPrefab != null && chestEveryXWaves > 0 && currentWave % chestEveryXWaves == 0;
    }

    private bool AreAllEnemiesDead()
    {
        aliveEnemies.RemoveAll(enemy => enemy == null);
        return aliveEnemies.Count == 0;
    }
}