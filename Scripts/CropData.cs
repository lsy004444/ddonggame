using UnityEngine;

[CreateAssetMenu(fileName = "NewCropData", menuName = "ScriptableObjects/Crop Data")]
public class CropData : ScriptableObject
{
    public string cropName;
    public float growthTime; // 작물이 완전히 자라는 데 걸리는 시간 (초 단위)
    public Sprite[] growthSprites; // 성장 단계별 이미지 (배열)
    public PoopType harvestRewardPoopType; // 수확 시 얻을 똥의 종류
    public int harvestRewardAmount; // 수확 시 얻을 똥의 개수
}