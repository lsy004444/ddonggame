using UnityEngine;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    public GameObject settingsPanel;
    public AudioMixer audioMixer;

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
        if (GameManager.instance != null)
            GameManager.instance.PauseGame();
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        if (GameManager.instance != null)
            GameManager.instance.ResumeGame();
    }

    public void SetBGMVolume(float volume)
    {
        float dB = volume > 0.0001f ? Mathf.Log10(volume) * 20 : -80f;
        audioMixer.SetFloat("BGM", dB);
    }

    public void SetSFXVolume(float volume)
    {
        float dB = volume > 0.0001f ? Mathf.Log10(volume) * 20 : -80f;
        audioMixer.SetFloat("SFX", dB);
    }

    public void SetMasterVolume(float volume)
    {
        float dB = volume > 0.0001f ? Mathf.Log10(volume) * 20 : -80f;
        audioMixer.SetFloat("Master", dB);
    }
}