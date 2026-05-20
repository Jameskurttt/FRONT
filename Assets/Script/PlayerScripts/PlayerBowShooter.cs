using UnityEngine;

public class PlayerBowShooter : MonoBehaviour
{
    [Header("References")]
    public PlayerWeaponPickup weaponPickup;
    public PlayerHealth playerStats;
    public Camera playerCamera;

    [Header("Arrow Setup")]
    public GameObject arrowPrefab;
    public string bowFirePointName = "BowShootPoint";

    [Header("Shoot Settings")]
    public float shootCooldown = 0.25f;
    public float arrowSpeed = 35f;
    public float maxAimDistance = 100f;
    public LayerMask aimLayerMask = ~0; // Hits everything by default

    private float nextShootTime;

    private void Start()
    {
        if (weaponPickup == null) weaponPickup = GetComponent<PlayerWeaponPickup>();
        if (playerStats == null) playerStats = GetComponent<PlayerHealth>();
        if (playerCamera == null) playerCamera = Camera.main;
    }

    // CALLED BY ANIMATION EVENT
    public void ShootArrow()
    {
        if (Time.time < nextShootTime) return;
        if (weaponPickup == null || !weaponPickup.HasBowEquipped()) return;
        if (playerStats == null || playerCamera == null) return;

        Weapon currentWeapon = weaponPickup.GetCurrentWeapon();
        if (currentWeapon == null) return;

        Transform shootPoint = FindSpawnPoint(currentWeapon);
        if (shootPoint == null)
        {
            Debug.LogWarning("BowShootPoint not found on current weapon.");
            return;
        }

        nextShootTime = Time.time + shootCooldown;

        // Calculate direction relative to where the crosshair is hitting in the world
        Vector3 shootDirection = GetCameraShootDirection(shootPoint.position);

        GameObject arrowObject = GetArrowObject();
        if (arrowObject == null) return;

        // Detach from parent to prevent animation/movement jitter
        arrowObject.transform.SetParent(null);
        arrowObject.transform.position = shootPoint.position;
        arrowObject.transform.rotation = Quaternion.LookRotation(shootDirection);
        arrowObject.SetActive(true);

        ArrowProjectile arrow = arrowObject.GetComponent<ArrowProjectile>();
        if (arrow != null)
        {
            int finalDamage = Mathf.RoundToInt(playerStats.GetTotalPhysicalAttack());
            arrow.SetDamage(finalDamage);
            arrow.Launch(shootDirection, arrowSpeed);
        }
    }

    private Transform FindSpawnPoint(Weapon currentWeapon)
    {
        Transform[] children = currentWeapon.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].name == bowFirePointName)
                return children[i];
        }
        return null;
    }

    private Vector3 GetCameraShootDirection(Vector3 spawnPoint)
    {
        // Viewport center (0.5, 0.5) corresponds directly to your 2D UI crosshair position
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 targetWorldPoint;

        // Determine what the player is actually looking at in 3D space
        if (Physics.Raycast(ray, out RaycastHit hit, maxAimDistance, aimLayerMask))
        {
            targetWorldPoint = hit.point;
        }
        else
        {
            // Fallback point if looking into the sky box or void
            targetWorldPoint = ray.GetPoint(maxAimDistance);
        }

        // Return vector point from the bow's position to the crosshair's intersection point
        return (targetWorldPoint - spawnPoint).normalized;
    }

    private GameObject GetArrowObject()
    {
        if (ArrowPool.Instance != null)
            return ArrowPool.Instance.GetArrow();

        if (arrowPrefab != null)
            return Instantiate(arrowPrefab);

        Debug.LogWarning("No ArrowPool found and arrowPrefab is missing.");
        return null;
    }
}