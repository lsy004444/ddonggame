
using UnityEngine;

public class PoopController : MonoBehaviour
{
    public float fallSpeed = 3f;

    void update()
    {
        transform.position += Vector3.down * fallSpeed * Time.deltaTime;

        if(transform.position.y < -6f)
        {
            Destroy(gameObject);
        }
    }
}
