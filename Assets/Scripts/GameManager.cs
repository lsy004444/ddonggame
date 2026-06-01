using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static GameManager instance => Instance;

    [Header("UI 패널 설정")]
    public GameObject settingsPanel;
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;

    [Header("타이머 설정")]
    public float playTimeLimit = 360f;
    private float currentTime;
    private bool gameOver = false;

    [Header("타이머 UI")]
    public TextMeshProUGUI timerText;
    public TextMesh timerTextMesh;

    [Header("똥 카운트")]
    public int poopCount = 0;
    public TextMeshProUGUI poopCountText;
    public TextMesh poopCountTextMesh;

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

    void Start()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        currentTime = playTimeLimit;
        Time.timeScale = 1f;
        gameOver = false;
        poopCount = 0;
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
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        string time = string.Format("{0:00}:{1:00}", minutes, seconds);
        if (timerText != null) timerText.text = time;
        if (timerTextMesh != null) timerTextMesh.text = time;
    }

    public void AddPoop(int amount)
    {
        poopCount += amount;
        if (poopCountText != null) poopCountText.text = "똥: " + poopCount;
        if (poopCountTextMesh != null) poopCountTextMesh.text = "똥: " + poopCount;
        Debug.Log("똥 개수 추가됨: " + poopCount);
    }

    public void EndGame()
    {
        Time.timeScale = 0f;
        int finalScore = poopCount;
        PlayerPrefs.SetInt("FinalScore", finalScore);
        SceneManager.LoadScene("EndingScene");
        Debug.Log("게임 오버! 최종 점수: " + finalScore);
    }

    public void RetryGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ShareScore()
    {
        int score = PlayerPrefs.GetInt("FinalScore", 0);
        string shareText = "나의 농장 방어 점수는 " + score + "점!";
        GUIUtility.systemCopyBuffer = shareText;
        Debug.Log("클립보드에 복사 완료: " + shareText);
    }
}