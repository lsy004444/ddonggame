using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewPoopType", menuName = "ScriptableObjects/Poop Type")]
public class PoopType : ScriptableObject
{
    public string poopName;
    public string description;
    public Sprite poopSprite;
    
    public enum Rarity { Common, Uncommon, Rare }
    public Rarity rarity;

    // 반드시 'SeedData'라고 적혀 있어야 합니다!
    public List<SeedData> possibleSeeds; 
}
