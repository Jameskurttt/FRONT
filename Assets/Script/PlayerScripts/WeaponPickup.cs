using UnityEngine;
using UnityEngine.UI;

public class PlayerWeaponPickup : MonoBehaviour
{
    public Transform weaponHolder;
    public float pickupRange = 2f;
    public LayerMask weaponLayer;
    public Text weaponUIText;

    private Weapon currentWeapon;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPickup();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            DropWeapon();
        }
    }

    void TryPickup()
    {
        Collider[] weapons = Physics.OverlapSphere(transform.position, pickupRange, weaponLayer);

        if (weapons.Length == 0) return;

        Weapon weapon = weapons[0].GetComponent<Weapon>();
        if (weapon == null) return;

        // Drop current weapon first
        if (currentWeapon != null)
        {
            Vector3 dropPos = transform.position + transform.forward * 1.5f;
            currentWeapon.Drop(dropPos);
        }

        // Pick up new weapon
        currentWeapon = weapon;
        weapon.Pickup(weaponHolder);

        // Update UI
        if (weaponUIText != null)
            weaponUIText.text = "Weapon: " + weapon.weaponName;
    }

    void DropWeapon()
    {
        if (currentWeapon == null) return;

        Vector3 dropPos = transform.position + transform.forward * 1.5f;

        currentWeapon.Drop(dropPos);
        currentWeapon = null;

        if (weaponUIText != null)
            weaponUIText.text = "Weapon: None";
    }
}