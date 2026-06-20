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
    // FarmManager가 접근할 수 있도록 2개의 핵심 변수를 public으로 열고, 인스펙터에 안 보이게 처리했습니다.
    [HideInInspector] public SeedData currentActiveSeed;
    [HideInInspector] public float plantedTime;
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

        if (cropImage != null)
        {
            if (currentActiveSeed.growthSprites != null && currentActiveSeed.growthSprites.Length > 0)
            {
                cropImage.gameObject.SetActive(true); 
                cropImage.enabled = true;             
                cropImage.color = Color.white;        
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
        float targetGrowthTime = currentActiveSeed.growthTime <= 0 ? 1f : currentActiveSeed.growthTime;
        float progress = elapsed / targetGrowthTime;
        progress = Mathf.Clamp01(progress);

        if (growthSlider != null) growthSlider.value = progress;

        if (plantText != null && textRect != null)
        {
            float currentY = Mathf.Lerp(startY, endY, progress);
            textRect.anchoredPosition = new Vector2(textRect.anchoredPosition.x, currentY);
        }

        if (cropImage != null && currentActiveSeed.growthSprites != null && currentActiveSeed.growthSprites.Length > 0)
        {
            int growthStage = Mathf.Min(currentActiveSeed.growthSprites.Length - 1, Mathf.FloorToInt(progress * currentActiveSeed.growthSprites.Length));
            if (cropImage.sprite != currentActiveSeed.growthSprites[growthStage])
            {
                cropImage.sprite = currentActiveSeed.growthSprites[growthStage];
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
        if (cropImage != null) cropImage.gameObject.SetActive(false); 
        if (harvestIcon != null) harvestIcon.SetActive(false);
        if (growthSlider != null) growthSlider.gameObject.SetActive(false);
    }

    // [완벽 복구 뼈대 추가] FarmManager가 이 함수를 원격 호출하여 기존 상태로 조립해줍니다.
    public void RestoreSlot(SeedData savedSeed, float savedPlantedTime, bool savedHarvestable)
    {
        currentActiveSeed = savedSeed; // 이제 IsEmpty가 자동으로 false가 됩니다!
        plantedTime = savedPlantedTime;
        isHarvestable = savedHarvestable;

        // 1. 텍스트 복구
        if (plantText != null && currentActiveSeed != null)
        {
            plantText.text = currentActiveSeed.seedName;
            plantText.enabled = true;
            if (textRect != null) 
                textRect.anchoredPosition = new Vector2(textRect.anchoredPosition.x, isHarvestable ? endY : startY);
        }

        // 2. 오브젝트 및 이미지 스프라이트 복구
        if (cropImage != null && currentActiveSeed != null && currentActiveSeed.growthSprites != null && currentActiveSeed.growthSprites.Length > 0)
        {
            cropImage.gameObject.SetActive(true);
            cropImage.enabled = true;
            cropImage.color = Color.white;

            if (isHarvestable)
            {
                cropImage.sprite = currentActiveSeed.growthSprites[currentActiveSeed.growthSprites.Length - 1];
            }
            else
            {
                float elapsed = Time.time - plantedTime;
                float targetGrowthTime = currentActiveSeed.growthTime <= 0 ? 1f : currentActiveSeed.growthTime;
                float progress = Mathf.Clamp01(elapsed / targetGrowthTime);
                int growthStage = Mathf.Min(currentActiveSeed.growthSprites.Length - 1, Mathf.FloorToInt(progress * currentActiveSeed.growthSprites.Length));
                cropImage.sprite = currentActiveSeed.growthSprites[growthStage];
            }
        }

        // 3. 슬라이더 및 수확 아이콘 UI 복구
        if (isHarvestable)
        {
            if (growthSlider != null) growthSlider.gameObject.SetActive(false);
            if (harvestIcon != null) harvestIcon.SetActive(true);
        }
        else
        {
            if (growthSlider != null) growthSlider.gameObject.SetActive(true);
            if (harvestIcon != null) harvestIcon.SetActive(false);
        }
    }
}