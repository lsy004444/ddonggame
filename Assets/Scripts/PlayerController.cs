using UnityEngine;
using UnityEngine.InputSystem; // New Input System 전용

public class PlayerController : MonoBehaviour
{
    [Header("이동 속도 설정")]
    public float moveSpeed = 10f; // 속도는 프로젝트 기획에 맞게 조절하세요!

    private float screenWidth;
    private Camera mainCamera;

    void Start()
    {
        screenWidth = Screen.width;
        mainCamera = Camera.main; // Camera.main을 캐싱하여 성능 최적화
    }

    void Update()
    {
        // 1. 카메라 화면 경계 계산 (팀원 코드의 화면 탈출 방지용)
        float camWidth = mainCamera.orthographicSize * mainCamera.aspect;
        float minX = -camWidth + 0.5f; // 캐릭터 크기를 고려한 왼쪽 벽 제한
        float maxX = camWidth - 0.5f;  // 캐릭터 크기를 고려한 오른쪽 벽 제한

        float moveX = 0f;
        bool isAbsoluteMovement = false;
        float targetX = transform.position.x;

        // 2. 키보드 입력 처리 (방향키)
        if (Keyboard.current != null)
        {
            moveX += (Keyboard.current.rightArrowKey.isPressed ? 1f : 0f) -
                     (Keyboard.current.leftArrowKey.isPressed ? 1f : 0f);
        }

        // 3. 모바일 터치 입력 처리 (지안님의 화면 반할 이동 시스템 최적화)
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            // 터치한 스크린의 X 좌표 구하기
            float touchX = Touchscreen.current.primaryTouch.position.x.ReadValue();
            
            if (touchX < screenWidth / 2f)
            {
                moveX = -1f; // 화면 왼쪽 터치 시 왼쪽 이동 방향 지정
            }
            else
            {
                moveX = 1f;  // 화면 오른쪽 터치 시 오른쪽 이동 방향 지정
            }
        }

        // 4. 마우스 입력 처리 (PC 에디터 테스트용 클릭 이동)
        // 키보드나 터치 입력이 없을 때 마우스 왼쪽 클릭이 들어오면 좌표를 계산해 이동시킵니다.
        if (moveX == 0f && Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(
                Mouse.current.position.x.ReadValue(),
                Mouse.current.position.y.ReadValue(), 0f));
            
            targetX = Mathf.Clamp(worldPos.x, minX, maxX);
            isAbsoluteMovement = true;
        }

        // 5. 최종 이동 연산 및 적용
        if (isAbsoluteMovement)
        {
            // 마우스 클릭 위치로 부드럽게 이동 (팀원 방식)
            transform.position = new Vector3(
                Mathf.MoveTowards(transform.position.x, targetX, moveSpeed * Time.deltaTime),
                transform.position.y,
                transform.position.z);
        }
        else if (moveX != 0f)
        {
            // 키보드 및 반할 터치 방향에 따른 등속 이동 (지안님 방식)
            transform.Translate(new Vector3(moveX, 0f, 0f) * moveSpeed * Time.deltaTime);
        }

        // 6. 플레이어 위치 최종 스크린 제한 오버랩 방지
        float clampedX = Mathf.Clamp(transform.position.x, minX, maxX);
        transform.position = new Vector3(clampedX, transform.position.y, transform.position.z);
    }
}