
using System.Buffers;
using UnityEngine;

public class PoopSpawner : MonoBehaviour {

    //건강한 똥
    public GameObject[] healthyPoop;
    //희귀똥
    public GameObject[] rarePoop;
    public float spawnInterval = 0.01f;
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if(timer > spawnInterval)
        {
            timer = 0f;
            SpawnPoop();
        }
    }
   
    void SpawnPoop()
    {
        
            float randomX = Random.Range(-2.5f, 2.5f);
            Vector3 spawnPos = new Vector3(randomX, 5f, 0f);
            GameObject selected = SelectPoop();
            Instantiate(selected, spawnPos, Quaternion.identity);
            //랜덤스폰
            spawnInterval = Random.Range(0.2f, 1.5f);
    }

    GameObject SelectPoop()
    {
        float rand = Random.Range(0f, 100f);

        if(rand < 50f)
            return rarePoop[Random.Range(0, rarePoop.Length)];
        else
            return healthyPoop[Random.Range(0, healthyPoop.Length)];
    }
}
