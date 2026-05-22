using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;

    [Header("Death Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip deathSound;

    private bool isGameOver = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        Time.timeScale = 1f;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (fadeImage != null)
            fadeImage.color = new Color(0, 0, 0, 0);

        if (audioSource != null)
            audioSource.playOnAwake = false;
    }

    public void GameOver()
    {
        if (isGameOver) return;

        isGameOver = true;
        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        Time.timeScale = 1f;

        if (audioSource != null && deathSound != null)
            audioSource.PlayOneShot(deathSound);

        yield return StartCoroutine(FadeToBlack());

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartGame()
    {
        StartCoroutine(FadeAndLoad(SceneManager.GetActiveScene().name));
    }

    public void GoToMenu()
    {
        StartCoroutine(FadeAndLoad("Menu"));
    }

    private IEnumerator FadeToBlack()
    {
        if (fadeImage == null)
            yield break;

        float elapsed = 0f;
        Color c = fadeImage.color;
        c.a = 0f;
        fadeImage.color = c;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            c.a = Mathf.Clamp01(elapsed / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = 1f;
        fadeImage.color = c;
    }

    private IEnumerator FadeAndLoad(string sceneName)
    {
        Time.timeScale = 1f;

        yield return StartCoroutine(FadeToBlack());

        SceneManager.LoadScene(sceneName);
    }
}