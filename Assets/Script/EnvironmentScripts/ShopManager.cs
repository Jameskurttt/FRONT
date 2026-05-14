using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [Header("References")]
    public PlayerHealth playerStats;
    public PlayerArmorEquipment armorEquipment;
    public GameObject shopPanel;

    [Header("Shop Items")]
    public ShopItemData[] shopItems;

    [Header("Buttons")]
    public ShopItemButton[] shopButtons;

    [Header("Message UI")]
    public TMP_Text messageText;

    private void Start()
    {
        if (shopPanel != null)
            shopPanel.SetActive(false);

        SetupShopButtons();
    }

    private void SetupShopButtons()
    {
        for (int i = 0; i < shopButtons.Length; i++)
        {
            if (i < shopItems.Length)
            {
                shopButtons[i].gameObject.SetActive(true);
                shopButtons[i].Setup(shopItems[i], this);
            }
            else
            {
                shopButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void OpenShop()
    {
        if (shopPanel != null)
            shopPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;

        ShowMessage("");
    }

    public void CloseShop()
    {
        if (shopPanel != null)
            shopPanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }

    public void BuyItem(ShopItemData item)
    {
        if (item == null) return;

        if (GoldManager.instance == null)
        {
            ShowMessage("Gold Manager missing.");
            return;
        }

        if (!GoldManager.instance.SpendGold(item.price))
        {
            ShowMessage("Not enough gold!");
            return;
        }

        if (item.itemType == ShopItemType.Potion)
        {
            ApplyPotion(item);
            ShowMessage("Bought " + item.itemName);
        }
        else if (item.itemType == ShopItemType.Armor)
        {
            BuyArmor(item);
        }
    }

    private void ApplyPotion(ShopItemData item)
    {
        if (playerStats == null) return;

        switch (item.potionEffect)
        {
            case PotionEffectType.Heal:
                playerStats.Heal(item.potionValue);
                break;

            case PotionEffectType.MaxHP:
                playerStats.IncreaseMaxHP(item.potionValue);
                break;

            case PotionEffectType.HPRegen:
                playerStats.IncreaseHPRegen(item.potionValue);
                break;

            case PotionEffectType.Armor:
                playerStats.IncreaseArmor(item.potionValue);
                break;

            case PotionEffectType.PhysicalAttack:
                playerStats.IncreasePhysicalAttack(item.potionValue);
                break;

            case PotionEffectType.MagicAttack:
                playerStats.IncreaseMagicAttack(item.potionValue);
                break;

            case PotionEffectType.AttackSpeed:
                playerStats.IncreaseAttackSpeed(item.potionValue);
                break;

            case PotionEffectType.MovementSpeed:
                playerStats.IncreaseMovementSpeed(item.potionValue);
                break;

            case PotionEffectType.PhysicalDefense:
                playerStats.IncreasePhysicalDefense(item.potionValue);
                break;

            case PotionEffectType.MagicDefense:
                playerStats.IncreaseMagicDefense(item.potionValue);
                break;
        }

        RefreshStatsUI();
    }

    private void BuyArmor(ShopItemData item)
    {
        if (armorEquipment == null)
        {
            ShowMessage("Armor Equipment missing.");
            return;
        }

        if (item.armorData == null)
        {
            ShowMessage("Armor data missing.");
            return;
        }

        armorEquipment.EquipArmor(item.armorData);
        ShowMessage("Equipped " + item.itemName);
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

    private void ShowMessage(string message)
    {
        if (messageText != null)
            messageText.text = message;
    }
}