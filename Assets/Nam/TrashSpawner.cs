using UnityEngine;

public class TrashSpawner : MonoBehaviour
{
    public GameObject[] trashPrefabs;
    public Transform[] spawnPoints;
    public Transform waterSurface; // Kéo Box Trigger nước vào đây
    public float spawnRate = 3f;
    public int maxTrashCount = 15; 

    private int spawnedCount = 0;
    private float timer;

    void Update() {
        if (spawnedCount < maxTrashCount) {
            timer += Time.deltaTime;
            if (timer >= spawnRate) {
                SpawnTrash();
                timer = 0;
            }
        }
    }

    void SpawnTrash() {
        if (spawnPoints.Length == 0 || trashPrefabs.Length == 0) return;

        int randomPoint = Random.Range(0, spawnPoints.Length);
        int randomTrash = Random.Range(0, trashPrefabs.Length);
        
        Vector3 pos = spawnPoints[randomPoint].position;
        // Ép rác sinh ra đúng độ cao của mặt nước
        if (waterSurface != null) pos.y = waterSurface.position.y + 0.1f;

        Instantiate(trashPrefabs[randomTrash], pos, trashPrefabs[randomTrash].transform.rotation);
        spawnedCount++;
    }
}