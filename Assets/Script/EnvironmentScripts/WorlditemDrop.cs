using System.Collections;
using UnityEngine;

public class WorldLootDrop : MonoBehaviour
{
    [Header("Item Data")]
    public ItemDropData itemData;

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
    public KeyCode pickupKey = KeyCode.E;

    [Header("Pickup Animation")]
    public float pickupAnimDuration = 0.15f;

    private Transform player;
    private Camera mainCamera;
    private PlayerWeaponPickup playerWeaponPickup;

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
        ApplyItemLook();
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

        Color rarityColor = GetRarityColor(itemData.rarity);

        if (iconRenderer != null)
        {
            iconRenderer.sprite = itemData.itemIcon;
        }

        if (glowRenderer != null)
        {
            glowRenderer.color = rarityColor;
        }

        visualsReady = true;
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
        {
            visualRoot.rotation = Quaternion.LookRotation(direction);
        }
    }

    private void PlayIdleAnimation()
    {
        if (visualRoot != null)
        {
            float floatOffset = Mathf.Sin(Time.time * floatSpeed) * floatHeight;
            visualRoot.localPosition = visualStartLocalPos + Vector3.up * floatOffset;
        }

        if (iconPivot != null)
        {
            iconPivot.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
        }

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
            if (itemData.autoPickup)
            {
                StartCoroutine(PickupRoutine());
            }
        }
    }

    private IEnumerator PickupRoutine()
    {
        if (isPickingUp)
            yield break;

        isPickingUp = true;

        if (playerWeaponPickup != null)
        {
            playerWeaponPickup.EquipFromLoot(itemData);
        }
        else
        {
            Debug.LogWarning("PlayerWeaponPickup was not found on the player.");
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

    private Color GetRarityColor(LootRarity rarity)
    {
        switch (rarity)
        {
            case LootRarity.Common:
                return new Color(1f, 1f, 1f);

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