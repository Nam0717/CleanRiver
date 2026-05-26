using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public enum GameDifficulty
    {
        Normal,
        Hard,
        Extreme
    }
    [System.Serializable]
    public struct DifficultyData
    {
        [Header("Game Time")]
        public float gameTime;

        [Header("Tree Destroy")]
        public float treeCheckInterval;
        public int minTreesToDestroy;
        public int maxTreesToDestroy;
        public int requiredHits;

        [Header("Seed")]
        public int startSeed;
        public int seedCost;

        [Header("Win Condition")]
        public int targetProtectionScore;
        public int targetTree; 
    }
    [Header("Difficulty")]
    public GameDifficulty currentDifficulty;

    public DifficultyData normalDifficulty;
    public DifficultyData hardDifficulty;
    public DifficultyData extremeDifficulty;

    void ApplyDifficulty()
    {
        DifficultyData data = normalDifficulty;

        switch (currentDifficulty)
        {
            case GameDifficulty.Normal:
                data = normalDifficulty;
                break;
            case GameDifficulty.Hard:
                data = hardDifficulty;
                break;
            case GameDifficulty.Extreme:
                data = extremeDifficulty;
                break;
        }

        // ===== GAME TIMER =====
        gameDuration = data.gameTime;
        gameTimer = gameDuration;

        // ===== TREE DESTROY =====
        treeCheckInterval = data.treeCheckInterval;
        minTreesToDestroy = data.minTreesToDestroy;
        maxTreesToDestroy = data.maxTreesToDestroy;

        // ===== SEED =====
        seedCount = data.startSeed;
        seedCostPerPlant = data.seedCost;

        // ===== WIN CONDITION =====
        targetProtectionScore = data.targetProtectionScore;

        // ===== APPLY REQUIRED HITS TO ALL TREES =====
        foreach (var tree in treesOnMap)
        {
            TreeDestroyHandler handler = tree.GetComponent<TreeDestroyHandler>();
            if (handler != null)
            {
                handler.requiredHits = data.requiredHits;
            }
        }
        // ===== TARGET =====
        targetTree = data.targetTree;

        // ===== EXTREME MODE TIMER =====
        if (currentDifficulty == GameDifficulty.Extreme)
        {
            gameTimer = 0f;              // đếm lên
        }
        else
        {
            gameTimer = gameDuration;    // đếm ngược
        }


        // Update UI
        UpdateSeedUI();
        UpdateTreeUI();
        UpdateProtectionScoreUI();
        UpdateGameTimerUI();
    }

    [Header("Seed Settings")]
    public int seedCount = 30;
    public int seedCostPerPlant = 3;

    [Header("UI - Seed")]
    public TextMeshProUGUI seedText;

    [Header("UI - Tree Count")]
    public TextMeshProUGUI treeCountText;   // 👈 TEXT HIỂN THỊ SỐ CÂY

    [Header("Trees On Map")]
    public List<SeedSpawnerSimple> treesOnMap = new List<SeedSpawnerSimple>();

    [Header("Tree Destroy Settings")]
    public float treeCheckInterval = 15f;
    public int minTreesToDestroy = 1;
    public int maxTreesToDestroy = 2;
    private float treeTimer;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip chopSound;
    
    [Header("Game Timer")]
    public float gameDuration = 300f; // tổng thời gian game (giây)
    private float gameTimer;
    private bool isGameEnded;

    [Header("UI - Game Timer")]
    public TextMeshProUGUI gameTimerText;
    public Image gameTimerImage;

    [Header("Tree Protection Score")]
    public int protectionScore = 0;
    public int scorePerPlant = 15;
    public int scoreLostPerTree = 30;

    [Header("UI - Protection Score")]
    public TextMeshProUGUI protectionScoreText;

    [Header("Win Condition")]
    public int targetProtectionScore;
    public int targetTree; 

    [Header("UI - End Game")]
    public GameObject winPanel;
    public GameObject losePanel;

    public enum GameState
    {
        Playing,
        Paused,
        Win,
        Lose
    }

    public GameState currentState = GameState.Playing;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Time.timeScale = 0f;
        ApplyDifficulty();
        gameTimer = gameDuration;
        UpdateGameTimerUI();

        UpdateSeedUI();
        UpdateTreeUI();
        UpdateProtectionScoreUI();

        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
    }
    void Update()
    {
        HandleGameTimer();
        HandleTreeDestroyCheck();
        CheckWinCondition();
    }
    void CheckWinCondition()
    {
        if (isGameEnded) return;

        bool hasEnoughTrees = treesOnMap.Count >= targetTree;
        bool hasEnoughScore = protectionScore >= targetProtectionScore;

        if (hasEnoughTrees && hasEnoughScore)
        {
            WinGame();
        }
    }

    void WinGame()
    {
        if (isGameEnded) return;

        isGameEnded = true;
        currentState = GameState.Win;

        if (winPanel != null)
            winPanel.SetActive(true);

        PauseManager.Instance.Pause();
    }

    void HandleGameTimer()
    {
        if (isGameEnded) return;

        if (currentDifficulty == GameDifficulty.Extreme)
        {
            // 🔴 EXTREME: ĐẾM LÊN
            gameTimer += Time.deltaTime;
            UpdateGameTimerUI();
        }
        else
        {
            // 🟢 NORMAL / HARD: ĐẾM NGƯỢC
            gameTimer -= Time.deltaTime;
            gameTimer = Mathf.Max(gameTimer, 0f);

            UpdateGameTimerUI();

            if (gameTimer <= 0f)
            {
                EndGame();
            }
        }
    }

    void UpdateGameTimerUI()
    {
        // ===== TEXT mm:ss =====
        if (gameTimerText != null)
        {
            int minutes = Mathf.FloorToInt(gameTimer / 60f);
            int seconds = Mathf.FloorToInt(gameTimer % 60f);

            gameTimerText.text = $"{minutes}:{seconds:00}";
        }

        // ===== IMAGE FILL =====
        if (gameTimerImage != null && gameDuration > 0f)
    {
        if (currentDifficulty == GameDifficulty.Extreme)
        {
            gameTimerImage.fillAmount = Mathf.Clamp01(gameTimer / gameDuration);
        }
        else
        {
            gameTimerImage.fillAmount = gameTimer / gameDuration;
        }
    }

    }
    void EndGame()
    {
        if (isGameEnded) return;

        isGameEnded = true;
        currentState = GameState.Lose;

        if (losePanel != null)
            losePanel.SetActive(true);

        PauseManager.Instance.Pause();
    }

    public void PlayChopSoundAt(Vector3 position)
    {
        if (audioSource == null || chopSound == null) return;

        audioSource.transform.position = position;
        audioSource.PlayOneShot(chopSound);
    }

    // ================= SEED LOGIC =================

    public bool CanPlant()
    {
        return seedCount >= seedCostPerPlant;
    }

    public void ConsumeSeeds()
    {
        seedCount -= seedCostPerPlant;
        seedCount = Mathf.Max(seedCount, 0);
        UpdateSeedUI();
    }

    public void AddSeeds(int amount)
    {
        seedCount += amount;
        UpdateSeedUI();
    }

    void UpdateSeedUI()
    {
        if (seedText != null)
        {
            seedText.text = $"Hạt: {seedCount}";
        }
    }

    // ================= TREE LOGIC =================

    DifficultyData GetCurrentDifficultyData()
    {
        switch (currentDifficulty)
        {
            case GameDifficulty.Hard:
                return hardDifficulty;
            case GameDifficulty.Extreme:
                return extremeDifficulty;
            default:
                return normalDifficulty;
        }
    }

    public void RegisterTree(SeedSpawnerSimple tree)
    {
        if (!treesOnMap.Contains(tree))
        {
            treesOnMap.Add(tree);

            // 🌳 + điểm khi trồng
            protectionScore += scorePerPlant;

            // 🔥 ÁP DỤNG requiredHits CHO CÂY MỚI
            TreeDestroyHandler handler = tree.GetComponent<TreeDestroyHandler>();
            if (handler != null)
            {
                handler.requiredHits = GetCurrentDifficultyData().requiredHits;
            }

            UpdateTreeUI();
            UpdateProtectionScoreUI();
        }
    }

    public void UnregisterTree(SeedSpawnerSimple tree)
    {
        if (treesOnMap.Contains(tree))
        {
            treesOnMap.Remove(tree);

            // 🪓 - điểm khi mất cây
            protectionScore -= scoreLostPerTree;
            protectionScore = Mathf.Max(protectionScore, 0);

            UpdateTreeUI();
            UpdateProtectionScoreUI();
        }
    }
    public void AddProtectionScore(int amount)
    {
        protectionScore += amount;
        UpdateProtectionScoreUI();
    }

    void UpdateProtectionScoreUI()
    {
        if (protectionScoreText != null)
        {
            protectionScoreText.text = $"Điểm: {protectionScore} /{targetProtectionScore}";
        }
    }

    void UpdateTreeUI()
    {
        if (treeCountText != null)
        {
            treeCountText.text = $"Cây: {treesOnMap.Count} / {targetTree}";
        }
    }

    void HandleTreeDestroyCheck()
    {
        treeTimer += Time.deltaTime;

        if (treeTimer >= treeCheckInterval)
        {
            treeTimer = 0f;
            TriggerRandomTrees();
        }
    }

    void TriggerRandomTrees()
    {
        if (treesOnMap.Count == 0) return;

        int count = Random.Range(
            minTreesToDestroy,
            Mathf.Min(maxTreesToDestroy + 1, treesOnMap.Count + 1)
        );

        List<SeedSpawnerSimple> shuffled = new List<SeedSpawnerSimple>(treesOnMap);

        for (int i = 0; i < shuffled.Count; i++)
        {
            int rand = Random.Range(i, shuffled.Count);
            (shuffled[i], shuffled[rand]) = (shuffled[rand], shuffled[i]);
        }

        for (int i = 0; i < count; i++)
        {
            TreeDestroyHandler destroyHandler =
                shuffled[i].GetComponent<TreeDestroyHandler>();

            if (destroyHandler != null)
            {
                destroyHandler.StartDestroyCondition();
            }
        }
    }

}
