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

    [Header("상태 변수 (★FarmManager 접근을 위해 public 유지)")]
    // FarmManager가 접근할 수 있도록 public은 유지하되, 인스펙터 혼선을 줄이기 위해 HideInInspector를 적용했습니다.
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
            // currentActiveSeed(현재 작물)의 rewardAmount(보상 수량)를 똥파리로 지급
            ResourceManager.Instance.AddPoopFlies(currentActiveSeed.rewardAmount); 
            Debug.Log($"<color=cyan>[보상 지급]</color> {currentActiveSeed.seedName} 수확 완료! 똥파리 {currentActiveSeed.rewardAmount}마리 획득!");
        }
        else
        {
            Debug.LogError("ResourceManager.Instance를 찾을 수 없습니다! 하이어라키에 ResourceManager가 있는지 확인하세요.");
        }
        //경작 bgm 추가
        if (SFXManager.Instance != null)
        SFXManager.Instance.PlayHarvest(); 

        currentActiveSeed = null;
        isHarvestable = false;

        if (plantText != null) plantText.enabled = false;
        if (cropImage != null) cropImage.gameObject.SetActive(false); 
        if (harvestIcon != null) harvestIcon.SetActive(false);
        if (growthSlider != null) growthSlider.gameObject.SetActive(false);
    }

    // ===================================================================
    // [완벽 복구] FarmManager가 미니게임에서 복귀할 때 호출하여 UI 및 연출을 복구하는 함수
    // ===================================================================
    public void RestoreSlot(SeedData savedSeed, float savedTime, bool savedHarvestable)
    {
        currentActiveSeed = savedSeed;
        plantedTime = savedTime;
        isHarvestable = savedHarvestable;

        if (currentActiveSeed != null)
        {
            // 1. 텍스트 UI 및 성장 연출 위치(Y축) 복구
            if (plantText != null)
            {
                plantText.text = currentActiveSeed.seedName;
                plantText.enabled = true;
                if (textRect != null) 
                    textRect.anchoredPosition = new Vector2(textRect.anchoredPosition.x, isHarvestable ? endY : startY);
            }

            // 2. 작물 이미지 오브젝트 및 진행도별 스프라이트 복구
            if (cropImage != null)
            {
                cropImage.gameObject.SetActive(true);
                cropImage.enabled = true;
                cropImage.color = Color.white;

                if (currentActiveSeed.growthSprites != null && currentActiveSeed.growthSprites.Length > 0)
                {
                    if (isHarvestable)
                    {
                        // 이미 완전히 다 자란 상태라면 마지막 성장 스프라이트로 고정
                        cropImage.sprite = currentActiveSeed.growthSprites[currentActiveSeed.growthSprites.Length - 1];
                    }
                    else
                    {
                        // 미니게임에 가 있던 시간 동안 실시간으로 자란 진행도를 계산해서 알맞은 스프라이트 매칭
                        float elapsed = Time.time - plantedTime;
                        float targetGrowthTime = currentActiveSeed.growthTime <= 0 ? 1f : currentActiveSeed.growthTime;
                        float progress = Mathf.Clamp01(elapsed / targetGrowthTime);
                        
                        int growthStage = Mathf.Min(currentActiveSeed.growthSprites.Length - 1, Mathf.FloorToInt(progress * currentActiveSeed.growthSprites.Length));
                        cropImage.sprite = currentActiveSeed.growthSprites[growthStage];
                    }
                }
            }

            // 3. 수확 가능 여부에 따라 슬라이더 혹은 수확 아이콘 켜기
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

            Debug.Log($"<color=green>[FarmSlot {myIndex}]</color> 복구 성공! UI 연출 및 성장 스프라이트 재동기화 완료.");
        }
    }
}