using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10f;
    private float screenWidth;
    private bool isDragging = false;

    void Start()
    {
        screenWidth = Screen.width;
    }

    void Update()
    {
        Camera cam = Camera.main;
        float camWidth = cam.orthographicSize * cam.aspect;
        float minX = -camWidth + 0.5f;
        float maxX = camWidth - 0.5f;

        // 키보드 입력
        if (Keyboard.current != null)
        {
            float moveX = (Keyboard.current.rightArrowKey.isPressed ? 1 : 0) -
                          (Keyboard.current.leftArrowKey.isPressed ? 1 : 0);
            transform.Translate(new Vector3(moveX, 0, 0) * moveSpeed * Time.deltaTime);
        }

        // 마우스 입력 (에디터 테스트용)
        if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame &&
                (EventSystem.current == null || !EventSystem.current.IsPointerOverGameObject()))
            {
                Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(
                    Mouse.current.position.x.ReadValue(),
                    Mouse.current.position.y.ReadValue(), 0));
                Collider2D hit = Physics2D.OverlapPoint(worldPos);
                if (hit != null && hit.gameObject == gameObject)
                {
                    isDragging = true;
                }
            }

            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                isDragging = false;
            }

            if (isDragging)
            {
                Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(
                    Mouse.current.position.x.ReadValue(),
                    Mouse.current.position.y.ReadValue(), 0));
                float targetX = Mathf.Clamp(worldPos.x, minX, maxX);
                transform.position = new Vector3(
                    Mathf.MoveTowards(transform.position.x, targetX, moveSpeed * Time.deltaTime),
                    transform.position.y, transform.position.z);
            }
        }

        // 터치 입력 (모바일)
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;
            int touchId = touch.touchId.ReadValue();
            bool overUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touchId);

            if (touch.press.wasPressedThisFrame && !overUI)
            {
                Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(
                    touch.position.x.ReadValue(), touch.position.y.ReadValue(), 0));
                Collider2D hit = Physics2D.OverlapPoint(worldPos);
                if (hit != null && hit.gameObject == gameObject)
                {
                    isDragging = true;
                }
            }

            if (touch.press.wasReleasedThisFrame)
            {
                isDragging = false;
            }

            if (isDragging && touch.press.isPressed)
            {
                Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(
                    touch.position.x.ReadValue(), touch.position.y.ReadValue(), 0));
                float targetX = Mathf.Clamp(worldPos.x, minX, maxX);
                transform.position = new Vector3(
                    Mathf.MoveTowards(transform.position.x, targetX, moveSpeed * Time.deltaTime),
                    transform.position.y, transform.position.z);
            }
        }

        // 이동 범위 제한
        float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }

    // 바구니 확대용
    public void SetBasketScale(float multiplier)
    {
        transform.localScale = Vector3.one * multiplier;
    }
}