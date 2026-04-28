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

        if (armorEquipment == null)
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
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickupRange, interactLayer))
        {
            WorldLootDrop lootDrop = hit.collider.GetComponentInParent<WorldLootDrop>();
            if (lootDrop != null && lootDrop.itemData != null)
            {
                targetLootDrop = lootDrop;

                if (interactUIText != null)
                    interactUIText.text = "Press E to <color=#FFD700>Pick Up</color>";

                if (pickupDescriptionText != null)
                    pickupDescriptionText.text = GetLootDropDescription(lootDrop);

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
                    pickupDescriptionText.text = weapon.description + "\nDamage: " + weapon.GetWeaponDamage();

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

    private string GetLootDropDescription(WorldLootDrop lootDrop)
    {
        if (lootDrop == null || lootDrop.itemData == null)
            return "";

        ItemDropData item = lootDrop.itemData;

        string text = "";

        text += item.itemName + "\n";
        text += "Rarity: " + item.rarity + "\n";

        if (!string.IsNullOrEmpty(item.description))
            text += item.description + "\n";

        if (item.itemType == DropItemType.Weapon)
        {
            text += "\nDamage: " + lootDrop.GetRolledWeaponDamage();
        }
        else if (item.itemType == DropItemType.Totem)
        {
            text += "\nTotem Stats:\n";

            if (item.bonusMaxHP != 0)
                text += "+" + item.bonusMaxHP + " Max HP\n";

            if (item.bonusHPRegen != 0)
                text += "+" + item.bonusHPRegen + " HP Regen\n";

            if (item.bonusArmor != 0)
                text += "+" + item.bonusArmor + " Armor\n";

            if (item.bonusPhysicalAttack != 0)
                text += "+" + item.bonusPhysicalAttack + " Physical Attack\n";

            if (item.bonusMagicAttack != 0)
                text += "+" + item.bonusMagicAttack + " Magic Attack\n";

            if (item.bonusAttackSpeed != 0)
                text += "+" + item.bonusAttackSpeed + " Attack Speed\n";

            if (item.bonusMovementSpeed != 0)
                text += "+" + item.bonusMovementSpeed + " Movement Speed\n";

            if (item.bonusPhysicalDefense != 0)
                text += "+" + item.bonusPhysicalDefense + " Physical Defense\n";

            if (item.bonusMagicDefense != 0)
                text += "+" + item.bonusMagicDefense + " Magic Defense\n";
        }

        return text;
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
            else
            {
                Debug.LogWarning("PlayerArmorEquipment is missing on the player.");
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

        Transform holderToUse = GetHolderForWeapon(currentWeapon);
        if (holderToUse == null)
        {
            Debug.LogWarning("No holder assigned for weapon type: " + currentWeapon.weaponType);
            return;
        }

        currentWeapon.Pickup(holderToUse);

        ApplyEquippedWeaponStats();

        targetWeapon = null;

        HidePickupUI();
        RefreshEquippedUI();
        RefreshStatsUI();
    }

    public void EquipFromLoot(ItemDropData itemData, int rolledWeaponDamage)
    {
        if (itemData == null)
        {
            Debug.LogWarning("EquipFromLoot failed. ItemDropData is missing.");
            return;
        }

        if (itemData.equippedWeaponPrefab == null)
        {
            Debug.LogWarning("EquipFromLoot failed. Equipped weapon prefab is missing for: " + itemData.itemName);
            return;
        }

        Weapon weaponPrefab = itemData.equippedWeaponPrefab.GetComponent<Weapon>();
        if (weaponPrefab == null)
        {
            Debug.LogWarning("EquipFromLoot failed. The equippedWeaponPrefab needs a Weapon component.");
            return;
        }

        if (currentWeapon != null)
            DropWeapon();

        Transform holderToUse = GetHolderForWeaponType(weaponPrefab.weaponType);
        if (holderToUse == null)
        {
            Debug.LogWarning("No holder assigned for weapon type: " + weaponPrefab.weaponType);
            return;
        }

        GameObject spawnedWeaponObject = Instantiate(itemData.equippedWeaponPrefab, holderToUse);
        spawnedWeaponObject.transform.localPosition = Vector3.zero;
        spawnedWeaponObject.transform.localRotation = Quaternion.identity;

        currentWeapon = spawnedWeaponObject.GetComponent<Weapon>();

        if (currentWeapon == null)
        {
            Debug.LogWarning("Spawned weapon does not contain a Weapon component.");
            Destroy(spawnedWeaponObject);
            return;
        }

        currentWeapon.SetWeaponDamage(rolledWeaponDamage);

        currentItemData = itemData;
        currentWeaponDamageBonus = rolledWeaponDamage;

        currentWeapon.Pickup(holderToUse);

        ApplyEquippedWeaponStats();

        HidePickupUI();
        RefreshEquippedUI();
        RefreshStatsUI();

        Debug.Log("Equipped from loot: " + itemData.itemName + " | Damage: " + rolledWeaponDamage);
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
        }

        return swordHolder;
    }

    private void DropWeapon()
    {
        if (currentWeapon == null)
            return;

        Vector3 finalDropPosition;

        if (dropPoint != null)
            finalDropPosition = dropPoint.position;
        else
            finalDropPosition = transform.position + transform.forward * 2.5f + transform.right * 1f + Vector3.up * 0.5f;

        if (currentItemData != null && lootDropPrefab != null)
        {
            GameObject dropObject = Instantiate(lootDropPrefab, finalDropPosition, Quaternion.identity);

            WorldLootDrop lootDrop = dropObject.GetComponent<WorldLootDrop>();
            if (lootDrop != null)
                lootDrop.Setup(currentItemData, currentWeaponDamageBonus);

            Destroy(currentWeapon.gameObject);
        }
        else
        {
            currentWeapon.Drop(finalDropPosition);
        }

        currentWeapon = null;
        currentItemData = null;
        currentWeaponDamageBonus = 0;

        if (playerStats != null)
            playerStats.ClearEquippedWeaponDamage();

        RefreshEquippedUI();
        RefreshStatsUI();
    }

    private void RefreshEquippedUI()
    {
        if (equippedWeaponSlotImage != null)
        {
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
    }

    private void RefreshStatsUI()
    {
        SkillTreeManager skillTreeManager = FindObjectOfType<SkillTreeManager>();
        if (skillTreeManager != null)
            skillTreeManager.RefreshStatsUI();

        PauseMenu pauseMenu = FindObjectOfType<PauseMenu>();
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

    public int GetCurrentWeaponDamageBonus()
    {
        return currentWeaponDamageBonus;
    }

    public bool HasBowEquipped()
    {
        return currentWeapon != null && currentWeapon.weaponType == WeaponType.Bow;
    }
}