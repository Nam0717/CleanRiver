using UnityEngine;

public class TrainTrackMapTrigger : MonoBehaviour
{
    [Header("Cấu hình phân loại Map")]
    [Tooltip("Tích chọn nếu đây là Map có Đèn Đỏ + Tàu chạy. Bỏ tích nếu đây là Map có Đèn Xanh đường đông đúc.")]
    public bool isRedLightMap = true; 

    [Header("Cấu hình Tàu Hỏa (Chỉ dùng cho Map Đèn Đỏ)")]
    public GameObject trainObject;         // Kéo mô hình đoàn tàu nằm trong Prefab vào đây
    public Vector3 trainMoveDirection = Vector3.right; // Hướng tàu chạy (Cắt ngang trước mặt xe người chơi)
    public float trainSpeed = 35f;         // Tốc độ tàu chạy (Chỉnh để vừa vặn lướt qua hết thân tàu trong 3 giây)

    private bool isTrainMoving = false;
    private Vector3 initialTrainLocalPosition; // Dùng để reset vị trí tàu khi map được tái chế sử dụng lại

    void Awake()
    {
        // Lưu lại vị trí ẩn ban đầu của tàu hỏa để reset khi tái sử dụng Pool
        if (trainObject != null)
        {
            initialTrainLocalPosition = trainObject.transform.localPosition;
        }
    }

    // Hàm gọi tự động khi khối map này được lấy ra từ Object Pool và SetActive(true)
    void OnEnable()
    {
        isTrainMoving = false;
        
        // Trở lại vị trí ẩn ban đầu để chuẩn bị cho lượt chạy mới
        if (trainObject != null)
        {
            trainObject.transform.localPosition = initialTrainLocalPosition;
        }

        // Tìm kiếm và kích hoạt chuỗi đèn trên HUD hệ thống
        RunnerStats stats = FindObjectOfType<RunnerStats>();
        if (stats != null)
        {
            stats.TriggerTrafficLightSequence(isRedLightMap, this);
        }
    }

    void Update()
    {
        // Di chuyển tịnh tiến đoàn tàu theo thời gian thực khi có lệnh kích hoạt đèn đỏ
        if (isRedLightMap && isTrainMoving && trainObject != null)
        {
            trainObject.transform.Translate(trainMoveDirection * trainSpeed * Time.deltaTime, Space.Self);
        }
    }

    // Hàm này sẽ được gọi chuẩn xác từ Coroutine của RunnerStats ngay khi đèn chuyển ĐỎ
    public void StartTrainMovement()
    {
        isTrainMoving = true;
        Debug.Log("[TrainTrackMapTrigger] Đèn đỏ bật -> Tàu hỏa xuất kích vượt giao lộ!");
    }
}