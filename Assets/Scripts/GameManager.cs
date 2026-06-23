using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static GameManager instance => Instance;

    private bool isMiniGame = false;

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
    public int poopCount = 0;
    public TextMeshProUGUI poopCountText;
    public TextMesh poopCountTextMesh;

    [Header("바구니 설정")]
    [Tooltip("바구니가 가득 차는 최대 똥 개수입니다.")]
    public int maxBasketCapacity = 10;
    private FarmerVisual farmerVisual;

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
        Debug.Log("OnSceneLoaded 불림: " + scene.name);

        if (scene.name == "MiniGame")
        {
            isMiniGame = true;
            //currentTime = playTimeLimit;
            //gameOver = false;
            poopCount = 0;
            Time.timeScale = 1f;
            StartCoroutine(FindUIAfterLoad());
        }
        if (scene.name == "EndingScene")
        {
            isMiniGame = false;
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                mainCam.transform.position = new Vector3(0, 0, -10);
                mainCam.orthographicSize = 5;
                mainCam.clearFlags = CameraClearFlags.SolidColor;
                mainCam.backgroundColor = Color.white;
            }
            return;
        }
        if (scene.name == "HomeScene")
        {
            isMiniGame = false;
        }

        Debug.Log("ResourceManager: " + ResourceManager.Instance);
        if (ResourceManager.Instance != null)
        {
            Debug.Log("똥파리 수: " + ResourceManager.Instance.GetPoopFliesCount());
        }
    }

    private System.Collections.IEnumerator FindUIAfterLoad()
    {
        yield return null;
        poopCountTextMesh = GameObject.Find("PoopCountText")?.GetComponent<TextMesh>();
        poopCountText = GameObject.Find("PoopCountText")?.GetComponent<TextMeshProUGUI>(); 

        timerTextMesh = GameObject.Find("TimerText")?.GetComponent<TextMesh>();
        poopFliesTextMesh = GameObject.Find("PoopFliesText")?.GetComponent<TextMesh>();
        timerSlider = GameObject.Find("TimeSlider")?.GetComponent<Slider>();

        farmerVisual = FindAnyObjectByType<FarmerVisual>();
        if (farmerVisual != null)
        {
            farmerVisual.UpdateBasketVisual(poopCount, maxBasketCapacity);
        }

        Debug.Log("코루틴 실행됨. poopFliesTextMesh: " + poopFliesTextMesh + " / FarmerVisual: " + farmerVisual);

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
        isMiniGame = (SceneManager.GetActiveScene().name == "MiniGame");
        currentTime = playTimeLimit;
        gameOver = false;
        poopCount = 0;
        Time.timeScale = 1f;

        poopFliesTextMesh = GameObject.Find("PoopFliesText")?.GetComponent<TextMesh>();

        if (SceneManager.GetActiveScene().name == "MiniGame")
        {
            poopCountTextMesh = GameObject.Find("PoopCountText")?.GetComponent<TextMesh>();
            timerTextMesh = GameObject.Find("TimerText")?.GetComponent<TextMesh>();

            farmerVisual = FindAnyObjectByType<FarmerVisual>();
            if (farmerVisual != null) farmerVisual.UpdateBasketVisual(poopCount, maxBasketCapacity);
        }

        Debug.Log("GameManager Start() 호출됨 - 현재 씬: " + SceneManager.GetActiveScene().name);
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

        Debug.Log($"[데이터 연동] 미니게임 똥 {poopCount}개가 메인 데이터에 누적 저장되었습니다! (총: {savedPoop + poopCount}개)");

        

        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (finalScoreText != null) finalScoreText.text = "최종 점수: " + finalScore.ToString();

        Debug.Log("EndingScene으로 이동 시도");
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

        if (poopFliesTextMesh != null && ResourceManager.Instance != null)
            poopFliesTextMesh.text = "똥파리: " + ResourceManager.Instance.GetPoopFliesCount();
    }

    public void AddPoop(int amount)
    {
        poopCount += amount;
        if (poopCount < 0) poopCount = 0;

        if (poopCountText != null) poopCountText.text = "똥: " + poopCount;
        if (poopCountTextMesh != null) poopCountTextMesh.text = "똥: " + poopCount;

        if (farmerVisual != null)
        {
            farmerVisual.UpdateBasketVisual(poopCount, maxBasketCapacity);
        }

        Debug.Log("똥 개수 추가됨: " + poopCount);
    }

    public void RetryGame()
    {
        isMiniGame = true;
        currentTime = playTimeLimit;
        gameOver=false;
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

    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }
}