using UnityEngine;

public class Shooting : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private float destroyAfter = 2f;
    [SerializeField] private float bulletSpeed = 50f;

    [Header("AI Settings")]
    [SerializeField] private bool ai = true; // enable AI
    [SerializeField] private float aiShootingTime = 1f; // shoots every 1 sec
    [SerializeField] private Transform player; // assign player here

    private float timer;

    void Update()
    {
        if (ai)
        {
            timer += Time.deltaTime;
            if (timer >= aiShootingTime)
            {
                timer = 0f;

                // Make enemy face player
                if (player != null)
                    transform.LookAt(player);

                Shoot();
            }
        }
    }

    void Shoot()
    {
        GameObject bulletObj = Instantiate(
            bullet,
            transform.position + transform.forward,
            transform.rotation
        );

        Rigidbody rb = bulletObj.GetComponent<Rigidbody>();
        if (rb != null)
            rb.AddForce(transform.forward * bulletSpeed, ForceMode.VelocityChange);

        Bullet bulletScript = bulletObj.GetComponent<Bullet>();
        if (bulletScript != null)
            bulletScript.SetDontCollide(gameObject); // enemy won't shoot itself

        Destroy(bulletObj, destroyAfter);
    }
}