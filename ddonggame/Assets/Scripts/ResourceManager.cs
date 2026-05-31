using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem; // 유니티 새 인풋 시스템 필수 적용

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [Header("UI 연결")]
    [SerializeField] private TextMeshProUGUI poopCountText;
    [SerializeField] private TextMeshProUGUI poopFliesText; // 화폐(똥파리) UI 텍스트 칸

    [Header("치트키 및 전체 똥 종류 등록")]
    // 유니티 인스펙터 창에서 프로젝트에 만든 9종의 똥(PoopType) 에셋을 이 배열에 꼭 드래그해서 등록해주세요!
    public PoopType[] availablePoopTypes; 

    [Header("화폐 시스템 데이터")]
    private int poopFlies = 0; // 이 게임의 최종 재화이자 점수인 '똥파리' 개수

    private Dictionary<PoopType, int> poopCounts = new Dictionary<PoopType, int>();
    private int generalPoopCount = 0; // 기존 null 데이터 백업용 일반 똥 개수

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        UpdatePoopCountUI();
        UpdatePoopFliesUI();
    }

    private void Update()
    {
        // P키를 누르면 9종의 똥 중 10개를 각각 완전히 독립적으로 랜덤하게 뽑아 주머니에 담아줍니다.
        if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
        {
            Debug.Log("<color=yellow>[치트키 발동]</color> 주머니에 랜덤 똥 10개가 개별적으로 골고루 섞여 추가됩니다!");
            
            if (availablePoopTypes != null && availablePoopTypes.Length > 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    PoopType randomCheatPoop = availablePoopTypes[Random.Range(0, availablePoopTypes.Length)];
                    AddPoop(randomCheatPoop, 1);
                }
            }
            else
            {
                Debug.LogWarning("availablePoopTypes 배열이 비어있습니다! 인스펙터에서 9종의 똥 에셋을 먼저 등록해주세요.");
                AddPoop(null, 10);
            }
        }
    }

    // 똥을 추가하는 함수 (미니게임 연동 및 치트키용)
    public void AddPoop(PoopType poopType, int amount)
    {
        if (poopType == null)
        {
            generalPoopCount += amount;
        }
        else
        {
            if (poopCounts.ContainsKey(poopType))
            {
                poopCounts[poopType] += amount;
            }
            else
            {
                poopCounts.Add(poopType, amount);
            }
        }
        UpdatePoopCountUI();
    }

    // 보유 중인 똥들의 실시간 개수를 모두 합쳐 '확률 풀'을 만든 뒤 진짜 제비뽑기를 수행합니다.
    public PoopType GetAnyPoopToPlant()
    {
        int totalCount = generalPoopCount;
        foreach (var count in poopCounts.Values)
        {
            totalCount += count;
        }

        if (totalCount <= 0) return null;

        int roll = Random.Range(0, totalCount);
        int cumulative = 0;

        if (generalPoopCount > 0)
        {
            cumulative += generalPoopCount;
            if (roll < cumulative)
            {
                generalPoopCount--;
                UpdatePoopCountUI();
                return null; 
            }
        }

        foreach (var kvp in poopCounts)
        {
            if (kvp.Value > 0)
            {
                cumulative += kvp.Value;
                if (roll < cumulative)
                {
                    poopCounts[kvp.Key]--;
                    UpdatePoopCountUI();
                    return kvp.Key; 
                }
            }
        }

        return null;
    }

    // 화폐(똥파리)를 누적하고 UI에 반영하는 전용 함수
    public void AddPoopFlies(int amount)
    {
        poopFlies += amount;
        Debug.Log($"<color=orange>[화폐 환산]</color> 똥파리 {amount}마리 획득! (현재 총액: {poopFlies}마리)");
        UpdatePoopFliesUI();
    }

    // ★[추가] GameManager 등 외부에서 현재 획득한 총 똥파리 개수를 읽어갈 수 있도록 하는 반환 함수
    public int GetPoopFliesCount()
    {
        return poopFlies;
    }

    public int GetTotalPoopCount()
    {
        int total = generalPoopCount;
        foreach (var count in poopCounts.Values)
        {
            total += count;
        }
        return total;
    }

    private void UpdatePoopCountUI()
    {
        if (poopCountText != null)
        {
            poopCountText.text = $"똥: {GetTotalPoopCount()}";
        }
    }

    private void UpdatePoopFliesUI()
    {
        if (poopFliesText != null)
        {
            poopFliesText.text = $"똥파리: {poopFlies}";
        }
    }
}