using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.InputSystem; // 유니티 6 필수

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI poopCountText; 
    private Dictionary<PoopType, int> poopCounts = new Dictionary<PoopType, int>(); 
    
    // [추가] 특정 종류가 없을 때 사용할 일반 똥 개수
    private int generalPoopCount = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; DontDestroyOnLoad(gameObject); }
    }

    private void Start()
    {
        UpdatePoopCountUI();
    }

    // 똥을 추가하는 함수
    public void AddPoop(PoopType poopType, int amount)
    {
        if (poopType == null) 
        {
            // 종류가 지정되지 않으면 일반 똥 개수를 올립니다 (치트키용)
            generalPoopCount += amount;
        }
        else
        {
            if (poopCounts.ContainsKey(poopType)) poopCounts[poopType] += amount;
            else poopCounts.Add(poopType, amount);
        }
        UpdatePoopCountUI();
    }

    // 종류 상관없이 심을 수 있는 똥 하나를 가져오고 개수를 줄임
    public PoopType GetAnyPoopToPlant()
    {
        // 1. 일반 똥(치트키로 얻은 것)이 있다면 먼저 소모
        if (generalPoopCount > 0)
        {
            generalPoopCount--;
            UpdatePoopCountUI();
            return null; // 특정 종류가 없는 똥임을 의미
        }

        // 2. 보유 중인 특정 종류의 똥 리스트 확인
        List<PoopType> availablePoops = new List<PoopType>();
        foreach (var poop in poopCounts.Keys)
        {
            if (poopCounts[poop] > 0) availablePoops.Add(poop);
        }

        if (availablePoops.Count > 0)
        {
            PoopType randomPoop = availablePoops[Random.Range(0, availablePoops.Count)];
            poopCounts[randomPoop]--;
            UpdatePoopCountUI();
            return randomPoop;
        }

        return null; // 진짜 하나도 없으면 null
    }

    public int GetTotalPoopCount()
    {
        int total = generalPoopCount;
        foreach (var count in poopCounts.Values) { total += count; }
        return total;
    }

    private void UpdatePoopCountUI()
    {
        if (poopCountText != null)
        {
            poopCountText.text = $"똥: {GetTotalPoopCount()}";
        }
    }

    private void Update()
    {
        // 유니티 6 새로운 입력 방식 (P키 누르면 10개 추가)
        if (Keyboard.current != null && Keyboard.current.pKey.wasPressedThisFrame)
        {
            Debug.Log("치트키 발동: 똥 10개 추가!");
            AddPoop(null, 10); 
        }
    }
}
