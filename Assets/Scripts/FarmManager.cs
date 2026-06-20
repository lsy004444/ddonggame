using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class FarmManager : MonoBehaviour
{
    public static FarmManager Instance { get; private set; }
    public List<FarmSlot> farmSlots = new List<FarmSlot>(); 

    private void Awake()
    {
        if (Instance == null) Instance = this;
        
        // 리스트에 들어있는 순서대로 밭 번호를 0, 1, 2... 로 강제 지정합니다.
        for (int i = 0; i < farmSlots.Count; i++)
        {
            if (farmSlots[i] != null)
            {
                farmSlots[i].myIndex = i; 
                Debug.Log($"<color=white>[FarmManager]</color> {i}번 밭 설정 완료.");
            }
        }
    }

    private void Start()
    {
        // 정적 데이터 저장소가 한 번도 초기화되지 않았다면 (게임 첫 실행 시) 초기화 크기 설정
        if (!FarmDataStorage.IsInitialized)
        {
            FarmDataStorage.Initialize(farmSlots.Count);
            return; 
        }

        // 미니게임에 갔다가 다시 홈 화면으로 돌아왔을 때 데이터 복구 프로세스
        for (int i = 0; i < farmSlots.Count; i++)
        {
            if (farmSlots[i] == null) continue;

            // [수정 완료] 새 구조에 맞게 SeedData, float, bool을 정적 저장소에서 꺼내옵니다.
            SeedData savedSeed = FarmDataStorage.PlantedSeeds[i];
            float savedTime = FarmDataStorage.PlantedTimes[i];
            bool savedHarvestable = FarmDataStorage.HarvestableStates[i];

            // 해당 자리에 심겨진 작물 데이터가 존재한다면 밭 복구 함수 호출
            if (savedSeed != null)
            {
                // [수정 완료] 새롭게 바뀐 FarmSlot의 RestoreSlot 인자값(SeedData, float, bool)과 완벽 매칭! (CS1503 해결)
                farmSlots[i].RestoreSlot(savedSeed, savedTime, savedHarvestable);
                Debug.Log($"<color=green>[FarmManager]</color> {i}번 밭 복구 완료: {savedSeed.seedName}");
            }
        }
    }

    private void OnDisable()
    {
        // 미니게임 씬으로 이동할 때, 현재 모든 밭의 최신 상태를 정적 클래스에 백업합니다.
        if (farmSlots == null || farmSlots.Count == 0) return;

        if (!FarmDataStorage.IsInitialized)
        {
            FarmDataStorage.Initialize(farmSlots.Count);
        }

        for (int i = 0; i < farmSlots.Count; i++)
        {
            if (farmSlots[i] == null) continue;

            if (farmSlots[i].IsEmpty)
            {
                FarmDataStorage.PlantedSeeds[i] = null;
                FarmDataStorage.PlantedTimes[i] = 0f;
                FarmDataStorage.HarvestableStates[i] = false;
            }
            else
            {
                // [수정 완료] 새 FarmSlot에 맞게 currentActiveSeed, plantedTime, isHarvestable을 저장합니다. (CS1061 해결)
                FarmDataStorage.PlantedSeeds[i] = farmSlots[i].currentActiveSeed;
                FarmDataStorage.PlantedTimes[i] = farmSlots[i].plantedTime;
                FarmDataStorage.HarvestableStates[i] = farmSlots[i].isHarvestable;
            }
        }
        Debug.Log("<color=yellow>[FarmManager]</color> 모든 밭 데이터가 안전하게 Static 저장소에 백업되었습니다.");
    }

    public void OnSlotClicked(int index)
    {
        if (index < 0 || index >= farmSlots.Count) return;

        FarmSlot slot = farmSlots[index];
        if (slot == null) return;

        Debug.Log($"<color=cyan>[FarmManager]</color> {index}번 밭 클릭 처리 시작.");

        if (slot.IsEmpty)
        {
            PoopType p = ResourceManager.Instance.GetRandomPoopFromInventory();
            if (p != null)
            {
                slot.PlantPoop(p); 
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

// ===================================================================
// [수정 완료] 지안님의 FarmSlot 시스템에 최적화된 정적 데이터 저장소
// ===================================================================
public static class FarmDataStorage
{
    public static bool IsInitialized { get; private set; } = false;

    public static List<SeedData> PlantedSeeds = new List<SeedData>();
    public static List<float> PlantedTimes = new List<float>();
    public static List<bool> HarvestableStates = new List<bool>();

    public static void Initialize(int count)
    {
        PlantedSeeds = new List<SeedData>(new SeedData[count]);
        PlantedTimes = new List<float>(new float[count]);
        HarvestableStates = new List<bool>(new bool[count]);
        IsInitialized = true;
    }
}