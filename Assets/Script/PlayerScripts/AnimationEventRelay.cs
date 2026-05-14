using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
    private PlayerWeaponPickup weaponPickup;
    private PlayerBowShooter bowShooter;

    private void Awake()
    {
        weaponPickup = GetComponentInParent<PlayerWeaponPickup>();
        bowShooter = GetComponentInParent<PlayerBowShooter>();
    }

    public void EnableDamageHitbox()
    {
        if (weaponPickup == null)
            return;

        Weapon currentWeapon = weaponPickup.GetCurrentWeapon();

        if (currentWeapon != null)
            currentWeapon.EnableDamageHitbox();
    }

    public void DisableDamageHitbox()
    {
        if (weaponPickup == null)
            return;

        Weapon currentWeapon = weaponPickup.GetCurrentWeapon();

        if (currentWeapon != null)
            currentWeapon.DisableDamageHitbox();
    }

    public void ShootArrow()
    {
        if (bowShooter != null)
            bowShooter.ShootArrow();
    }
}