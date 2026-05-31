using UnityEngine;

public class SettingsButtonHandler : MonoBehaviour
{
    public SettingsManager settingsManager;

    private void OnMouseDown()
    {
        settingsManager.OpenSettings();
    }
}
