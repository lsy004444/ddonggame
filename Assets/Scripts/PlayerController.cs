using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private float screenWidth;
    //플레이어 이동범위 제한
    public float minX = -2.3f;
    public float maxX = 2.3f;
    void Start()
    {
        screenWidth = Screen.width;
    }

    void Update()
    {
        Vector2 move = Keyboard.current != null ? 
            new Vector2(
                (Keyboard.current.rightArrowKey.isPressed ? 1 : 0) - 
                (Keyboard.current.leftArrowKey.isPressed ? 1 : 0), 0) 
            : Vector2.zero;

        transform.Translate(new Vector3(move.x, 0, 0) * moveSpeed * Time.deltaTime);

       

        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            float touchX = touch.position.x;

            if(touchX < screenWidth / 2)
            {
                transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
            }
            else
            {
                transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
            }
        }

        //이동 범위 제한
        float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }
}
