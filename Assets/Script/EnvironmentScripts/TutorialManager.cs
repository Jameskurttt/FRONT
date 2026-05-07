using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header("Tutorial")]
    public GameObject tutorialPanel;
    public Image tutorialImage;

    [Header("Buttons")]
    public Button nextButton;
    public Button backButton;
    public Button okayButton;

    [Header("Tutorial Pictures")]
    public Sprite[] tutorialSprites;

    [Header("Disable These While Tutorial Is Open")]
    public MonoBehaviour[] scriptsToDisable;

    private int currentIndex = 0;

    public static bool isGamePaused;

    void Start()
    {
        OpenTutorial();
    }

    void OpenTutorial()
    {
        tutorialPanel.SetActive(true);

        currentIndex = 0;

        ShowImage();
        UpdateButtons();

        // PAUSE GAME
        Time.timeScale = 0f;
        isGamePaused = true;

        // DISABLE PLAYER + CAMERA SCRIPTS
        foreach (MonoBehaviour script in scriptsToDisable)
        {
            if (script != null)
            {
                script.enabled = false;
            }
        }

        // SHOW CURSOR
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void NextTutorial()
    {
        if (currentIndex < tutorialSprites.Length - 1)
        {
            currentIndex++;
        }

        ShowImage();
        UpdateButtons();
    }

    public void PreviousTutorial()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
        }

        ShowImage();
        UpdateButtons();
    }

    public void CloseTutorial()
    {
        tutorialPanel.SetActive(false);

        // RESUME GAME
        Time.timeScale = 1f;
        isGamePaused = false;

        // ENABLE PLAYER + CAMERA AGAIN
        foreach (MonoBehaviour script in scriptsToDisable)
        {
            if (script != null)
            {
                script.enabled = true;
            }
        }

        // HIDE CURSOR
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void ShowImage()
    {
        if (tutorialSprites.Length > 0)
        {
            tutorialImage.sprite = tutorialSprites[currentIndex];
        }
    }

    void UpdateButtons()
    {
        // BACK BUTTON
        backButton.gameObject.SetActive(currentIndex > 0);

        // LAST IMAGE
        bool isLast = currentIndex == tutorialSprites.Length - 1;

        nextButton.gameObject.SetActive(!isLast);
        okayButton.gameObject.SetActive(isLast);
    }
}