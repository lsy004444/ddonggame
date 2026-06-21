using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    // 유니티 인스펙터창에서 연결할 설정창 패널 오브젝트
    public GameObject settingsPanel;

    // 설정창을 여는 함수 (SettingsButtonHandler나 버튼 이벤트에서 호출됨)
    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true); // 패널 켜기
            Debug.Log("설정창이 열렸습니다.");
        }
        else
        {
            Debug.LogWarning("settingsPanel이 연결되지 않았습니다!");
        }
    }

    // 설정창을 닫는 함수
    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false); // 패널 끄기
            Debug.Log("설정창이 닫혔습니다.");
        }
    }
}