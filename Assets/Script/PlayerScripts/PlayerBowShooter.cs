using UnityEngine;

public class PlayerBowShooter : MonoBehaviour
{
    [Header("References")]
    public PlayerWeaponPickup weaponPickup;
    public Camera playerCamera;
    public Transform arrowSpawnPoint;

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

        if (playerCamera == null)
        {
            Debug.LogWarning("PlayerBowShooter: playerCamera is missing.");
            return;
        }

        if (arrowSpawnPoint == null)
        {
            Debug.LogWarning("PlayerBowShooter: arrowSpawnPoint is missing.");
            return;
        }

        if (ArrowPool.Instance == null)
        {
            Debug.LogWarning("PlayerBowShooter: No ArrowPool found in scene.");
            return;
        }

        nextShootTime = Time.time + shootCooldown;

        Vector3 shootDirection = GetSafeShootDirection();

        GameObject arrowObject = ArrowPool.Instance.GetArrow();
        arrowObject.transform.position = arrowSpawnPoint.position;
        arrowObject.transform.rotation = Quaternion.LookRotation(shootDirection);
        arrowObject.SetActive(true);

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
            arrow.Launch(shootDirection, arrowSpeed);
    }

    private Vector3 GetSafeShootDirection()
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