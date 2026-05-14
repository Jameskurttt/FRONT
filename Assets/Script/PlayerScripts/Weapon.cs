using UnityEngine;

public enum WeaponType
{
    Sword,
    Bow
}

public class Weapon : MonoBehaviour
{
    [Header("Info")]
    public string weaponName = "Sword";
    [TextArea] public string description;
    public Sprite weaponIcon;
    public WeaponType weaponType = WeaponType.Sword;

    [Header("Stats")]
    public int baseWeaponDamage = 5;

    [Header("References")]
    public Collider pickupCollider;
    public Collider damageHitbox;
    public Rigidbody rb;

    private int currentWeaponDamage;
    private bool isEquipped;

    private void Awake()
    {
        currentWeaponDamage = baseWeaponDamage;

        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (pickupCollider == null)
            pickupCollider = GetComponent<Collider>();

        if (damageHitbox != null)
            damageHitbox.enabled = false;
    }

    public void Pickup(Transform holder)
    {
        transform.SetParent(holder, false);

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        isEquipped = true;

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (pickupCollider != null)
            pickupCollider.enabled = false;

        DisableDamageHitbox();
    }

    public void Drop(Vector3 dropPosition)
    {
        transform.SetParent(null, true);
        transform.position = dropPosition;

        isEquipped = false;

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        if (pickupCollider != null)
            pickupCollider.enabled = true;

        DisableDamageHitbox();
    }

    public void EnableDamageHitbox()
    {
        if (!isEquipped)
            return;

        if (damageHitbox != null)
        {
            SwordDamage swordDamage = damageHitbox.GetComponent<SwordDamage>();

            if (swordDamage != null)
                swordDamage.StartDamage();

            damageHitbox.enabled = true;
        }
    }

    public void DisableDamageHitbox()
    {
        if (damageHitbox != null)
            damageHitbox.enabled = false;
    }

    public void SetWeaponDamage(int newDamage)
    {
        currentWeaponDamage = Mathf.Max(0, newDamage);
    }

    public int GetWeaponDamage()
    {
        return currentWeaponDamage;
    }

    public bool IsEquipped()
    {
        return isEquipped;
    }
}