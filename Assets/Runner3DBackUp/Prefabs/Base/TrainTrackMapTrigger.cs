using UnityEngine;
using System.Collections;

public class TrainTrackMapTrigger : MonoBehaviour
{
    [Header("Cấu hình phân loại Map")]
    [Tooltip("Tích chọn nếu đây là Map Đèn Đỏ có tàu. Bỏ tích nếu đây là Map Đèn Xanh đường đông đúc.")]
    public bool isRedLightMap = true; 

    [Header("Cấu hình Căn Chỉnh Thời Gian Từng Lượt")]
    [Tooltip("Số giây chờ cho MAP ĐẶC BIỆT ĐẦU TIÊN xuất hiện trong game để dừng đúng vạch.")]
    public float initialTriggerDelay = 8.0f; 

    [Tooltip("Số giây chờ cho CÁC LẦN XUẤT HIỆN TIẾP THEO (Từ map đặc biệt thứ 2 trở đi).")]
    public float subsequentTriggerDelay = 9.0f; 

    [Header("Cấu hình Tàu Hỏa (Chỉ dùng cho Map Đèn Đỏ)")]
    public GameObject trainObject;         
    public Vector3 trainMoveDirection = Vector3.right; 
    public float trainSpeed = 45f;         

    // 🔥 CẤU HÌNH MỚI: ÂM THANH TÀU CHẠY
    [Header("Âm Thanh Tàu Hỏa (Mới thêm)")]
    public AudioSource trainAudioSource;   // Kéo thành phần AudioSource của trạm/tàu vào đây
    public AudioClip trainRunningSound;    // Kéo file âm thanh tiếng tàu chạy/hú còi vào đây

    private bool isTrainMoving = false;
    private Vector3 initialTrainLocalPosition; 
    private Coroutine delayCoroutine; 

    void Awake()
    {
        if (trainObject != null)
        {
            initialTrainLocalPosition = trainObject.transform.localPosition;
        }

        // Tự động tìm AudioSource trên chính Object này nếu Nam quên không kéo thả
        if (trainAudioSource == null)
        {
            trainAudioSource = GetComponent<AudioSource>();
        }
    }

    void OnEnable()
    {
        isTrainMoving = false;
        
        if (trainObject != null)
        {
            trainObject.transform.localPosition = initialTrainLocalPosition;
        }

        // Tắt âm thanh tàu của lượt cũ nếu có khi vừa kích hoạt lại từ Pool
        if (trainAudioSource != null)
        {
            trainAudioSource.Stop();
        }

        if (delayCoroutine != null)
        {
            StopCoroutine(delayCoroutine);
        }

        RunnerStats stats = FindObjectOfType<RunnerStats>();
        if (stats != null)
        {
            stats.trafficMapCount++;
            float chosenDelay = (stats.trafficMapCount == 1) ? initialTriggerDelay : subsequentTriggerDelay;
            delayCoroutine = StartCoroutine(DelayedTrafficTrigger(chosenDelay, stats));
        }
    }

    private IEnumerator DelayedTrafficTrigger(float delay, RunnerStats stats)
    {
        yield return new WaitForSeconds(delay);
        if (stats != null)
        {
            stats.TriggerTrafficLightSequence(isRedLightMap, this);
        }
    }

    void Update()
    {
        if (isRedLightMap && isTrainMoving && trainObject != null)
        {
            trainObject.transform.Translate(trainMoveDirection * trainSpeed * Time.deltaTime, Space.Self);
        }
    }

    // Hàm này được RunnerStats gọi chuẩn xác ngay khi đèn chuyển sang màu ĐỎ
    public void StartTrainMovement()
    {
        isTrainMoving = true;
        Debug.Log("[TrainTrackMapTrigger] Đèn đỏ sáng lên -> Tàu hỏa xuất kích vượt giao lộ!");

        // 🔥 KÍCH HOẠT PHÁT ÂM THANH TÀU CHẠY
        if (trainAudioSource != null && trainRunningSound != null)
        {
            trainAudioSource.clip = trainRunningSound;
            trainAudioSource.Play();
        }
    }

    void OnDisable()
    {
        // 🔥 TỰ ĐỘNG DỌN DẸP: Tắt ngay âm thanh khi miếng map bị thu hồi về Pool để tránh lỗi chồng tiếng
        if (trainAudioSource != null)
        {
            trainAudioSource.Stop();
        }

        if (delayCoroutine != null)
        {
            StopCoroutine(delayCoroutine);
            delayCoroutine = null;
        }
    }
}