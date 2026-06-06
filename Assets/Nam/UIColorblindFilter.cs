using UnityEngine;
using UnityEngine.UI; // Thư viện để quản lý UI Image cơ bản

public class UIClusterColorblindFilter : MonoBehaviour
{
    [Header("Danh sách các Cụm UI Cha (Panel/GameObject)")]
    [SerializeField] private GameObject[] uiGroups; 

    [Header("Material Đen Trắng")]
    [SerializeField] private Material grayscaleMaterial; // Kéo file Mat_UIGrayscale vào đây

    private bool isGrayscale = false;

    void Update()
    {
        // Ấn phím C để test nhanh đồng bộ cả cụm UI
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleUIClusterFilter();
        }
    }

    // Hàm public để gắn vào sự kiện OnClick của Button UI
    public void ToggleUIClusterFilter()
    {
        isGrayscale = !isGrayscale;

        // Duyệt qua từng cụm UI cha mà Nam đã kéo vào danh sách
        foreach (GameObject group in uiGroups)
        {
            if (group == null) continue;

            // 🔥 TỰ ĐỘNG LÙNG SỤC: Tìm tất cả các thành phần Image nằm trong cụm UI này
            Image[] allImages = group.GetComponentsInChildren<Image>(true);
            foreach (Image img in allImages)
            {
                // Áp Material đen trắng nếu bật, trả về null (màu gốc) nếu tắt
                img.material = isGrayscale ? grayscaleMaterial : null;
            }

            // Tự động tìm luôn các thành phần RawImage (nếu nhóm bạn có xài để hiển thị render texture)
            RawImage[] allRawImages = group.GetComponentsInChildren<RawImage>(true);
            foreach (RawImage rawImg in allRawImages)
            {
                rawImg.material = isGrayscale ? grayscaleMaterial : null;
            }
        }

        Debug.Log("Đã hoán đổi bộ lọc đen trắng cho toàn bộ cụm UI: " + (isGrayscale ? "BẬT" : "TẮT"));
    }
}