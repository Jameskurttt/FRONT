using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int xpReward = 20; // XP player gains when killed

    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    // Call this from sword or bullet
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} died");

        // Give XP to the player safely
        PlayerStats player = FindObjectOfType<PlayerStats>();
        if (player != null)
        {
            player.GainXP(xpReward);
        }

        Destroy(gameObject);
    }
}