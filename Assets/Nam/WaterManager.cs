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
    [Tooltip("Số lượng rác cố định cần phải nộp vào thùng để THẮNG (Giữ nguyên ban đầu, không tăng).")]
    public int totalTrashToClean = 15; 
    
    // 🔥 BIẾN MỚI: Số rác mục tiêu PHẢI VỚT (Biến này sẽ tăng lên nếu bị phạt)
    private int totalTrashToCollect;   
    private int collectedCount = 0;   
    private int depositedCount = 0;   
    private float cleanProgress = 0f;
    private int missedTrashCount = 0; 

    [Header("Giao diện UI")]
    public TextMeshProUGUI progressText;  
    public TextMeshProUGUI depositText;   
    public TextMeshProUGUI feedbackText; 
    public GameObject victoryPanel;      
    public Button restartButton;         

    void Start() {
        // 🔥 KHỞI TẠO: Ban đầu số rác phải vớt sẽ bằng y chang số rác cần nộp thùng
        totalTrashToCollect = totalTrashToClean;

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

        // 🔥 SO SÁNH VỚI MỤC TIÊU VỚT: Đạt đủ số lượng vớt (bao gồm cả rác phạt) thì dừng Spawner
        if (collectedCount >= totalTrashToCollect) {
            TrashSpawner spawner = FindObjectOfType<TrashSpawner>();
            if (spawner != null) spawner.enabled = false;
        }
    }

    public void DecreaseCleanProgress() {
        missedTrashCount++; 

        if (missedTrashCount % 2 == 0) {
            // 🔥 CHỈ PHẠT TĂNG: Số lượng rác mục tiêu cần phải vớt lên 1
            totalTrashToCollect++; 
            
            if (feedbackText != null) {
                feedbackText.text = "2 RÁC TRÔI MẤT! BẠN BỊ PHẠT VỚT BÙ THÊM 1 RÁC!";
                feedbackText.color = Color.red;
            }
        } 
        else {
            if (feedbackText != null) {
                feedbackText.text = "1 RÁC TRÔI MẤT! CẨN THẬN ĐỪNG ĐỂ TRÔI THÊM!";
                feedbackText.color = Color.yellow;
            }
        }
        
        UpdateWaterColor();
        UpdateUI();
        
        CancelInvoke("ClearFeedback");
        Invoke("ClearFeedback", 2f);
    }

    void UpdateWaterColor() {
        if (waterRenderer != null) {
            // Màu nước sẽ tính toán dựa trên tiến độ vớt thực tế / số rác cần phải vớt
            cleanProgress = Mathf.Clamp01((float)collectedCount / totalTrashToCollect);
            waterRenderer.material.color = Color.Lerp(dirtyColor, cleanColor, cleanProgress);
        }
    }

    public void AddDepositProgress() {
        depositedCount++;
        UpdateUI();
        ShowFeedback(true);

        // 🔥 ĐIỀU KIỆN THẮNG CỐ ĐỊNH: Chỉ so sánh với totalTrashToClean (mục tiêu nộp ban đầu)
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
            // Hiển thị số rác đã vớt trên tổng mục tiêu vớt (mẫu số này CÓ TĂNG khi phạt)
            progressText.text = $"Đã vớt: {collectedCount}/{totalTrashToCollect}";
        }
        if (depositText != null) {
            // Hiển thị số rác đã nộp trên tổng số cố định ban đầu (mẫu số này GIỮ NGUYÊN)
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