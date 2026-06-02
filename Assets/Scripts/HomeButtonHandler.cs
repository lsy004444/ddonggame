using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeButtonHandler : MonoBehaviour
{
    private void OnMouseUp()
    {
        SceneManager.LoadScene("HomeScene");
    }
}