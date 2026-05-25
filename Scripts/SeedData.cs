using UnityEngine;

[CreateAssetMenu(fileName = "NewSeedData", menuName = "ScriptableObjects/Seed Data")]
public class SeedData : ScriptableObject
{
    public string seedName;
    public float growthTime;
    public Sprite[] growthSprites;
    
    // [추가] 수확 시 어떤 똥을 몇 개 줄지 설정
    public PoopType rewardPoopType; 
    public int rewardAmount;
}
