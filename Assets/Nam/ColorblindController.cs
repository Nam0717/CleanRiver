using UnityEngine;
using UnityEngine.Rendering; // Thư viện điều khiển URP Volume
using UnityEngine.Rendering.Universal; // 🔥 BẮT BUỘC THÊM: Thư viện quản lý thuộc tính nâng cao của Camera URP

public class ColorblindController : MonoBehaviour
{
    [Header("Camera Settings")]
    [SerializeField] private Camera targetCamera; // 🔥 Ô MỚI: Kéo Camera của Scene mới vào đây kìa Nam ơi!

    [Header("URP Volume Settings")]
    [SerializeField] private Volume originalVolume;   // Kéo Volume gốc (đang có màu) vào đây
    [SerializeField] private Volume colorblindVolume; // Kéo Volume mù màu (đen trắng) vào đây

    private bool isColorblindModeActive = false;

    void Start()
    {
        // 🔥 TỰ ĐỘNG KÍCH HOẠT: Ép Camera URP phải bật Post Processing bằng Code
        if (targetCamera == null)
        {
            targetCamera = Camera.main; // Nếu quên kéo thả, tự động tìm Main Camera của Scene
        }

        if (targetCamera != null)
        {
            // Lấy thành phần dữ liệu mở rộng của URP Camera
            var cameraData = targetCamera.GetComponent<UniversalAdditionalCameraData>();
            if (cameraData != null)
            {
                cameraData.renderPostProcessing = true; // Bật dấu tích Post Processing lên!
                Debug.Log($"<Color=Green>Đã kích hoạt Post Processing cho: {targetCamera.name}</Color>");
            }
        }
        else
        {
            Debug.LogError("Không tìm thấy Camera nào trong Scene để áp bộ lọc cả!");
        }

        // Khởi tạo ban đầu: Volume gốc bật (Weight = 1), Volume mù màu tắt (Weight = 0)
        if (originalVolume != null) originalVolume.weight = 1f;
        if (colorblindVolume != null) colorblindVolume.weight = 0f;
    }

    void Update()
    {
        // Ấn phím C để test nhanh trên bàn phím
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleColorblindMode();
        }
    }

    // Hàm gọi khi Click vào UI Button
    public void ToggleColorblindMode()
    {
        if (originalVolume == null || colorblindVolume == null)
        {
            Debug.LogError("Chưa kéo đủ 2 Volume vào GameManager kìa Nam ơi!");
            return;
        }

        // Đảo trạng thái true/false
        isColorblindModeActive = !isColorblindModeActive;

        if (isColorblindModeActive)
        {
            // BẬT CHẾ ĐỘ MÙ MÀU: Hạ volume gốc về 0, Đẩy volume đen trắng lên 1
            originalVolume.weight = 0f;
            colorblindVolume.weight = 1f;
        }
        else
        {
            // TẮT CHẾ ĐỘ MÙ MÀU: Đẩy volume gốc lên 1, Hạ volume đen trắng về 0
            originalVolume.weight = 1f;
            colorblindVolume.weight = 0f;
        }

        Debug.Log("Chế độ mù màu: " + (isColorblindModeActive ? "ĐÃ BẬT" : "ĐÃ TẮT"));
    }
}