using UnityEngine;
using UnityEngine.UI;

public class FarmSlot : MonoBehaviour
{
    public int myIndex; // 0, 1, 2...
    public Image cropImage;          
    public Slider growthSlider;      
    public GameObject harvestIcon;   

    private SeedData currentActiveSeed;
    private float plantedTime;
    public bool isHarvestable = false;

    // 매니저가 "비었니?" 물어볼 때 답하는 변수
    public bool IsEmpty => currentActiveSeed == null;

    // 클릭하면 매니저에게 알림
    public void OnClick()
    {
        if (FarmManager.Instance != null)
            FarmManager.Instance.OnSlotClicked(myIndex);
    }

    // 매니저가 심으라고 할 때 실행
    public void PlantPoop(PoopType poop)
    {
        if (poop == null || poop.possibleSeeds.Count == 0) return;

        int randomIndex = Random.Range(0, poop.possibleSeeds.Count);
        currentActiveSeed = poop.possibleSeeds[randomIndex];

        plantedTime = Time.time;
        isHarvestable = false;
        
        cropImage.sprite = currentActiveSeed.growthSprites[0];
        cropImage.enabled = true;
        if(growthSlider != null) growthSlider.gameObject.SetActive(true);
        if(harvestIcon != null) harvestIcon.SetActive(false);
    }

    public void UpdateGrowth()
    {
        if (IsEmpty || isHarvestable) return;

        float elapsed = Time.time - plantedTime;
        float progress = elapsed / currentActiveSeed.growthTime;

        if (growthSlider != null) growthSlider.value = Mathf.Clamp01(progress);

        int spriteIndex = Mathf.FloorToInt(progress * currentActiveSeed.growthSprites.Length);
        spriteIndex = Mathf.Clamp(spriteIndex, 0, currentActiveSeed.growthSprites.Length - 1);
        cropImage.sprite = currentActiveSeed.growthSprites[spriteIndex];

        if (progress >= 1.0f)
        {
            isHarvestable = true;
            if (growthSlider != null) growthSlider.gameObject.SetActive(false);
            if (harvestIcon != null) harvestIcon.SetActive(true);
        }
    }

    public void Harvest()
    {
        if (currentActiveSeed == null) return;
        ResourceManager.Instance.AddPoop(null, currentActiveSeed.rewardAmount);
        currentActiveSeed = null;
        cropImage.enabled = false;
        isHarvestable = false;
        if (harvestIcon != null) harvestIcon.SetActive(false);
    }
}
