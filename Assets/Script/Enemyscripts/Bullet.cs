using UnityEngine;

public class Bullet : MonoBehaviour
{
    private GameObject dontCollideObj;

    public float lifeTime = 3f;

    public void SetDontCollide(GameObject obj)
    {
        dontCollideObj = obj;
    }

    void OnEnable()
    {
        Invoke(nameof(DisableBullet), lifeTime);
    }

    void OnDisable()
    {
        CancelInvoke();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == dontCollideObj)
            return;

        PlayerStats stats = collision.gameObject.GetComponentInParent<PlayerStats>();
        if (stats != null)
        {
            stats.TakeDamage(5f);
        }

        DisableBullet();
    }

    void DisableBullet()
    {
        gameObject.SetActive(false);
    }
}