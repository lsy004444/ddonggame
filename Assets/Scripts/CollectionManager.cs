using UnityEngine;
using TMPro;
using System.Text;

public class CollectionManager : MonoBehaviour
{
    public TextMeshProUGUI collectionListText;
    public ResourceManager resourceManager;

    public void OpenCollection()
    {
        gameObject.SetActive(true);

        if(resourceManager == null)
            resourceManager = ResourceManager.Instance;
            
        UpdateCollectionText();
    }

    public void CloseCollection()
    {
        gameObject.SetActive(false);
    }

    void UpdateCollectionText()
    {
        Debug.Log("resourceManager: " + resourceManager); 
        if (resourceManager == null || resourceManager.availablePoopTypes == null) return;
        Debug.Log("똥 타입 개수: " + resourceManager.availablePoopTypes.Length);

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("도감\n");

        foreach (var type in resourceManager.availablePoopTypes)
        {
            if (type == null) continue;
            Debug.Log("타입: " + type.poopName + " discovered: " + resourceManager.IsDiscovered(type));

            bool discovered = resourceManager.IsDiscovered(type);
            sb.AppendLine(discovered ? type.poopName : "???");
        }

        collectionListText.text = sb.ToString();
        //확인용콘솔
        Debug.Log("최종 텍스트: " + sb.ToString()); 
    }
}