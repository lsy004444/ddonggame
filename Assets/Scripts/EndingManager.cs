
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingManager : MonoBehaviour
{
    public TextMesh resultText;
    public TextMesh scoreText;
    public TextMesh highScoreText;
    public TextMesh poopFliesResultText;

    public Sprite happyFarmer;
    public Sprite sadFarmer;
    public Sprite sosoFarmer;
    public SpriteRenderer farmerImage;


    //엔딩화면 구현
    //수집한 똥 총 개수
    void Start()
    {
        int poop = GameManager.instance.poopCount;
        int poopFlies = ResourceManager.Instance != null ? ResourceManager.Instance.GetPoopFliesCount() : 0;
        int finalScore = (poop * 10) + poopFlies;

        if (resultText != null )resultText.text = "수집한 똥: " + poop;
        if (scoreText != null) scoreText.text = "점수: " + (poop * 100);
        if (poopFliesResultText != null) poopFliesResultText.text = "똥파리: " + poopFlies;

        int bestScore = PlayerPrefs.GetInt("BestScore", 0);
        if (finalScore > bestScore)
        {
            PlayerPrefs.SetInt("BestScore", finalScore);
            if (highScoreText != null ) highScoreText.text ="★ 최고 기록 갱신! ★";
            if (farmerImage != null) farmerImage.sprite = happyFarmer;
        }
        else if (finalScore > bestScore * 0.7f )
        {
            if(highScoreText != null) highScoreText.text = "최고 기록: " + bestScore;
            if(farmerImage != null) farmerImage.sprite = sosoFarmer;
        }
        else
        {
            if (highScoreText != null) highScoreText.text = "최고 기록: " + bestScore;
            if (farmerImage != null) farmerImage.sprite = sadFarmer;
        }
    }

    //게임 재시작 버튼
    public void RestartGame()
    {
        if(ResourceManager.Instance != null)
        {
            int carry = Mathf.FloorToInt(ResourceManager.Instance.GetPoopFliesCount() * 0.2f);
            PlayerPrefs.SetInt("CarryOverPoopFlies", carry);
        }
        SceneManager.LoadScene("MiniGame");
    }
}
