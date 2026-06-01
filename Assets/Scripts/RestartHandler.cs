using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartHandler : MonoBehaviour
{
    private void OnMouseDown()
    {
        SceneManager.LoadScene("MiniGame");
    }
}