using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public const string MasterVolumeKey = "Settings.MasterVolume";
    public const string MusicVolumeKey = "Settings.MusicVolume";
    public const string SFXVolumeKey = "Settings.SFXVolume";

    public const float DefaultMasterVolume = 1.0f;
    public const float DefaultMusicVolume = 1.0f;
    public const float DefaultSFXVolume = 1.0f;

    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;


    private void Start()
    {
        Initialize(masterVolumeSlider, MasterVolumeKey, DefaultMasterVolume);
        Initialize(musicVolumeSlider, MusicVolumeKey, DefaultMusicVolume);
        Initialize(sfxVolumeSlider, SFXVolumeKey, DefaultSFXVolume);
    }

    private void Initialize(Slider slider, string key, float defaultValue)
    {
        slider.SetValueWithoutNotify(PlayerPrefs.GetFloat(key, defaultValue));
        addCallback(slider, key);
    }
   
    
    
    
    private void addCallback(Slider slider, string key)
    {
        slider.onValueChanged.AddListener((float value)=> 
        {
            PlayerPrefs.SetFloat(key, value);
        });
    }
}


