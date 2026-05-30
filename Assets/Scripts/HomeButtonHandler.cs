using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeButtonHandler : MonoBehaviour
{
    private void OnMouseDown()
    {
        SceneManager.LoadScene("HomeScreen");
    }
}
