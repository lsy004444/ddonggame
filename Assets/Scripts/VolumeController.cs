using UnityEngine;
using UnityEngine.Audio; // 오디오 믹서 사용을 위해 필수
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    public AudioMixer audioMixer;

    // 슬라이더의 OnValueChanged에 연결할 함수들
    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("Master", Mathf.Log10(volume) * 20); // 데시벨 변환 공식
    }
}