using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public static BackgroundMusic instance;

    private AudioSource audioSource;

    private void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();

        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.8f);
        audioSource.volume = savedVolume;

        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
        audioSource.volume = 1f;
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();

        Debug.Log("Music volume set to: " + volume);
    }
}