using UnityEngine;

[CreateAssetMenu(fileName = "New Armor", menuName = "Armor/Armor Item Data")]
public class ArmorItemData : ScriptableObject
{
    [Header("Basic Info")]
    public string armorName;

    [TextArea]
    public string description;

    public Sprite armorIcon;
    public ArmorSlot armorSlot;

    [Header("Stats")]
    public float hpBonus;
    public float armorBonus;
    public float movementSpeedBonus;
    public float attackSpeedBonus;
    public float physicalDefenseBonus;
}