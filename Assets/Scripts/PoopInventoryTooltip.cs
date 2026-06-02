using UnityEngine;
using UnityEngine.EventSystems;
using TMPro; // TextMeshPro를 제어하기 위해 필요합니다.

public class PoopInventoryTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI Setup")]
    public GameObject tooltipPanel;
    public TextMeshProUGUI tooltipText;

    private void Start()
    {
        // 게임 시작할 때는 툴팁 패널이 안 보이도록 자동으로 꺼줍니다.
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }

    // 마우스가 글자 위에 올라왔을 때 실행되는 함수
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("[호버 성공] 마우스가 똥 카운트 텍스트에 들어왔습니다!");
        
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(true); // 툴팁 켜기
            if (tooltipText != null)
            {
                tooltipText.text = "현재 보유 중인 똥의 총개수입니다."; // 원하는 설명 입력
            }
        }
    }

    // 마우스가 글자 밖으로 나갔을 때 실행되는 함수
    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false); // 툴팁 끄기
        }
    }
}