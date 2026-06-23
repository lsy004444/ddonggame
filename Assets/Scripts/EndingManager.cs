using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class EndingManager : MonoBehaviour
{
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI poopFliesResultText;

    public Sprite happyFarmer;
    public Sprite sadFarmer;
    public Sprite sosoFarmer;
    public Image farmerImage;

    void Start()
    {
        int poop = GameManager.instance != null ? GameManager.instance.poopCount : 0;
        int poopFlies = ResourceManager.Instance != null ? ResourceManager.Instance.GetPoopFliesCount() : 0;
        int finalScore = (poop * 10) + poopFlies;

        if (resultText != null) resultText.text = "Poop: " + poop;
        if (scoreText != null) scoreText.text = "Score: " + (poop * 100);
        if (poopFliesResultText != null) poopFliesResultText.text = "Flies: " + poopFlies;

        int bestScore = PlayerPrefs.GetInt("BestScore", 0);
        if (finalScore > bestScore)
        {
            PlayerPrefs.SetInt("BestScore", finalScore);
            if (highScoreText != null) highScoreText.text = "★ New Record!★";
            if (farmerImage != null) farmerImage.sprite = happyFarmer;
        }
        else if (finalScore > bestScore * 0.7f)
        {
            if (highScoreText != null) highScoreText.text = "Best: " + bestScore;
            if (farmerImage != null) farmerImage.sprite = sosoFarmer;
        }
        else
        {
            if (highScoreText != null) highScoreText.text = "Best: " + bestScore;
            if (farmerImage != null) farmerImage.sprite = sadFarmer;
        }
    }

    public void ShareScore()
    {
        int score = PlayerPrefs.GetInt("BestScore", 0);
        string shareText = "똥모아태산 점수: " + score + "점!";
        GUIUtility.systemCopyBuffer = shareText;
        Debug.Log("클립보드 복사: " + shareText);
    }

    public void RestartGame()
    {
        if (ResourceManager.Instance != null)
        {
            int carry = Mathf.FloorToInt(ResourceManager.Instance.GetPoopFliesCount() * 0.2f);
            PlayerPrefs.SetInt("CarryOverPoopFlies", carry);
            ResourceManager.Instance.ResetDiscoveredData();
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RetryGame();   // ← SceneManager.LoadScene 직접 호출 대신 이걸로 교체
        }
        else
        {
            SceneManager.LoadScene("MiniGame");
        }
    }
}