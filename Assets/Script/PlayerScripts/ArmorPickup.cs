using UnityEngine;

public class ArmorPickup : MonoBehaviour
{
    [Header("Armor Data")]
    public ArmorItemData armorData;

    [Header("UI")]
    public string interactMessage = "Press E to Equip Armor";

    public string GetDescription()
    {
        if (armorData == null)
            return "No armor data assigned.";

        string text = armorData.description;

        if (armorData.hpBonus != 0)
            text += "\nHP: " + FormatStat(armorData.hpBonus);

        if (armorData.armorBonus != 0)
            text += "\nArmor: " + FormatStat(armorData.armorBonus);

        if (armorData.physicalDefenseBonus != 0)
            text += "\nPhysical Resistance: " + FormatStat(armorData.physicalDefenseBonus);

        if (armorData.attackSpeedBonus != 0)
            text += "\nAttack Speed: " + FormatStat(armorData.attackSpeedBonus);

        if (armorData.movementSpeedBonus != 0)
            text += "\nMovement Speed: " + FormatStat(armorData.movementSpeedBonus);

        return text;
    }

    private string FormatStat(float value)
    {
        return value > 0 ? "+" + value : value.ToString();
    }
}