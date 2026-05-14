using UnityEngine;

public class PlayerBowShooter : MonoBehaviour
{
    [Header("References")]
    public PlayerWeaponPickup weaponPickup;
    public PlayerHealth playerStats;
    public Camera playerCamera;

    [Header("Arrow Setup")]
    public GameObject arrowPrefab;
    public string bowFirePointName = "Bow Shoot Point";

    [Header("Shoot Settings")]
    public float shootCooldown = 0.25f;
    public float arrowSpeed = 35f;
    public float maxAimDistance = 100f;
    public LayerMask aimLayers = ~0;

    [Header("Aim Safety")]
    public float minDistanceFromSpawn = 1.25f;

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

        Transform arrowSpawnPoint = FindSpawnPoint(currentWeapon);

        if (arrowSpawnPoint == null)
        {
            Debug.LogWarning(
                "Bow Shoot Point not found. Make sure your bow has a child named: "
                + bowFirePointName
            );

            return;
        }

        nextShootTime = Time.time + shootCooldown;

        Vector3 shootDirection = GetSafeShootDirection(arrowSpawnPoint);

        GameObject arrowObject = GetArrowObject();

        if (arrowObject == null)
            return;

        arrowObject.transform.position = arrowSpawnPoint.position;
        arrowObject.transform.rotation = Quaternion.LookRotation(shootDirection);

        arrowObject.SetActive(true);

        IgnorePlayerCollision(arrowObject);

        ArrowProjectile arrow = arrowObject.GetComponent<ArrowProjectile>();

        if (arrow != null)
        {
            int finalDamage =
                Mathf.RoundToInt(playerStats.GetTotalPhysicalAttack());

            arrow.SetDamage(finalDamage);
            arrow.Launch(shootDirection, arrowSpeed);

            Debug.Log(
                "Bow animation shot damage: " + finalDamage
            );
        }
    }

    private Transform FindSpawnPoint(Weapon currentWeapon)
    {
        if (currentWeapon == null)
            return null;

        Transform[] children =
            currentWeapon.GetComponentsInChildren<Transform>(true);

        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].name == bowFirePointName)
                return children[i];
        }

        return null;
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

    private void IgnorePlayerCollision(GameObject arrowObject)
    {
        Collider arrowCollider = arrowObject.GetComponent<Collider>();

        if (arrowCollider == null)
            return;

        Collider[] playerColliders =
            GetComponentsInChildren<Collider>();

        for (int i = 0; i < playerColliders.Length; i++)
        {
            if (playerColliders[i] != null)
            {
                Physics.IgnoreCollision(
                    arrowCollider,
                    playerColliders[i],
                    true
                );
            }
        }
    }

    private Vector3 GetSafeShootDirection(Transform arrowSpawnPoint)
    {
        Ray ray =
            playerCamera.ViewportPointToRay(
                new Vector3(0.5f, 0.5f, 0f)
            );

        Vector3 targetPoint;

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            maxAimDistance,
            aimLayers
        ))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint =
                ray.origin +
                ray.direction * maxAimDistance;
        }

        Vector3 shootDirection =
            targetPoint - arrowSpawnPoint.position;

        if (shootDirection.magnitude < minDistanceFromSpawn)
            shootDirection = playerCamera.transform.forward;

        return shootDirection.normalized;
    }
}