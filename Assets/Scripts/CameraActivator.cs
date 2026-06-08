using UnityEngine;

public class CameraActivator : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Camera>().enabled = true;
    }
}