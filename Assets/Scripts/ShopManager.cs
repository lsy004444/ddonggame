using UnityEngine;
using System.Collections;

public class ShopManager : MonoBehaviour
{
    [Header("연동할 스크립트")]
    public PoopSpawner poopSpawner;
    public PlayerController playerController;

    [Header("아이템 가격")]
    public int feverTimeCost = 30;
    public int basketSizeCost = 20;
    public int slowFallCost = 25;
    public int magnetCost = 35;

    [Header("효과 지속시간")]
    public float feverDuration = 15f;
    public float basketDuration = 20f;
    public float slowFallDuration = 15f;
    public float magnetDuration = 10f;

    [Header("바구니 확대 설정")]
    public float basketScaleMultiplier = 1.8f;

    [Header("저속 낙하 설정")]
    public float slowFallMultiplier = 0.5f; // 낙하속도를 50%로

    [Header("자석 설정")]
    public float magnetRadius = 3f;

    private float originalFallSpeed;

    void Start()
    {
        if (poopSpawner != null)
            originalFallSpeed = poopSpawner.poopFallSpeed;
    }

    // 1. 피버타임 구매
    public void BuyFeverTime()
    {
        if (!TrySpendCurrency(feverTimeCost)) return;
        StartCoroutine(FeverTimeRoutine());
    }

    IEnumerator FeverTimeRoutine()
    {
        poopSpawner.feverTime = true;
        Debug.Log("피버타임 시작!");
        yield return new WaitForSeconds(feverDuration);
        poopSpawner.feverTime = false;
        Debug.Log("피버타임 종료");
    }

    // 2. 바구니 확대 구매
    public void BuyBasketSize()
    {
        if (!TrySpendCurrency(basketSizeCost)) return;
        StartCoroutine(BasketSizeRoutine());
    }

    IEnumerator BasketSizeRoutine()
    {
        playerController.SetBasketScale(basketScaleMultiplier);
        Debug.Log("바구니 확대!");
        yield return new WaitForSeconds(basketDuration);
        playerController.SetBasketScale(1f);
        Debug.Log("바구니 원래대로");
    }

    // 3. 저속 낙하 구매
    public void BuySlowFall()
    {
        if (!TrySpendCurrency(slowFallCost)) return;
        StartCoroutine(SlowFallRoutine());
    }

    IEnumerator SlowFallRoutine()
    {
        poopSpawner.poopFallSpeed = originalFallSpeed * slowFallMultiplier;
        Debug.Log("저속 낙하 시작!");
        yield return new WaitForSeconds(slowFallDuration);
        poopSpawner.poopFallSpeed = originalFallSpeed;
        Debug.Log("저속 낙하 종료");
    }

    // 4. 자석 바구니 구매
    public void BuyMagnet()
    {
        if (!TrySpendCurrency(magnetCost)) return;
        StartCoroutine(MagnetRoutine());
    }

    IEnumerator MagnetRoutine()
    {
        float elapsed = 0f;
        Debug.Log("자석 바구니 시작!");
        while (elapsed < magnetDuration)
        {
            PullNearbyPoop();
            elapsed += Time.deltaTime;
            yield return null;
        }
        Debug.Log("자석 바구니 종료");
    }

    void PullNearbyPoop()
    {
        GameObject[] poops = GameObject.FindGameObjectsWithTag("Poop");
        foreach (GameObject poop in poops)
        {
            float dist = Vector3.Distance(poop.transform.position, playerController.transform.position);
            if (dist < magnetRadius)
            {
                poop.transform.position = Vector3.MoveTowards(
                    poop.transform.position,
                    playerController.transform.position,
                    Time.deltaTime * 8f
                );
            }
        }
    }

    // 재화 차감 공통 함수
    bool TrySpendCurrency(int cost)
    {
        if (ResourceManager.Instance == null) return false;

        int current = ResourceManager.Instance.GetPoopFliesCount();
        if (current < cost)
        {
            Debug.Log("똥파리 부족!");
            return false;
        }

        ResourceManager.Instance.AddPoopFlies(-cost);
        return true;
    }
}