using UnityEngine;

public class TrainCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // 1. Kiểm tra xem đối tượng đâm vào tàu có phải là Xe của Người chơi không
        // Check theo Tag "Player" (Bạn nhớ gán Tag Player cho xe của mình nhé)
        if (other.CompareTag("Player"))
        {
            // 2. Tìm thành phần RunnerStats để thực hiện trừ máu
            RunnerStats playerStats = other.GetComponent<RunnerStats>();

            // Nếu script RunnerStats nằm ở Object tổng khác, ta tìm diện rộng
            if (playerStats == null)
            {
                playerStats = FindObjectOfType<RunnerStats>();
            }

            // 3. Tiến hành trừ máu do tai nạn va chạm với tàu hỏa
            if (playerStats != null && !playerStats.isGameOver)
            {
                playerStats.TakeDamage(1);
                Debug.Log("💥 TAI NẠN: Xe nhả phanh sớm lao lên đâm sầm vào đoàn tàu đang băng qua đường ray!");
            }
        }
    }
}