using UnityEngine;

public enum ShopItemType
{
    Potion,
    Armor
}

public enum PotionEffectType
{
    Heal,
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

[CreateAssetMenu(fileName = "New Shop Item", menuName = "Shop/Shop Item Data")]
public class ShopItemData : ScriptableObject
{
    [Header("Basic Info")]
    public string itemName;
    [TextArea] public string description;
    public Sprite itemIcon;
    public int price = 50;

    [Header("Item Type")]
    public ShopItemType itemType;

    [Header("Potion Settings")]
    public PotionEffectType potionEffect;
    public float potionValue = 10f;

    [Header("Armor Settings")]
    public ArmorItemData armorData;
}