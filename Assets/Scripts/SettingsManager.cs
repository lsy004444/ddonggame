using UnityEngine;
using UnityEngine.Audio;


public class SettingsManager : MonoBehaviour {
    public GameObject settingsPanel;
    public AudioMixer audioMixer;

    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    public void SetBGMVolume(float volume)
    {
        audioMixer.SetFloat("BGM", volume);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFX", volume);
         }
}
