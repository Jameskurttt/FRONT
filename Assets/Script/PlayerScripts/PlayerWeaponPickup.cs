using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerWeaponPickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public Transform swordHolder;
    public Transform bowHolder;
    public float pickupRange = 3f;
    public LayerMask interactLayer;
    public Camera playerCamera;

    [Header("Drop Settings")]
    public Transform dropPoint;
    public GameObject lootDropPrefab;

    [Header("Hover UI")]
    public TMP_Text interactUIText;
    public TMP_Text pickupDescriptionText;
    public GameObject pickupDescriptionPanel;

    [Header("Equipped UI")]
    public Image equippedWeaponSlotImage;
    public Sprite emptySlotSprite;

    [Header("Player Stats")]
    public PlayerHealth playerStats;

    private Weapon currentWeapon;
    private ItemDropData currentItemData;
    private int currentWeaponDamageBonus;
    private LootRarity currentWeaponRarity = LootRarity.Common;

    private Weapon targetWeapon;
    private ChestInteractable targetChest;
    private WorldLootDrop targetLootDrop;
    private ArmorPickup targetArmorPickup;

    private PlayerArmorEquipment armorEquipment;

    private void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        if (playerStats == null)
            playerStats = GetComponent<PlayerHealth>();

        armorEquipment = GetComponent<PlayerArmorEquipment>();

        HidePickupUI();
        RefreshEquippedUI();
        RefreshStatsUI();
    }

    private void Update()
    {
        CheckForInteractable();

        if (Input.GetKeyDown(KeyCode.E))
            TryInteract();

        if (Input.GetKeyDown(KeyCode.G))
            DropWeapon();
    }

    private void CheckForInteractable()
    {
        targetWeapon = null;
        targetChest = null;
        targetLootDrop = null;
        targetArmorPickup = null;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, interactLayer))
        {
            WorldLootDrop lootDrop = hit.collider.GetComponentInParent<WorldLootDrop>();

            if (lootDrop != null && lootDrop.itemData != null)
            {
                targetLootDrop = lootDrop;

                if (interactUIText != null)
                    interactUIText.text = "Press E to <color=#FFD700>Pick Up</color>";

                if (pickupDescriptionText != null)
                    pickupDescriptionText.text = GetItemDescription(lootDrop.itemData, lootDrop.GetRolledWeaponDamage());

                ShowPickupUI();
                return;
            }

            Weapon weapon = hit.collider.GetComponent<Weapon>();

            if (weapon != null)
            {
                targetWeapon = weapon;

                if (interactUIText != null)
                    interactUIText.text = "Press E to <color=#FFD700>Equip</color>";

                if (pickupDescriptionText != null)
                    pickupDescriptionText.text =
                        weapon.description +
                        "\nDamage: " + weapon.GetWeaponDamage();

                ShowPickupUI();
                return;
            }

            ArmorPickup armorPickup = hit.collider.GetComponent<ArmorPickup>();

            if (armorPickup != null && armorPickup.armorData != null)
            {
                targetArmorPickup = armorPickup;

                if (interactUIText != null)
                    interactUIText.text = armorPickup.interactMessage;

                if (pickupDescriptionText != null)
                    pickupDescriptionText.text = armorPickup.GetDescription();

                ShowPickupUI();
                return;
            }

            ChestInteractable chest = hit.collider.GetComponent<ChestInteractable>();

            if (chest != null && !chest.hasOpened)
            {
                targetChest = chest;

                if (interactUIText != null)
                    interactUIText.text = chest.interactMessage;

                if (pickupDescriptionText != null)
                    pickupDescriptionText.text = chest.chestDescription;

                ShowPickupUI();
                return;
            }
        }

        HidePickupUI();
    }

    private string GetItemDescription(ItemDropData itemData, int rolledWeaponDamage)
    {
        if (itemData == null)
            return "";

        string text = "";

        if (!string.IsNullOrEmpty(itemData.itemName))
            text += itemData.itemName + "\n";

        if (!string.IsNullOrEmpty(itemData.description))
            text += itemData.description + "\n";

        text += "Type: " + itemData.itemType + "\n";
        text += "Rarity: " + itemData.rarity;

        if (itemData.itemType == DropItemType.Weapon)
        {
            text += "\nDamage: " + rolledWeaponDamage;
        }

        if (itemData.itemType == DropItemType.Totem)
        {
            AddStatLine(ref text, "Max HP", itemData.bonusMaxHP);
            AddStatLine(ref text, "HP Regen", itemData.bonusHPRegen);
            AddStatLine(ref text, "Armor", itemData.bonusArmor);
            AddStatLine(ref text, "Physical Attack", itemData.bonusPhysicalAttack);
            AddStatLine(ref text, "Magic Attack", itemData.bonusMagicAttack);
            AddStatLine(ref text, "Attack Speed", itemData.bonusAttackSpeed);
            AddStatLine(ref text, "Movement Speed", itemData.bonusMovementSpeed);
            AddStatLine(ref text, "Physical Defense", itemData.bonusPhysicalDefense);
            AddStatLine(ref text, "Magic Defense", itemData.bonusMagicDefense);
        }

        if (itemData.itemType == DropItemType.Armor && itemData.armorData != null)
        {
            ArmorItemData armor = itemData.armorData;

            text += "\nArmor Slot: " + armor.armorSlot;

            AddStatLine(ref text, "HP", armor.hpBonus);
            AddStatLine(ref text, "Armor", armor.armorBonus);
            AddStatLine(ref text, "Movement Speed", armor.movementSpeedBonus);
            AddStatLine(ref text, "Attack Speed", armor.attackSpeedBonus);
            AddStatLine(ref text, "Physical Defense", armor.physicalDefenseBonus);
        }

        return text;
    }

    private void AddStatLine(ref string text, string statName, float value)
    {
        if (value == 0)
            return;

        string sign = value > 0 ? "+" : "";
        text += "\n" + statName + ": " + sign + value.ToString("F1");
    }

    private void TryInteract()
    {
        if (targetLootDrop != null)
        {
            targetLootDrop.TryPickupFromPlayer();
            targetLootDrop = null;
            HidePickupUI();
            return;
        }

        if (targetWeapon != null)
        {
            TryPickupWeapon();
            return;
        }

        if (targetArmorPickup != null)
        {
            if (armorEquipment != null)
            {
                armorEquipment.EquipArmor(targetArmorPickup.armorData);
                Destroy(targetArmorPickup.gameObject);
            }

            targetArmorPickup = null;
            HidePickupUI();
            return;
        }

        if (targetChest != null)
        {
            targetChest.Interact();
            targetChest = null;
            HidePickupUI();
        }
    }

    private void TryPickupWeapon()
    {
        if (targetWeapon == null)
            return;

        if (currentWeapon != null)
            DropWeapon();

        currentWeapon = targetWeapon;
        currentItemData = null;
        currentWeaponDamageBonus = currentWeapon.GetWeaponDamage();
        currentWeaponRarity = LootRarity.Common;

        Transform holderToUse = GetHolderForWeapon(currentWeapon);

        if (holderToUse == null)
            return;

        currentWeapon.Pickup(holderToUse);
        ApplyEquippedWeaponStats();

        targetWeapon = null;

        HidePickupUI();
        RefreshEquippedUI();
        RefreshStatsUI();
    }

    public void EquipFromLoot(ItemDropData itemData, int rolledWeaponDamage, LootRarity rarity)
    {
        Debug.Log("EquipFromLoot called.");

        if (itemData == null)
        {
            Debug.LogError("FAILED: itemData is null.");
            return;
        }

        if (itemData.equippedWeaponPrefab == null)
        {
            Debug.LogError("FAILED: Equipped Weapon Prefab is empty on " + itemData.itemName);
            return;
        }

        Weapon weaponPrefab = itemData.equippedWeaponPrefab.GetComponent<Weapon>();

        if (weaponPrefab == null)
        {
            Debug.LogError("FAILED: Equipped Weapon Prefab has NO Weapon script.");
            return;
        }

        Transform holderToUse = GetHolderForWeaponType(weaponPrefab.weaponType);

        if (holderToUse == null)
        {
            Debug.LogError("FAILED: Holder is missing for weapon type: " + weaponPrefab.weaponType);
            return;
        }

        Debug.Log("Spawning weapon: " + itemData.equippedWeaponPrefab.name + " into holder: " + holderToUse.name);

        if (currentWeapon != null)
            DropWeapon();

        GameObject spawnedWeaponObject = Instantiate(itemData.equippedWeaponPrefab, holderToUse);

        spawnedWeaponObject.transform.localPosition = Vector3.zero;
        spawnedWeaponObject.transform.localRotation = Quaternion.identity;
        spawnedWeaponObject.transform.localScale = Vector3.one;

        currentWeapon = spawnedWeaponObject.GetComponent<Weapon>();

        if (currentWeapon == null)
        {
            Debug.LogError("FAILED: Spawned weapon has no Weapon script.");
            Destroy(spawnedWeaponObject);
            return;
        }

        currentWeapon.SetWeaponDamage(rolledWeaponDamage);

        currentItemData = itemData;
        currentWeaponDamageBonus = rolledWeaponDamage;
        currentWeaponRarity = rarity;

        currentWeapon.Pickup(holderToUse);

        ApplyEquippedWeaponStats();

        HidePickupUI();
        RefreshEquippedUI();
        RefreshStatsUI();

        Debug.Log("SUCCESS: Weapon equipped: " + currentWeapon.name);
    }

    private void ApplyEquippedWeaponStats()
    {
        if (playerStats != null)
            playerStats.SetEquippedWeaponDamage(currentWeaponDamageBonus);
    }

    private Transform GetHolderForWeapon(Weapon weapon)
    {
        if (weapon == null)
            return null;

        return GetHolderForWeaponType(weapon.weaponType);
    }

    private Transform GetHolderForWeaponType(WeaponType weaponType)
    {
        switch (weaponType)
        {
            case WeaponType.Bow:
                return bowHolder;

            case WeaponType.Sword:
                return swordHolder;

            default:
                return swordHolder;
        }
    }

    private void DropWeapon()
    {
        if (currentWeapon == null)
            return;

        Vector3 finalDropPosition;

        if (dropPoint != null)
            finalDropPosition = dropPoint.position;
        else
            finalDropPosition =
                transform.position +
                transform.forward * 2.5f +
                transform.right * 1f +
                Vector3.up * 0.5f;

        if (currentItemData != null && lootDropPrefab != null)
        {
            GameObject dropObject = Instantiate(lootDropPrefab, finalDropPosition, Quaternion.identity);

            WorldLootDrop lootDrop = dropObject.GetComponent<WorldLootDrop>();

            if (lootDrop != null)
                lootDrop.Setup(currentItemData, currentWeaponDamageBonus, currentWeaponRarity);

            Destroy(currentWeapon.gameObject);
        }
        else
        {
            currentWeapon.Drop(finalDropPosition);
        }

        currentWeapon = null;
        currentItemData = null;
        currentWeaponDamageBonus = 0;
        currentWeaponRarity = LootRarity.Common;

        if (playerStats != null)
            playerStats.ClearEquippedWeaponDamage();

        RefreshEquippedUI();
        RefreshStatsUI();
    }

    private void RefreshEquippedUI()
    {
        if (equippedWeaponSlotImage == null)
            return;

        equippedWeaponSlotImage.preserveAspect = true;
        equippedWeaponSlotImage.color = Color.white;

        if (currentWeapon != null && currentWeapon.weaponIcon != null)
        {
            equippedWeaponSlotImage.sprite = currentWeapon.weaponIcon;
            equippedWeaponSlotImage.enabled = true;
        }
        else
        {
            if (emptySlotSprite != null)
            {
                equippedWeaponSlotImage.sprite = emptySlotSprite;
                equippedWeaponSlotImage.enabled = true;
            }
            else
            {
                equippedWeaponSlotImage.sprite = null;
                equippedWeaponSlotImage.enabled = false;
            }
        }
    }

    private void RefreshStatsUI()
    {
        SkillTreeManager skillTreeManager = FindAnyObjectByType<SkillTreeManager>();

        if (skillTreeManager != null)
            skillTreeManager.RefreshStatsUI();

        PauseMenu pauseMenu = FindAnyObjectByType<PauseMenu>();

        if (pauseMenu != null)
            pauseMenu.RefreshPauseStats();
    }

    private void ShowPickupUI()
    {
        if (interactUIText != null)
            interactUIText.gameObject.SetActive(true);

        if (pickupDescriptionText != null)
            pickupDescriptionText.gameObject.SetActive(true);

        if (pickupDescriptionPanel != null)
            pickupDescriptionPanel.SetActive(true);
    }

    private void HidePickupUI()
    {
        if (interactUIText != null)
            interactUIText.gameObject.SetActive(false);

        if (pickupDescriptionText != null)
            pickupDescriptionText.gameObject.SetActive(false);

        if (pickupDescriptionPanel != null)
            pickupDescriptionPanel.SetActive(false);
    }

    public Weapon GetCurrentWeapon()
    {
        return currentWeapon;
    }

    public ItemDropData GetCurrentItemData()
    {
        return currentItemData;
    }

    public int GetCurrentWeaponDamageBonus()
    {
        return currentWeaponDamageBonus;
    }

    public LootRarity GetCurrentWeaponRarity()
    {
        return currentWeaponRarity;
    }

    public bool HasBowEquipped()
    {
        return currentWeapon != null && currentWeapon.weaponType == WeaponType.Bow;
    }

    public void UpgradeEquippedWeapon()
    {
        if (currentItemData == null)
            return;

        if (currentWeaponRarity == LootRarity.Legendary)
            return;

        currentWeaponRarity = GetNextRarity(currentWeaponRarity);

        currentWeaponDamageBonus += 5;

        if (currentWeapon != null)
            currentWeapon.SetWeaponDamage(currentWeaponDamageBonus);

        ApplyEquippedWeaponStats();
        RefreshStatsUI();
    }

    private LootRarity GetNextRarity(LootRarity rarity)
    {
        switch (rarity)
        {
            case LootRarity.Common:
                return LootRarity.Uncommon;

            case LootRarity.Uncommon:
                return LootRarity.Rare;

            case LootRarity.Rare:
                return LootRarity.Epic;

            case LootRarity.Epic:
                return LootRarity.Legendary;

            default:
                return LootRarity.Legendary;
        }
    }
}