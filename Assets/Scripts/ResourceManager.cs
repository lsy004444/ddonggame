using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem; // 유니티 새 인풋 시스템 필수 적용
using System.Linq; // [핵심] LINQ 메서드 (Where, Select) 사용을 위해 필요합니다!
using UnityEngine.SceneManagement;

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

    // [수정] 똥 인벤토리: 각 PoopType별 개수를 저장합니다.
    private Dictionary<PoopType, int> poopCounts = new Dictionary<PoopType, int>();

    private void OnEnable()
{
    SceneManager.sceneLoaded += OnSceneLoaded;
}

private void OnDisable()
{
    SceneManager.sceneLoaded -= OnSceneLoaded;
}

private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
{
    poopCountText = null;
    poopFliesText = null;
    UpdatePoopCountUI();
    UpdatePoopFliesUI();
}

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

        // [수정] 인벤토리 초기화
        foreach (var type in availablePoopTypes)
        {
            if (!poopCounts.ContainsKey(type))
            {
                poopCounts.Add(type, 0);
            }
        }
    }

    private void Start()
    {
        // [테스트용] 시작 시 모든 PoopType에 대해 10개씩 지급 (나중에 삭제하거나 미니게임 연동)
        // foreach (var type in availablePoopTypes)
        // {
        //     AddPoop(type, 10);
        // }

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
            }
        }
    }

    // 똥을 추가하는 함수 (미니게임 연동 및 치트키용)
    public void AddPoop(PoopType poopType, int amount)
    {
        if (poopType == null)
        {
            Debug.LogWarning("null PoopType을 추가하려고 시도했습니다. 무시됩니다.");
            return;
        }

        if (poopCounts.ContainsKey(poopType))
        {
            poopCounts[poopType] += amount;
        }
        else
        {
            poopCounts.Add(poopType, amount);
        }
        Debug.Log($"{poopType.poopName} 획득! 현재: {poopCounts[poopType]}개");
        UpdatePoopCountUI();
    }

    // [추가] 가진 똥 중에서 랜덤하게 하나를 꺼내오는 함수 (FarmManager에서 호출)
    public PoopType GetRandomPoopFromInventory()
    {
        var availablePoopsInInventory = poopCounts.Where(pair => pair.Value > 0).Select(pair => pair.Key).ToList();

        if (availablePoopsInInventory.Count > 0)
        {
            PoopType selected = availablePoopsInInventory[Random.Range(0, availablePoopsInInventory.Count)];
            poopCounts[selected]--;
            Debug.Log($"<color=orange>{selected.poopName}</color>을(를) 인벤토리에서 꺼냈습니다. 남은 개수: {poopCounts[selected]}개");
            UpdatePoopCountUI();
            return selected;
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
        return poopCounts.Values.Sum();
    }

    private void UpdatePoopCountUI()
    {
        if (poopCountText == null)
        {
            GameObject obj = GameObject.Find("PoopCountText");
            if (obj != null) poopCountText = obj.GetComponent<TextMeshProUGUI>();
            
        }
        if (poopCountText != null)
        {
            poopCountText.text = $"똥: {GetTotalPoopCount()}";
        }
    }

    private void UpdatePoopFliesUI()
    {
        if (poopFliesText == null)
        {
            GameObject obj = GameObject.Find("PoopFliesText");
            if(obj != null) poopFliesText = obj.GetComponent<TextMeshProUGUI>();
        }
        if (poopFliesText != null)
        {
            poopFliesText.text = $"똥파리: {poopFlies}";
        }
    }
}
