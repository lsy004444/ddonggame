using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem; // 유니티 새 인풋 시스템 필수 적용
using System.Linq; // [핵심] LINQ 메서드 (Where, Select) 사용을 위해 필요합니다!
using UnityEngine.SceneManagement;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    public HashSet<string> discoveredPoops = new HashSet<string>();

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

    [ContextMenu("도감 초기화 (테스트용)")]
    public void ResetDiscoveredData()
    {
        foreach (var type in availablePoopTypes)
        {
            if (type != null)
                PlayerPrefs.DeleteKey("Discovered_" + type.name);
        }
        discoveredPoops.Clear();
        PlayerPrefs.Save();
        Debug.Log("도감 초기화 완료");
    }

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
        StartCoroutine(ReconnectUI());
    }

    private System.Collections.IEnumerator ReconnectUI()
    {
        yield return null;
        
        GameObject countObj = GameObject.Find("PoopCountText");
        if (countObj != null) poopCountText = countObj.GetComponent<TextMeshProUGUI>();
        
        GameObject fliesObj = GameObject.Find("PoopFliesText");
        if (fliesObj != null) poopFliesText = fliesObj.GetComponent<TextMeshProUGUI>();
        
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
            LoadData();
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
        if (!discoveredPoops.Contains(poopType.name))
        {
            discoveredPoops.Add(poopType.name);
            PlayerPrefs.SetInt("Discovered_" + poopType.name, 1);
            PlayerPrefs.Save();
            Debug.Log($"<color=cyan>[도감 등록]</color> {poopType.poopName} 최초 발견!");
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
    public void AddPoopByName(string assetName, int amount)
    {
        if (availablePoopTypes == null || availablePoopTypes.Length == 0) return;

        // 인스펙터에 등록된 9종의 똥 중 파일 이름(예: "HealthyPoop")이 일치하는 것을 찾습니다.
        PoopType target = System.Array.Find(availablePoopTypes, p => p.name == assetName);
        
        if (target != null)
        {
            AddPoop(target, amount); // 기존 인벤토리 추가 함수 호출
            SaveData();              // 변경된 데이터 기기에 영구 저장
        }
        else
        {
            Debug.LogError($"[ResourceManager] '{assetName}' 에셋을 availablePoopTypes 배열에서 찾을 수 없습니다! 인스펙터를 확인하세요.");
        }
    }


    public void SaveData()
    {
        PlayerPrefs.SetInt("PoopFliesData", poopFlies); // 똥파리 개수 저장
        
        foreach (var pair in poopCounts)
        {
            if (pair.Key != null)
            {
                // 각 똥 에셋의 이름을 키값으로 삼아 개수를 저장 (예: Poop_HealthyPoop)
                PlayerPrefs.SetInt("Poop_" + pair.Key.name, pair.Value);
            }
        }
        PlayerPrefs.Save();
        Debug.Log("<color=green>[데이터 저장 완료]</color> 똥 인벤토리와 똥파리가 저장되었습니다.");
    }
    
    public void LoadData()
    {
        poopFlies = PlayerPrefs.GetInt("PoopFliesData", 0);
        
        // 딕셔너리에 등록된 모든 똥 종류의 기존 저장값을 불러옴
        var keys = new List<PoopType>(poopCounts.Keys);
        foreach (var type in keys)
        {
            if (type != null)
            {
                poopCounts[type] = PlayerPrefs.GetInt("Poop_" + type.name, 0);
                if (PlayerPrefs.GetInt("Discovered_" + type.name, 0) == 1)
                    {
                        discoveredPoops.Add(type.name);
                    }
            }
        }
        Debug.Log("<color=green>[데이터 로드 완료]</color> 이전 저장 데이터를 성공적으로 불러왔습니다.");
    }

    public bool IsDiscovered(PoopType type)
    {
        return type != null && discoveredPoops.Contains(type.name);
    }
    public string GetInventoryBreakdown()
    {
        // 텍스트를 효율적으로 이어 붙이기 위한 툴입니다.
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        
        sb.AppendLine("<size=110%><b><color=#FFD700>💩 똥 보관함 목록</color></b></size>");
        sb.AppendLine("-----------------------------");

        bool hasItem = false;
        foreach (var pair in poopCounts)
        {
            if (pair.Key != null && pair.Value > 0) // 0개보다 많이 가진 똥만 보여줍니다.
            {
                sb.AppendLine($"{pair.Key.poopName} : <color=white>{pair.Value}개</color>");
                hasItem = true;
            }
        }

        if (!hasItem)
        {
            sb.AppendLine("<color=gray>보관함이 비어있습니다.</color>");
        }

        return sb.ToString();
    }
}
