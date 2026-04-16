using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    [Header("References")]
    public EnemyChase chaseScript;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float shootRange = 10f;
    public float shootCooldown = 1.5f;
    public float bulletSpeed = 20f;

    private float shootTimer;

    private void Awake()
    {
        if (chaseScript == null)
            chaseScript = GetComponent<EnemyChase>();
    }

    private void Update()
    {
        if (chaseScript == null)
            return;

        Transform player = chaseScript.GetTarget();
        if (player == null)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= shootRange)
        {
            chaseScript.FaceTarget(player.position);
        }

        shootTimer -= Time.deltaTime;

        if (distanceToPlayer <= shootRange && shootTimer <= 0f)
        {
            Shoot();
            shootTimer = shootCooldown;
        }
    }

    private void Shoot()
    {
        if (bulletPrefab == null || firePoint == null)
            return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = firePoint.forward * bulletSpeed;
        }
    }
}