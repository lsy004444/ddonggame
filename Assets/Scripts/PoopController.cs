
using UnityEngine;

public class PoopController : MonoBehaviour
{
    public float fallSpeed = 8f;
    public PoopType poopType;

    void Update()
    {
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        Camera cam = Camera.main;
        float bottomY = -cam.orthographicSize - 1f;
        if (transform.position.y < bottomY)
        {
            Destroy(gameObject);
        }
    }
}