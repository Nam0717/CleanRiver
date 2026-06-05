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

    // 🔥 BIẾN MỚI THEO Ý BẠN
    [Tooltip("Số giây chờ (Delay) trước khi tàu chạy ở MAP ĐÈN XANH (để tạo hiệu ứng tàu chạy qua trước mắt người chơi từ xa).")]
    public float greenLightTrainDelay = 3.0f; 

    [Header("Cấu hình Tàu Hỏa")]
    public GameObject trainObject;         
    public Vector3 trainMoveDirection = Vector3.right; 
    public float trainSpeed = 45f;         

    [Header("Âm Thanh Tàu Hỏa")]
    public AudioSource trainAudioSource;   // Kéo thành phần AudioSource của trạm/tàu vào đây
    public AudioClip trainRunningSound;    // Kéo file âm thanh tiếng tàu chạy/hú còi vào đây

    private bool isTrainMoving = false;
    private Vector3 initialTrainLocalPosition; 
    private Coroutine delayCoroutine; 
    private Coroutine greenTrainCoroutine; // 🔥 Quản lý tiến trình delay tàu đèn xanh

    void Awake()
    {
        if (trainObject != null)
        {
            initialTrainLocalPosition = trainObject.transform.localPosition;
        }

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

        if (trainAudioSource != null)
        {
            trainAudioSource.Stop();
        }

        // Dọn dẹp các Coroutine cũ tránh bị chồng lặp khi tái sử dụng từ Pool
        if (delayCoroutine != null)
        {
            StopCoroutine(delayCoroutine);
            delayCoroutine = null;
        }
        if (greenTrainCoroutine != null)
        {
            StopCoroutine(greenTrainCoroutine);
            greenTrainCoroutine = null;
        }

        // KÍCH HOẠT LOGIC TÀU CHẠY CHO ĐÈN XANH (CÓ DELAY)
        if (!isRedLightMap)
        {
            greenTrainCoroutine = StartCoroutine(DelayedGreenLightTrain());
        }

        RunnerStats stats = FindObjectOfType<RunnerStats>();
        if (stats != null)
        {
            stats.trafficMapCount++;
            float chosenDelay = (stats.trafficMapCount == 1) ? initialTriggerDelay : subsequentTriggerDelay;
            delayCoroutine = StartCoroutine(DelayedTrafficTrigger(chosenDelay, stats));
        }
    }

    // 🔥 COROUTINE MỚI: Xử lý delay tàu chạy ở map đèn xanh
    private IEnumerator DelayedGreenLightTrain()
    {
        // Chờ đúng số giây Nam cấu hình trên Inspector rồi mới cho tàu chạy
        yield return new WaitForSeconds(greenLightTrainDelay);
        
        isTrainMoving = true;
        if (trainAudioSource != null && trainRunningSound != null)
        {
            trainAudioSource.clip = trainRunningSound;
            trainAudioSource.Play();
        }
        Debug.Log($"[TrainTrackMapTrigger] Đèn Xanh: Hết {greenLightTrainDelay}s delay -> Tàu hỏa bắt đầu chạy qua giao lộ từ xa!");
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
        if (isTrainMoving && trainObject != null)
        {
            trainObject.transform.Translate(trainMoveDirection * trainSpeed * Time.deltaTime, Space.Self);
        }
    }

    // Hàm này được RunnerStats gọi chuẩn xác ngay khi đèn chuyển sang màu ĐỎ (Chỉ dùng cho Map Đèn Đỏ)
    public void StartTrainMovement()
    {
        isTrainMoving = true;
        Debug.Log("[TrainTrackMapTrigger] Đèn đỏ sáng lên -> Tàu hỏa xuất kích vượt giao lộ!");

        if (trainAudioSource != null && trainRunningSound != null)
        {
            trainAudioSource.clip = trainRunningSound;
            trainAudioSource.Play();
        }
    }

    void OnDisable()
    {
        if (trainAudioSource != null)
        {
            trainAudioSource.Stop();
        }

        if (delayCoroutine != null)
        {
            StopCoroutine(delayCoroutine);
            delayCoroutine = null;
        }

        // 🔥 DỌN DẸP AN TOÀN: Hủy tiến trình chờ tàu nếu map bị thu hồi sớm
        if (greenTrainCoroutine != null)
        {
            StopCoroutine(greenTrainCoroutine);
            greenTrainCoroutine = null;
        }
    }
}