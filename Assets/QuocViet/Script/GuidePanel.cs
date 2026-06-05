using UnityEngine;
using UnityEngine.UI;

public class GuideUI : MonoBehaviour
{
    public Image guideImage;          // Image để hiển thị hình
    public Sprite[] guideSprites;     // danh sách hình hướng dẫn

    private int index = 0;

    private void OnEnable()
    {
        // 🔥 ĐỒNG BỘ DỪNG GAME: Đóng băng toàn bộ thời gian hệ thống ngay khi bảng UI xuất hiện
        Time.timeScale = 1f;
        Debug.Log("[GuideUI] Đã dừng game để người chơi xem hướng dẫn.");
    }

    void Start()
    {
        ShowImage(0);
    }

    void ShowImage(int i)
    {
        index = Mathf.Clamp(i, 0, guideSprites.Length - 1);

        if (guideImage != null && guideSprites.Length > 0)
        {
            guideImage.sprite = guideSprites[index];
        }
    }

    public void Next()
    {
        if (guideSprites.Length == 0) return;

        index++;
        if (index >= guideSprites.Length) index = 0; // quay vòng
        ShowImage(index);
    }

    public void Prev()
    {
        if (guideSprites.Length == 0) return;

        index--;
        if (index < 0) index = guideSprites.Length - 1; // quay vòng
        ShowImage(index);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        // 🔥 ĐỒNG BỘ CHẠY TIẾP: Trả lại tốc độ thời gian bình thường khi bảng UI bị ẩn đi
        Time.timeScale = 1f;
        Debug.Log("[GuideUI] Đã đóng hướng dẫn, tiếp tục trò chơi.");
    }
}