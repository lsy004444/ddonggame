using UnityEngine;
using UnityEngine.UI; // UI 요소를 사용하기 위해 추가

public class FarmSlot : MonoBehaviour
{
    public CropData currentCropData; // 현재 심어진 작물의 데이터
    public float plantedTime; // 작물이 심어진 시간 (Time.time)
    public Image cropImage; // 작물 이미지를 표시할 UI Image 컴포넌트

    // 이 슬롯이 비어있는지 확인하는 속성
    public bool IsEmpty => currentCropData == null;

    // 작물을 심는 메서드 (2주차에 구현 예정)
    public void PlantCrop(CropData crop) 
    {
        currentCropData = crop;
        plantedTime = Time.time;
        // 초기 작물 이미지 설정 (성장 단계 0)
        if (cropImage != null && currentCropData.growthSprites.Length > 0)
        {
            cropImage.sprite = currentCropData.growthSprites[0];
            cropImage.enabled = true; // 이미지를 보이게 함
        }
    }

    // 작물을 수확하는 메서드 (2주차에 구현 예정)
    public void HarvestCrop()
    {
        // 수확 로직 (예: 자원 증가, 슬롯 초기화)
        currentCropData = null;
        plantedTime = 0;
        if (cropImage != null)
        {
            cropImage.enabled = false; // 이미지를 숨김
        }
    }

    // 작물 성장 상태 업데이트 (2주차에 구현 예정)
    public void UpdateGrowth()
    {
        if (!IsEmpty && currentCropData.growthTime > 0)
        {
            float elapsed = Time.time - plantedTime;
            float progress = elapsed / currentCropData.growthTime;

            int currentStage = Mathf.FloorToInt(progress * currentCropData.growthSprites.Length);
            currentStage = Mathf.Clamp(currentStage, 0, currentCropData.growthSprites.Length - 1);

            if (cropImage != null && currentCropData.growthSprites.Length > 0)
            {
                cropImage.sprite = currentCropData.growthSprites[currentStage];
            }
        }
    }
}