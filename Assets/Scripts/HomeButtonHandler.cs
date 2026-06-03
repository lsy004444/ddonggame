using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class HomeButtonHandler : MonoBehaviour
{
   private void Start()
{
    GetComponent<Button>().onClick.AddListener(() => {
        Debug.Log("홈버튼 클릭됨");
        SceneManager.LoadScene("HomeScene");
    });
}
}