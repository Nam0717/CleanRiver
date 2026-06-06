using UnityEngine;
using UnityEngine.Rendering; // Thư viện điều khiển URP Volume

public class ColorblindController : MonoBehaviour
{
    [Header("URP Volume Settings")]
    [SerializeField] private Volume originalVolume;   // Kéo Volume gốc (đang có màu) vào đây
    [SerializeField] private Volume colorblindVolume; // Kéo Volume mù màu (đen trắng) vào đây

    private bool isColorblindModeActive = false;

    void Start()
    {
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