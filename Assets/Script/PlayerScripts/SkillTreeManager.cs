using UnityEngine;
using TMPro;

public class SkillTreeManager : MonoBehaviour
{
    [Header("Player Reference")]
    public PlayerHealth playerStats;

    [Header("EXP Reference")]
    public ExpManager expManager;

    [Header("UI")]
    public TMP_Text statsText;

    [Header("Upgrade Values")]
    public float maxHPIncrease = 20f;
    public float hpRegenIncrease = 1f;
    public float armorIncrease = 3f;
    public float physicalAttackIncrease = 5f;
    public float magicAttackIncrease = 5f;
    public float attackSpeedIncrease = 0.15f;
    public float movementSpeedIncrease = 0.5f;
    public float physicalDefenseIncrease = 3f;
    public float magicDefenseIncrease = 3f;

    void Start()
    {
        RefreshStatsUI();
    }

    public void UpgradeMaxHP()
    {
        if (!CanUpgrade()) return;

        playerStats.IncreaseMaxHP(maxHPIncrease);
        AfterUpgrade("Max HP upgraded!");
    }

    public void UpgradeHPRegen()
    {
        if (!CanUpgrade()) return;

        playerStats.IncreaseHPRegen(hpRegenIncrease);
        AfterUpgrade("HP Regen upgraded!");
    }

    public void UpgradeArmor()
    {
        if (!CanUpgrade()) return;

        playerStats.IncreaseArmor(armorIncrease);
        AfterUpgrade("Armor upgraded!");
    }

    public void UpgradePhysicalAttack()
    {
        if (!CanUpgrade()) return;

        playerStats.IncreasePhysicalAttack(physicalAttackIncrease);
        AfterUpgrade("Physical Attack upgraded!");
    }

    public void UpgradeMagicAttack()
    {
        if (!CanUpgrade()) return;

        playerStats.IncreaseMagicAttack(magicAttackIncrease);
        AfterUpgrade("Magic Attack upgraded!");
    }

    public void UpgradeAttackSpeed()
    {
        if (!CanUpgrade()) return;

        playerStats.IncreaseAttackSpeed(attackSpeedIncrease);
        AfterUpgrade("Attack Speed upgraded!");
    }

    public void UpgradeMovementSpeed()
    {
        if (!CanUpgrade()) return;

        playerStats.IncreaseMovementSpeed(movementSpeedIncrease);
        AfterUpgrade("Movement Speed upgraded!");
    }

    public void UpgradePhysicalDefense()
    {
        if (!CanUpgrade()) return;

        playerStats.IncreasePhysicalDefense(physicalDefenseIncrease);
        AfterUpgrade("Physical Defense upgraded!");
    }

    public void UpgradeMagicDefense()
    {
        if (!CanUpgrade()) return;

        playerStats.IncreaseMagicDefense(magicDefenseIncrease);
        AfterUpgrade("Magic Defense upgraded!");
    }

    bool CanUpgrade()
    {
        if (playerStats == null)
        {
            Debug.LogWarning("PlayerHealth is not assigned in SkillTreeManager.");
            return false;
        }

        if (expManager == null)
        {
            Debug.LogWarning("ExpManager is not assigned in SkillTreeManager.");
            return false;
        }

        if (!expManager.UseSkillPoint())
        {
            Debug.Log("Not enough skill points.");
            return false;
        }

        return true;
    }

    void AfterUpgrade(string message)
    {
        Debug.Log(message);
        RefreshStatsUI();
    }

    public void RefreshStatsUI()
    {
        if (playerStats == null || statsText == null)
            return;

        statsText.text =
            "HP: " + Mathf.RoundToInt(playerStats.GetCurrentHP()) + " / " + Mathf.RoundToInt(playerStats.GetMaxHP()) + "\n" +
            "HP Regen: " + playerStats.GetHPRegen().ToString("F1") + "\n" +
            "Armor: " + playerStats.GetArmor().ToString("F1") + "\n" +
            "Physical Attack: " + playerStats.GetPhysicalAttack().ToString("F1") + "\n" +
            "Magic Attack: " + playerStats.GetMagicAttack().ToString("F1") + "\n" +
            "Attack Speed: " + playerStats.GetAttackSpeed().ToString("F2") + "\n" +
            "Movement Speed: " + playerStats.GetMovementSpeed().ToString("F1") + "\n" +
            "Physical Defense: " + playerStats.GetPhysicalDefense().ToString("F1") + "\n" +
            "Magic Defense: " + playerStats.GetMagicDefense().ToString("F1");
    }
}