using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArenaManager : MonoBehaviour
{
    public GameObject[] exitBarriers; // Drag your barrier objects here
    public WaveManager waveManager;   // Reference to your spawning script
    public bool isArenaActive = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isArenaActive)
        {
            StartCoroutine(StartArenaBattle());
        }
    }

    IEnumerator StartArenaBattle()
    {
        isArenaActive = true;
        // 1. Activate Barriers
        foreach (GameObject barrier in exitBarriers) barrier.SetActive(true);

        // 2. Start Waves
        yield return StartCoroutine(waveManager.SpawnWaves());

        // 3. Deactivate Barriers
        foreach (GameObject barrier in exitBarriers) barrier.SetActive(false);
    }
}
    