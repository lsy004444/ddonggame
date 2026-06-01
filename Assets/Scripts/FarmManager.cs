using UnityEngine;
using System.Collections.Generic;
using System.Linq; // ResourceManager에서 사용될 수 있으므로 추가

public class FarmManager : MonoBehaviour
{
    public static FarmManager Instance { get; private set; }
    public List<FarmSlot> farmSlots = new List<FarmSlot>(); 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        
        // [핵심] 리스트에 들어있는 순서대로 밭 번호를 0, 1, 2... 로 강제 지정합니다.
        // 인스펙터에서 숫자를 안 고쳐도 여기서 다 고쳐줍니다!
        for (int i = 0; i < farmSlots.Count; i++)
        {
            if (farmSlots[i] != null)
            {
                farmSlots[i].myIndex = i; 
                Debug.Log($"<color=white>[FarmManager]</color> {i}번 밭 설정 완료.");
            }
        }
    }

    public void OnSlotClicked(int index)
    {
        // 0번만 작동하는 문제를 막기 위해 전달받은 index를 그대로 사용합니다.
        if (index < 0 || index >= farmSlots.Count) return;

        FarmSlot slot = farmSlots[index];
        if (slot == null) return;

        Debug.Log($"<color=cyan>[FarmManager]</color> {index}번 밭 클릭 처리 시작.");

        if (slot.IsEmpty)
        {
            // [수정] ResourceManager에서 PoopType을 직접 가져와 FarmSlot에 전달합니다.
            PoopType p = ResourceManager.Instance.GetRandomPoopFromInventory();
            if (p != null)
            {
                slot.PlantPoop(p); // FarmSlot의 PlantPoop은 이제 PoopType을 받습니다.
            }
            else
            {
                Debug.LogWarning($"<color=red>[FarmManager]</color> 심을 수 있는 똥이 인벤토리에 없습니다!");
            }
        }
        else if (slot.isHarvestable)
        {
            slot.Harvest();
        }
    }
}
