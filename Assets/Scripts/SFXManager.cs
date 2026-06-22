using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance { get; private set; }

    public AudioSource sfxSource;

    public AudioClip buttonClickClip;
    public AudioClip poopCatchClip;
    public AudioClip harvestClip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayButtonClick()
    {
        //확인용
        Debug.Log("버튼 클릭 사운드 재생 시도"); 
        if (sfxSource != null && buttonClickClip != null)
            sfxSource.PlayOneShot(buttonClickClip);
    }

    public void PlayPoopCatch()
    {
        if (sfxSource != null && poopCatchClip != null)
            sfxSource.PlayOneShot(poopCatchClip);
    }

    public void PlayHarvest()
    {
        if (sfxSource != null && harvestClip != null)
            sfxSource.PlayOneShot(harvestClip);
    }
}