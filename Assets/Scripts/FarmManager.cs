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
        // 씬이 시작될 때 저장된 밭 데이터를 불러옵니다.
        LoadFarmData();
    }

    private void OnDisable()
    {
        // 미니게임 씬 등으로 이동하거나 꺼질 때, 현재 모든 밭의 최신 상태를 백업합니다.
        SaveFarmData();
    }

    // ★ [구조 수정] SceneLoader 및 OnDisable에서 안전하게 호출할 수 있는 데이터 저장 함수
    public void SaveFarmData()
    {
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
                // 빈 밭이라면 저장소 데이터 초기화
                FarmDataStorage.PlantedSeeds[i] = null;
                FarmDataStorage.PlantedTimes[i] = 0f;
                FarmDataStorage.HarvestableStates[i] = false;
            }
            else
            {
                // 작물이 자라는 중이라면 현재 슬롯의 실시간 정보 백업
                FarmDataStorage.PlantedSeeds[i] = farmSlots[i].currentActiveSeed;
                FarmDataStorage.PlantedTimes[i] = farmSlots[i].plantedTime;
                FarmDataStorage.HarvestableStates[i] = farmSlots[i].isHarvestable;
            }
        }
        Debug.Log("<color=yellow>[FarmManager]</color> 모든 밭 데이터가 안전하게 Static 저장소에 백업되었습니다.");
    }

    // ★ [구조 수정] 첫 실행 혹은 미니게임에서 홈 화면으로 돌아왔을 때 데이터를 복구하는 함수
    public void LoadFarmData()
    {
        // 정적 데이터 저장소가 한 번도 초기화되지 않았다면 (게임 첫 실행 시) 초기화 크기만 설정 후 리턴
        if (!FarmDataStorage.IsInitialized)
        {
            FarmDataStorage.Initialize(farmSlots.Count);
            return; 
        }

        // 저장소에 기록된 데이터들을 기반으로 밭 상태 복구 프로세스 진행
        for (int i = 0; i < farmSlots.Count; i++)
        {
            if (farmSlots[i] == null) continue;

            // 정적 저장소에서 알맞은 인덱스의 데이터를 꺼내옵니다.
            SeedData savedSeed = FarmDataStorage.PlantedSeeds[i];
            float savedTime = FarmDataStorage.PlantedTimes[i];
            bool savedHarvestable = FarmDataStorage.HarvestableStates[i];

            // 해당 자리에 심겨진 작물 데이터가 존재한다면 밭 복구 함수 호출
            if (savedSeed != null)
            {
                farmSlots[i].RestoreSlot(savedSeed, savedTime, savedHarvestable);
                Debug.Log($"<color=green>[FarmManager]</color> {i}번 밭 복구 완료: {savedSeed.seedName}");
            }
        }
    }

    public void OnSlotClicked(int index)
    {
        if (index < 0 || index >= farmSlots.Count) return;

        FarmSlot slot = farmSlots[index];
        if (slot == null) return;

        if (slot.IsEmpty)
        {
            // 🔍 [방어 코드 추가] ResourceManager 싱글톤 인스턴스 존재 여부 체크
            if (ResourceManager.Instance == null)
            {
                Debug.LogError("<color=red>[FarmManager]</color> ResourceManager.Instance가 null입니다! Hierarchy 창에 ResourceManager 오브젝트가 유실되었거나 Script 연결이 깨졌는지 확인하세요.");
                return;
            }

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
// 지안님의 FarmSlot 시스템에 최적화된 정적 데이터 저장소 (Static Class)
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