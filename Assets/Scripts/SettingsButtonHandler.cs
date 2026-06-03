using UnityEngine;
using UnityEngine.UI;

public class SettingsButtonHandler : MonoBehaviour
{
    public SettingsManager settingsManager;

    private void Start()
{
    GetComponent<Button>().onClick.AddListener(() => {
        Debug.Log("설정버튼 클릭됨");
        settingsManager.OpenSettings();
    });
}
}
