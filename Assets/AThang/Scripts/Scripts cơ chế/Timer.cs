using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;
using TMPro;

public class TimerController : MonoBehaviour
{
    public float timeLeft = 120f; // 2 phút = 120 giây
    public TextMeshProUGUI timeText;         // Text hiển thị thời gian
    public FadePanel losePanel;  // Panel thua

    private bool isGameOver = false;


    void Update()
    {
        if (isGameOver) return;

        timeLeft -= Time.deltaTime;

        if (timeLeft <= 0)
        {
            timeLeft = 0;
            GameOver();
        }

        UpdateTimeUI();
    }

    void UpdateTimeUI()
    {
        int minutes = Mathf.FloorToInt(timeLeft / 60);
        int seconds = Mathf.FloorToInt(timeLeft % 60);
        timeText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    void GameOver()
    {
        isGameOver = true;
        losePanel.FadeIn();
  
    }

    // Nút chơi lại
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Backscene()
    {
        SceneManager.LoadScene("Thang");
    }
    public void BackSceneMain()
    {
        SceneManager.LoadScene("MainScene");
    }
}
