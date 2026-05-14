using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum BlacksmithSlot
{
    Weapon,
    Head,
    Body,
    Boots
}

public class BlacksmithUpgrade : MonoBehaviour
{
    [Header("References")]
    public PlayerWeaponPickup weaponPickup;
    public PlayerArmorEquipment armorEquipment;

    [Header("Main UI")]
    public GameObject blacksmithPanel;
    public TMP_Text goldText;
    public TMP_Text messageText;

    [Header("Item Display")]
    public TMP_Text itemNameText;
    public TMP_Text itemDescriptionText;
    public TMP_Text currentStatText;
    public TMP_Text upgradeStatText;
    public Image itemIconImage;

    [Header("Buttons")]
    public Button WeaponButton;
    public Button headButton;
    public Button bodyButton;
    public Button bootsButton;
    public Button upgradeButton;
    public Button backButton;

    [Header("Upgrade Cost")]
    public int commonCost = 100;
    public int uncommonCost = 250;
    public int rareCost = 500;
    public int epicCost = 1000;

    [Header("Upgrade Bonus")]
    public int weaponDamageIncrease = 5;
    public float armorHPIncrease = 5;
    public float armorArmorIncrease = 2;
    public float armorDefenseIncrease = 2;

    private BlacksmithSlot selectedSlot = BlacksmithSlot.Weapon;

    private void Start()
    {
        if (blacksmithPanel != null)
            blacksmithPanel.SetActive(false);

        SetupButtons();
        SelectWeapon();
    }

    private void SetupButtons()
    {
        if (WeaponButton != null)
            WeaponButton.onClick.AddListener(SelectWeapon);

        if (headButton != null)
            headButton.onClick.AddListener(SelectHead);

        if (bodyButton != null)
            bodyButton.onClick.AddListener(SelectBody);

        if (bootsButton != null)
            bootsButton.onClick.AddListener(SelectBoots);

        if (upgradeButton != null)
            upgradeButton.onClick.AddListener(UpgradeSelectedItem);

        if (backButton != null)
            backButton.onClick.AddListener(CloseBlacksmith);
    }

    public void OpenBlacksmith()
    {
        if (blacksmithPanel != null)
            blacksmithPanel.SetActive(true);

        SelectWeapon();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    public void CloseBlacksmith()
    {
        if (blacksmithPanel != null)
            blacksmithPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }

    public void SelectWeapon()
    {
        selectedSlot = BlacksmithSlot.Weapon;
        RefreshUI();
    }

    public void SelectHead()
    {
        selectedSlot = BlacksmithSlot.Head;
        RefreshUI();
    }

    public void SelectBody()
    {
        selectedSlot = BlacksmithSlot.Body;
        RefreshUI();
    }

    public void SelectBoots()
    {
        selectedSlot = BlacksmithSlot.Boots;
        RefreshUI();
    }

    public void UpgradeSelectedItem()
    {
        if (selectedSlot == BlacksmithSlot.Weapon)
        {
            UpgradeWeapon();
            return;
        }

        if (armorEquipment == null)
        {
            ShowMessage("Armor system missing.");
            return;
        }

        if (selectedSlot == BlacksmithSlot.Head)
            UpgradeArmor(armorEquipment.GetHeadArmor(), "Head Armor");

        if (selectedSlot == BlacksmithSlot.Body)
            UpgradeArmor(armorEquipment.GetBodyArmor(), "Body Armor");

        if (selectedSlot == BlacksmithSlot.Boots)
            UpgradeArmor(armorEquipment.GetBootsArmor(), "Boots Armor");
    }

    private void UpgradeWeapon()
    {
        if (weaponPickup == null)
        {
            ShowMessage("Weapon system missing.");
            return;
        }

        ItemDropData weaponData = weaponPickup.GetCurrentItemData();

        if (weaponData == null)
        {
            ShowMessage("No weapon equipped.");
            return;
        }

        LootRarity currentRarity = weaponPickup.GetCurrentWeaponRarity();

        if (currentRarity == LootRarity.Legendary)
        {
            ShowMessage("Weapon is already Legendary.");
            return;
        }

        int cost = GetUpgradeCost(currentRarity);

        if (GoldManager.instance == null || !GoldManager.instance.SpendGold(cost))
        {
            ShowMessage("Not enough gold.");
            return;
        }

        weaponPickup.UpgradeEquippedWeapon();

        ShowMessage("Weapon upgraded to " + weaponPickup.GetCurrentWeaponRarity() + "!");
        RefreshUI();
    }

    private void UpgradeArmor(ArmorItemData armor, string armorName)
    {
        if (armor == null)
        {
            ShowMessage(armorName + " slot is empty.");
            return;
        }

        if (armor.rarity == LootRarity.Legendary)
        {
            ShowMessage(armorName + " is already Legendary.");
            return;
        }

        int cost = GetUpgradeCost(armor.rarity);

        if (GoldManager.instance == null || !GoldManager.instance.SpendGold(cost))
        {
            ShowMessage("Not enough gold.");
            return;
        }

        armorEquipment.UpgradeArmor(armor);

        ShowMessage(armorName + " upgraded!");
        RefreshUI();
    }

    private void RefreshUI()
    {
        RefreshGoldText();

        if (messageText != null)
            messageText.text = "";

        if (selectedSlot == BlacksmithSlot.Weapon)
            ShowWeaponInfo();

        if (selectedSlot == BlacksmithSlot.Head)
            ShowArmorInfo(armorEquipment != null ? armorEquipment.GetHeadArmor() : null, "Head");

        if (selectedSlot == BlacksmithSlot.Body)
            ShowArmorInfo(armorEquipment != null ? armorEquipment.GetBodyArmor() : null, "Body");

        if (selectedSlot == BlacksmithSlot.Boots)
            ShowArmorInfo(armorEquipment != null ? armorEquipment.GetBootsArmor() : null, "Boots");
    }

    private void ShowWeaponInfo()
    {
        if (weaponPickup == null)
        {
            SetEmptyUI("Sword");
            return;
        }

        ItemDropData weaponData = weaponPickup.GetCurrentItemData();

        if (weaponData == null)
        {
            SetEmptyUI("Sword");
            return;
        }

        LootRarity currentRarity = weaponPickup.GetCurrentWeaponRarity();

        int currentDamage = weaponPickup.GetCurrentWeaponDamageBonus();
        int upgradedDamage = currentDamage + weaponDamageIncrease;

        if (itemNameText != null)
            itemNameText.text = weaponData.itemName + " [" + currentRarity + "]";

        if (itemDescriptionText != null)
            itemDescriptionText.text = weaponData.description;

        if (currentStatText != null)
            currentStatText.text = "Current Stat\nDamage: " + currentDamage;

        if (upgradeStatText != null)
        {
            if (currentRarity == LootRarity.Legendary)
            {
                upgradeStatText.text = "Upgrade Stat\nMax rarity";
            }
            else
            {
                upgradeStatText.text =
                    "Upgrade Stat\nDamage: " + upgradedDamage +
                    "\nNext Rarity: " + GetNextRarityName(currentRarity) +
                    "\nCost: " + GetUpgradeCost(currentRarity) + " Gold";
            }
        }

        if (itemIconImage != null)
        {
            itemIconImage.sprite = weaponData.itemIcon;
            itemIconImage.enabled = weaponData.itemIcon != null;
            itemIconImage.preserveAspect = true;
        }

        if (upgradeButton != null)
            upgradeButton.interactable = currentRarity != LootRarity.Legendary;
    }

    private void ShowArmorInfo(ArmorItemData armor, string slotName)
    {
        if (armor == null)
        {
            SetEmptyUI(slotName);
            return;
        }

        if (itemNameText != null)
            itemNameText.text = armor.armorName + " [" + armor.rarity + "]";

        if (itemDescriptionText != null)
            itemDescriptionText.text = armor.description;

        if (currentStatText != null)
        {
            currentStatText.text =
                "Current Stat\n" +
                "HP: " + armor.hpBonus +
                "\nArmor: " + armor.armorBonus +
                "\nDefense: " + armor.physicalDefenseBonus;
        }

        if (upgradeStatText != null)
        {
            if (armor.rarity == LootRarity.Legendary)
            {
                upgradeStatText.text = "Upgrade Stat\nMax rarity";
            }
            else
            {
                upgradeStatText.text =
                    "Upgrade Stat\n" +
                    "HP: " + (armor.hpBonus + armorHPIncrease) +
                    "\nArmor: " + (armor.armorBonus + armorArmorIncrease) +
                    "\nDefense: " + (armor.physicalDefenseBonus + armorDefenseIncrease) +
                    "\nCost: " + GetUpgradeCost(armor.rarity) + " Gold";
            }
        }

        if (itemIconImage != null)
        {
            itemIconImage.sprite = armor.armorIcon;
            itemIconImage.enabled = armor.armorIcon != null;
            itemIconImage.preserveAspect = true;
        }

        if (upgradeButton != null)
            upgradeButton.interactable = armor.rarity != LootRarity.Legendary;
    }

    private void SetEmptyUI(string slotName)
    {
        if (itemNameText != null)
            itemNameText.text = slotName + ": Empty";

        if (itemDescriptionText != null)
            itemDescriptionText.text = "No item equipped.";

        if (currentStatText != null)
            currentStatText.text = "Current Stat\nNone";

        if (upgradeStatText != null)
            upgradeStatText.text = "Upgrade Stat\nNone";

        if (itemIconImage != null)
        {
            itemIconImage.sprite = null;
            itemIconImage.enabled = false;
        }

        if (upgradeButton != null)
            upgradeButton.interactable = false;
    }

    private void RefreshGoldText()
    {
        if (goldText == null)
            return;

        if (GoldManager.instance != null)
            goldText.text = GoldManager.instance.currentGold.ToString();
        else
            goldText.text = "0";
    }

    private int GetUpgradeCost(LootRarity rarity)
    {
        switch (rarity)
        {
            case LootRarity.Common:
                return commonCost;

            case LootRarity.Uncommon:
                return uncommonCost;

            case LootRarity.Rare:
                return rareCost;

            case LootRarity.Epic:
                return epicCost;

            default:
                return 0;
        }
    }

    private void ShowMessage(string message)
    {
        if (messageText != null)
            messageText.text = message;

        Debug.Log(message);
    }

    private string GetNextRarityName(LootRarity rarity)
    {
        switch (rarity)
        {
            case LootRarity.Common:
                return "Uncommon";

            case LootRarity.Uncommon:
                return "Rare";

            case LootRarity.Rare:
                return "Epic";

            case LootRarity.Epic:
                return "Legendary";

            default:
                return "Max";
        }
    }
}
