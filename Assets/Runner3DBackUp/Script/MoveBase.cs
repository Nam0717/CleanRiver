using UnityEngine;

public class MoveBase : MonoBehaviour
{
    [Tooltip("Tốc độ mặc định nếu không tìm thấy GameManager")]
    public float speed = 10f;

    // Biến để lưu tham chiếu đến bộ quản lý game
    private RunnerStats gameManager;

    void Start()
    {
        // Tự động tìm object có chứa script RunnerStats trong màn chơi
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

        // 🔥 ĐÃ SỬA: Lấy tốc độ đồng bộ thời gian thực từ RunnerStats
        // Khi người chơi đè giữ Space -> gameManager.runSpeed sẽ về 0 -> currentSpeed lập tức về 0
        float currentSpeed = (gameManager != null) ? gameManager.runSpeed : speed;

        // Base di chuyển về sau trục Z dựa theo tốc độ thực tế của màn chơi
        transform.Translate(Vector3.back * currentSpeed * Time.deltaTime);
    }
}