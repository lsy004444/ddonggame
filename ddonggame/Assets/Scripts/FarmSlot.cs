using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using UnityEngine.EventSystems; // 마우스 좌/우클릭 멀티 레이아웃 감지 필수 라이브러리

public class FarmSlot : MonoBehaviour, IPointerClickHandler
{
    [Header("설정 및 인덱스")]
    public int myIndex; // FarmManager에서 시작 시 순서대로 0~8 배정함

    [Header("UI 연결")]
    public TextMeshProUGUI plantText; // 기존 'PlantImage' 역할의 텍스트 오브젝트 (고추, 토마토, 벼 이름 출력)
    public Slider growthSlider;      
    public GameObject harvestIcon;   

    [Header("성장 연출 설정 (Y축 이동)")]
    public float startY = -50f;      // 자라나기 전 땅 밑 시작 Y 위치
    public float endY = 0f;          // 다 자랐을 때 최종 Y 위치
    private RectTransform textRect;

    [Header("상태 변수")]
    private SeedData currentActiveSeed;
    private float plantedTime;
    public bool isHarvestable = false;

    public bool IsEmpty => currentActiveSeed == null;

    private void Awake()
    {
        if (plantText != null)
        {
            textRect = plantText.GetComponent<RectTransform>();
            plantText.enabled = false; 
        }
        if (growthSlider != null) growthSlider.gameObject.SetActive(false);
        if (harvestIcon != null) harvestIcon.SetActive(false);
    }

    // 마우스 클릭 이벤트 인터페이스 감지 (유니티 버튼 컴포넌트 없이 작동)
    public void OnPointerClick(PointerEventData eventData)
    {
        // 1. 마우스 좌클릭: 똥 소모 및 작물 심기 요청
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (IsEmpty && FarmManager.Instance != null)
            {
                Debug.Log($"<color=cyan>[FarmSlot {myIndex}]</color> 좌클릭 감지 -> FarmManager에 심기 요청 전달");
                FarmManager.Instance.OnSlotClicked(myIndex);
            }
        }
        // 2. 마우스 우클릭: 다 자란 작물 수확 요청
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (isHarvestable)
            {
                Debug.Log($"<color=cyan>[FarmSlot {myIndex}]</color> 우클릭 감지 -> 수확 연산 시작");
                Harvest();
            }
        }
    }

    // ResourceManager에서 무작위 주머니 섞기로 엄선되어 뽑힌 똥 데이터를 매개변수로 받습니다.
    public void PlantPoop(PoopType poop)
    {
        if (poop == null || poop.possibleSeeds == null || poop.possibleSeeds.Count == 0)
        {
            Debug.LogWarning($"[FarmSlot {myIndex}] 주머니에 보유 중인 똥이 없어 심기가 취소되었습니다.");
            return; 
        }

        int randomIndex = Random.Range(0, poop.possibleSeeds.Count);
        currentActiveSeed = poop.possibleSeeds[randomIndex];

        plantedTime = Time.time;
        isHarvestable = false;

        if (plantText != null)
        {
            plantText.text = currentActiveSeed.seedName;
            plantText.enabled = true;
            
            if (textRect != null)
            {
                textRect.anchoredPosition = new Vector2(textRect.anchoredPosition.x, startY);
            }
        }

        if (growthSlider != null)
        {
            growthSlider.value = 0f;
            growthSlider.gameObject.SetActive(true);
        }
        if (harvestIcon != null) harvestIcon.SetActive(false);

        Debug.Log($"<color=green>[FarmSlot {myIndex}]</color> 주머니에서 진짜 똥 소모 완료! " +
                  $"꺼낸 똥: <color=orange>[{poop.poopName}]</color> ➡️ 심긴 작물: <color=yellow>[{currentActiveSeed.seedName}]</color>");
    }

    public void UpdateGrowth()
    {
        if (IsEmpty || isHarvestable || currentActiveSeed == null) return;

        float elapsed = Time.time - plantedTime;
        float progress = elapsed / currentActiveSeed.growthTime;
        progress = Mathf.Clamp01(progress);

        if (growthSlider != null)
        {
            growthSlider.value = progress;
        }

        if (plantText != null)
        {
            plantText.text = currentActiveSeed.seedName;

            if (textRect != null)
            {
                float currentY = Mathf.Lerp(startY, endY, progress);
                textRect.anchoredPosition = new Vector2(textRect.anchoredPosition.x, currentY);
            }
        }

        if (progress >= 1.0f)
        {
            isHarvestable = true;
            if (growthSlider != null) growthSlider.gameObject.SetActive(false);
            if (harvestIcon != null) harvestIcon.SetActive(true);
            Debug.Log($"<color=green>★성장 완료!★</color> {myIndex}번 밭 작물이 다 자랐습니다. 우클릭하여 수확하세요.");
        }
    }

    private void Update()
    {
        UpdateGrowth();
    }

    public void Harvest()
    {
        if (currentActiveSeed == null) return;

        if (ResourceManager.Instance != null)
        {
            ResourceManager.Instance.AddPoopFlies(currentActiveSeed.rewardAmount);
        }

        currentActiveSeed = null;
        isHarvestable = false;

        if (plantText != null) plantText.enabled = false;
        if (harvestIcon != null) harvestIcon.SetActive(false);
        if (growthSlider != null)
        {
            growthSlider.gameObject.SetActive(false);
        }
    }
}