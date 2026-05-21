using UnityEngine;

public class BowMultishotSkill : MonoBehaviour
{
    [Header("Skill Controls")]
    public KeyCode skillKey = KeyCode.E;
    public float skillCooldown = 5f;
    private float nextSkillTime;

    [Header("Multishot Settings")]
    public float sideArrowAngle = 15f;          // Spread angle of the cone
    public Vector3 bigArrowScale = new Vector3(10f, 10f, 10f); // Size of the skill arrows

    [Header("Dependencies")]
    private PlayerBowShooter bowShooter;

    private void Start()
    {
        bowShooter = GetComponent<PlayerBowShooter>();
    }

    private void Update()
    {
        // Handle input and cooldown
        if (Input.GetKeyDown(skillKey) && Time.time >= nextSkillTime)
        {
            if (CanCastSkill())
            {
                ExecuteMultishot();
                nextSkillTime = Time.time + skillCooldown;
            }
        }
    }

    private bool CanCastSkill()
    {
        if (bowShooter == null || bowShooter.weaponPickup == null) return false;
        return bowShooter.weaponPickup.HasBowEquipped();
    }

    private void ExecuteMultishot()
    {
        Weapon currentWeapon = bowShooter.weaponPickup.GetCurrentWeapon();
        if (currentWeapon == null) return;

        // Locate the shoot point on your current bow
        Transform shootPoint = FindSpawnPointDeep(currentWeapon.transform, bowShooter.bowFirePointName);
        if (shootPoint == null)
        {
            Debug.LogWarning($"Multishot: {bowShooter.bowFirePointName} not found on bow.");
            return;
        }

        // Calculate the central aim direction using your camera raycast system
        Vector3 centerDirection = GetSkillShootDirection(shootPoint.position);
        Quaternion centerRotation = Quaternion.LookRotation(centerDirection);

        // Calculate left and right fanned directions along the horizontal Y-axis
        Quaternion leftRotation = centerRotation * Quaternion.Euler(0, -sideArrowAngle, 0);
        Quaternion rightRotation = centerRotation * Quaternion.Euler(0, sideArrowAngle, 0);

        // Spawn all 3 arrows
        SpawnSkillArrow(shootPoint.position, leftRotation, leftRotation * Vector3.forward);
        SpawnSkillArrow(shootPoint.position, centerRotation, centerDirection);
        SpawnSkillArrow(shootPoint.position, rightRotation, rightRotation * Vector3.forward);
    }

    private void SpawnSkillArrow(Vector3 position, Quaternion rotation, Vector3 direction)
    {
        GameObject arrowObject = GetArrowInstance();
        if (arrowObject == null) return;

        // Match your setup: unparent, place, and orient
        arrowObject.transform.SetParent(null);
        arrowObject.transform.position = position;
        arrowObject.transform.rotation = rotation;

        // Apply the massive size to this specific arrow instance
        arrowObject.transform.localScale = bigArrowScale;

        arrowObject.SetActive(true);

        ArrowProjectile arrowScript = arrowObject.GetComponent<ArrowProjectile>();
        if (arrowScript != null && bowShooter.playerStats != null)
        {
            // Optional: Multiplied by 1.5 so your big skill shots deal extra damage!
            int finalDamage = Mathf.RoundToInt(bowShooter.playerStats.GetTotalPhysicalAttack() * 1.5f);
            arrowScript.SetDamage(finalDamage);
            arrowScript.Launch(direction, bowShooter.arrowSpeed);
        }
    }

    private GameObject GetArrowInstance()
    {
        if (ArrowPool.Instance != null) return ArrowPool.Instance.GetArrow();
        if (bowShooter.arrowPrefab != null) return Instantiate(bowShooter.arrowPrefab);
        return null;
    }

    private Vector3 GetSkillShootDirection(Vector3 spawnPoint)
    {
        Ray ray = bowShooter.playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 targetWorldPoint;

        if (Physics.Raycast(ray, out RaycastHit hit, bowShooter.maxAimDistance, bowShooter.aimLayerMask))
        {
            targetWorldPoint = hit.point;
        }
        else
        {
            targetWorldPoint = ray.GetPoint(bowShooter.maxAimDistance);
        }

        return (targetWorldPoint - spawnPoint).normalized;
    }

    private Transform FindSpawnPointDeep(Transform current, string targetName)
    {
        if (current.name == targetName) return current;
        foreach (Transform child in current)
        {
            Transform found = FindSpawnPointDeep(child, targetName);
            if (found != null) return found;
        }
        return null;
    }
}