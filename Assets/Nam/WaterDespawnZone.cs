using UnityEngine;

public class WaterDespawnZone : MonoBehaviour
{
    private WaterManager waterMan;

    void Start() {
        // Tìm Manager trong Scene để báo cáo tình hình
        waterMan = FindObjectOfType<WaterManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        WaterTrashObject trash = other.GetComponent<WaterTrashObject>();
        
        // Kiểm tra nếu vật chạm vào là Rác và nó vẫn đang ở dưới nước (chưa được vớt)
        if (trash != null && trash.isOnWater)
        {
            Debug.Log("<color=orange>Rác đã trôi thoát mất! Sông bẩn trở lại.</color>");
            
            // Gọi hàm giảm tiến trình sạch và làm đục nước
            if (waterMan != null) {
                waterMan.DecreaseCleanProgress();
            }

            // Xóa rác để giải phóng bộ nhớ
            Destroy(other.gameObject);
        }
    }
}