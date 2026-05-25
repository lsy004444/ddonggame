using UnityEngine;
using UnityEngine.UI;

public class FarmSlot : MonoBehaviour
{
    [Header("Data References")]
    public SeedData currentSeedData; // 현재 심어진 씨앗 데이터
    public Image cropImage;          // 작물 이미지를 보여줄 UI Image

    [Header("Growth Status")]
    public float plantedTime;        // 심어진 시점의 시간
    public bool isHarvestable = false; // 수확 가능 여부

    // 슬롯이 비어있는지 확인하는 속성 (대문자 I)
    public bool IsEmpty => currentSeedData == null;

    private void Update()
    {
        // 매 프레임 성장을 업데이트합니다.
        UpdateGrowth();
    }

    /// <summary>
    /// 밭을 클릭했을 때 실행될 함수입니다. (버튼 OnClick에 연결)
    /// </summary>
    public void OnSlotClicked()
    {
        if (IsEmpty)
        {
            // 1. 비어있다면 심기 시도
            PlantPoop();
        }
        else if (isHarvestable)
        {
            // 2. 다 자랐다면 수확 시도
            Harvest();
        }
        else
        {
            Debug.Log("작물이 자라는 중입니다...");
        }
    }

    /// <summary>
    /// 리소스 매니저로부터 똥을 받아와 랜덤한 씨앗을 심습니다.
    /// </summary>
    public void PlantPoop()
    {
        // 리소스 매니저에게 보유 중인 똥 하나를 요청
        PoopType poopToPlant = ResourceManager.Instance.GetAnyPoopToPlant();

        if (poopToPlant == null)
        {
            Debug.Log("심을 똥이 하나도 없습니다! 미니게임을 통해 똥을 모아오세요.");
            return;
        }

        if (poopToPlant.possibleSeeds == null || poopToPlant.possibleSeeds.Count == 0)
        {
            Debug.LogError($"{poopToPlant.poopName} 에셋에 연결된 씨앗(Possible Seeds)이 없습니다!");
            return;
        }

        // 똥에 설정된 씨앗 리스트 중 하나를 랜덤으로 결정
        int randomIndex = Random.Range(0, poopToPlant.possibleSeeds.Count);
        currentSeedData = poopToPlant.possibleSeeds[randomIndex];
        
        plantedTime = Time.time;
        isHarvestable = false;
        
        // 초기 시각적 업데이트 (0단계)
        UpdateVisual(0);
        Debug.Log($"{poopToPlant.poopName}을 사용하여 {currentSeedData.seedName}을 심었습니다!");
    }

    /// <summary>
    /// 시간에 따른 성장 단계를 계산하고 이미지를 업데이트합니다.
    /// </summary>
    public void UpdateGrowth()
    {
        if (IsEmpty || isHarvestable) return;

        float elapsed = Time.time - plantedTime;
        float progress = elapsed / currentSeedData.growthTime;

        // 성장 단계 계산 (0 ~ 이미지 개수-1)
        int currentStage = Mathf.FloorToInt(progress * currentSeedData.growthSprites.Length);
        currentStage = Mathf.Clamp(currentStage, 0, currentSeedData.growthSprites.Length - 1);

        UpdateVisual(currentStage);

        // 성장이 완료되었는지 체크
        if (progress >= 1.0f)
        {
            isHarvestable = true;
            Debug.Log($"{currentSeedData.seedName}이(가) 모두 자랐습니다! 클릭하여 수확하세요.");
        }
    }

    /// <summary>
    /// 작물 이미지를 현재 단계에 맞게 변경합니다.
    /// </summary>
    private void UpdateVisual(int stage)
    {
        if (cropImage != null && currentSeedData != null && currentSeedData.growthSprites.Length > stage)
        {
            cropImage.sprite = currentSeedData.growthSprites[stage];
            cropImage.enabled = true;
        }
    }

    /// <summary>
    /// 작물을 수확하고 보상을 지급한 뒤 슬롯을 초기화합니다.
    /// </summary>
    public void Harvest()
    {
        if (isHarvestable && currentSeedData != null)
        {
            // 씨앗 데이터에 설정된 보상 똥 종류와 개수를 지급
            ResourceManager.Instance.AddPoop(currentSeedData.rewardPoopType, currentSeedData.rewardAmount);
            
            Debug.Log($"{currentSeedData.seedName} 수확 완료! 보상이 지급되었습니다.");

            // 슬롯 초기화
            currentSeedData = null;
            isHarvestable = false;
            if (cropImage != null)
            {
                cropImage.enabled = false;
            }
        }
    }
}
