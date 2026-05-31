using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingManager : MonoBehaviour
{
    public TextMesh resultText;
    public TextMesh scoreText;

    //엔딩화면 구현
    //수집한 똥 총 개수
    void Start()
    {
        int poop = GameManager.instance.poopCount;
        resultText.text = "수집한 똥: " + poop;
        scoreText.text = "점수: " + (poop * 100);
    }

    //게임 재시작 버튼
    public void RestartGame()
    {
        SceneManager.LoadScene("MiniGame");
    }
}
