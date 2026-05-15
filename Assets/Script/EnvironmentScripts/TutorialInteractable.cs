using UnityEngine;
using UnityEngine.UI;

public class TutorialInteractable : MonoBehaviour
{
    [Header("Tutorial UI")]
    public GameObject tutorialPanel;
    public Image tutorialImage;

    [Header("Buttons")]
    public Button nextButton;
    public Button backButton;
    public Button okayButton;

    [Header("Tutorial Images")]
    public Sprite[] tutorialSprites;

    [Header("Start Settings")]
    public bool showOnStart = true;

    private int currentIndex = 0;
    private bool tutorialOpen = false;

    void Start()
    {
        if (showOnStart)
        {
            OpenTutorial();
        }
        else
        {
            if (tutorialPanel != null)
                tutorialPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (tutorialOpen)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void Interact()
    {
        OpenTutorial();
    }

    public void OpenTutorial()
    {
        tutorialOpen = true;

        if (tutorialPanel != null)
            tutorialPanel.SetActive(true);

        currentIndex = 0;

        ShowImage();
        UpdateButtons();

        FreezeGame();
    }

    public void NextTutorial()
    {
        if (currentIndex < tutorialSprites.Length - 1)
            currentIndex++;

        ShowImage();
        UpdateButtons();
    }

    public void PreviousTutorial()
    {
        if (currentIndex > 0)
            currentIndex--;

        ShowImage();
        UpdateButtons();
    }

    public void CloseTutorial()
    {
        tutorialOpen = false;

        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);

        UnfreezeGame();
    }

    void FreezeGame()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void UnfreezeGame()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void ShowImage()
    {
        if (tutorialImage != null && tutorialSprites != null && tutorialSprites.Length > 0)
            tutorialImage.sprite = tutorialSprites[currentIndex];
    }

    void UpdateButtons()
    {
        bool hasImages = tutorialSprites != null && tutorialSprites.Length > 0;
        bool isFirstImage = currentIndex == 0;
        bool isLastImage = !hasImages || currentIndex == tutorialSprites.Length - 1;

        if (backButton != null)
            backButton.gameObject.SetActive(!isFirstImage && hasImages);

        if (nextButton != null)
            nextButton.gameObject.SetActive(!isLastImage && hasImages);

        if (okayButton != null)
            okayButton.gameObject.SetActive(isLastImage);
    }
}