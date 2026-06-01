using UnityEngine;
using UnityEngine.UI;

public class CropGrowth : MonoBehaviour
{
    [Header("UI 연결 (인스펙터에서 드래그)")]
    public Image plantImage;       // 작물 이미지 오브젝트
    public GameObject clickText;   // 'Click' 텍스트 오브젝트 (GameObject여야 함)
    
    private SeedData currentCrop; 
    private int currentStage = 0; 

    private void Start()
    {
        // 처음엔 이미지 끄고, 텍스트 켬
        if (plantImage != null) plantImage.gameObject.SetActive(false);
        if (clickText != null) clickText.SetActive(true);
    }

    public void Plant(SeedData data)
    {
        if (currentStage != 0) return;

        currentCrop = data;
        currentStage = 1;

        // 심는 순간 텍스트 숨기고, 이미지 보이기
        if (clickText != null) clickText.SetActive(false);
        if (plantImage != null) plantImage.gameObject.SetActive(true);
        
        UpdateCropVisual();
    }

    public void Grow()
    {
        if (currentCrop == null || currentStage >= 3) return;
        currentStage++;
        UpdateCropVisual();
    }

    public void Harvest()
    {
        if (currentStage != 3) return;
        
        // 수확 후 텍스트 다시 켜기
        if (clickText != null) clickText.SetActive(true);
        if (plantImage != null) plantImage.gameObject.SetActive(false);
        
        currentCrop = null;
        currentStage = 0;
    }

    private void UpdateCropVisual()
    {
        if (currentCrop != null && currentCrop.growthSprites.Length > currentStage)
        {
            plantImage.sprite = currentCrop.growthSprites[currentStage];
        }
    }
}