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
        //확인용 주석 추가
        Debug.Log("OnSceneLoaded 불림: " + scene.name);
        Debug.Log("씬 로드됨: " + scene.name + " isMiniGame: " + isMiniGame); 
        if(scene.name == "MiniGame")
            {
                isMiniGame = true;
                currentTime = playTimeLimit;
                gameOver = false;
                poopCount = 0;
                Time.timeScale = 1f;
                Debug.Log("MiniGame 리셋됨 currentTime: " + currentTime + " isMiniGame: " + isMiniGame);
                StartCoroutine(FindUIAfterLoad());
            }
        if(scene.name == "EndingScene")
            {
                isMiniGame = false;
                Camera mainCam = Camera.main;
                if(mainCam != null)
                {
                    mainCam.transform.position = new Vector3(0, 0, -10);
                    mainCam.orthographicSize = 5;
                    mainCam.clearFlags = CameraClearFlags.SolidColor;
                    mainCam.backgroundColor = Color.white;
                }
                return;
            }
        if(scene.name == "HomeScene")
        {
            isMiniGame = false;
        }
        
        Debug.Log("ResourceManager: " + ResourceManager.Instance);
        Debug.Log("똥파리 수: " + ResourceManager.Instance?.GetPoopFliesCount());
    }

    private System.Collections.IEnumerator FindUIAfterLoad()
    {
        yield return null;
    poopCountTextMesh = GameObject.Find("PoopCountText")?.GetComponent<TextMesh>();
    poopFliesTextMesh = GameObject.Find("PoopFliesText")?.GetComponent<TextMesh>();
    timerSlider = GameObject.Find("TimeSlider")?.GetComponent<Slider>(); // ← 추가
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

        poopFliesTextMesh = GameObject.Find("PoopFliesText")?.GetComponent<TextMesh>();

        currentTime = playTimeLimit;
        gameOver = false;
        poopCount = 0;
        Time.timeScale = 1f;

        if (SceneManager.GetActiveScene().name == "MiniGame")
        {
            poopCountTextMesh = GameObject.Find("PoopCountText")?.GetComponent<TextMesh>();
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

    PlayerPrefs.SetInt("FinalScore", poopCount);
    int savedPoop = PlayerPrefs.GetInt("UnhealthyPoop", 0);
    PlayerPrefs.SetInt("UnhealthyPoop", savedPoop + poopCount);
    PlayerPrefs.Save();

   

    Debug.Log("EndingScene으로 이동 시도");
    SceneManager.LoadScene("EndingScene");
}

    private void UpdateTimerUI()
    {
        if(timerSlider != null)
            timerSlider.value = currentTime;
        if (poopFliesTextMesh != null && ResourceManager.Instance != null)
            poopFliesTextMesh.text = "똥파리: " + ResourceManager.Instance.GetPoopFliesCount();
    }

    public void AddPoop(int amount)
    {
        poopCount += amount;
        if (poopCount < 0 ) poopCount = 0;
        
        if (poopCountText != null) poopCountText.text = "" + poopCount;
        if (poopCountTextMesh != null) poopCountTextMesh.text = "" + poopCount;
        Debug.Log("똥 개수 추가됨: " + poopCount);
    }

    
    public void RetryGame()
    {
        isMiniGame = true;
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

    //setting 창 열었을 때 게임 멈추기
    public void PauseGame()
    {
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
    }
}