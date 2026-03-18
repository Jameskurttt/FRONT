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
        col.enabled = false;

        transform.SetParent(holder);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void Drop(Vector3 dropPosition)
    {
        transform.SetParent(null);
        transform.position = dropPosition;

        rb.isKinematic = false;
        col.enabled = true;

        isDropped = true;
        destroyTimer = destroyAfterDrop;
    }
}