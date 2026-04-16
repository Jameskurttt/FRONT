using UnityEngine;

public enum WeaponType
{
    Sword,
    Bow
}

public class Weapon : MonoBehaviour
{
    [Header("Weapon Info")]
    public string weaponName = "Sword";
    [TextArea] public string description = "Weapon description here";
    public Sprite weaponIcon;
    public WeaponType weaponType = WeaponType.Sword;

    [Header("Equip Offset")]
    public Vector3 equipLocalPosition;
    public Vector3 equipLocalRotation;

    [Header("Drop Settings")]
    public float destroyAfterDrop = 60f;

    private Rigidbody rb;
    private Collider col;
    private float destroyTimer;
    private bool isDropped;
    private bool isEquipped;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    private void Update()
    {
        if (isDropped)
        {
            destroyTimer -= Time.deltaTime;

            if (destroyTimer <= 0f)
                Destroy(gameObject);
        }
    }

    public void Pickup(Transform holder)
    {
        isDropped = false;
        isEquipped = true;

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (col != null)
            col.enabled = false;

        transform.SetParent(holder);
        transform.localPosition = equipLocalPosition;
        transform.localRotation = Quaternion.Euler(equipLocalRotation);
    }

    public void Drop(Vector3 dropPosition)
    {
        transform.SetParent(null);
        transform.position = dropPosition;

        if (rb != null)
            rb.isKinematic = false;

        if (col != null)
            col.enabled = true;

        isDropped = true;
        isEquipped = false;
        destroyTimer = destroyAfterDrop;
    }

    public bool IsEquipped()
    {
        return isEquipped;
    }
}