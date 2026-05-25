using UnityEngine;
using System.Collections.Generic;

public class FarmManager : MonoBehaviour
{
    public static FarmManager Instance { get; private set; }

    // 화면에 배치된 FarmSlot들을 연결하는 리스트
    public List<FarmSlot> farmSlots = new List<FarmSlot>(); 

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            // 씬 전환 시에도 정보를 유지하고 싶다면 유지, 아니라면 주석 처리 가능
            // DontDestroyOnLoad(gameObject); 
        }
    }

    void Update()
    {
        // 각 슬롯이 스스로 성장 로직을 처리하도록 시킵니다.
        foreach (var slot in farmSlots)
        {
            if (slot != null)
            {
                slot.UpdateGrowth();
            }
        }
    }

    // 2주차 가이드에 맞춘 심기 로직
    public void PlantPoopToSlot(int slotIndex, PoopType plantedPoop)
    {
        if (slotIndex < 0 || slotIndex >= farmSlots.Count) return;

        FarmSlot slot = farmSlots[slotIndex];
        if (slot.IsEmpty) // 주의: 대문자 I입니다.
        {
            slot.PlantPoop();
            Debug.Log($"{slotIndex}번 밭에 {plantedPoop.poopName} 심기 완료!");
        }
    }

    // 수확 로직 (FarmSlot의 Harvest를 호출)
    public void HarvestFromSlot(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= farmSlots.Count) return;

        FarmSlot slot = farmSlots[slotIndex];
        if (slot.isHarvestable)
        {
            slot.Harvest();
            Debug.Log($"{slotIndex}번 밭 수확 완료!");
        }
    }
}
