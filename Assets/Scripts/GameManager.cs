using UnityEngine;
using TMPro; // TextMeshProUGUI 사용을 위해 필수!
using UnityEngine.SceneManagement; // 씬 전환 및 재시작을 위해 필수!

public class GameManager : MonoBehaviour
{
    // 쟌님의 대문자 Instance
    public static GameManager Instance { get; private set; }

    // ★ [오류 해결 꿀팁] 팀원의 소문자 instance 호출이 에러 나지 않도록 연결고리(브릿지)를 만들어줍니다!
    public static GameManager instance => Instance;

    [Header("UI 패널 설정")]
    public GameObject settingsPanel;  // 인스펙터에서 SettingsPanel 드래그앤드롭
    public GameObject gameOverPanel;  // 인스펙터에서 GameOverPanel 드래그앤드롭
    public TextMeshProUGUI finalScoreText; // GameOverPanel 내부에 있는 최종 점수 텍스트 연결

    [Header("타이머 설정")]
    public float playTimeLimit = 360f; // 한판 6분 (GDD 기준)
    private float currentTime;
    private bool gameOver = false;

    [Header("타이머 UI 연결")]
    public TextMeshProUGUI timerText; 

    // --- 💡 팀원 코드에서 가져온 변수들 ---
    [Header("팀원 미니게임 설정 (똥 카운트)")]
    public int poopCount = 0;
    // 쟌님의 UI 스타일에 맞추어 TextMeshProUGUI로 변경했습니다.
    // (만약 팀원이 구형 3D TextMesh를 꼭 써야 한다면 TextMesh로 바꾸셔도 됩니다)
    public TextMeshProUGUI poopCountText; 

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // 팀원이 엔딩 씬("EndingScene")으로 화면을 넘기는 방식을 쓴다면 
            // GameManager가 파괴되지 않아야 하므로 이 라인을 켜두는 것이 안전합니다.
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 1. 게임이 시작되는 순간 모든 팝업창을 강제로 꺼줍니다.
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        
        // 2. 타이머 시간 및 게임 상태 초기화
        currentTime = playTimeLimit;
        Time.timeScale = 1f; 
        gameOver = false;
        poopCount = 0; // 게임 재시작 시 똥 개수도 초기화
    }

    private void Update()
    {
        if (!gameOver && currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerUI();

            if (currentTime <= 0)
            {
                currentTime = 0;
                gameOver = true;
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

    // --- 💡 팀원 코드에서 가져온 똥 추가 함수 ---
    public void AddPoop(int amount)
    {
        poopCount += amount;
        if (poopCountText != null)
            poopCountText.text = "똥: " + poopCount;
        
        Debug.Log("똥 개수 추가됨: " + poopCount);
    }

    // 6분 타이머 종료 혹은 게임 오버 시 호출될 함수
    public void EndGame()
    {
        Time.timeScale = 0f; // 게임을 일시정지 상태로 만듭니다.

        // 최종 점수 계산 (팀원의 poopCount 혹은 기존의 파리 개수 중 기획에 맞게 선택하세요!)
        int finalScore = poopCount; 
        if (ResourceManager.Instance != null)
        {
            finalScore = ResourceManager.Instance.GetPoopFliesCount();
        }

        // 1. 점수를 로컬에 안전하게 저장
        PlayerPrefs.SetInt("FinalScore", finalScore);

        // 2. 쟌님의 방식: 메인 씬에 배치한 게임오버 패널을 띄웁니다.
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = "최종 점수: " + finalScore.ToString();
        }

        // 3. 팀원의 방식: 만약 패널이 아니라 아예 새로운 엔딩 씬으로 이동하고 싶다면 아래 주석을 해제하세요!
        // SceneManager.LoadScene("EndingScene");

        Debug.Log("게임 오버! 최종 점수: " + finalScore);
    }

    // --- 버튼 이벤트 연결용 함수들 ---
    public void RetryGame()
    {
        Time.timeScale = 1f; // 일시정지 풀기
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 현재 메인씬 재시작
    }

    public void ShareScore()
    {
        int score = PlayerPrefs.GetInt("FinalScore", 0);
        string shareText = "나의 농장 방어 점수는 " + score + "점! 똥과 파리를 이겨내세요!";
        GUIUtility.systemCopyBuffer = shareText; 
        Debug.Log("클립보드에 복사 완료: " + shareText);
    }
}
