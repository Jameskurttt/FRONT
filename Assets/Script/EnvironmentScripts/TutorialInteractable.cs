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

    private int currentIndex = 0;

    void Start()
    {
        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);
    }

    public void Interact()
    {
        OpenTutorial();
    }

    void OpenTutorial()
    {
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
        if (tutorialImage != null && tutorialSprites.Length > 0)
            tutorialImage.sprite = tutorialSprites[currentIndex];
    }

    void UpdateButtons()
    {
        if (backButton != null)
            backButton.gameObject.SetActive(currentIndex > 0);

        bool isLastImage = currentIndex == tutorialSprites.Length - 1;

        if (nextButton != null)
            nextButton.gameObject.SetActive(!isLastImage);

        if (okayButton != null)
            okayButton.gameObject.SetActive(isLastImage);
    }
}