using UnityEngine;
using UnityEngine.UI;

public class SettingsVolumeUI : MonoBehaviour
{
    [Header("Volume Sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Start()
    {
        if (GameAudioManager.Instance == null)
            return;

        if (masterSlider != null)
        {
            masterSlider.value = GameAudioManager.Instance.masterVolume;
            masterSlider.onValueChanged.AddListener(GameAudioManager.Instance.SetMasterVolume);
        }

        if (musicSlider != null)
        {
            musicSlider.value = GameAudioManager.Instance.musicVolume;
            musicSlider.onValueChanged.AddListener(GameAudioManager.Instance.SetMusicVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = GameAudioManager.Instance.sfxVolume;
            sfxSlider.onValueChanged.AddListener(GameAudioManager.Instance.SetSfxVolume);
        }
    }
}