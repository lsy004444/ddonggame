using UnityEngine;

[CreateAssetMenu(fileName = "NewPoopType", menuName = "ScriptableObjects/Poop Type")]
public class PoopType : ScriptableObject
{
    public string poopName;
    [TextArea]
    public string description;
    public Sprite poopSprite; // 똥의 이미지
    public PoopRarity rarity; // 희귀도 (예: Common, Uncommon, Rare)
    // 필요하다면 추가적인 속성 (예: 가치, 효과 등)
}

public enum PoopRarity
{
    Common,
    Uncommon,
    Rare
}