using UnityEngine;

public class PoopController : MonoBehaviour
{
    public float fallSpeed = 3f;
    public PoopType poopType;

    void Update()
    {
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        if (transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }
}