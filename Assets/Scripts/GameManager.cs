using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Resources;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static GameManager instance => Instance;

    [Header("UI 패널 설정")]
    public GameObject settingsPanel;
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;

    [Header ("타이머 설정")]
    public float playTimeLimit = 360f;
    private float currentTime;
    private bool gameOver = false;

    [Header ("타이머 슬라이더")]
    public Slider timerSlider;

    [Header("똥 카운트")]
    public TextMesh poopFliesTextMesh;
    public int poopCount = 0;
    public TextMeshProUGUI poopCountText;
    public TextMesh poopCountTextMesh;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
        if(scene.name == "MiniGame")
        {
            // poopCountTextMesh = GameObject.Find("PoopCountText")?.GetComponent<TextMesh>();
            // timerTextMesh = GameObject.Find("TimerText")?.GetComponent<TextMesh>();
            // poopFliesTextMesh = GameObject.Find("PoopFliesText")?.GetComponent<TextMesh>();

            // if(poopFliesTextMesh != null && ResourceManager.Instance != null)
            // {
            //     poopFliesTextMesh.text = "똥파리: " + ResourceManager.Instance.GetPoopFliesCount();
            // }
            StartCoroutine(FindUIAfterLoad());
        }

        Debug.Log("poopFliesTextMesh: " + poopFliesTextMesh);
        Debug.Log("ResourceManager: " + ResourceManager.Instance);
        Debug.Log("똥파리 수: " + ResourceManager.Instance?.GetPoopFliesCount());
    }

    private System.Collections.IEnumerator FindUIAfterLoad()
    {
        yield return null; // 한 프레임 기다리기
        poopCountTextMesh = GameObject.Find("PoopCountText")?.GetComponent<TextMesh>();
        poopFliesTextMesh = GameObject.Find("PoopFliesText")?.GetComponent<TextMesh>();
        Debug.Log("코루틴 실행됨. poopFliesTextMesh: " + poopFliesTextMesh);
        if (poopFliesTextMesh != null && ResourceManager.Instance != null)
            poopFliesTextMesh.text = "똥파리: " + ResourceManager.Instance.GetPoopFliesCount();
    }

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

        poopFliesTextMesh = GameObject.Find("PoopFliesText")?.GetComponent<TextMesh>();

        currentTime = playTimeLimit;
        gameOver = false;
        poopCount = 0;
        Time.timeScale = 1f;
        
        if (SceneManager.GetActiveScene().name == "MiniGame")
        {   poopCountTextMesh = GameObject.Find("PoopCountText")?.GetComponent<TextMesh>();
            
        }
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
        if(timerSlider != null)
            timerSlider.value = currentTime;
        if (poopFliesTextMesh != null && ResourceManager.Instance != null)
        poopFliesTextMesh.text = "똥파리: " + ResourceManager.Instance.GetPoopFliesCount();
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
        gameOver = true;
        Time.timeScale = 1f;
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