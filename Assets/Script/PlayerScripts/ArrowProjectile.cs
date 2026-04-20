using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    [Header("Arrow Settings")]
    public int damage = 25;
    public float lifeTime = 3f;

    private Rigidbody rb;
    private Collider col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        CancelInvoke();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (col != null)
            col.enabled = true;

        Invoke(nameof(DisableArrow), lifeTime);
    }

    public void SetDamage(int newDamage)
    {
        damage = Mathf.Max(0, newDamage);
    }

    public void Launch(Vector3 direction, float speed)
    {
        if (rb == null)
        {
            Debug.LogWarning("ArrowProjectile: Rigidbody is missing.");
            return;
        }

        direction = direction.normalized;
        rb.linearVelocity = direction * speed;
        transform.forward = direction;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            DisableArrow();
            return;
        }

        BossHealth boss = other.GetComponentInParent<BossHealth>();
        if (boss != null)
        {
            boss.TakeDamage(damage);
            DisableArrow();
            return;
        }

        DisableArrow();
    }

    private void DisableArrow()
    {
        CancelInvoke();
        gameObject.SetActive(false);
    }
}