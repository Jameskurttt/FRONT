using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using TMPro;
public class ExpManager : MonoBehaviour
{
    public int level;   
    public int currentExp;  
    public int expToLevel = 10;
    public float expGrowthMultiplier = 1.2f; // Multiplier for increasing exp needed for next levels
    public Slider expSlider;
    public TMP_Text currentLevelText;

    private void Start()
    {
        UpdateUI();
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
        if (currentExp >= expToLevel)
        {
            LevelUp();
        }
        UpdateUI();
    }
    public void LevelUp()
    {
        level++;
        currentExp -= expToLevel; // Subtract the exp needed for the level up
        expToLevel = Mathf.RoundToInt(expToLevel * expGrowthMultiplier); // Increase the exp needed for the next level
        Debug.Log($"Leveled Up! Current Level: {level}, Exp to Level: {expToLevel}");
    }
    public void UpdateUI()
    {
        expSlider.maxValue = expToLevel;
        expSlider.value = currentExp;  
        currentLevelText.text = "Level: " + level;
    }
}
