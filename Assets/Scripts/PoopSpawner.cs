
using System.Buffers;
using UnityEngine;

public class PoopSpawner : MonoBehaviour {
    public GameObject[] healthyPoop;
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
            GameObject selected = healthyPoop[Random.Range(0, healthyPoop.Length)];
            Instantiate(selected, spawnPos, Quaternion.identity);
        
        spawnInterval = Random.Range(0.2f, 1.5f);
    }
}
