using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundControll : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;

    public string musicValuePlayerPrefs = "musicPref";
    public string musicGroupMixer = "music";
    public string sfxValuePlayerPrefs = "sfxPref";
    public string sfxGroupMixer = "sfx";
    public AudioMixer mixer;


    private void OnEnable()
    {
        musicSlider.value = PlayerPrefs.GetFloat(musicValuePlayerPrefs, musicSlider.maxValue);
        sfxSlider.value = PlayerPrefs.GetFloat(sfxValuePlayerPrefs, sfxSlider.maxValue);

    }

    private void Start()
    {
        mixer.SetFloat(musicGroupMixer, PlayerPrefs.GetFloat(musicValuePlayerPrefs, musicSlider.maxValue));
        mixer.SetFloat(sfxGroupMixer, PlayerPrefs.GetFloat(sfxValuePlayerPrefs, sfxSlider.maxValue));
    }

    public void OnMusicValueChange(float newValue)
    {
        PlayerPrefs.SetFloat(musicValuePlayerPrefs, newValue);
        mixer.SetFloat(musicGroupMixer, newValue);
    }

    public void OnSfxValueChanged(float newValue)
    {
        PlayerPrefs.SetFloat(sfxValuePlayerPrefs, newValue);
        mixer.SetFloat(sfxGroupMixer, newValue);
    }
}
