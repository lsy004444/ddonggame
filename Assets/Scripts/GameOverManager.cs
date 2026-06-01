using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // 씬 재시작을 위해 필수

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverPanel; // 게임오버 UI 패널
    public TextMeshProUGUI scoreText; // 최종 점수 텍스트
    private int finalScore = 1500; // 예시 점수

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true); // 창 띄우기
        scoreText.text = "최종 점수: " + finalScore.ToString();
        Time.timeScale = 0f; // 게임 일시정지 (선택 사항)
    }

    // "한 번 더 하기" 버튼 클릭 시 실행
    public void RetryGame()
    {
        Time.timeScale = 1f; // 시간 다시 흐르게 복구
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 현재 씬 다시 불러오기
    }

    // "점수 공유" 버튼 클릭 시 실행
    public void ShareScore()
    {
        string shareText = "나의 농장 방어 점수는 " + finalScore + "점! 똥과 파리를 이겨내세요!";
        
        // PC 테스트용: 클립보드에 텍스트 복사 (모바일은 Native Share 플러그인 권장)
        GUIUtility.systemCopyBuffer = shareText; 
        Debug.Log("클립보드에 복사되었습니다: " + shareText);
    }
}