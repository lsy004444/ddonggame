using UnityEngine;
using UnityEngine.SceneManagement; 

public class SceneLoader : MonoBehaviour
{
    /// <summary>
    /// 버튼에서 호출할 씬 전환 함수입니다. (인스펙터 창에서 이름 입력 가능)
    /// </summary>
    public void LoadScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            // [안전장치] 어떤 씬으로 가든 가기 전에 무조건 현재 밭 상태를 강제 저장!
            if (FarmManager.Instance != null)
            {
                FarmManager.Instance.SaveFarmData();
            }

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
        // 홈으로 갈 때도 혹시 모르니 데이터 세이브 유도
        if (FarmManager.Instance != null)
        {
            FarmManager.Instance.SaveFarmData();
        }
        SceneManager.LoadScene("HomeScene");
    }

    /// <summary>
    /// 미니게임 화면으로 바로 이동하는 편리한 함수입니다.
    /// </summary>
    public void GoToMiniGame()
    {
        // 미니게임 가기 직전에 밭 매니저의 데이터를 확실하게 먼저 수동 백업!
        if (FarmManager.Instance != null)
        {
            FarmManager.Instance.SaveFarmData();
            // 타이밍 버그 방지를 위해 오브젝트도 안전하게 꺼줍니다.
            FarmManager.Instance.gameObject.SetActive(false); 
        }

        // [수정 완료] GameManager 시스템과 일치하도록 씬 이름을 "MiniGame"으로 로드합니다.
        SceneManager.LoadScene("MiniGame");
    }
}