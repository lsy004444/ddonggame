using UnityEngine;
using UnityEngine.SceneManagement; // 씬 관리를 위한 필수 라이브러리

public class SceneLoader : MonoBehaviour
{
    /// <summary>
    /// 버튼에서 호출할 씬 전환 함수입니다.
    /// 인스펙터 창의 OnClick()에서 전환할 씬 이름을 직접 입력할 수 있습니다.
    /// </summary>
    public void LoadScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            Debug.Log(sceneName + " 씬으로 이동합니다.");
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("씬 이름이 비어있습니다! 이름을 확인해 주세요.");
        }
    }

    /// <summary>
    /// 홈 화면으로 바로 이동하는 편리한 함수입니다.
    /// </summary>
    public void GoToHome()
    {
        SceneManager.LoadScene("HomeScene");
    }

    /// <summary>
    /// 미니게임 화면으로 바로 이동하는 편리한 함수입니다.
    /// </summary>
    public void GoToMiniGame()
    {
        SceneManager.LoadScene("MiniGameScene");
    }
}

