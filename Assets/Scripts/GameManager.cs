using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static GameManager instance => Instance;

    private bool isRetrying = false;

    [Header("UI 패널 설정")]
    public GameObject settingsPanel;
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;

    [Header("타이머 설정")]
    public float playTimeLimit = 360f;
    private float currentTime;
    private bool gameOver = false;

    [Header("타이머 슬라이더")]
    public Slider timerSlider;

    [Header("타이머 UI (TextMeshPro / 일반 Text)")]
    public TextMeshProUGUI timerText;
    public TextMesh timerTextMesh;

    [Header("똥 & 파리 카운트 UI")]
    public TextMesh poopFliesTextMesh;
    public TextMeshProUGUI poopFliesText;
    public int poopCount = 0;
    public TextMeshProUGUI poopCountText;
    public TextMesh poopCountTextMesh;

    [Header("바구니 설정")]
    public int maxBasketCapacity = 10;
    private FarmerVisual farmerVisual;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            currentTime = playTimeLimit;
            gameOver = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private bool IsPlayableScene(string sceneName)
    {
        return sceneName == "MiniGame" || sceneName == "HomeScene";
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Time.timeScale = 1f;

        // 재시작(한번더)일 때만 타이머/똥카운트/도감 리셋
        if (isRetrying && scene.name == "MiniGame")
        {
            currentTime = playTimeLimit;
            gameOver = false;
            poopCount = 0;
            isRetrying = false;

            if (ResourceManager.Instance != null)
                ResourceManager.Instance.ResetDiscoveredData();
        }

        if (scene.name == "EndingScene")
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                mainCam.transform.position = new Vector3(0, 0, -10);
                mainCam.orthographicSize = 5;
                mainCam.clearFlags = CameraClearFlags.SolidColor;
                mainCam.backgroundColor = Color.white;
            }
            return; // 엔딩에서는 UI 재탐색 불필요
        }

        if (IsPlayableScene(scene.name))
        {
            StartCoroutine(FindUIAfterLoad());
        }
    }

    private System.Collections.IEnumerator FindUIAfterLoad()
    {
        yield return null;

        poopCountTextMesh = GameObject.Find("PoopCountText")?.GetComponent<TextMesh>();
        poopCountText = GameObject.Find("PoopCountText")?.GetComponent<TextMeshProUGUI>();
        timerTextMesh = GameObject.Find("TimerText")?.GetComponent<TextMesh>();
        timerText = GameObject.Find("TimerText")?.GetComponent<TextMeshProUGUI>();
        poopFliesTextMesh = GameObject.Find("PoopFliesText")?.GetComponent<TextMesh>();
        timerSlider = GameObject.Find("TimeSlider")?.GetComponent<Slider>();

        if (timerSlider != null)
        {
            timerSlider.maxValue = playTimeLimit;
            timerSlider.value = currentTime; 
        }

        farmerVisual = FindAnyObjectByType<FarmerVisual>();
        if (farmerVisual != null)
        {
            farmerVisual.UpdateBasketVisual(poopCount, maxBasketCapacity);
        }

        RefreshAllUI();
    }

    private void RefreshAllUI()
    {
        if (poopCountText != null) poopCountText.text = "똥: " + poopCount;
        if (poopCountTextMesh != null) poopCountTextMesh.text = "똥: " + poopCount;

        int flies = ResourceManager.Instance != null ? ResourceManager.Instance.GetPoopFliesCount() : 0;
        if (poopFliesTextMesh != null) poopFliesTextMesh.text = "똥파리: " + flies;
        if (poopFliesText != null) poopFliesText.text = "똥파리: " + flies;

        UpdateTimerUI();
    }
    private void Update()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (!gameOver && IsPlayableScene(currentScene) && currentTime > 0)
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

    public void EndGame()
    {
        if (gameOver) return;
        gameOver = true;
        Time.timeScale = 1f;

        int finalScore = poopCount;
        PlayerPrefs.SetInt("FinalScore", finalScore);

        int savedPoop = PlayerPrefs.GetInt("UnhealthyPoop", 0);
        PlayerPrefs.SetInt("UnhealthyPoop", savedPoop + poopCount);
        PlayerPrefs.Save();

        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (finalScoreText != null) finalScoreText.text = "최종 점수: " + finalScore.ToString();

        SceneManager.LoadScene("EndingScene");
    }

    private void UpdateTimerUI()
    {
        if (timerSlider != null)
            timerSlider.value = currentTime;

        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        string time = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (timerText != null) timerText.text = time;
        if (timerTextMesh != null) timerTextMesh.text = time;
    }

    public void AddPoop(int amount)
    {
        poopCount += amount;
        if (poopCount < 0) poopCount = 0;

        RefreshAllUI();

        if (farmerVisual != null)
        {
            farmerVisual.UpdateBasketVisual(poopCount, maxBasketCapacity);
        }
    }

    // EndingManager의 "한번더" 버튼이 이거 호출
    public void RetryGame()
    {
        isRetrying = true;
        SceneManager.LoadScene("MiniGame");
       
    }

    public void ShareScore()
    {
        int score = PlayerPrefs.GetInt("FinalScore", 0);
        string shareText = "나의 농장 방어 점수는 " + score + "점!";
        GUIUtility.systemCopyBuffer = shareText;
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }
}