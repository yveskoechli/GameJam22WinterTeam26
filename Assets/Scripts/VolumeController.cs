using System;
using FMOD.Studio;
using UnityEngine;
using FMODUnity;

public class VolumeController : MonoBehaviour
{
    private VCA master;
    private VCA music;
    private VCA sfx;

    [SerializeField] private StudioEventEmitter mainMenuMusic;

    private void Start()
    {
        Invoke(nameof(PlayMusic), 1);
        try
        {
            master = RuntimeManager.GetVCA("vca:/Master");
            music = RuntimeManager.GetVCA("vca:/Music");
            sfx = RuntimeManager.GetVCA("vca:/SFX");

        }
        catch (Exception)
        {
            Debug.LogError("Error: wrong VCA Name");
            throw;
        }
    }

    private void Update()
    {
        UpdateVolumes();
    }


    private void OnDestroy()
    {
        mainMenuMusic.Stop();
    }

    private void UpdateVolumes()
    {
        master.setVolume(PlayerPrefs.GetFloat(SettingsMenu.MasterVolumeKey, SettingsMenu.DefaultMasterVolume));
        music.setVolume(PlayerPrefs.GetFloat(SettingsMenu.MusicVolumeKey, SettingsMenu.DefaultMusicVolume));
        sfx.setVolume(PlayerPrefs.GetFloat(SettingsMenu.SFXVolumeKey, SettingsMenu.DefaultSFXVolume));
    }

    private void PlayMusic()
    {
        mainMenuMusic.Play();
    }
    
}

