
using UnityEngine;


public class PlayerHealth : MonoBehaviour
{
    public Camera playerCamera;
    public Camera deathCamera;

    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    [SerializeField] private HealthBar healthBar;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetSliderMax(maxHealth);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        healthBar.SetSlider(currentHealth);
        Debug.Log("Player HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("Player Died!");
            GameManager.instance.GameOver();
            gameObject.SetActive(false);
        }
    }
    public void Die()
    {
        playerCamera.enabled = false;
        deathCamera.enabled = true;
    }
}