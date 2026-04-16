using UnityEngine;
using UnityEngine.UI;

public class SliderLoader : MonoBehaviour
{
    public Slider musicSlider;

    private void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.8f);
        musicSlider.value = savedVolume;

        if (BackgroundMusic.instance != null)
        {
            BackgroundMusic.instance.SetVolume(savedVolume);
        }
    }
}