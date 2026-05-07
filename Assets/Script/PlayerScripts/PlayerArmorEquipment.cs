using UnityEngine;
using UnityEngine.UI;

public class PlayerArmorEquipment : MonoBehaviour
{
    [Header("References")]
    public PlayerHealth playerStats;
    public SkillTreeManager skillTreeManager;
    public PauseMenu pauseMenu;

    [Header("Armor Slot Frames")]
    public Image headSlotFrame;
    public Image bodySlotFrame;
    public Image bootsSlotFrame;

    [Header("Armor Slot Icons")]
    public Image headArmorIcon;
    public Image bodyArmorIcon;
    public Image bootsArmorIcon;

    [Header("Frame Sprites")]
    public Sprite activeSlotSprite;
    public Sprite inactiveSlotSprite;

    private ArmorItemData equippedHead;
    private ArmorItemData equippedBody;
    private ArmorItemData equippedBoots;

    private void Start()
    {
        if (playerStats == null)
            playerStats = GetComponent<PlayerHealth>();

        RefreshArmorUI();
        RefreshAllStatsUI();
    }

    public void EquipArmor(ArmorItemData newArmor)
    {
        if (newArmor == null)
        {
            Debug.LogWarning("EquipArmor failed. Armor data is missing.");
            return;
        }

        switch (newArmor.armorSlot)
        {
            case ArmorSlot.Head:
                ReplaceArmor(ref equippedHead, newArmor);
                break;

            case ArmorSlot.Body:
                ReplaceArmor(ref equippedBody, newArmor);
                break;

            case ArmorSlot.Boots:
                ReplaceArmor(ref equippedBoots, newArmor);
                break;
        }

        RefreshArmorUI();
        RefreshAllStatsUI();

        Debug.Log("Equipped armor: " + newArmor.armorName);
    }

    public void EquipArmorFromLoot(ItemDropData itemData)
    {
        if (itemData == null)
        {
            Debug.LogWarning("EquipArmorFromLoot failed. ItemDropData is missing.");
            return;
        }

        if (itemData.armorData == null)
        {
            Debug.LogWarning("EquipArmorFromLoot failed. ArmorData is missing inside ItemDropData.");
            return;
        }

        EquipArmor(itemData.armorData);
    }

    private void ReplaceArmor(ref ArmorItemData currentArmor, ArmorItemData newArmor)
    {
        if (currentArmor != null)
            RemoveArmorStats(currentArmor);

        currentArmor = newArmor;
        ApplyArmorStats(newArmor);
    }

    private void ApplyArmorStats(ArmorItemData armor)
    {
        if (armor == null || playerStats == null)
            return;

        if (armor.hpBonus != 0)
            playerStats.IncreaseMaxHP(armor.hpBonus);

        if (armor.armorBonus != 0)
            playerStats.IncreaseArmor(armor.armorBonus);

        if (armor.movementSpeedBonus != 0)
            playerStats.IncreaseMovementSpeed(armor.movementSpeedBonus);

        if (armor.attackSpeedBonus != 0)
            playerStats.IncreaseAttackSpeed(armor.attackSpeedBonus);

        if (armor.physicalDefenseBonus != 0)
            playerStats.IncreasePhysicalDefense(armor.physicalDefenseBonus);
    }

    private void RemoveArmorStats(ArmorItemData armor)
    {
        if (armor == null || playerStats == null)
            return;

        if (armor.hpBonus != 0)
            playerStats.IncreaseMaxHP(-armor.hpBonus);

        if (armor.armorBonus != 0)
            playerStats.IncreaseArmor(-armor.armorBonus);

        if (armor.movementSpeedBonus != 0)
            playerStats.IncreaseMovementSpeed(-armor.movementSpeedBonus);

        if (armor.attackSpeedBonus != 0)
            playerStats.IncreaseAttackSpeed(-armor.attackSpeedBonus);

        if (armor.physicalDefenseBonus != 0)
            playerStats.IncreasePhysicalDefense(-armor.physicalDefenseBonus);
    }

    private void RefreshArmorUI()
    {
        UpdateSlotUI(equippedHead, headSlotFrame, headArmorIcon);
        UpdateSlotUI(equippedBody, bodySlotFrame, bodyArmorIcon);
        UpdateSlotUI(equippedBoots, bootsSlotFrame, bootsArmorIcon);
    }

    private void UpdateSlotUI(ArmorItemData armor, Image frameImage, Image iconImage)
    {
        if (frameImage != null)
            frameImage.sprite = armor != null ? activeSlotSprite : inactiveSlotSprite;

        if (iconImage != null)
        {
            if (armor != null && armor.armorIcon != null)
            {
                iconImage.sprite = armor.armorIcon;
                iconImage.enabled = true;
                iconImage.preserveAspect = true;
                iconImage.color = Color.white;
            }
            else
            {
                iconImage.sprite = null;
                iconImage.enabled = false;
            }
        }
    }

    private void RefreshAllStatsUI()
    {
        if (skillTreeManager != null)
            skillTreeManager.RefreshStatsUI();

        if (pauseMenu != null)
            pauseMenu.RefreshPauseStats();
    }
}