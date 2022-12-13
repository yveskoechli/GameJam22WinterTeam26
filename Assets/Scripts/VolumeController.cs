using System;
using FMOD.Studio;
using UnityEngine;
using FMODUnity;

public class VolumeController : MonoBehaviour
{
    private VCA master;
    private VCA music;
    private VCA sfx;

    private void Awake()
    {
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
    
        

    private void UpdateVolumes()
    {
        master.setVolume(PlayerPrefs.GetFloat(SettingsMenu.MasterVolumeKey, SettingsMenu.DefaultMasterVolume));
        music.setVolume(PlayerPrefs.GetFloat(SettingsMenu.MusicVolumeKey, SettingsMenu.DefaultMusicVolume));
        sfx.setVolume(PlayerPrefs.GetFloat(SettingsMenu.SFXVolumeKey, SettingsMenu.DefaultSFXVolume));
    }
    
}

