using UnityEngine;

public class BlacksmithTrigger : MonoBehaviour
{
    public BlacksmithUpgrade blacksmithUpgrade;
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

            // Play Sound
            if (audioSource != null && enterSound != null)
            {
                audioSource.PlayOneShot(enterSound);
            }

            // Open Blacksmith
            if (blacksmithUpgrade != null)
                blacksmithUpgrade.OpenBlacksmith();
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