using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCardUI : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public Button button;

    private LevelUpManager levelUpManager;
    private UpgradeChoice currentChoice;

    public void Setup(LevelUpManager manager, UpgradeChoice choice)
    {
        levelUpManager = manager;
        currentChoice = choice;

        if (titleText != null)
            titleText.text = choice.title;

        if (descriptionText != null)
            descriptionText.text = choice.description;

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnCardClicked);
        }
    }

    void OnCardClicked()
    {
        if (levelUpManager != null && currentChoice != null)
        {
            levelUpManager.SelectUpgrade(currentChoice);
        }
    }
}