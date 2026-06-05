using UnityEngine;

public class TrashSpawner : MonoBehaviour
{
    [Header("Cấu hình Prefabs")]
    public GameObject[] trashPrefabs;
    public Transform[] spawnPoints;
    public Transform waterSurface; // Kéo Box Trigger nước vào đây

    [Header("Cấu hình Tốc độ & Giới hạn")]
    [Tooltip("Thời gian giãn cách giữa mỗi lần spawn (giây). Càng nhỏ rác ra càng nhanh!")]
    public float spawnRate = 1.0f; 
    
    [Tooltip("Tổng số lượng rác tối đa được phép sinh ra trong màn chơi.")]
    public int maxTrashCount = 60; 

    [Header("Cấu hình Số lượng sinh ra mỗi lượt")]
    [Tooltip("Số lượng rác tối thiểu sinh ra trong MỘT LẦN đến nhịp timer.")]
    public int minSpawnAmount = 1;

    [Tooltip("Số lượng rác tối đa sinh ra trong MỘT LẦN đến nhịp timer.")]
    public int maxSpawnAmount = 3;

    private int spawnedCount = 0;
    private float timer;

    void Update() {
        if (spawnedCount < maxTrashCount) {
            timer += Time.deltaTime;
            if (timer >= spawnRate) {
                SpawnTrashGroup();
                timer = 0;
            }
        }
    }

    void SpawnTrashGroup() {
        if (spawnPoints.Length == 0 || trashPrefabs.Length == 0) return;

        // 1. Tính toán ngẫu nhiên số lượng rác sẽ bùng ra trong lượt này
        int amountToSpawn = Random.Range(minSpawnAmount, maxSpawnAmount + 1);

        for (int i = 0; i < amountToSpawn; i++) {
            // Kiểm tra nếu chạm trần giới hạn tổng của màn chơi thì dừng ngay
            if (spawnedCount >= maxTrashCount) break;

            int randomPoint = Random.Range(0, spawnPoints.Length);
            int randomTrash = Random.Range(0, trashPrefabs.Length);
            
            Vector3 pos = spawnPoints[randomPoint].position;
            
            // 🔥 CẢI TIẾN: Thêm độ lệch ngẫu nhiên nhỏ (Offset) x và z 
            // Để khi rác spawn trúng cùng 1 vị trí thì chúng không bị đè khít/dính chặt vào nhau
            pos.x += Random.Range(-0.6f, 0.6f);
            pos.z += Random.Range(-0.6f, 0.6f);

            // Ép rác sinh ra đúng độ cao của mặt nước
            if (waterSurface != null) pos.y = waterSurface.position.y + 0.1f;

            Instantiate(trashPrefabs[randomTrash], pos, trashPrefabs[randomTrash].transform.rotation);
            spawnedCount++;
        }
    }

    // Mẹo nhỏ: Nếu bạn muốn khi người chơi vớt bớt rác đi, hệ thống sẽ tự động 
    // trừ spawnedCount xuống để spawn thêm rác mới bù vào, hãy gọi hàm này từ script nhặt rác nhé!
    public void OnTrashCollected() {
        if (spawnedCount > 0) spawnedCount--;
    }
}