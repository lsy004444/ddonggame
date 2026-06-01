using UnityEngine;
using UnityEngine.SceneManagement; // 씬 전환 및 재시작을 위해 필수!

public class HomeMenuManager : MonoBehaviour
{
    [Header("설정창 패널 연결")]
    public GameObject settingsPanel;

    void Start()
    {
        // 홈 씬이 시작될 때는 설정창을 안전하게 꺼둡니다.
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    // 1. 설정창 열기 (홈 씬의 '설정' 버튼에 연결)
    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    // 2. 설정창 닫기 (★설정창 내부의 'X' 또는 '돌아가기' 버튼에 연결)
    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    // 3. 다시 시작 / 게임 시작 (설정창의 '다시 시작' 버튼에 연결)
    public void RestartGame()
    {
        Time.timeScale = 1f; // 혹시 정지되어 있을지 모를 시간을 정상 복구
        
        // 현재 열려있는 홈 씬을 처음부터 다시 불러옵니다.
        // (만약 인게임 씬으로 바로 넘어가게 하고 싶다면 "MainScene" 등 씬 이름을 직접 적으셔도 됩니다)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // 4. 게임 종료 (설정창의 '게임 종료' 버튼에 연결)
    public void QuitGame()
    {
        Debug.Log("게임 종료 버튼이 눌렸습니다.");

        #if UNITY_EDITOR
        // 유니티 에디터에서 테스트 중일 때는 플레이 모드를 종료
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        // 실제 빌드된 앱(PC/모바일)에서는 프로그램 완전히 종료
        Application.Quit();
        #endif
    }
}