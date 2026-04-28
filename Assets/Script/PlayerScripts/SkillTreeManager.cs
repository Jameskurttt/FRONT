using UnityEngine;
using TMPro;

public class SkillTreeManager : MonoBehaviour
{
    [Header("Player Reference")]
    public PlayerHealth playerStats;

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

    public void ApplyUpgrade(LevelUpgradeType upgradeType)
    {
        if (playerStats == null)
        {
            Debug.LogWarning("PlayerHealth is not assigned in SkillTreeManager.");
            return;
        }

        switch (upgradeType)
        {
            case LevelUpgradeType.MaxHP:
                playerStats.IncreaseMaxHP(maxHPIncrease);
                Debug.Log("Max HP upgraded!");
                break;

            case LevelUpgradeType.HPRegen:
                playerStats.IncreaseHPRegen(hpRegenIncrease);
                Debug.Log("HP Regen upgraded!");
                break;

            case LevelUpgradeType.Armor:
                playerStats.IncreaseArmor(armorIncrease);
                Debug.Log("Armor upgraded!");
                break;

            case LevelUpgradeType.PhysicalAttack:
                playerStats.IncreasePhysicalAttack(physicalAttackIncrease);
                Debug.Log("Physical Attack upgraded!");
                break;

            case LevelUpgradeType.MagicAttack:
                playerStats.IncreaseMagicAttack(magicAttackIncrease);
                Debug.Log("Magic Attack upgraded!");
                break;

            case LevelUpgradeType.AttackSpeed:
                playerStats.IncreaseAttackSpeed(attackSpeedIncrease);
                Debug.Log("Attack Speed upgraded!");
                break;

            case LevelUpgradeType.MovementSpeed:
                playerStats.IncreaseMovementSpeed(movementSpeedIncrease);
                Debug.Log("Movement Speed upgraded!");
                break;

            case LevelUpgradeType.PhysicalDefense:
                playerStats.IncreasePhysicalDefense(physicalDefenseIncrease);
                Debug.Log("Physical Defense upgraded!");
                break;

            case LevelUpgradeType.MagicDefense:
                playerStats.IncreaseMagicDefense(magicDefenseIncrease);
                Debug.Log("Magic Defense upgraded!");
                break;
        }

        RefreshStatsUI();
    }

    public void RefreshStatsUI()
    {
        if (playerStats == null || statsText == null)
            return;

        float basePhysicalAttack = playerStats.GetPhysicalAttack();
        int weaponDamage = playerStats.GetEquippedWeaponDamage();
        float totalPhysicalAttack = playerStats.GetTotalPhysicalAttack();

        statsText.text =
            "HP: " + Mathf.RoundToInt(playerStats.GetCurrentHP()) + " / " + Mathf.RoundToInt(playerStats.GetMaxHP()) + "\n" +
            "HP Regen: " + playerStats.GetHPRegen().ToString("F1") + "\n" +
            "Armor: " + playerStats.GetArmor().ToString("F1") + "\n" +
            "Physical Attack: " + totalPhysicalAttack.ToString("F1") + " (" + weaponDamage + ")\n" +
            "Base Physical Attack: " + basePhysicalAttack.ToString("F1") + "\n" +
            "Magic Attack: " + playerStats.GetMagicAttack().ToString("F1") + "\n" +
            "Attack Speed: " + playerStats.GetAttackSpeed().ToString("F2") + "\n" +
            "Movement Speed: " + playerStats.GetMovementSpeed().ToString("F1") + "\n" +
            "Physical Defense: " + playerStats.GetPhysicalDefense().ToString("F1") + "\n" +
            "Magic Defense: " + playerStats.GetMagicDefense().ToString("F1");
    }
}

public enum LevelUpgradeType
{
    MaxHP,
    HPRegen,
    Armor,
    PhysicalAttack,
    MagicAttack,
    AttackSpeed,
    MovementSpeed,
    PhysicalDefense,
    MagicDefense
}