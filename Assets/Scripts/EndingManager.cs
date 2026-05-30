using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingManager : MonoBehaviour
{
    public TextMesh resultText;
    //엔딩화면 구현
    //수집한 똥 총 개수
    void Start()
    {
        resultText.text = "수집한 똥: " + GameManager.instance.poopCount;
    }

    //게임 재시작 버튼
    public void RestartGame()
    {
        SceneManager.LoadScene("MiniGame");
    }
}
