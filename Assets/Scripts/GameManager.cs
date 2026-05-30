
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int poopCount = 0;

    //플레이타임 6 분으로 설정
    public float gameTime = 360f;
    private bool gameOver = false;
    //똥 개수 
    public TextMesh poopCountText;
    //플레이타임 설정
    public TextMesh timerText;

    void Awake()
    {
        if(instance == null)
            instance = this;
    }

    void Update()
    {
        if(!gameOver)
        {
            gameTime -= Time.deltaTime;

            if(gameTime <= 0f)
            {
                gameTime = 0f;
                gameOver = true;
                EndGame();
            }

            int minutes = (int)(gameTime / 60);
            int seconds = (int)(gameTime % 60);
            timerText.text = string.Format("{0:00} : {1:00}", minutes, seconds);
        }
    }

    public void AddPoop(int amount)
    {
        poopCount += amount;
        poopCountText.text = "똥: " + poopCount;
        Debug.Log("똥 개수: " + poopCount);
    }

    void EndGame()
    {
        SceneManager.LoadScene("EndingScene");
    }
}
