using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string weaponName = "Weapon";
    public float destroyAfterDrop = 60f;

    Rigidbody rb;
    Collider col;
    float destroyTimer;
    bool isDropped;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    void Update()
    {
        if (isDropped)
        {
            destroyTimer -= Time.deltaTime;

            if (destroyTimer <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public void Pickup(Transform holder)
    {
        isDropped = false;

        rb.isKinematic = true;
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.None; // reset just in case

        col.enabled = false;

        transform.SetParent(holder);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Drop(Vector3 dropPosition)
    {
        transform.SetParent(null);

        // Spawn slightly above ground
        transform.position = dropPosition + Vector3.up * 1f;

        rb.isKinematic = false;
        rb.useGravity = true;

        col.enabled = true;

        // Reset movement
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Small forward force (optional)
        rb.AddForce(transform.forward * 2f, ForceMode.Impulse);

        isDropped = true;
        destroyTimer = destroyAfterDrop;
    }
}