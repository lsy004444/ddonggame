using UnityEngine;

[CreateAssetMenu(fileName = "NewSeedData", menuName = "ScriptableObjects/Seed Data")]
public class SeedData : ScriptableObject
{
    public string seedName;
    public float growthTime;
    public Sprite[] growthSprites = new Sprite[4]; // 작물마다 다른 성장 단계 이미지 배열
    
    public int rewardPoopType; // 똥 타입 (Enum이 있다면 타입 수정 가능)
    public int rewardAmount;   // 수확 시 보상 개수
}