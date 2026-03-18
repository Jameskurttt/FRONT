using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int level = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 50;
    public float health = 100f; // make it float so bullets can deal decimal damage
    public int damage = 25;

    // Give XP
    public void GainXP(int xp)
    {
        currentXP += xp;
        Debug.Log("Gained " + xp + " XP");

        while (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentXP -= xpToNextLevel;
        level++;
        xpToNextLevel += 50;

        health += 20;
        damage += 5;

        Debug.Log("Level Up! Level: " + level);
        Debug.Log("Health: " + health + ", Damage: " + damage);
    }

    //  NEW: Take damage function
    public void TakeDamage(float dmg)
    {
        health -= dmg;
        Debug.Log("Player took " + dmg + " damage. Health: " + health);

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player Died!");
        // You can add respawn or game over logic here
    }
}