using UnityEngine;
using TMPro; // TextMeshProUGUI 사용을 위해 필수!
using UnityEngine.SceneManagement; // 씬 재시작(한번더)을 위해 필수!

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI 패널 설정")]
    public GameObject settingsPanel;  // 인스펙터에서 SettingsPanel 드래그앤드롭
    public GameObject gameOverPanel;  // 인스펙터에서 GameOverPanel 드래그앤드롭
    public TextMeshProUGUI finalScoreText; // GameOverPanel 내부에 있는 최종 점수 텍스트 연결

    [Header("타이머 설정")]
    public float playTimeLimit = 360f; // 한판 6분 (GDD 기준)
    private float currentTime;

    [Header("타이머 UI 연결")]
    public TextMeshProUGUI timerText; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 중복되던 Start()를 하나로 예쁘게 합쳤습니다!
    void Start()
    {
        // 1. 게임이 시작되는 순간 모든 팝업창을 강제로 꺼줍니다.
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        
        // 2. 타이머 시간 초기화 및 시간 흐르게 하기
        currentTime = playTimeLimit;
        Time.timeScale = 1f; 
    }

    private void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerUI();

            if (currentTime <= 0)
            {
                currentTime = 0;
                EndGame();
            }
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    // 6분 타이머 종료 혹은 게임 오버 시 호출될 함수
    public void EndGame()
    {
        Time.timeScale = 0f; // 게임을 일시정지 상태로 만듭니다.

        int finalScore = 0;
        if (ResourceManager.Instance != null)
        {
            //ResourceManager에서 파리(또는 점수) 개수를 가져옵니다.
            finalScore = ResourceManager.Instance.GetPoopFliesCount();
        }

        // 1. 점수를 로컬에 안전하게 저장 (나중에 활용 가능)
        PlayerPrefs.SetInt("FinalScore", finalScore);

        // 2. [수정] 다른 씬으로 안 가고, 메인 씬에 배치한 게임오버 패널을 띄웁니다!
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // 3. 게임 오버 창에 있는 텍스트에 최종 점수를 그려줍니다.
        if (finalScoreText != null)
        {
            finalScoreText.text = "최종 점수: " + finalScore.ToString();
        }

        Debug.Log("게임 오버! 최종 점수: " + finalScore);
    }

    // --- 버튼 이벤트 연결용 함수들 ---

    // 1. "한 번 더 하기" 버튼에 연결할 함수
    public void RetryGame()
    {
        Time.timeScale = 1f; // 일시정지 풀기
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 현재 메인씬 재시작
    }

    // 2. "점수 공유" 버튼에 연결할 함수
    public void ShareScore()
    {
        int score = PlayerPrefs.GetInt("FinalScore", 0);
        string shareText = "나의 농장 방어 점수는 " + score + "점! 똥과 파리를 이겨내세요!";
        
        // PC/에디터 테스트용 클립보드 복사
        GUIUtility.systemCopyBuffer = shareText; 
        Debug.Log("클립보드에 복사 완료: " + shareText);
    }
}