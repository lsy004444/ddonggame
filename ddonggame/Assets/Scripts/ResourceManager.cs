using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI poopCountText; 
    private Dictionary<PoopType, int> poopCounts = new Dictionary<PoopType, int>(); 

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
    }

    // 똥을 추가하는 함수
    public void AddPoop(PoopType poopType, int amount)
    {
        if (poopType == null) return; // 에러 방지

        if (poopCounts.ContainsKey(poopType))
        {
            poopCounts[poopType] += amount;
        }
        else
        {
            poopCounts.Add(poopType, amount);
        }
        UpdatePoopCountUI();
    }

    // [신규] 종류 상관없이 심을 수 있는 똥 하나를 가져오고 개수를 줄임
    public PoopType GetAnyPoopToPlant()
    {
        // 1. 현재 1개 이상 보유 중인 똥들의 리스트를 만듭니다.
        List<PoopType> availablePoops = new List<PoopType>();
        foreach (var poop in poopCounts.Keys)
        {
            if (poopCounts[poop] > 0) availablePoops.Add(poop);
        }

        // 2. 보유 중인 종류가 있다면 그중 하나를 랜덤하게 뽑습니다.
        if (availablePoops.Count > 0)
        {
            PoopType randomPoop = availablePoops[Random.Range(0, availablePoops.Count)];
            poopCounts[randomPoop]--;
            UpdatePoopCountUI();
            return randomPoop;
        }

        return null; // 하나도 없으면 null
    }


    public int GetTotalPoopCount()
    {
        int total = 0;
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
}
