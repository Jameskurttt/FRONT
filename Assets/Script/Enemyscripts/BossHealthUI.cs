using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHealthUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject root;
    public Slider healthSlider;
    public TMP_Text bossNameText;

    private void Awake()
    {
        if (root != null)
            root.SetActive(false);
    }

    public void Setup(int maxHealth, string bossName)
    {
        if (root != null)
            root.SetActive(true);

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
        }

        if (bossNameText != null)
        {
            bossNameText.text = bossName;
        }
    }

    public void UpdateHealth(int currentHealth)
    {
        if (healthSlider != null)
            healthSlider.value = currentHealth;
    }

    public void Hide()
    {
        if (root != null)
            root.SetActive(false);
    }
}