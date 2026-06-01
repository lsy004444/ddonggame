using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ResultManager : MonoBehaviour
{
    [Header("UI 연결")]
    public TextMeshProUGUI finalScoreText; 

    private void Start()
    {
        int score = PlayerPrefs.GetInt("FinalScore", 0);
        if (finalScoreText != null)
        {
            finalScoreText.text = $"{score} FLIES";
        }
    }

    public void OnRetryButtonClicked()
    {
        PlayerPrefs.DeleteKey("FinalScore");
        SceneManager.LoadScene("HomeScene");
    }
}