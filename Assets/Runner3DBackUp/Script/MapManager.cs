using UnityEngine;
using System.Collections.Generic;

public class BaseManager : MonoBehaviour
{
    [Header("Pool Settings")]
    public List<GameObject> basePrefabs;  // 10 prefab base bình thường
    public GameObject base1;              // Base cố định đầu tiên
    public int poolSizePerPrefab = 3;     // Số lượng mỗi prefab có trong pool

    [Header("Special Traffic Maps (New)")]
    public GameObject redLightBasePrefab;   // Prefab trạm tàu hỏa ĐÈN ĐỎ
    public GameObject greenLightBasePrefab; // Prefab trạm tàu hỏa ĐÈN XANH
    public int specialPoolSize = 2;         // Số lượng lưu trữ cho mỗi map đặc biệt trong pool

    [Header("Spawner Settings")]
    public int initialSpawnCount = 5;     // Tổng base spawn khi bắt đầu
    public float baseLength = 200f;
    public Transform playerCam;

    private List<GameObject> activeBases = new List<GameObject>();
    private Dictionary<GameObject, Queue<GameObject>> poolDict;
    private float nextZ = 0f;
    private int spawnCount = 0;           // Biến đếm vòng lặp xuất hiện map

    void Awake()
    {
        CreatePool();
    }

    void Start()
    {
        // Base đầu tiên luôn là base1
        SpawnSpecificBase(base1);
        nextZ += baseLength;

        // Spawn tiếp các base ban đầu
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

        // Nếu Base ra khỏi camera → recycle về pool
        if (playerCam.position.z - firstBase.transform.position.z > baseLength + 50)
        {
            RecycleBase(firstBase);
            activeBases.RemoveAt(0);

            SpawnRandomBase();
            nextZ += baseLength;
        }
    }

    // ========================================================
    //                        OBJECT POOLING
    // ========================================================
    void CreatePool()
    {
        poolDict = new Dictionary<GameObject, Queue<GameObject>>();

        // Pool cho các base thường
        foreach (var prefab in basePrefabs)
        {
            InitializeQueue(prefab, poolSizePerPrefab);
        }

        // Pool cho Base1
        InitializeQueue(base1, 1);

        // Khởi tạo Pool cho 2 loại map đặc biệt để tối ưu hiệu năng
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

        // Tìm prefab gốc để trả đúng pool hàng đợi
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
    //                        SPAWN BASE
    // ========================================================
    void SpawnSpecificBase(GameObject prefab)
    {
        if (prefab == null) return;

        GameObject newBase = GetFromPool(prefab);
        newBase.SetActive(true); // Kích hoạt Active -> Sẽ kích hoạt OnEnable của map ngay lập tức

        // Lấy EndPoint của base cuối cùng để nối mạch đồ họa
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

    void SpawnRandomBase()
    {
        spawnCount++;

        // QUY LUẬT: Cứ 3 map xuất hiện 1 lần 1 trong 2 loại map đường ray cố định
        if (spawnCount % 3 == 0)
        {
            GameObject specialPrefab = (Random.Range(0, 2) == 0) ? redLightBasePrefab : greenLightBasePrefab;
            SpawnSpecificBase(specialPrefab);
            Debug.Log($"[BaseManager] Đã kích hoạt Trạm Giao Thông tại mốc map thứ: {spawnCount}");
        }
        else
        {
            if (basePrefabs.Count > 0)
            {
                int rand = Random.Range(0, basePrefabs.Count);
                SpawnSpecificBase(basePrefabs[rand]);
            }
        }
    }
}