using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    [Header("Projectile Settings")]
    public GameObject projectile;
    public float bulletSpeed = 20f;     // adjustable speed
    public float timeBetweenShots = 1f; // cooldown
    public float rotationSpeed = 5f;    // how fast enemy turns to face player
    public float shootAngleThreshold = 10f; // degrees within which enemy can shoot

    private float shotTimer = 0f;

    private void Update()
    {
        if (shotTimer > 0)
            shotTimer -= Time.deltaTime;
    }

    public void ShootAt(Transform target)
    {
        if (shotTimer > 0) return; // cooldown

        // direction to player
        Vector3 lookPos = target.position - transform.position;
        lookPos.y = 0; // only rotate horizontally
        if (lookPos == Vector3.zero) return;

        // smooth rotation toward player
        Quaternion targetRotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // check if enemy is roughly facing the player
        float angleToPlayer = Vector3.Angle(transform.forward, lookPos);
        if (angleToPlayer > shootAngleThreshold)
            return; // not facing player yet, do not shoot

        // spawn bullet
        GameObject proj = Instantiate(projectile, transform.position + transform.forward, Quaternion.identity);

        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            Vector3 direction = lookPos.normalized;
            rb.AddForce(direction * bulletSpeed, ForceMode.Impulse);
        }

        shotTimer = timeBetweenShots; // reset cooldown
    }

    public void StopShooting()
    {
        shotTimer = 0f;
        CancelInvoke();
    }
}