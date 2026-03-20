using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Image fadeImage; // assign your black panel Image here
    [SerializeField] private float fadeDuration = 1f;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        Time.timeScale = 1f; // ensure game runs normally

        // hide fade at start
        if (fadeImage != null)
            fadeImage.color = new Color(0, 0, 0, 0);
    }

    public void GameOver()
    {
        Time.timeScale = 1f;
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
        StartCoroutine(FadeAndLoad("Menu")); // replace with your menu scene name
    }

    private IEnumerator FadeAndLoad(string sceneName)
    {
        // resume time in case game was paused
        Time.timeScale = 1f;

        // fade to black
        if (fadeImage != null)
        {
            float elapsed = 0f;
            Color c = fadeImage.color;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime; // use unscaled time because timeScale = 0
                c.a = Mathf.Clamp01(elapsed / fadeDuration);
                fadeImage.color = c;
                yield return null;
            }
        }

        // load scene after fade
        SceneManager.LoadScene(sceneName);
    }
}