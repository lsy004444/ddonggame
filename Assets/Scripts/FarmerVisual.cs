using UnityEngine;

public class FarmerVisual : MonoBehaviour
{
    [Header("연결할 2D 스프라이트 렌더러")]
    public SpriteRenderer farmerRenderer; 
    public SpriteRenderer basketRenderer; 

    [Header("농부 스프라이트")]
    public Sprite defaultFarmer;   
    public Sprite armsUpFarmer;    

    [Header("바구니 스프라이트 단계별")]
    public Sprite emptyBasket;     
    public Sprite halfBasket;      
    public Sprite fullBasket;      

    public void UpdateBasketVisual(int currentCount, int maxCapacity)
    {
        // [진단 로그] 이 함수가 실제로 실행되고 있는지, 값은 잘 들어오는지 확인
        Debug.Log($"<color=cyan>[FarmerVisual]</color> Update 함수 호출됨! 현재 점수: {currentCount} / 최대 용량: {maxCapacity}");

        if (maxCapacity <= 0)
        {
            Debug.LogWarning("<color=red>[FarmerVisual]</color> 경고: maxCapacity가 0 이하입니다! 함수가 리턴됩니다.");
            return;
        }

        float progress = (float)currentCount / maxCapacity;
        Debug.Log($"<color=cyan>[FarmerVisual]</color> 현재 진행도(Percent): {progress * 100}%");

        if (basketRenderer == null)
        {
            Debug.LogError("<color=red>[FarmerVisual]</color> 에러: basketRenderer(인펙터 빈칸)가 비어있습니다!");
            return;
        }

        // 스프라이트 변경 로직
        if (progress >= 1.0f)
        {
            basketRenderer.sprite = fullBasket;
            SetFarmerPose(true);
            Debug.Log("<color=green>[FarmerVisual]</color> 바구니 가득 참 (Full Sprite 적용)");
        }
        else if (progress >= 0.5f)
        {
            basketRenderer.sprite = halfBasket;
            SetFarmerPose(false);
            Debug.Log("<color=yellow>[FarmerVisual]</color> 바구니 절반 참 (Half Sprite 적용)");
        }
        else
        {
            basketRenderer.sprite = emptyBasket;
            SetFarmerPose(false);
            Debug.Log("<color=white>[FarmerVisual]</color> 바구니 비었음 (Empty Sprite 적용)");
        }
    }

    public void SetFarmerPose(bool isArmsUp)
    {
        if (farmerRenderer == null) return;
        farmerRenderer.sprite = isArmsUp ? armsUpFarmer : defaultFarmer;
    }
}