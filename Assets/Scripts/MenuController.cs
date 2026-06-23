using UnityEngine;
using UnityEngine.SceneManagement; // 씬 전환을 위해 필수!

public class MenuController : MonoBehaviour
{
    // 유니티 에디터에서 가이드 팝업창(GuidePopup) 오브젝트를 드래그해서 연결할 변수입니다.
    public GameObject guidePopup; 

    // [기존 코드] 게임 시작 버튼을 눌렀을 때 실행되는 함수
    public void ClickGameStart()
    {
        //게임시작 버튼 클릭 시 미니게임으로 이동
        SceneManager.LoadScene("MiniGame"); 
    }

    // 가이드 버튼(책 모양)을 눌렀을 때 팝업창을 켜는 함수
    public void ClickOpenGuide()
    {
        if (guidePopup != null)
        {
            guidePopup.SetActive(true); // 팝업창 활성화(켜기)
        }
    }

    // 팝업창 내부의 닫기 버튼(X)을 눌렀을 때 팝업창을 끄는 함수
    public void ClickCloseGuide()
    {
        if (guidePopup != null)
        {
            guidePopup.SetActive(false); // 팝업창 비활성화(끄기)
        }
    }
}