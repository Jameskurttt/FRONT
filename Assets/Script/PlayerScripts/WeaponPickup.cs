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
    public TMP_Text equippedWeaponNameText;
    public Image equippedWeaponSlotImage;
    public Sprite emptySlotSprite;

    private Weapon currentWeapon;
    private ItemDropData currentItemData;

    private Weapon targetWeapon;
    private ChestInteractable targetChest;
    private WorldLootDrop targetLootDrop;

    private void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        HidePickupUI();
        RefreshEquippedUI();
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
                    pickupDescriptionText.text = lootDrop.itemData.description;

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
                    pickupDescriptionText.text = weapon.description;

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
        currentItemData = null; // this old system pickup has no ItemDropData linked

        Transform holderToUse = GetHolderForWeapon(currentWeapon);
        if (holderToUse == null)
        {
            Debug.LogWarning("No holder assigned for weapon type: " + currentWeapon.weaponType);
            return;
        }

        currentWeapon.Pickup(holderToUse);

        targetWeapon = null;

        HidePickupUI();
        RefreshEquippedUI();
    }

    public void EquipFromLoot(ItemDropData itemData)
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
        // DO NOT force localScale = Vector3.one
        // Keep prefab's original scale

        currentWeapon = spawnedWeaponObject.GetComponent<Weapon>();

        if (currentWeapon == null)
        {
            Debug.LogWarning("Spawned weapon does not contain a Weapon component.");
            Destroy(spawnedWeaponObject);
            return;
        }

        currentItemData = itemData;

        currentWeapon.Pickup(holderToUse);

        HidePickupUI();
        RefreshEquippedUI();

        Debug.Log("Equipped from loot: " + itemData.itemName);
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
            finalDropPosition = transform.position + transform.forward * 1.5f + Vector3.up * 0.5f;

        // If this weapon came from loot data, drop the 2D loot version
        if (currentItemData != null && lootDropPrefab != null)
        {
            GameObject dropObject = Instantiate(lootDropPrefab, finalDropPosition, Quaternion.identity);

            WorldLootDrop lootDrop = dropObject.GetComponent<WorldLootDrop>();
            if (lootDrop != null)
            {
                lootDrop.Setup(currentItemData);
            }

            Destroy(currentWeapon.gameObject);
        }
        else
        {
            // Fallback for old 3D weapon pickup system
            currentWeapon.Drop(finalDropPosition);
        }

        currentWeapon = null;
        currentItemData = null;

        RefreshEquippedUI();
    }

    private void RefreshEquippedUI()
    {
        if (equippedWeaponNameText != null)
        {
            if (currentWeapon != null)
            {
                equippedWeaponNameText.text = currentWeapon.weaponName;
                equippedWeaponNameText.gameObject.SetActive(true);
            }
            else
            {
                equippedWeaponNameText.text = "";
                equippedWeaponNameText.gameObject.SetActive(false);
            }
        }

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

    public bool HasBowEquipped()
    {
        return currentWeapon != null && currentWeapon.weaponType == WeaponType.Bow;
    }
}