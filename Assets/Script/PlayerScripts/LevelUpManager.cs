using System.Collections.Generic;
using UnityEngine;

public class LevelUpManager : MonoBehaviour
{
    [Header("References")]
    public SkillTreeManager skillTreeManager;
    public ExpManager expManager;

    [Header("UI")]
    public GameObject levelUpPanel;
    public UpgradeCardUI[] cardUIs;

    [Header("Reroll")]
    public int rerollsPerLevelUp = 1;

    private List<UpgradeChoice> allChoices = new List<UpgradeChoice>();

    private int pendingLevelUps = 0;
    private int currentRerollsLeft = 0;
    private bool isShowingChoices = false;

    private void Awake()
    {
        BuildUpgradeList();

        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(false);
        }
    }

    void BuildUpgradeList()
    {
        allChoices.Clear();

        allChoices.Add(new UpgradeChoice(LevelUpgradeType.MaxHP, "Vitality", "+20 Max HP"));
        allChoices.Add(new UpgradeChoice(LevelUpgradeType.HPRegen, "Recovery", "+1 HP Regen"));
        allChoices.Add(new UpgradeChoice(LevelUpgradeType.Armor, "Armor Up", "+3 Armor"));
        allChoices.Add(new UpgradeChoice(LevelUpgradeType.PhysicalAttack, "Power", "+5 Physical Attack"));
        allChoices.Add(new UpgradeChoice(LevelUpgradeType.MagicAttack, "Arcane Boost", "+5 Magic Attack"));
        allChoices.Add(new UpgradeChoice(LevelUpgradeType.AttackSpeed, "Quick Hands", "+0.15 Attack Speed"));
        allChoices.Add(new UpgradeChoice(LevelUpgradeType.MovementSpeed, "Agility", "+0.5 Movement Speed"));
        allChoices.Add(new UpgradeChoice(LevelUpgradeType.PhysicalDefense, "Guard", "+3 Physical Defense"));
        allChoices.Add(new UpgradeChoice(LevelUpgradeType.MagicDefense, "Ward", "+3 Magic Defense"));
    }

    public void QueueLevelUpChoice()
    {
        pendingLevelUps++;

        if (!isShowingChoices)
        {
            ShowNextLevelUpChoices();
        }
    }

    void ShowNextLevelUpChoices()
    {
        if (pendingLevelUps <= 0)
        {
            ClosePanelCompletely();
            return;
        }

        if (levelUpPanel == null || skillTreeManager == null || cardUIs == null || cardUIs.Length < 3)
        {
            Debug.LogWarning("LevelUpManager references are missing.");
            return;
        }

        isShowingChoices = true;
        currentRerollsLeft = rerollsPerLevelUp;

        levelUpPanel.SetActive(true);
        Time.timeScale = 0f;

        if (expManager != null)
        {
            expManager.SetCursorState(true);
        }

        DisplayRandomChoices();
    }

    void DisplayRandomChoices()
    {
        List<UpgradeChoice> randomChoices = GetRandomChoices(3);

        for (int i = 0; i < cardUIs.Length; i++)
        {
            if (i < randomChoices.Count)
            {
                cardUIs[i].gameObject.SetActive(true);
                cardUIs[i].Setup(this, randomChoices[i]);
            }
            else
            {
                cardUIs[i].gameObject.SetActive(false);
            }
        }
    }

    List<UpgradeChoice> GetRandomChoices(int amount)
    {
        List<UpgradeChoice> pool = new List<UpgradeChoice>(allChoices);
        List<UpgradeChoice> result = new List<UpgradeChoice>();

        int finalAmount = Mathf.Min(amount, pool.Count);

        for (int i = 0; i < finalAmount; i++)
        {
            int randomIndex = Random.Range(0, pool.Count);
            result.Add(pool[randomIndex]);
            pool.RemoveAt(randomIndex);
        }

        return result;
    }

    public void SelectUpgrade(UpgradeChoice choice)
    {
        if (skillTreeManager != null)
        {
            skillTreeManager.ApplyUpgrade(choice.upgradeType);
        }

        pendingLevelUps--;

        if (pendingLevelUps > 0)
        {
            ShowNextLevelUpChoices();
        }
        else
        {
            ClosePanelCompletely();
        }
    }

    public void RerollChoices()
    {
        if (!isShowingChoices)
            return;

        if (currentRerollsLeft <= 0)
        {
            Debug.Log("No rerolls left.");
            return;
        }

        currentRerollsLeft--;
        DisplayRandomChoices();
    }

    void ClosePanelCompletely()
    {
        isShowingChoices = false;

        if (levelUpPanel != null)
        {
            levelUpPanel.SetActive(false);
        }

        Time.timeScale = 1f;

        if (expManager != null)
        {
            expManager.SetCursorState(false);
        }
    }
}

[System.Serializable]
public class UpgradeChoice
{
    public LevelUpgradeType upgradeType;
    public string title;
    public string description;

    public UpgradeChoice(LevelUpgradeType type, string newTitle, string newDescription)
    {
        upgradeType = type;
        title = newTitle;
        description = newDescription;
    }
}