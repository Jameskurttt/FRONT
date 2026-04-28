using UnityEngine;

public enum Lootrarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

public enum DropItemType
{
    Weapon,
    Totem
}

[CreateAssetMenu(fileName = "New Item Drop", menuName = "Loot/Item Drop Data")]
public class ItemDropData : ScriptableObject
{
    [Header("Basic Info")]
    public string itemName;
    public Sprite itemIcon;

    [TextArea]
    public string description;

    [Header("Item Type")]
    public DropItemType itemType = DropItemType.Weapon;

    [Header("Rarity")]
    public LootRarity rarity = LootRarity.Common;

    [Header("Pickup")]
    public bool autoPickup = true;

    [Header("Weapon Settings")]
    public GameObject equippedWeaponPrefab;
    public int minWeaponDamage = 5;
    public int maxWeaponDamage = 10;

    [Header("Totem Bonus Stats")]
    public float bonusMaxHP;
    public float bonusHPRegen;
    public float bonusArmor;
    public float bonusPhysicalAttack;
    public float bonusMagicAttack;
    public float bonusAttackSpeed;
    public float bonusMovementSpeed;
    public float bonusPhysicalDefense;
    public float bonusMagicDefense;
}