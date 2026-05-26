using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CarGameManager : MonoBehaviour
{
    public static CarGameManager Instance;

    [Header("Win / Lose")]
    public int winTarget = 20;
    public int maxMiss = 5;

    [Header("Difficulty")]
    public float difficultyInterval = 20f;

    [Header("UI")]
    public TMP_Text cleanedText;
    public TMP_Text missedText;
    public TMP_Text timeText;
    public TMP_Text difficultyText;

    [Header("Result Canvas")]
    public GameObject winCanvas;
    public GameObject loseCanvas;

    private int cleanedCount = 0;
    private int missedCount = 0;

    private float elapsedTime = 0f;
    private float nextDifficultyTime = 20f;

    public int CurrentDifficultyLevel { get; private set; } = 0;

    private bool gameEnded = false;
    public PauseMenuUI pause;

    void Start()
    {
        pause.PauseGame();
    }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        if (gameEnded) return;

        elapsedTime += Time.deltaTime;
        UpdateTimeUI();

        if (elapsedTime >= nextDifficultyTime)
        {
            IncreaseDifficulty();
            nextDifficultyTime += difficultyInterval;
        }
    }

    void UpdateTimeUI()
    {
        int seconds = Mathf.FloorToInt(elapsedTime);
        timeText.text = $"Time: {seconds}s";
    }

    void IncreaseDifficulty()
    {
        CurrentDifficultyLevel++;
        difficultyText.text = $"Difficulty: {CurrentDifficultyLevel}";

        Debug.Log($"🔥 Difficulty Up! Level {CurrentDifficultyLevel}");
    }

    public void OnCarCleaned()
    {
        if (gameEnded) return;

        cleanedCount++;
        cleanedText.text = $"Cleaned: {cleanedCount}/{winTarget}";

        if (cleanedCount >= winTarget)
        {
            Win();
        }
    }

    public void OnCarMissed()
    {
        if (gameEnded) return;

        missedCount++;
        missedText.text = $"Missed: {missedCount}/{maxMiss}";

        if (missedCount >= maxMiss)
        {
            Lose();
        }
    }

    void Win()
    {
        gameEnded = true;
        winCanvas.SetActive(true);
        Debug.Log("YOU WIN!");
        Time.timeScale = 0f;
    }

    void Lose()
    {
        gameEnded = true;
        loseCanvas.SetActive(true);
        Debug.Log("YOU LOSE!");
        Time.timeScale = 0f;
    }

    public void OnAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1f;
    }

    public void OnExit()
    {
        SceneManager.LoadScene("Kien1"); // đổi đúng tên scene map của bạn
        Time.timeScale =1f;
    }
}
