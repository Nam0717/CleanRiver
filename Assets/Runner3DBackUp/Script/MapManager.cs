using UnityEngine;
using System.Collections.Generic;

public class BaseManager : MonoBehaviour
{
    [Header("Pool Settings")]
    public List<GameObject> basePrefabs;  // Danh sách các prefab base bình thường
    public GameObject base1;              // Base cố định đầu tiên khi vào game
    public int poolSizePerPrefab = 3;     // Số lượng mỗi prefab có trong pool

    [Header("Cấu hình Bản Đồ Đặc Biệt (Giao Thông)")]
    public GameObject redLightBasePrefab;   // Prefab trạm tàu hỏa ĐÈN ĐỎ
    public GameObject greenLightBasePrefab; // Prefab trạm tàu hỏa ĐÈN XANH
    public int specialPoolSize = 2;         // Số lượng lưu trữ cho map đặc biệt

    [Header("Cài đặt Spawner")]
    public int initialSpawnCount = 5;     // Tổng số base spawn khi bắt đầu
    public float baseLength = 200f;
    public Transform playerCam;

    private List<GameObject> activeBases = new List<GameObject>();
    private Dictionary<GameObject, Queue<GameObject>> poolDict;
    private float nextZ = 0f;
    private int spawnCount = 0; // Biến đếm số lượng map segment đã tạo

    void Awake()
    {
        CreatePool();
    }

    void Start()
    {
        // Base đầu tiên luôn là base1
        SpawnSpecificBase(base1);
        nextZ += baseLength;

        // Sinh các base tiếp theo khi mới vào game
        for (int i = 1; i < initialSpawnCount; i++)
        {
            SpawnRandomBase();
            nextZ += baseLength;
        }
    }

    void Update()
    {
        if (activeBases.Count == 0) return;

        GameObject firstBase = activeBases[0];

        // Nếu Base ra khỏi camera -> Thu hồi về Pool và sinh Base mới
        if (playerCam.position.z - firstBase.transform.position.z > baseLength + 50)
        {
            RecycleBase(firstBase);
            activeBases.RemoveAt(0);

            SpawnRandomBase();
            nextZ += baseLength;
        }
    }

    // ========================================================
    //                         OBJECT POOLING
    // ========================================================
    void CreatePool()
    {
        poolDict = new Dictionary<GameObject, Queue<GameObject>>();

        // 1. Khởi tạo pool cho các base bình thường
        foreach (var prefab in basePrefabs)
        {
            InitializeQueue(prefab, poolSizePerPrefab);
        }

        // 2. Khởi tạo pool cho Base1
        InitializeQueue(base1, 1);

        // 3. Khởi tạo pool cho 2 loại Map Giao thông đặc biệt để tránh lag khi gọi giữa màn
        if (redLightBasePrefab != null) InitializeQueue(redLightBasePrefab, specialPoolSize);
        if (greenLightBasePrefab != null) InitializeQueue(greenLightBasePrefab, specialPoolSize);
    }

    void InitializeQueue(GameObject prefab, int size)
    {
        Queue<GameObject> queue = new Queue<GameObject>();
        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            queue.Enqueue(obj);
        }
        poolDict[prefab] = queue;
    }

    GameObject GetFromPool(GameObject prefab)
    {
        if (!poolDict.ContainsKey(prefab) || poolDict[prefab].Count == 0)
        {
            GameObject extra = Instantiate(prefab);
            extra.SetActive(false);
            return extra;
        }
        return poolDict[prefab].Dequeue();
    }

    void RecycleBase(GameObject obj)
    {
        obj.SetActive(false);

        // Tìm prefab gốc dựa trên chuỗi tên để trả về đúng hàng đợi
        foreach (var entry in poolDict)
        {
            if (obj.name.Contains(entry.Key.name))
            {
                poolDict[entry.Key].Enqueue(obj);
                return;
            }
        }
        poolDict[base1].Enqueue(obj);
    }

    // ========================================================
    //                        SPAWN LOGIC
    // ========================================================
    void SpawnSpecificBase(GameObject prefab)
    {
        if (prefab == null) return;

        GameObject newBase = GetFromPool(prefab);
        newBase.SetActive(true);

        // Nối đuôi chính xác bằng EndPoint và StartPoint định vị sẵn
        if (activeBases.Count > 0)
        {
            Transform lastEnd = activeBases[activeBases.Count - 1].transform.Find("EndPoint");
            Transform newStart = newBase.transform.Find("StartPoint");

            if (lastEnd != null && newStart != null)
            {
                Vector3 offset = newBase.transform.position - newStart.position;
                newBase.transform.position = lastEnd.position + offset;
            }
            else
            {
                newBase.transform.position = new Vector3(0, 0, nextZ);
            }
        }
        else
        {
            newBase.transform.position = Vector3.zero;
        }

        activeBases.Add(newBase);
    }

    // Hàm chọn map ngẫu nhiên áp dụng quy luật vòng lặp 3 map
    void SpawnRandomBase()
    {
        spawnCount++; // Tăng biến đếm segment

        // KIỂM TRA: Cứ mỗi mốc map thứ 3 (3, 6, 9, 12...)
        if (spawnCount % 3 == 0)
        {
            // Ngẫu nhiên chọn 1 trong 2 loại map Trạm Chắn Tàu Hỏa
            GameObject specialPrefab = (Random.Range(0, 2) == 0) ? redLightBasePrefab : greenLightBasePrefab;
            SpawnSpecificBase(specialPrefab);
            Debug.Log($"[BaseManager] Đã sinh map Giao thông đặc biệt tại mốc thứ: {spawnCount}");
        }
        else
        {
            // Sinh map bình thường từ danh sách pool có sẵn
            if (basePrefabs.Count > 0)
            {
                int rand = Random.Range(0, basePrefabs.Count);
                SpawnSpecificBase(basePrefabs[rand]);
            }
        }
    }
}