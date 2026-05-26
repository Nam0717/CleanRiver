using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaterManager : MonoBehaviour
{
    [Header("Cấu hình mặt nước")]
    public Renderer waterRenderer; 
    public Color dirtyColor = new Color(0.2f, 0.2f, 0.1f); 
    public Color cleanColor = new Color(0.3f, 0.7f, 1.0f); 
    
    [Header("Tiến trình")]
    public int totalTrashToClean = 15; // Đây là con số mục tiêu ban đầu
    private int collectedCount = 0;   
    private int depositedCount = 0;   
    private float cleanProgress = 0f;

    [Header("Giao diện UI")]
    public TextMeshProUGUI progressText;  
    public TextMeshProUGUI depositText;   
    public TextMeshProUGUI feedbackText; 
    public GameObject victoryPanel;      
    public Button restartButton;         

    void Start() {
        if (waterRenderer != null) {
            waterRenderer.material.color = dirtyColor;
        }
        if (victoryPanel != null) victoryPanel.SetActive(false);
        if (feedbackText != null) feedbackText.text = ""; 
        UpdateUI();
    }

    public void AddCleanProgress() {
        collectedCount++;
        UpdateWaterColor();
        UpdateUI();

        // Kiểm tra nếu đã đạt mục tiêu mới (bao gồm cả số rác bị trôi)
        if (collectedCount >= totalTrashToClean) {
            TrashSpawner spawner = FindObjectOfType<TrashSpawner>();
            if (spawner != null) spawner.enabled = false;
        }
    }

    // --- HÀM SỬA THEO Ý BẠN: Rác trôi mất sẽ tăng tổng số rác cần nhặt ---
    public void DecreaseCleanProgress() {
        // Tăng tổng số lượng rác mục tiêu lên 1
        totalTrashToClean++; 
        
        // Cập nhật lại màu nước (nước sẽ bẩn đi vì mẫu số totalTrashToClean to hơn)
        UpdateWaterColor();
        UpdateUI();
        
        // Hiển thị thông báo cảnh báo
        if (feedbackText != null) {
            feedbackText.text = "RÁC TRÔI MẤT! BẠN CẦN VỚT THÊM RÁC!";
            feedbackText.color = Color.yellow;
            CancelInvoke("ClearFeedback");
            Invoke("ClearFeedback", 2f);
        }
    }

    void UpdateWaterColor() {
        if (waterRenderer != null) {
            // Tỷ lệ sạch sẽ giảm xuống khi totalTrashToClean tăng lên
            cleanProgress = Mathf.Clamp01((float)collectedCount / totalTrashToClean);
            waterRenderer.material.color = Color.Lerp(dirtyColor, cleanColor, cleanProgress);
        }
    }

    public void AddDepositProgress() {
        depositedCount++;
        UpdateUI();
        ShowFeedback(true);

        // Thắng game khi số rác bỏ vào thùng bằng với tổng số rác mục tiêu hiện tại
        if (depositedCount >= totalTrashToClean) {
            FinishCleaningPhase();
        }
    }

    public void ShowFeedback(bool isCorrect) {
        if (feedbackText == null) return;
        if (isCorrect) {
            feedbackText.text = "CHÍNH XÁC!";
            feedbackText.color = Color.green;
        } else {
            feedbackText.text = "SAI THÙNG RỒI!";
            feedbackText.color = Color.red;
        }
        CancelInvoke("ClearFeedback");
        Invoke("ClearFeedback", 2f);
    }

    void ClearFeedback() {
        if (feedbackText != null) feedbackText.text = "";
    }

    void UpdateUI() {
        if (progressText != null) {
            progressText.text = $"Đã vớt: {collectedCount}/{totalTrashToClean}";
        }
        if (depositText != null) {
            depositText.text = $"Đã nộp thùng: {depositedCount}/{totalTrashToClean}";
        }
    }

    void FinishCleaningPhase() {
        if (victoryPanel != null) {
            victoryPanel.SetActive(true);
        }
    }

    public void RestartLevel() {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}