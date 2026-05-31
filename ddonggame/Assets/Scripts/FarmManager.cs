using UnityEngine;
using System.Collections.Generic;

public class FarmManager : MonoBehaviour
{
    public static FarmManager Instance { get; private set; }
    public List<FarmSlot> farmSlots = new List<FarmSlot>();

    private void Awake()
    {
        if (Instance == null) Instance = this;

        for (int i = 0; i < farmSlots.Count; i++)
        {
            if (farmSlots[i] != null)
            {
                farmSlots[i].myIndex = i;
            }
        }
    }

    public void OnSlotClicked(int index)
    {
        if (index < 0 || index >= farmSlots.Count) return;
        FarmSlot slot = farmSlots[index];

        if (slot == null) return;

        // 슬롯이 비어있을 때만 작동
        if (slot.IsEmpty)
        {
            // 인벤토리에서 랜덤 똥을 하나 꺼내옵니다.
            PoopType extractedPoop = ResourceManager.Instance.GetAnyPoopToPlant();
            
            // 꺼낸 똥이 실제로 있다면 심습니다.
            if (extractedPoop != null)
            {
                slot.PlantPoop(extractedPoop);
            }
            else
            {
                Debug.Log("심을 똥이 인벤토리에 없습니다!");
            }
        }
    }
}