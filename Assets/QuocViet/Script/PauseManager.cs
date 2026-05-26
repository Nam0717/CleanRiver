using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance;

    public bool IsPaused { get; private set; }

    void Awake()
    {
        // 🔥 CỨU GAME MỖI KHI LOAD SCENE / CRASH
        Time.timeScale = 1f;

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Escape))
    //     {
    //         TogglePause();
    //     }
    // }

    void OnApplicationQuit()
    {
        Time.timeScale = 1f;
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            // failsafe: KHÔNG cho kẹt timeScale
            Time.timeScale = 1f;
            IsPaused = false;
        }
    }

    public void Pause()
    {
        if (IsPaused) return;

        IsPaused = true;
        Time.timeScale = 0f;
    }

    public void Resume()
    {
        if (!IsPaused) return;

        IsPaused = false;
        Time.timeScale = 1f;
    }

    public void TogglePause()
    {
        if (IsPaused)
            Resume();
        else
            Pause();
    }
}
