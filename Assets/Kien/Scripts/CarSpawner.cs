using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    [Header("Cars")]
    public GameObject[] carPrefabs;   // 3 loại xe

    [Header("Lanes")]
    public Transform topSpawn;
    public Transform topEnd;
    public Transform bottomSpawn;
    public Transform bottomEnd;

    [Header("Spawn Settings")]
    public float spawnInterval = 4f;

    void Start()
    {
        InvokeRepeating(nameof(SpawnCar), 1f, spawnInterval);
    }

    void SpawnCar()
    {
        // Chọn lane ngẫu nhiên
        bool isTopLane = Random.value > 0.5f;

        Transform spawnPoint = isTopLane ? topSpawn : bottomSpawn;
        Transform endPoint   = isTopLane ? topEnd   : bottomEnd;

        // Chọn xe ngẫu nhiên
        GameObject carPrefab = carPrefabs[Random.Range(0, carPrefabs.Length)];

        GameObject car = Instantiate(carPrefab, spawnPoint.position, Quaternion.identity);

        Vector3 dir = (endPoint.position - spawnPoint.position).normalized;

        CarMovement movement = car.GetComponent<CarMovement>();
        movement.Init(dir);

        // Xoay xe cho đúng hướng
        Quaternion lookRot = Quaternion.LookRotation(dir);
        car.transform.rotation = Quaternion.Euler(0, lookRot.eulerAngles.y, 0);
    }
}
