using System.Collections;
using System.Collections.Generic;
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

    [Header("Skill Points")]
    public int skillPoints = 0;

    [Header("UI")]
    public Slider expSlider;
    public TMP_Text currentLevelText;
    public TMP_Text skillPointsText;

    [Header("Skill Tree")]
    public GameObject skillTreePanel;

    private void Start()
    {
        UpdateUI();

        if (skillTreePanel != null)
        {
            skillTreePanel.SetActive(false);
        }

        SetCursorState(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            ToggleSkillTree();
        }
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
        skillPoints++;

        Debug.Log("LEVEL UP!");
        Debug.Log("Current Level: " + level);
        Debug.Log("Skill Points: " + skillPoints);

        UpdateUI();
    }

    public bool UseSkillPoint()
    {
        if (skillPoints <= 0)
        {
            Debug.Log("No skill points!");
            return false;
        }

        skillPoints--;
        UpdateUI();
        return true;
    }

    public void ToggleSkillTree()
    {
        if (skillTreePanel == null)
        {
            Debug.LogWarning("Skill Tree Panel is not assigned.");
            return;
        }

        bool isOpen = !skillTreePanel.activeSelf;
        skillTreePanel.SetActive(isOpen);
        SetCursorState(isOpen);
    }

    private void SetCursorState(bool isUIOpen)
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

        if (skillPointsText != null)
        {
            skillPointsText.text = "Skill Points: " + skillPoints;
        }
    }
}