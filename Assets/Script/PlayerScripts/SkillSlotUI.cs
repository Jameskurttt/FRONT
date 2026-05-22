using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillSlotUI : MonoBehaviour
{
    [Header("References")]
    public PlayerWeaponPickup weaponPickup;

    [Header("UI Slots")]
    public Image skillIcon;          // The main visual icon of the bow skill
    public GameObject lockedOverlay;  // Dark panel/lock icon when weapon is missing

    [Header("Cooldown UI")]
    public Image cooldownOverlay;    // The dark overlay that uses "Filled" image type
    public TMP_Text cooldownText;    // TextMeshPro text for the numbers

    [Header("Skill Sprite Asset")]
    public Sprite bowSkillSprite;    // Drag your Bow Skill Sprite artwork here!

    private BowMultishotSkill bowSkill;

    private void Start()
    {
        bowSkill = FindAnyObjectByType<BowMultishotSkill>();

        // Hide cooldown elements initially
        cooldownOverlay.gameObject.SetActive(false);
        cooldownText.gameObject.SetActive(false);

        // Setup the base icon asset immediately if available
        if (skillIcon != null && bowSkillSprite != null)
        {
            skillIcon.sprite = bowSkillSprite;
            skillIcon.enabled = true;
        }
    }

    private void Update()
    {
        UpdateSkillVisibility();
        UpdateCooldownUI();
    }

    void UpdateSkillVisibility()
    {
        if (skillIcon == null) return;

        bool hasBow = weaponPickup != null && weaponPickup.HasBowEquipped();

        if (hasBow)
        {
            // 1. Hide the locked overlay screen
            if (lockedOverlay != null) lockedOverlay.SetActive(false);

            // 2. Make sure the icon is fully visible and has its sprite assigned
            skillIcon.enabled = true;
            if (bowSkillSprite != null && skillIcon.sprite != bowSkillSprite)
            {
                skillIcon.sprite = bowSkillSprite;
            }
            skillIcon.color = Color.white;
        }
        else
        {
            // Show locked overlay when bow is unequipped
            if (lockedOverlay != null) lockedOverlay.SetActive(true);

            // Keep the skill icon enabled, but dim it so they can see what goes there
            skillIcon.enabled = true;
            skillIcon.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        }
    }

    void UpdateCooldownUI()
    {
        if (bowSkill == null || cooldownOverlay == null || cooldownText == null)
            return;

        float remaining = bowSkill.GetRemainingCooldown();

        if (remaining > 0)
        {
            // Turn on ONLY the cooldown visuals if they are sleeping
            if (!cooldownOverlay.gameObject.activeSelf)
            {
                cooldownOverlay.gameObject.SetActive(true);
                cooldownText.gameObject.SetActive(true);
            }

            // Shrink the overlay down based on time
            cooldownOverlay.fillAmount = remaining / bowSkill.skillCooldown;
            cooldownText.text = Mathf.Ceil(remaining).ToString();
        }
        else
        {
            // Cooldown complete: ONLY vanish the overlay and text, leave icon alone!
            if (cooldownOverlay.gameObject.activeSelf)
            {
                cooldownOverlay.gameObject.SetActive(false);
                cooldownText.gameObject.SetActive(false);
            }
        }
    }
}