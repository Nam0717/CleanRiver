using UnityEngine;

public class MoveBase : MonoBehaviour
{
    public float speed = 10f;

    // Biến để lưu tham chiếu đến bộ quản lý game
    private RunnerStats gameManager;

    void Start()
    {
        // Tự động tìm object có chứa script RunnerStats trong màn chơi
        // Cách này giúp bạn không cần phải kéo thả thủ công
        gameManager = FindObjectOfType<RunnerStats>();

        if (gameManager == null)
        {
            Debug.LogError("MoveBase không tìm thấy RunnerStats! Hãy chắc chắn bạn đã tạo GameController.");
        }
    }

    void Update()
    {
        // 1. Kiểm tra xem đã tìm thấy GameManager chưa
        // 2. Kiểm tra xem Game có đang chạy không (isGameRunning)
        // Nếu game KHÔNG chạy -> Dừng hàm Update tại đây (không di chuyển nữa)
        if (gameManager != null && !gameManager.isGameRunning)
        {
            return; 
        }

        // Nếu code chạy xuống được đây nghĩa là Game Đang Chạy
        // Base di chuyển về sau trục Z
        transform.Translate(Vector3.back * speed * Time.deltaTime);
    }
}