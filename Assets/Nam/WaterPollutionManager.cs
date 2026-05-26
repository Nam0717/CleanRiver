using UnityEngine;
using UnityEngine.SceneManagement;

public class WaterPollutionManager : MonoBehaviour
{
    // Singleton để các script khác dễ dàng truy cập
    public static WaterPollutionManager Instance;

    [Header("Game Stats")]
    public int missedTrashCount = 0;
    public int maxMissedAllowed = 3;
    public int totalSorted = 0;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void OnTrashMissed()
    {
        missedTrashCount++;
        Debug.Log("Rác đã thoát: " + missedTrashCount + "/" + maxMissedAllowed);

        if (missedTrashCount > maxMissedAllowed)
        {
            HandleGameOver();
        }
    }

    public void OnTrashSorted(bool correct)
    {
        if (correct)
        {
            totalSorted++;
            Debug.Log("Lọc đúng! Tổng điểm: " + totalSorted);
        }
    }

    void HandleGameOver()
    {
        Debug.Log("Thua cuộc! Quá nhiều rác trôi mất.");
        // Load lại mini-game hoặc hiện UI
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}