using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public GameObject settingsPanel;
    public GameObject settingsButton;
    public AudioMixer audioMixer;

    [Header("슬라이더 UI 연결")]
    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider sfxSlider;

    private bool isReadyToChangeVolume = false;

    void Start()
    {
        SetMasterVolume(0.5f);
        SetBGMVolume(0.5f);
        SetSFXVolume(0.5f);

        Invoke("EnableVolumeControl", 0.1f);
    }

    private void EnableVolumeControl()
    {
        isReadyToChangeVolume = true;
    }

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        if (GameManager.instance != null)
            GameManager.instance.PauseGame();
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        if (settingsButton != null) settingsButton.SetActive(true);
        if (GameManager.instance != null)
            GameManager.instance.ResumeGame();
    }

    public void SetMasterVolume(float volume)
    {
        if (!isReadyToChangeVolume) return;

        float dB = volume > 0.0001f ? (Mathf.Log10(volume) + 0.3f) * 40 : -80f;
        audioMixer.SetFloat("Master", dB);
    }

    public void SetBGMVolume(float volume)
    {
        if (!isReadyToChangeVolume) return;

        float dB = volume > 0.0001f ? (Mathf.Log10(volume) + 0.3f) * 40 : -80f;
        audioMixer.SetFloat("BGM", dB);
    }

    public void SetSFXVolume(float volume)
    {
        if (!isReadyToChangeVolume) return;

        float dB = volume > 0.0001f ? (Mathf.Log10(volume) + 0.3f) * 40 : -80f;
        audioMixer.SetFloat("SFX", dB);
    }
}