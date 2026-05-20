
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int poopCount = 0;

    void Awake()
    {
        if(instance == null)
            instance = this;
    }

    public void AddPoop(int amount)
    {
        poopCount += amount;
        Debug.Log("똥 개수: " + poopCount);
    }
}
