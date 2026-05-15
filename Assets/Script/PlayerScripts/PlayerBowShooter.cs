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

    private float nextShootTime;

    private void Start()
    {
        if (weaponPickup == null)
            weaponPickup = GetComponent<PlayerWeaponPickup>();

        if (playerStats == null)
            playerStats = GetComponent<PlayerHealth>();

        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    // CALLED BY ANIMATION EVENT
    public void ShootArrow()
    {
        if (Time.time < nextShootTime)
            return;

        if (weaponPickup == null)
            return;

        if (!weaponPickup.HasBowEquipped())
            return;

        if (playerStats == null)
            return;

        if (playerCamera == null)
            return;

        Weapon currentWeapon = weaponPickup.GetCurrentWeapon();

        if (currentWeapon == null)
            return;

        Transform shootPoint = FindSpawnPoint(currentWeapon);

        if (shootPoint == null)
        {
            Debug.LogWarning("BowShootPoint not found.");
            return;
        }

        nextShootTime = Time.time + shootCooldown;

        Vector3 shootDirection = GetCameraShootDirection();

        GameObject arrowObject = GetArrowObject();

        if (arrowObject == null)
            return;

        // IMPORTANT: detach from pool/player/bow so it will not jitter
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

    private Vector3 GetCameraShootDirection()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        return ray.direction.normalized;
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