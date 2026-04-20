using UnityEngine;

public class PlayerBowShooter : MonoBehaviour
{
    [Header("References")]
    public PlayerWeaponPickup weaponPickup;
    public PlayerHealth playerStats;
    public Camera playerCamera;

    [Header("Arrow Setup")]
    public GameObject arrowPrefab;
    public string bowFirePointName = "FirePoint";

    [Header("Shoot Settings")]
    public float shootCooldown = 0.25f;
    public float arrowSpeed = 35f;
    public float maxAimDistance = 100f;
    public LayerMask aimLayers = ~0;

    [Header("Aim Safety")]
    public float minDistanceFromSpawn = 1.25f;
    public float backwardAimDotLimit = 0.25f;

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

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            TryShoot();
    }

    private void TryShoot()
    {
        if (Time.time < nextShootTime)
            return;

        if (weaponPickup == null)
        {
            Debug.LogWarning("PlayerBowShooter: weaponPickup is missing.");
            return;
        }

        if (!weaponPickup.HasBowEquipped())
            return;

        if (playerStats == null)
        {
            Debug.LogWarning("PlayerBowShooter: playerStats is missing.");
            return;
        }

        if (playerCamera == null)
        {
            Debug.LogWarning("PlayerBowShooter: playerCamera is missing.");
            return;
        }

        if (arrowPrefab == null)
        {
            Debug.LogWarning("PlayerBowShooter: arrowPrefab is missing.");
            return;
        }

        Weapon currentWeapon = weaponPickup.GetCurrentWeapon();
        if (currentWeapon == null)
        {
            Debug.LogWarning("PlayerBowShooter: No current weapon found.");
            return;
        }

        Transform arrowSpawnPoint = currentWeapon.transform.Find(bowFirePointName);
        if (arrowSpawnPoint == null)
        {
            Debug.LogWarning("PlayerBowShooter: FirePoint was not found on the equipped bow. Make sure the bow prefab has a child named FirePoint.");
            return;
        }

        nextShootTime = Time.time + shootCooldown;

        Vector3 shootDirection = GetSafeShootDirection(arrowSpawnPoint);

        GameObject arrowObject = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.LookRotation(shootDirection));

        Collider arrowCollider = arrowObject.GetComponent<Collider>();
        Collider[] playerColliders = GetComponentsInChildren<Collider>();

        if (arrowCollider != null)
        {
            for (int i = 0; i < playerColliders.Length; i++)
            {
                if (playerColliders[i] != null)
                    Physics.IgnoreCollision(arrowCollider, playerColliders[i], true);
            }
        }

        ArrowProjectile arrow = arrowObject.GetComponent<ArrowProjectile>();
        if (arrow != null)
        {
            int finalDamage = Mathf.RoundToInt(playerStats.GetTotalPhysicalAttack());
            arrow.SetDamage(finalDamage);
            arrow.Launch(shootDirection, arrowSpeed);

            Debug.Log("Bow shot damage: " + finalDamage);
        }
        else
        {
            Debug.LogWarning("PlayerBowShooter: Arrow prefab does not have an ArrowProjectile component.");
        }
    }

    private Vector3 GetSafeShootDirection(Transform arrowSpawnPoint)
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 fallbackDirection = playerCamera.transform.forward;

        if (Physics.Raycast(ray, out RaycastHit hit, maxAimDistance, aimLayers))
        {
            Vector3 toHit = hit.point - arrowSpawnPoint.position;
            float distanceFromSpawn = toHit.magnitude;

            if (distanceFromSpawn < minDistanceFromSpawn)
                return fallbackDirection;

            Vector3 directionToHit = toHit.normalized;
            float dot = Vector3.Dot(arrowSpawnPoint.forward, directionToHit);

            if (dot < backwardAimDotLimit)
                return fallbackDirection;

            return directionToHit;
        }

        return fallbackDirection;
    }
}