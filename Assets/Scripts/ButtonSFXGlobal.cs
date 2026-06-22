using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonSFXGlobal : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        RegisterButtons();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RegisterButtons();
    }

    void RegisterButtons()
    {
        Button[] allButtons = FindObjectsOfType<Button>(true);
        foreach (Button btn in allButtons)
        {
            btn.onClick.RemoveListener(PlaySound); // 중복 방지
            btn.onClick.AddListener(PlaySound);
        }
    }

    void PlaySound()
    {
        if (SFXManager.Instance != null)
            SFXManager.Instance.PlayButtonClick();
    }
}