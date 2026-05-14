using UnityEngine;

public class ShopTriggerZone : MonoBehaviour
{
    public ShopManager shopManager;
    public string playerTag = "Player";

    [Header("Blacksmith Sound")]
    public AudioSource audioSource;
    public AudioClip enterSound;

    private bool hasOpened = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasOpened) return;

        if (other.CompareTag(playerTag))
        {
            hasOpened = true;

            if (audioSource != null && enterSound != null)
            {
                audioSource.PlayOneShot(enterSound);
            }

            if (shopManager != null)
            {
                shopManager.OpenShop();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            hasOpened = false;
        }
    }
}