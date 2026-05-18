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

    public void AddPoop(PoopType poopType, int amount)
    {
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
