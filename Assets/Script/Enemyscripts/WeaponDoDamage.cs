using UnityEngine;

public class WeaponDoDamage : MonoBehaviour
{
    [SerializeField] private float weaponHitRadius = 1f;
    [SerializeField] private int damage = 2;
    [SerializeField] private LayerMask targetLayer;

    private void Update()
    {
        DetectHit();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, weaponHitRadius);
    }

    private void DetectHit()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, weaponHitRadius, targetLayer);

        if (hitColliders.Length > 0)
        {
            EnemyHealth enemy = hitColliders[0].GetComponent<EnemyHealth>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }


        }
    }
}