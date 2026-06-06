using UnityEngine;

// Khai báo Enum để phân loại màu kẹo
public enum CandyColor { Red, Green }

public class CandyItem : MonoBehaviour
{
    [Header("Candy Settings")]
    public CandyColor candyColor; // Chọn màu cho viên kẹo trong Inspector
    
    [HideInInspector] public Vector3 originalPosition; // Lưu vị trí gốc trên bàn
    private bool isSelected = false;

    void Start()
    {
        // Lưu lại vị trí ban đầu ngay khi vào game
        originalPosition = transform.position;
    }

    // Hàm nhấc viên kẹo lên cao hơn một chút khi được Click
    public void Select(float liftHeight)
    {
        if (!isSelected)
        {
            isSelected = true;
            transform.position = originalPosition + Vector3.up * liftHeight;
        }
    }

    // Hàm hạ viên kẹo về vị trí cũ nếu người chơi đổi ý chọn viên khác
    public void Deselect()
    {
        if (isSelected)
        {
            isSelected = false;
            transform.position = originalPosition;
        }
    }
}