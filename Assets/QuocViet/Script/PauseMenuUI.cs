using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    [Header("UI")]
    public GameObject pausePanel;

    [Header("Scene")]
    public int menuSceneIndex = 0; // index scene menu trong Build Settings

    void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }

    void Update()
    {
        if (PauseManager.Instance == null || GameManager.Instance == null)
            return;

        // ❌ Không hiện pause menu khi Win / Lose
        if (GameManager.Instance.currentState == GameManager.GameState.Win ||
            GameManager.Instance.currentState == GameManager.GameState.Lose)
        {
            pausePanel.SetActive(false);
            return;
        }

        pausePanel.SetActive(PauseManager.Instance.IsPaused);
    }

    // ===== BUTTON EVENTS =====

    // Nút PAUSE (bấm từ HUD)
    public void PauseGame()
    {
        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.Pause();
        }

        // Bật panel nếu đang tắt
        if (pausePanel != null && !pausePanel.activeSelf)
        {
            pausePanel.SetActive(true);
        }
    }
    public void PauseGame2()
    {
        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.Pause();
        }
    }


    // Nút RESUME
    public void ResumeGame()
    {
        Time.timeScale = 1f;
        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.Resume();
        }

        // Tắt panel
        if (pausePanel != null && pausePanel.activeSelf)
        {
            pausePanel.SetActive(false);
        }
    }

    // Nút BACK TO MENU
    public void BackToMenu()
    {
        // 🔒 đảm bảo không bị kẹt timeScale
        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.Resume();
        }

        SceneManager.LoadScene(menuSceneIndex);
    }

    public void RestartScene()
    {
        // 🔒 đảm bảo không bị kẹt pause
        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.Resume();
        }

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    // Quit Game
    public void QuitGame()
    {
        // 🔒 đảm bảo không bị kẹt pause
        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.Resume();
        }

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }

}
