using UnityEngine;

public class HealEffect : MonoBehaviour, IEffect
{
    public float healAmount = 25f;

    public void ApplyEffect(GameObject player)
    {
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.Heal(healAmount);
        }
    }
}