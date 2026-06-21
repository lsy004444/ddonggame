using UnityEngine;
using TMPro; // TextMeshProUGUI 사용을 위해 필수!
using UnityEngine.SceneManagement; // 씬 전환 및 재시작을 위해 필수!
using System.Collections.Generic;

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

    [Header("타이머 UI (TextMeshPro / 일반 Text)")]
    public TextMeshProUGUI timerText;
    public TextMesh timerTextMesh;

    [Header("똥 & 파리 카운트 UI")]
    public TextMesh poopFliesTextMesh;
    public int poopCount = 0;
    public TextMeshProUGUI poopCountText;
    public TextMesh poopCountTextMesh;

    [Header("바구니 설정")]
    [Tooltip("바구니가 가득 차는 최대 똥 개수입니다. 인스펙터에서 수정 가능합니다.")]
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
        if (scene.name == "MiniGame")
        {
            StartCoroutine(FindUIAfterLoad());
        }

        Debug.Log("poopFliesTextMesh: " + poopFliesTextMesh);
        Debug.Log("ResourceManager: " + ResourceManager.Instance);
        if (ResourceManager.Instance != null)
        {
            Debug.Log("똥파리 수: " + ResourceManager.Instance.GetPoopFliesCount());
        }
    }

    private System.Collections.IEnumerator FindUIAfterLoad()
    {
        yield return null; // 한 프레임 기다려서 씬 오브젝트들이 완전히 로드된 후 찾기
        poopCountTextMesh = GameObject.Find("PoopCountText")?.GetComponent<TextMesh>();
        timerTextMesh = GameObject.Find("TimerText")?.GetComponent<TextMesh>();
        poopFliesTextMesh = GameObject.Find("PoopFliesText")?.GetComponent<TextMesh>();
        
        // [수정 완료] 경고 억제를 위해 최신 FindAnyObjectByType 사용 및 비주얼 동기화
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
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        poopFliesTextMesh = GameObject.Find("PoopFliesText")?.GetComponent<TextMesh>();

        currentTime = playTimeLimit;
        gameOver = false;
        poopCount = 0;
        Time.timeScale = 1f;
        
        if (SceneManager.GetActiveScene().name == "MiniGame")
        {   
            poopCountTextMesh = GameObject.Find("PoopCountText")?.GetComponent<TextMesh>();
            timerTextMesh = GameObject.Find("TimerText")?.GetComponent<TextMesh>();
            
            // [수정 완료] 최신 FindAnyObjectByType 사용 및 시작 시 바구니 초기화
            farmerVisual = FindAnyObjectByType<FarmerVisual>();
            if (farmerVisual != null) farmerVisual.UpdateBasketVisual(poopCount, maxBasketCapacity);
        }
       
        Debug.Log("timerTextMesh: " + timerTextMesh);
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
        
        if (poopFliesTextMesh != null && ResourceManager.Instance != null)
            poopFliesTextMesh.text = "똥파리: " + ResourceManager.Instance.GetPoopFliesCount();
    }

    public void AddPoop(int amount)
    {
        poopCount += amount;
        if (poopCountText != null) poopCountText.text = "똥: " + poopCount;
        if (poopCountTextMesh != null) poopCountTextMesh.text = "똥: " + poopCount;
        
        if (farmerVisual != null)
        {
            farmerVisual.UpdateBasketVisual(poopCount, maxBasketCapacity);
        }

        Debug.Log("똥 개수 추가됨: " + poopCount);
    }

    public void EndGame()
    {
        gameOver = true;
        Time.timeScale = 1f;
        int finalScore = poopCount;
        PlayerPrefs.SetInt("FinalScore", finalScore);

        // 미니게임에서 모은 똥을 메인 데이터에 누적 저장
        int savedPoop = PlayerPrefs.GetInt("UnhealthyPoop", 0);
        PlayerPrefs.SetInt("UnhealthyPoop", savedPoop + poopCount); 
        PlayerPrefs.Save();

        Debug.Log($"[데이터 연동] 미니게임 똥 {poopCount}개가 메인 데이터에 누적 저장되었습니다! (총: {savedPoop + poopCount}개)");

        if (ResourceManager.Instance != null)
        {
            // ★ 기획하신 보상 종류에 맞춰 "HealthyPoop" 또는 "UnHealthyPoop"으로 이름을 맞춰주세요!
            ResourceManager.Instance.AddPoopByName("HealthyPoop", poopCount); 
        }
        else
        {
            Debug.LogError("ResourceManager 인스턴스를 찾을 수 없어 미니게임 데이터가 연동되지 않았습니다.");
        }
        
        // 엔딩 씬으로 가기 전 현재 화면에 UI 패널을 띄우고 싶다면 아래 코드가 작동합니다.
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (finalScoreText != null) finalScoreText.text = "최종 점수: " + finalScore.ToString();

        // 엔딩 씬으로 전환
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