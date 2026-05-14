using System.Collections;
using UnityEngine;

public class WorldLootDrop : MonoBehaviour
{
    [Header("Item Data")]
    public ItemDropData itemData;

    [Header("Rolled Stats")]
    [SerializeField] private int rolledWeaponDamage;

    [Header("Runtime Rarity")]
    [SerializeField] private LootRarity runtimeRarity = LootRarity.Common;

    [Header("Visuals")]
    public Transform visualRoot;
    public Transform iconPivot;
    public SpriteRenderer iconRenderer;
    public SpriteRenderer glowRenderer;

    [Header("Spawn Animation")]
    public float spawnRiseHeight = 0.6f;
    public float spawnDuration = 0.25f;
    public float spawnScaleMultiplier = 1.25f;

    [Header("Idle Animation")]
    public float floatHeight = 0.2f;
    public float floatSpeed = 2f;
    public float rotationSpeed = 45f;
    public float glowPulseSpeed = 3f;
    public float glowPulseAmount = 0.15f;

    [Header("Pickup")]
    public float pickupRange = 1.8f;
    public float magnetRange = 3f;
    public float magnetSpeed = 6f;

    [Header("Pickup Animation")]
    public float pickupAnimDuration = 0.15f;

    private Transform player;
    private Camera mainCamera;
    private PlayerWeaponPickup playerWeaponPickup;
    private PlayerArmorEquipment playerArmorEquipment;

    private Vector3 visualStartLocalPos;
    private Vector3 baseScale;
    private Vector3 glowBaseScale;

    private bool isPickingUp;
    private bool visualsReady;

    private void Awake()
    {
        if (visualRoot != null)
        {
            visualStartLocalPos = visualRoot.localPosition;
            baseScale = visualRoot.localScale;
        }

        if (glowRenderer != null)
        {
            glowBaseScale = glowRenderer.transform.localScale;
        }
    }

    private void Start()
    {
        mainCamera = Camera.main;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
            playerWeaponPickup = playerObj.GetComponent<PlayerWeaponPickup>();
            playerArmorEquipment = playerObj.GetComponent<PlayerArmorEquipment>();
        }

        ApplyItemLook();
        StartCoroutine(PlaySpawnAnimation());
    }

    private void Update()
    {
        if (!visualsReady || isPickingUp)
            return;

        FaceCamera();
        PlayIdleAnimation();
        HandlePickupLogic();
    }

    public void Setup(ItemDropData newItemData)
    {
        itemData = newItemData;

        if (itemData != null)
            runtimeRarity = itemData.rarity;

        if (itemData != null && itemData.itemType == DropItemType.Weapon)
            rolledWeaponDamage = RollWeaponDamage();
        else
            rolledWeaponDamage = 0;

        ApplyItemLook();
    }

    public void Setup(ItemDropData newItemData, int forcedWeaponDamage)
    {
        itemData = newItemData;

        if (itemData != null)
            runtimeRarity = itemData.rarity;

        rolledWeaponDamage = Mathf.Max(0, forcedWeaponDamage);

        ApplyItemLook();
    }

    public void Setup(ItemDropData newItemData, int forcedWeaponDamage, LootRarity forcedRarity)
    {
        itemData = newItemData;
        rolledWeaponDamage = Mathf.Max(0, forcedWeaponDamage);
        runtimeRarity = forcedRarity;

        ApplyItemLook();
    }

    public int GetRolledWeaponDamage()
    {
        return rolledWeaponDamage;
    }

    public LootRarity GetRuntimeRarity()
    {
        return runtimeRarity;
    }

    public void TryPickupFromPlayer()
    {
        if (isPickingUp)
            return;

        StartCoroutine(PickupRoutine());
    }

    private void ApplyItemLook()
    {
        if (itemData == null)
            return;

        Color rarityColor = GetRarityColor(runtimeRarity);

        if (iconRenderer != null)
            iconRenderer.sprite = itemData.itemIcon;

        if (glowRenderer != null)
            glowRenderer.color = rarityColor;

        visualsReady = true;
    }

    private int RollWeaponDamage()
    {
        if (itemData == null)
            return 0;

        int minDamage = itemData.minWeaponDamage;
        int maxDamage = itemData.maxWeaponDamage;

        switch (runtimeRarity)
        {
            case LootRarity.Common:
                break;

            case LootRarity.Uncommon:
                minDamage += 2;
                maxDamage += 3;
                break;

            case LootRarity.Rare:
                minDamage += 4;
                maxDamage += 6;
                break;

            case LootRarity.Epic:
                minDamage += 7;
                maxDamage += 10;
                break;

            case LootRarity.Legendary:
                minDamage += 11;
                maxDamage += 15;
                break;
        }

        if (maxDamage < minDamage)
            maxDamage = minDamage;

        return Random.Range(minDamage, maxDamage + 1);
    }

    private IEnumerator PlaySpawnAnimation()
    {
        if (visualRoot == null)
            yield break;

        Vector3 startPos = visualStartLocalPos + Vector3.up * spawnRiseHeight;
        Vector3 endPos = visualStartLocalPos;

        Vector3 startScale = baseScale * spawnScaleMultiplier;
        Vector3 endScale = baseScale;

        float timer = 0f;

        visualRoot.localPosition = startPos;
        visualRoot.localScale = startScale;

        while (timer < spawnDuration)
        {
            timer += Time.deltaTime;

            float t = timer / spawnDuration;
            t = 1f - Mathf.Pow(1f - t, 3f);

            visualRoot.localPosition = Vector3.Lerp(startPos, endPos, t);
            visualRoot.localScale = Vector3.Lerp(startScale, endScale, t);

            yield return null;
        }

        visualRoot.localPosition = endPos;
        visualRoot.localScale = endScale;
    }

    private void FaceCamera()
    {
        if (mainCamera == null || visualRoot == null)
            return;

        Vector3 direction = visualRoot.position - mainCamera.transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.001f)
            visualRoot.rotation = Quaternion.LookRotation(direction);
    }

    private void PlayIdleAnimation()
    {
        if (visualRoot != null)
        {
            float floatOffset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
            visualRoot.localPosition = visualStartLocalPos + Vector3.up * floatOffset;
        }

        if (iconPivot != null)
            iconPivot.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);

        if (glowRenderer != null)
        {
            float pulse = 1f + Mathf.Sin(Time.time * glowPulseSpeed) * glowPulseAmount;
            glowRenderer.transform.localScale = glowBaseScale * pulse;
        }
    }

    private void HandlePickupLogic()
    {
        if (player == null || itemData == null)
            return;

        if (itemData.itemType == DropItemType.Armor)
            return;

        if (!itemData.autoPickup)
            return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= magnetRange && distance > pickupRange)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                player.position,
                magnetSpeed * Time.deltaTime
            );
        }

        if (distance <= pickupRange)
        {
            StartCoroutine(PickupRoutine());
        }
    }

    private IEnumerator PickupRoutine()
    {
        if (isPickingUp)
            yield break;

        isPickingUp = true;

        if (itemData.itemType == DropItemType.Totem)
        {
            ApplyTotemStats();
        }
        else if (itemData.itemType == DropItemType.Armor)
        {
            ApplyArmorPickup();
        }
        else if (itemData.itemType == DropItemType.Weapon)
        {
            if (playerWeaponPickup != null)
            {
                playerWeaponPickup.EquipFromLoot(itemData, rolledWeaponDamage, runtimeRarity);
            }
        }

        if (visualRoot != null)
        {
            float timer = 0f;

            Vector3 startScale = visualRoot.localScale;
            Vector3 endScale = Vector3.zero;

            while (timer < pickupAnimDuration)
            {
                timer += Time.deltaTime;

                float t = timer / pickupAnimDuration;

                visualRoot.localScale = Vector3.Lerp(startScale, endScale, t);

                yield return null;
            }
        }

        Destroy(gameObject);
    }

    private void ApplyArmorPickup()
    {
        if (playerArmorEquipment == null)
            return;

        playerArmorEquipment.EquipArmorFromLoot(itemData);
    }

    private void ApplyTotemStats()
    {
        if (player == null)
            return;

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

        if (playerHealth == null)
            return;

        playerHealth.IncreaseMaxHP(itemData.bonusMaxHP);
        playerHealth.IncreaseHPRegen(itemData.bonusHPRegen);
        playerHealth.IncreaseArmor(itemData.bonusArmor);
        playerHealth.IncreasePhysicalAttack(itemData.bonusPhysicalAttack);
        playerHealth.IncreaseMagicAttack(itemData.bonusMagicAttack);
        playerHealth.IncreaseAttackSpeed(itemData.bonusAttackSpeed);
        playerHealth.IncreaseMovementSpeed(itemData.bonusMovementSpeed);
        playerHealth.IncreasePhysicalDefense(itemData.bonusPhysicalDefense);
        playerHealth.IncreaseMagicDefense(itemData.bonusMagicDefense);
    }

    private Color GetRarityColor(LootRarity rarity)
    {
        switch (rarity)
        {
            case LootRarity.Common:
                return Color.white;

            case LootRarity.Uncommon:
                return new Color(0.35f, 1f, 0.35f);

            case LootRarity.Rare:
                return new Color(0.35f, 0.65f, 1f);

            case LootRarity.Epic:
                return new Color(0.75f, 0.4f, 1f);

            case LootRarity.Legendary:
                return new Color(1f, 0.7f, 0.2f);

            default:
                return Color.white;
        }
    }
}