using UnityEngine;
using System.Collections.Generic;

public class SeedSpawnerSimple : MonoBehaviour
{
    [Header("Seed")]
    public GameObject seedPrefab;

    [Header("Spawn Points")]
    public List<Transform> spawnPoints;

    [Header("Timing")]
    public float spawnInterval = 5f;
    public int minSpawn = 1;
    public int maxSpawn = 2;

    [Header("Register To GameManager")]
    public bool autoRegisterToGameManager = true; // 👈 BẬT / TẮT JOIN

    private float timer;

    void Start()
    {
        // 👉 TỰ JOIN VÀO LIST CÂY
        if (autoRegisterToGameManager && GameManager.Instance != null)
        {
            GameManager.Instance.RegisterTree(this);
        }
    }

    void OnDestroy()
    {
        // 👉 GỠ KHỎI LIST KHI CÂY BỊ HUỶ
        if (autoRegisterToGameManager && GameManager.Instance != null)
        {
            GameManager.Instance.UnregisterTree(this);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnSeeds();
        }
    }

    void SpawnSeeds()
    {
        if (seedPrefab == null || spawnPoints.Count == 0) return;

        int spawnCount = Random.Range(minSpawn, maxSpawn + 1);

        for (int i = 0; i < spawnCount; i++)
        {
            Transform point = spawnPoints[Random.Range(0, spawnPoints.Count)];

            Instantiate(
                seedPrefab,
                point.position,
                Quaternion.identity
            );
        }
    }
}
