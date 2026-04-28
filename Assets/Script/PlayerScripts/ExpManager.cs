using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExpManager : MonoBehaviour
{
    [Header("Level")]
    public int level = 1;
    public int currentExp = 0;
    public int expToLevel = 10;
    public float expGrowthMultiplier = 1.2f;

    [Header("Level Up Cards")]
    public LevelUpManager levelUpManager;

    [Header("UI")]
    public Slider expSlider;
    public TMP_Text currentLevelText;

    private void Start()
    {
        UpdateUI();
        SetCursorState(false);
    }

    private void OnEnable()
    {
        EnemyHealth.OnMonsterDefeated += GainExperience;
    }

    private void OnDisable()
    {
        EnemyHealth.OnMonsterDefeated -= GainExperience;
    }

    public void GainExperience(int amount)
    {
        currentExp += amount;
        Debug.Log("Gained EXP: " + amount);

        while (currentExp >= expToLevel)
        {
            LevelUp();
        }

        UpdateUI();
    }

    public void LevelUp()
    {
        level++;
        currentExp -= expToLevel;
        expToLevel = Mathf.RoundToInt(expToLevel * expGrowthMultiplier);

        Debug.Log("LEVEL UP!");
        Debug.Log("Current Level: " + level);

        UpdateUI();

        if (levelUpManager != null)
        {
            levelUpManager.QueueLevelUpChoice();
        }
    }

    public void SetCursorState(bool isUIOpen)
    {
        if (isUIOpen)
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

    public void UpdateUI()
    {
        if (expSlider != null)
        {
            expSlider.maxValue = expToLevel;
            expSlider.value = currentExp;
        }

        if (currentLevelText != null)
        {
            currentLevelText.text = "Level: " + level;
        }
    }
}