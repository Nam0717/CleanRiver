using UnityEngine;
using UnityEngine.SceneManagement;

public class ShipMoveToPoint : MonoBehaviour
{
    public Transform pointB;        // Điểm đích
    public float speed = 5f;        // Tốc độ di chuyển
    public string sceneToLoad;      // Tên scene sẽ chuyển

    private bool isMoving = false;

    void Update()
    {
        if (isMoving)
        {
            MoveToPoint();
        }
    }

    void MoveToPoint()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            pointB.position,
            speed * Time.deltaTime
        );

        // Kiểm tra nếu đã tới nơi
        if (Vector3.Distance(transform.position, pointB.position) < 0.1f)
        {
            isMoving = false;
            FindAnyObjectByType<FadeManager>().FadeAndLoadScene(sceneToLoad);
        }
    }

    // Hàm này sẽ gọi khi nhấn button
    public void StartMoving()
    {
        isMoving = true;
    }
}