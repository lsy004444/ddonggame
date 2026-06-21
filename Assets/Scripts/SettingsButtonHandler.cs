using UnityEngine;

public class SettingsButtonHandler : MonoBehaviour {
    // 유니티 인스펙터 창에서 연결해둔 SettingsManager
    public SettingsManager settingsManager; 

    // 마우스로 이 2D 오브젝트(콜라이더 영역)를 클릭했을 때 실행되는 함수
    private void OnMouseDown()
    {
        if (settingsManager != null)
        {
            // ⭐️ 메인화면 온클릭에 연결했던 그 함수를 코드로 직접 실행!
            settingsManager.OpenSettings();
        }
        else
        {
            Debug.LogWarning("SettingsButt 오브젝트에 SettingsManager가 연결되어 있지 않습니다!");
        }
    }
}
