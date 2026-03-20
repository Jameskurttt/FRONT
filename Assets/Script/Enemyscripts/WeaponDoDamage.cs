using UnityEngine;

public class WeaponDoDamage : MonoBehaviour
{
    [SerializeField] private float weaponHitRadius = 1f;
    [SerializeField] private int damage = 25;
    [SerializeField] private LayerMask targetLayer;

    public void DetectHit()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, weaponHitRadius, targetLayer);

        foreach (Collider hit in hitColliders)
        {
            EnemyHealth enemy = hit.GetComponentInParent<EnemyHealth>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, weaponHitRadius);
    }
}