using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    public SwordDamage swordDamage;

    public void EnableSwordHitbox()
    {
        swordDamage.EnableSwordHitbox();
    }

    public void DisableSwordHitbox()
    {
        swordDamage.DisableSwordHitbox();
    }
}