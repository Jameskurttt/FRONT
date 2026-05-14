using UnityEngine;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    [Header("Main Panels")]
    public GameObject pauseMenuCanvas;
    public GameObject settingsPanel;
    public GameObject quitConfirmPanel;

    [Header("Pause Stats UI")]
    public TMP_Text pauseStatsText;
    public PlayerHealth playerStats;

    [Header("Cursor")]
    public bool showCursorWhenPaused = true;

    [Header("Pause Sounds")]
    public AudioSource uiAudioSource;
    public AudioClip openSound;
    public AudioClip closeSound;

    private bool isPaused = false;

    void Start()
    {
        if (uiAudioSource != null)
        {
            uiAudioSource.playOnAwake = false;
            uiAudioSource.ignoreListenerPause = true;
        }

        ResumeGame(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandlePauseInput();
        }
    }

    void HandlePauseInput()
    {
        if (!isPaused)
        {
            PauseGame();
            return;
        }

        if (quitConfirmPanel != null && quitConfirmPanel.activeSelf)
        {
            CloseQuitConfirmation();
            return;
        }

        if (settingsPanel != null && settingsPanel.activeSelf)
        {
            CloseSettings();
            return;
        }

        ResumeGame();
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        PlaySound(openSound);

        if (pauseMenuCanvas != null)
            pauseMenuCanvas.SetActive(true);

      

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (quitConfirmPanel != null)
            quitConfirmPanel.SetActive(false);

        RefreshPauseStats();
        SetCursorState(true);
    }

    public void ResumeGame()
    {
        ResumeGame(true);
    }

    void ResumeGame(bool playCloseSound)
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (playCloseSound)
            PlaySound(closeSound);

        if (pauseMenuCanvas != null)
            pauseMenuCanvas.SetActive(false);

        SetCursorState(false);
    }

    void PlaySound(AudioClip clip)
    {
        if (uiAudioSource != null && clip != null)
        {
            uiAudioSource.PlayOneShot(clip);
        }
    }

    public void OpenSettings()
    {
        
        if (settingsPanel != null)
            settingsPanel.SetActive(true);

        if (quitConfirmPanel != null)
            quitConfirmPanel.SetActive(false);

        RefreshPauseStats();
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

       
    }

    public void OpenQuitConfirmation()
    {
        

        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (quitConfirmPanel != null)
            quitConfirmPanel.SetActive(true);
    }

    public void CloseQuitConfirmation()
    {
        if (quitConfirmPanel != null)
            quitConfirmPanel.SetActive(false);

        
    }

    public void RefreshPauseStats()
    {
        if (playerStats == null || pauseStatsText == null)
            return;

        float basePhysicalAttack = playerStats.GetPhysicalAttack();
        int weaponDamage = playerStats.GetEquippedWeaponDamage();
        float totalPhysicalAttack = playerStats.GetTotalPhysicalAttack();

        pauseStatsText.text =
            "<size=150%><b>Player Stats</b></size>\n\n" +
            $"HP: {Mathf.RoundToInt(playerStats.GetCurrentHP())} / {Mathf.RoundToInt(playerStats.GetMaxHP())}\n\n" +
            $"HP Regen: {playerStats.GetHPRegen():F1}\n\n" +
            $"Armor: {playerStats.GetArmor():F1}\n\n" +
            $"Physical Attack: {totalPhysicalAttack:F1} ({weaponDamage})\n\n" +
            $"Base Physical Attack: {basePhysicalAttack:F1}\n\n" +
            $"Magic Attack: {playerStats.GetMagicAttack():F1}\n\n" +
            $"Attack Speed: {playerStats.GetAttackSpeed():F2}\n\n" +
            $"Movement Speed: {playerStats.GetMovementSpeed():F1}\n\n" +
            $"Physical Defense: {playerStats.GetPhysicalDefense():F1}\n\n" +
            $"Magic Defense: {playerStats.GetMagicDefense():F1}";
    }

    void SetCursorState(bool paused)
    {
        if (paused && showCursorWhenPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}