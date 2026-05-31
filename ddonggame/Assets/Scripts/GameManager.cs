using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("타이머 설정")]
    public float playTimeLimit = 360f; // 한판 6분 (GDD 기준)
    private float currentTime;

    [Header("UI 연결")]
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

    private void Start()
    {
        currentTime = playTimeLimit;
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

    private void EndGame()
    {
        if (ResourceManager.Instance != null)
        {
            int finalScore = ResourceManager.Instance.GetPoopFliesCount();
            PlayerPrefs.SetInt("FinalScore", finalScore);
        }
        else
        {
            PlayerPrefs.SetInt("FinalScore", 0);
        }

        Debug.Log("6분 시간 종료! 결과 화면으로 이동합니다.");
        SceneManager.LoadScene("ResultScene");
    }
}