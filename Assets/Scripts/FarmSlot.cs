using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class FarmSlot : MonoBehaviour, IPointerClickHandler
{
    [Header("설정 및 인덱스")]
    public int myIndex; 

    [Header("UI 연결")]
    public Image cropImage; 
    public TextMeshProUGUI plantText; 
    public Slider growthSlider;
    public GameObject harvestIcon;

    [Header("성장 연출 설정 (Y축 이동)")]
    public float startY = -50f;      
    public float endY = 0f;          
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

        // [수정] 컴포넌트가 아니라 게임 오브젝트 자체를 끕니다.
        if (cropImage != null) cropImage.gameObject.SetActive(false); 
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (IsEmpty && FarmManager.Instance != null)
            {
                FarmManager.Instance.OnSlotClicked(myIndex);
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (isHarvestable)
            {
                Harvest();
            }
        }
    }

    public void PlantPoop(PoopType poop)
    {
        if (poop == null || poop.possibleSeeds == null || poop.possibleSeeds.Count == 0) return;

        int randomIndex = Random.Range(0, poop.possibleSeeds.Count);
        currentActiveSeed = poop.possibleSeeds[randomIndex];

        plantedTime = Time.time;
        isHarvestable = false;

        if (plantText != null)
        {
            plantText.text = currentActiveSeed.seedName;
            plantText.enabled = true; 
            if (textRect != null) textRect.anchoredPosition = new Vector2(textRect.anchoredPosition.x, startY);
        }

        // [수정] 오브젝트를 확실하게 켜고 색상을 불투명하게 만듭니다.
        if (cropImage != null)
        {
            if (currentActiveSeed.growthSprites != null && currentActiveSeed.growthSprites.Length > 0)
            {
                cropImage.gameObject.SetActive(true); // 오브젝트 활성화
                cropImage.enabled = true;             // 컴포넌트 활성화
                cropImage.color = Color.white;        // 투명도 리셋 (Alpha 255)
                cropImage.sprite = currentActiveSeed.growthSprites[0]; 
                
                Debug.Log($"<color=orange>[FarmSlot {myIndex}]</color> 작물 이미지 오브젝트 켜짐. 초기 이미지: 0번");
            }
        }

        if (growthSlider != null)
        {
            growthSlider.value = 0f;
            growthSlider.gameObject.SetActive(true);
        }
        if (harvestIcon != null) harvestIcon.SetActive(false);
    }

    public void UpdateGrowth()
    {
        if (IsEmpty || isHarvestable || currentActiveSeed == null) return;

        float elapsed = Time.time - plantedTime;
        
        // 안전장치: 성장 시간이 0 이하이면 강제로 1초로 고정
        float targetGrowthTime = currentActiveSeed.growthTime <= 0 ? 1f : currentActiveSeed.growthTime;
        float progress = elapsed / targetGrowthTime;
        progress = Mathf.Clamp01(progress);

        if (growthSlider != null) growthSlider.value = progress;

        if (plantText != null && textRect != null)
        {
            float currentY = Mathf.Lerp(startY, endY, progress);
            textRect.anchoredPosition = new Vector2(textRect.anchoredPosition.x, currentY);
        }

        // 이미지 실시간 업데이트 + 디버그 로그 추가
        if (cropImage != null && currentActiveSeed.growthSprites != null && currentActiveSeed.growthSprites.Length > 0)
        {
            int growthStage = Mathf.Min(currentActiveSeed.growthSprites.Length - 1, Mathf.FloorToInt(progress * currentActiveSeed.growthSprites.Length));
            if (cropImage.sprite != currentActiveSeed.growthSprites[growthStage])
            {
                cropImage.sprite = currentActiveSeed.growthSprites[growthStage];
                // 이미지 바뀔 때마다 콘솔창에 신호를 보냅니다.
                Debug.Log($"<color=yellow>[FarmSlot {myIndex}]</color> 작물 성장 중! 현재 이미지 단계: {growthStage} (진행도: {progress * 100}%)");
            }
        }

        if (progress >= 1.0f)
        {
            isHarvestable = true;
            if (growthSlider != null) growthSlider.gameObject.SetActive(false);
            if (harvestIcon != null) harvestIcon.SetActive(true);
            Debug.Log($"<color=green>★성장 완료!★</color> {myIndex}번 밭 최종 단계 도달.");
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
        // currentActiveSeed(현재 작물)의 rewardAmount(보상 수량)를 똥파리로 지급
            ResourceManager.Instance.AddPoopFlies(currentActiveSeed.rewardAmount); 
            Debug.Log($"<color=cyan>[보상 지급]</color> {currentActiveSeed.seedName} 수확 완료! 똥파리 {currentActiveSeed.rewardAmount}마리 획득!");
        }
        else
        {
            Debug.LogError("ResourceManager.Instance를 찾을 수 없습니다! 하이어라키에 ResourceManager가 있는지 확인하세요.");
        }

        currentActiveSeed = null;
        isHarvestable = false;

        if (plantText != null) plantText.enabled = false;
        if (cropImage != null) cropImage.gameObject.SetActive(false); // 수확 시 오브젝트 끄기
        if (harvestIcon != null) harvestIcon.SetActive(false);
        if (growthSlider != null) growthSlider.gameObject.SetActive(false);
    }
}