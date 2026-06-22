using UnityEngine;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    public GameObject settingsPanel;
    public AudioMixer audioMixer;

    //게임 시작 볼륨 고정
    void Start()
    {
        SetMasterVolume(0.5f);
        SetBGMVolume(0.5f);
        SetSFXVolume(0.5f);
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
        if (GameManager.instance != null)
            GameManager.instance.ResumeGame();
    }

    public void SetBGMVolume(float volume)
    {
        float dB = volume > 0.0001f ? (Mathf.Log10(volume) + 0.3f) * 40 : -80f;
        audioMixer.SetFloat("BGM", dB);
    }

    public void SetSFXVolume(float volume)
    {
        float dB = volume > 0.0001f ? (Mathf.Log10(volume) + 0.3f) * 40 : -80f;
        audioMixer.SetFloat("SFX", dB);
    }

    public void SetMasterVolume(float volume)
    {
        //확인용 로그
        Debug.Log("Master 볼륨 변경 시도: " + volume + " / dB 변환값: " + (volume > 0.0001f ? Mathf.Log10(volume) * 20 : -80f));
        float dB = volume > 0.0001f ? ( Mathf.Log10(volume) + 0.3f) * 40 : -80f;
        audioMixer.SetFloat("Master", dB);
    }
}