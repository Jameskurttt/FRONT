using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 5f;
    public float lifeTime = 3f;

    private GameObject dontCollideObj;

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
        Debug.Log("Bullet hit: " + collision.gameObject.name);

        if (collision.gameObject == dontCollideObj)
            return;

        PlayerHealth stats = collision.gameObject.GetComponent<PlayerHealth>();

        if (stats == null)
        {
            stats = collision.gameObject.GetComponentInParent<PlayerHealth>();
        }

        if (stats != null)
        {
            Debug.Log("HIT PLAYER!");
            stats.TakeDamage(damage);
        }

        DisableBullet();
    }

    void DisableBullet()
    {
        gameObject.SetActive(false);
    }
}