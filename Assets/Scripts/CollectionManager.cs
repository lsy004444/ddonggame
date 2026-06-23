using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CollectionManager : MonoBehaviour
{
    [System.Serializable]
    public class PoopSlotUI
    {
        public Image iconImage;
        public TextMeshProUGUI questionText;
        public int dataIndex = -1;
    }

    public ResourceManager resourceManager;

    [Header("페이지 1: 6칸")]
    public List<PoopSlotUI> page1Slots; // Slot1~6
    [Header("페이지 2: 3칸")]
    public List<PoopSlotUI> page2Slots; // Slot7~9

    public Sprite unknownSprite; // "???" 이미지

    public GameObject page1Root;
    public GameObject page2Root;

    private int currentPage = 1;

    public void OpenCollection()
    {
        gameObject.SetActive(true);

        if (resourceManager == null)
            resourceManager = ResourceManager.Instance;

        ShowPage(1);
        UpdateCollectionGrid();
    }

    public void CloseCollection()
    {
        gameObject.SetActive(false);
    }

    void UpdateCollectionGrid()
    {
        Debug.Log("UpdateCollectionGrid 호출됨, resourceManager: " + resourceManager + ", availablePoopTypes: " + (resourceManager?.availablePoopTypes?.Length ?? -1));

        if (resourceManager == null || resourceManager.availablePoopTypes == null) return;

        var types = resourceManager.availablePoopTypes; // 총 9종 가정

        FillSlots(page1Slots, types);  // 0~5번 인덱스
        FillSlots(page2Slots, types);  // 6~8번 인덱스
    }

    void FillSlots(List<PoopSlotUI> slots, PoopType[] types)
    {
        foreach (var slot in slots)
        {
            if (slot.iconImage == null) continue;

            // 빈 칸 처리 (Slot 자체를 꺼도 됨 — 이 경우는 텍스트도 필요 없으니까)
            if (slot.dataIndex < 0 || slot.dataIndex >= types.Length || types[slot.dataIndex] == null)
            {
                slot.iconImage.gameObject.SetActive(false);
                continue;
            }

            // 빈 칸이 아니면 Slot 자체는 항상 켜둠
            slot.iconImage.gameObject.SetActive(true);

            var type = types[slot.dataIndex];
            bool discovered = resourceManager.IsDiscovered(type);
            Debug.Log($"slot dataIndex={slot.dataIndex}, type={type.poopName}, discovered={discovered}, questionText null?={slot.questionText == null}");

            if (discovered)
            {
                slot.iconImage.enabled = true;           // 이미지만 켜기
                slot.iconImage.sprite = type.poopSprite;
                if (slot.questionText != null) slot.questionText.gameObject.SetActive(false);
            }
            else
            {
                slot.iconImage.enabled = false;           // 이미지만 끄기 (부모는 안 건드림)
                if (slot.questionText != null) slot.questionText.gameObject.SetActive(true);
            }
        }
    }
    public void NextPage()
    {
        if (currentPage == 1) ShowPage(2);
    }

    public void PrevPage()
    {
        if (currentPage == 2) ShowPage(1);
    }

    void ShowPage(int page)
    {
        currentPage = page;
        if (page1Root != null) page1Root.SetActive(page == 1);
        if (page2Root != null) page2Root.SetActive(page == 2);
    }
}