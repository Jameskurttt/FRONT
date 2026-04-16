using UnityEngine;

public class UISoundManager : MonoBehaviour
{
    public static UISoundManager instance;

    [Header("Audio Source")]
    public AudioSource audioSource;

    [Header("UI Sounds")]
    public AudioClip hoverSound;
    public AudioClip clickSound;

    [Header("Timing")]
    public float hoverBlockTimeAfterClick = 0.1f;

    private float lastClickTime;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayHover()
    {
        if (Time.unscaledTime - lastClickTime < hoverBlockTimeAfterClick)
            return;

        if (hoverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

    public void PlayClick()
    {
        lastClickTime = Time.unscaledTime;

        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}