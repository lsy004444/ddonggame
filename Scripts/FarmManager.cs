using UnityEngine;
using System.Collections.Generic;

public class FarmManager : MonoBehaviour
{
    public static FarmManager Instance { get; private set; }

    // 실제 화면에 배치된 FarmSlot들을 담을 리스트입니다.
    // 유니티 인스펙터에서 9개의 슬롯을 여기에 드래그해서 넣어주면 됩니다.
    public List<FarmSlot> farmSlots = new List<FarmSlot>(); 

    private void Awake()
    {
        // 싱글톤 설정 (어디서든 FarmManager에 접근 가능하게 함)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        Debug.Log("밭 시스템 준비 완료! 총 슬롯 수: " + farmSlots.Count);
    }

    // 모든 슬롯의 성장 상태를 업데이트하는 함수 (2주차에 활용)
    private void Update()
    {
        foreach (var slot in farmSlots)
        {
            if (slot != null)
            {
                slot.UpdateGrowth();
            }
        }
    }

    // 특정 슬롯에 작물을 심는 함수 (예시)
    public void PlantSeedToSlot(int index, CropData data)
    {
        if (index >= 0 && index < farmSlots.Count)
        {
            if (farmSlots[index].IsEmpty)
            {
                farmSlots[index].PlantCrop(data);
                Debug.Log($"{index}번 자리에 {data.cropName}을(를) 심었습니다!");
            }
        }
    }
}
