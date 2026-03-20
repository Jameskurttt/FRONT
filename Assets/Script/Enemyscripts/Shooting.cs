using UnityEngine;

public class Shooting : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 50f;
    [SerializeField] private float bulletLifetime = 2f;
    [SerializeField] private Transform firePoint;

    [Header("AI Settings")]
    [SerializeField] private bool useAI = true;
    [SerializeField] private float shootInterval = 1f;
    [SerializeField] private Transform playerTarget;

    private float shootTimer;

    void Update()
    {
        if (!useAI) return;
        if (playerTarget == null) return;

        shootTimer += Time.deltaTime;

        FacePlayer();

        if (shootTimer >= shootInterval)
        {
            shootTimer = 0f;
            Shoot();
        }
    }

    void FacePlayer()
    {
        Vector3 direction = playerTarget.position - transform.position;
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    void Shoot()
    {
        Transform shootFrom = firePoint != null ? firePoint : transform;

        Vector3 spawnPosition = shootFrom.position;
        Quaternion spawnRotation = shootFrom.rotation;

        GameObject newBullet = Instantiate(bulletPrefab, spawnPosition, spawnRotation);

        Rigidbody bulletRb = newBullet.GetComponent<Rigidbody>();
        if (bulletRb != null)
        {
            bulletRb.linearVelocity = shootFrom.forward * bulletSpeed;
        }

        Bullet bulletScript = newBullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.SetDontCollide(gameObject);
        }

        Destroy(newBullet, bulletLifetime);
    }
}