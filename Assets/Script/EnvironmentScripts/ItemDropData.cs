using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Loot/Item Drop Data")]
public class ItemDropData : ScriptableObject
{
    [Header("Basic Info")]
    public string itemName;

    [TextArea]
    public string description;

    public Sprite itemIcon;

    [Header("3D Weapon Prefab")]
    public GameObject equippedWeaponPrefab;

    [Header("Rarity")]
    public LootRarity rarity = LootRarity.Common;

    [Header("Pickup")]
    public bool autoPickup = false;
}