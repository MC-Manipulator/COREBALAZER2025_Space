using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettingBoard : MonoBehaviour
{

    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    private void Awake()
    {
        BindManager();
        UpdateBoard();
    }

    private void OnEnable()
    {
        masterVolumeSlider.onValueChanged.AddListener(UpdateMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(UpdateMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(UpdateSFXVolume);
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        masterVolumeSlider.onValueChanged.RemoveListener(UpdateMasterVolume);
        musicVolumeSlider.onValueChanged.RemoveListener(UpdateMusicVolume);
        sfxVolumeSlider.onValueChanged.RemoveListener(UpdateSFXVolume);
    }

    public void BindManager()
    {
        AudioManager.Instance.audioSettingBoard = this;
    }

    public void UpdateBoard()
    {
        SetVolume(AudioManager.Instance.masterVolume,
            AudioManager.Instance.musicVolume,
            AudioManager.Instance.sfxVolume);
    }

    public void UpdateMasterVolume(float value)
    {
        AudioManager.Instance.SetMasterVolume(value);
    }

    public void UpdateMusicVolume(float value)
    {
        AudioManager.Instance.SetMusicVolume(value);
    }

    public void UpdateSFXVolume(float value)
    {
        AudioManager.Instance.SetSFXVolume(value);
    }

    public void SetVolume(float masterVolume, float musicVolume, float sfxVolume)
    {
        masterVolumeSlider.value = masterVolume;
        musicVolumeSlider.value = musicVolume;
        sfxVolumeSlider.value = sfxVolume;
    }
}
