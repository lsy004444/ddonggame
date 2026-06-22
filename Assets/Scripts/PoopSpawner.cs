
using System.Buffers;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PoopSpawner : MonoBehaviour {

    //건강한 똥
    public GameObject[] healthyPoop;
    //희귀똥
    public GameObject[] rarePoop;
    //안 건강한 똥
    public GameObject[] unhealthyPoop;
    //휴지 프리팹
    public GameObject tissuePrefab;

    public float spawnInterval = 0.01f;

    public float poopScale = 1.5f;
    private float timer;
        
    public float poopFallSpeed = 10f;


    [Header("피버타임")]
    public bool feverTime = false;
    public float feverSpawnMultiplier = 0.3f;

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
        Camera cam = Camera.main;
        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        float randomX = Random.Range(-camWidth + 0.5f, camWidth - 0.5f);
        Vector3 spawnPos = new Vector3(randomX, camHeight + 1f, 0f);
        GameObject selected = SelectPoop();
        GameObject poop = Instantiate(selected, spawnPos, Quaternion.identity);

        poop.transform.localScale = Vector3.one * poopScale;
        
        PoopController pc = poop.GetComponent<PoopController>();
        if (pc != null) pc.fallSpeed = poopFallSpeed;
        
        spawnInterval = Random.Range(0.2f, 1.5f);
    }

    GameObject SelectPoop()
    {
        float rand = Random.Range(0f, 100f);
        if ( rand < 10f && tissuePrefab != null)
            return tissuePrefab;
        else if(rand < 25f)
            return rarePoop[Random.Range(0, rarePoop.Length)];
        else if (rand <62f)
            return unhealthyPoop[Random.Range(0, unhealthyPoop.Length)];
        else
            return healthyPoop[Random.Range(0, healthyPoop.Length)];
    }
}
