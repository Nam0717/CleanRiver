using UnityEngine;

public class ObstacleDamage : MonoBehaviour
{
    [Header("Cấu hình Sát thương")]
    public int damageAmount = 1; 

    private bool hasHit = false; 
    
    // Không cần biến Collider và Renderer nữa vì ta sẽ không tắt chúng
    
    private void OnEnable()
    {
        ResetObstacle();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Kiểm tra nếu đã va chạm rồi thì bỏ qua ngay
        if (hasHit) return; 

        if (other.CompareTag("Player"))
        {
            // 1. Kích hoạt hiệu ứng trên Player (Rung, Nháy đỏ, Bất tử 1s)
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.HitObstacleEffect(); 
            }

            // 2. Trừ máu
            RunnerStats stats = null;
            if (player != null) stats = player.GetComponent<RunnerStats>();
            
            if (stats == null)
            {
                GameObject gm = GameObject.Find("GameManager"); 
                if (gm != null) stats = gm.GetComponent<RunnerStats>();
            }

            if (stats != null)
            {
                stats.TakeDamage(damageAmount);
                Debug.Log("Đã va chạm vật cản: " + gameObject.name);
            }

            // 3. ĐÁNH DẤU ĐÃ VA CHẠM
            // Chỉ đánh dấu là đã đụng để không trừ máu lặp lại.
            // KHÔNG ẩn vật cản, KHÔNG tắt collider của vật cản.
            hasHit = true; 
        }
    }

    // Hàm Reset dùng cho Pooling (khi map quay vòng lại)
    public void ResetObstacle()
    {
        // Chỉ cần reset trạng thái va chạm để lần sau đụng tiếp
        hasHit = false;
    }
}