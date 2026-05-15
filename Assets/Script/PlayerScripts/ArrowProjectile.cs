using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    [Header("Arrow Settings")]
    public int damage = 25;
    public float speed = 35f;
    public float lifeTime = 3f;

    private Vector3 moveDirection;
    private float timer;
    private Rigidbody rb;
    private Collider col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        timer = lifeTime;

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (col != null)
            col.isTrigger = true;
    }

    public void SetDamage(int newDamage)
    {
        damage = Mathf.Max(0, newDamage);
    }

    public void Launch(Vector3 direction, float arrowSpeed)
    {
        moveDirection = direction.normalized;
        speed = arrowSpeed;

        transform.forward = moveDirection;
    }

    private void LateUpdate()
    {
        transform.position += moveDirection * speed * Time.deltaTime;

        timer -= Time.deltaTime;

        if (timer <= 0f)
            DisableArrow();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        if (other.GetComponentInParent<PlayerMovement>() != null)
            return;

        if (other.GetComponentInParent<Weapon>() != null)
            return;

        if (other.GetComponentInParent<ArrowProjectile>() != null)
            return;

        EnemyHealth enemy = other.GetComponentInParent<EnemyHealth>();

        if (enemy != null)
        {
            enemy.TakeDamage(damage);

            PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();

            if (playerMovement != null)
                playerMovement.PlayBowHitEnemySound();

            DisableArrow();
            return;
        }

        BossHealth boss = other.GetComponentInParent<BossHealth>();

        if (boss != null)
        {
            boss.TakeDamage(damage);

            PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();

            if (playerMovement != null)
                playerMovement.PlayBowHitEnemySound();

            DisableArrow();
            return;
        }

        DisableArrow();
    }

    private void DisableArrow()
    {
        gameObject.SetActive(false);
    }
}