using UnityEngine;
using System.Collections.Generic;

public class FarmManager : MonoBehaviour
{
    public static FarmManager Instance { get; private set; }

    [Header("--- 관리 대상 ---")]
    public List<FarmSlot> farmSlots = new List<FarmSlot>(); 
    public PoopType defaultPoopData; 

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; }
        if (farmSlots == null || farmSlots.Count == 0)
        {
            Debug.LogWarning("⚠️ FarmManager에 등록된 FarmSlot이 없습니다! 인스펙터에서 밭들을 등록해주세요.");
        }
    }

    public void OnSlotClicked(int slotIndex)
    {
        // 1. 리스트 범위 확인
        if (slotIndex < 0 || slotIndex >= farmSlots.Count) return;

        // 2. 슬롯이 실제로 연결되어 있는지 확인 
        FarmSlot slot = farmSlots[slotIndex];
        if (slot == null)
        {
            Debug.LogError($"<color=red>[Error]</color> {slotIndex}번 슬롯이 FarmManager 리스트에 연결되지 않았습니다! 인스펙터를 확인하세요.");
            return;
        }

        // 3. 리소스 매니저 존재 확인
        if (ResourceManager.Instance == null)
        {
            Debug.LogError("<color=red>[Error]</color> ResourceManager가 씬에 없습니다!");
            return;
        }

        if (slot.IsEmpty)
        {
            PoopType selectedPoop = ResourceManager.Instance.GetAnyPoopToPlant();
            if (selectedPoop == null) selectedPoop = defaultPoopData;

            if (selectedPoop != null)
            {
                slot.PlantPoop(selectedPoop);
            }
            else
            {
                Debug.LogWarning("심을 똥이 없습니다.");
            }
        }
        else if (slot.isHarvestable)
        {
            slot.Harvest();
        }
    }

    void Update()
    {
        foreach (var slot in farmSlots)
        {
            if (slot != null) slot.UpdateGrowth();
        }
    }
}
