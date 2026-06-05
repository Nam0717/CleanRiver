using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class RunnerStats : MonoBehaviour
{
    [Header("Cấu hình Game")]
    public float runSpeed = 10f;
    public int maxHP = 3;
    public float distancePerLevel = 100f;

    [Header("Cấu hình Sao (Mốc khoảng cách)")]
    public float dist1Star = 100f;
    public float dist2Star = 200f;
    public float dist3Star = 300f;

    [Header("Âm thanh Đạt Mốc")]
    public AudioSource sfxSource;       
    public AudioClip starUnlockSound;   
    public float soundInterval = 100f; 
    private float nextSoundMilestone;  

    [Header("Thông số hiện tại")]
    public int currentHP;
    public float currentDistance = 0f;
    private int currentLevel = 0;
    
    public bool isGameRunning = false; 
    public bool isGameOver = false;
    public static bool autoStart = false; 

    [Header("UI Trong Game (HUD)")]
    public Slider hpBar;
    public Slider seedBar; 
    public TextMeshProUGUI scoreText;

    [Header("UI Menu (Start Screen)")]
    public GameObject menuPanel; 
    public TextMeshProUGUI menuHighScoreText;

    [Header("UI Kết Thúc (Lose)")]
    public GameObject losePanel;
    public TextMeshProUGUI finalScoreText;
    
    [Header("UI Sao Kết Quả")]
    public Image[] starImages; 
    public Color starUnlockedColor = Color.white; 
    public Color starLockedColor = new Color(0.3f, 0.3f, 0.3f, 1f); 

    [Header("Hiệu ứng Visual")]
    public float punchScale = 1.5f;
    public float punchDuration = 0.2f;
    private Coroutine punchCoroutine;

    // ================= HỆ THỐNG ĐÈN GIAO THÔNG CHẮN TÀU =================
    [Header("UI Đèn Giao Thông")]
    public GameObject trafficLightPanel; 
    public Image yellowLightImg;
    public Image redLightImg;
    public Image greenLightImg;
    public GameObject spacePromptUI;     // Gợi ý UI bấm phanh dưới chân đèn

    private float baseRunSpeed;           
    private enum LightState { None, Yellow, Red, Green }
    private LightState currentLightState = LightState.None;
    private bool hasProcessedDamage = false; 

    void Awake()
    {
        if (!autoStart)
        {
            Time.timeScale = 0f;    
            isGameRunning = false;  
            if (menuPanel != null) menuPanel.SetActive(true);
            if (losePanel != null) losePanel.SetActive(false);
        }
    }

    void Start()
    {
        currentHP = maxHP;
        currentDistance = 0f;
        currentLevel = 0;
        isGameOver = false;
        
        baseRunSpeed = runSpeed; 
        nextSoundMilestone = soundInterval; 
        
        if (sfxSource == null) sfxSource = GetComponent<AudioSource>(); 

        if (trafficLightPanel != null) trafficLightPanel.SetActive(false);
        if (spacePromptUI != null) spacePromptUI.SetActive(false);

        UpdateUI(); 

        if (autoStart) StartGameLogic();
        else ShowMenuLogic();
    }

    void Update()
    {
        if (!isGameRunning || isGameOver) return;

        // --- XỬ LÝ NHẤN GIỮ PHÍM SPACE ĐỂ PHANH XE ---
        if (currentLightState != LightState.None)
        {
            if (Input.GetKey(KeyCode.Space)) // Đang giữ phanh
            {
                runSpeed = 0f; // Map dừng chạy, người chơi dừng trước đường ray

                // BẪY PHẠT: Đang đèn Xanh mà phanh gấp -> Xe sau đâm -> Trừ máu
                if (currentLightState == LightState.Green && !hasProcessedDamage)
                {
                    TakeDamage(1);
                    hasProcessedDamage = true;
                    Debug.Log("Lỗi: Đèn xanh không được dừng! Bị xe sau tông trúng.");
                }
            }
            else // Không nhấn giữ phanh (Thả tự do)
            {
                runSpeed = baseRunSpeed; 

                // BẪY PHẠT: Đang đèn Đỏ không chịu dừng -> Đâm vào tàu hỏa -> Trừ máu
                if (currentLightState == LightState.Red && !hasProcessedDamage)
                {
                    TakeDamage(1);
                    hasProcessedDamage = true;
                    Debug.Log("Lỗi: Vượt đèn đỏ! Va chạm dữ dội với tàu hỏa.");
                }
            }
        }
        else
        {
            runSpeed = baseRunSpeed;
        }

        currentDistance += runSpeed * Time.deltaTime;
        CheckStarSound(); 

        int calculatedLevel = Mathf.FloorToInt(currentDistance / distancePerLevel);
        if (calculatedLevel > currentLevel)
        {
            currentLevel = calculatedLevel;
            PlayLevelUpEffect();
        }

        UpdateUI();
    }

    void UpdateUI()
    {
        if (hpBar != null) { hpBar.maxValue = maxHP; hpBar.value = currentHP; }
        if (seedBar != null) { seedBar.maxValue = distancePerLevel; seedBar.value = currentDistance % distancePerLevel; }
        if (scoreText != null) scoreText.text = currentDistance.ToString("F0") + " m";
    }

    public void TriggerTrafficLightSequence(bool isRedLightMap, TrainTrackMapTrigger activeMap)
    {
        if (currentLightState == LightState.None && isGameRunning && !isGameOver)
        {
            StartCoroutine(TrafficLightRoutine(isRedLightMap, activeMap));
        }
    }

    private IEnumerator TrafficLightRoutine(bool isRedLightMap, TrainTrackMapTrigger activeMap)
    {
        hasProcessedDamage = false;
        if (trafficLightPanel != null) trafficLightPanel.SetActive(true);

        // 1. Chu kỳ đèn Vàng: Nháy 3 lần (Thời gian đệm đưa vạch dừng đến sát mặt)
        currentLightState = LightState.Yellow;
        if (spacePromptUI != null) spacePromptUI.SetActive(false);

        for (int i = 0; i < 3; i++)
        {
            SetLightVisuals(false, true, false); yield return new WaitForSeconds(0.4f);
            SetLightVisuals(false, false, false); yield return new WaitForSeconds(0.4f);
        }

        // 2. Chuyển sang Đỏ (Map 1) hoặc Xanh (Map 2) theo thiết lập cố định
        currentLightState = isRedLightMap ? LightState.Red : LightState.Green;
        if (spacePromptUI != null) spacePromptUI.SetActive(true);

        if (currentLightState == LightState.Red)
        {
            SetLightVisuals(true, false, false);
            
            // Gọi tàu hỏa chạy cắt ngang qua trong đúng 3 giây
            if (activeMap != null) activeMap.StartTrainMovement();
            
            yield return new WaitForSeconds(3f); 
        }
        else // Hàng đèn Xanh
        {
            SetLightVisuals(false, false, true);
            yield return new WaitForSeconds(3f); // 3 giây trôi qua khu vực đông đúc
        }

        // 3. Reset hệ thống đèn
        currentLightState = LightState.None;
        if (trafficLightPanel != null) trafficLightPanel.SetActive(false);
        if (spacePromptUI != null) spacePromptUI.SetActive(false);
        runSpeed = baseRunSpeed;
    }

    private void SetLightVisuals(bool red, bool yellow, bool green)
    {
        if (redLightImg != null) redLightImg.gameObject.SetActive(red);
        if (yellowLightImg != null) yellowLightImg.gameObject.SetActive(yellow);
        if (greenLightImg != null) greenLightImg.gameObject.SetActive(green);
    }

    // --- CÁC HÀM CŨ GIỮ NGUYÊN VẸN ---
    void CheckStarSound() { if (currentDistance >= nextSoundMilestone) { PlayStarSound(); nextSoundMilestone += soundInterval; } }
    void PlayStarSound() { if (sfxSource != null && starUnlockSound != null) sfxSource.PlayOneShot(starUnlockSound, 1f); PlayLevelUpEffect(); }
    void ShowMenuLogic() { isGameRunning = false; Time.timeScale = 0f; if (menuPanel != null) { menuPanel.SetActive(true); if (menuHighScoreText != null) menuHighScoreText.text = "BEST: " + PlayerPrefs.GetFloat("HighScore", 0f).ToString("F0") + "m"; } if (losePanel != null) losePanel.SetActive(false); }
    void StartGameLogic() { isGameRunning = true; Time.timeScale = 1f; if (menuPanel != null) menuPanel.SetActive(false); if (losePanel != null) losePanel.SetActive(false); }
    public void OnStartButton() => StartGameLogic();
    public void OnReplayButton() { autoStart = true; Time.timeScale = 1f; SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void OnHomeButton() { autoStart = false; Time.timeScale = 1f; SceneManager.LoadScene(SceneManager.GetActiveScene().name); }
    public void TakeDamage(int damage) { if (isGameOver || !isGameRunning) return; currentHP -= damage; UpdateUI(); if (currentHP <= 0) GameOver(); }
    void PlayLevelUpEffect() { if (scoreText == null) return; if (punchCoroutine != null) StopCoroutine(punchCoroutine); punchCoroutine = StartCoroutine(PunchScaleRoutine()); }
    IEnumerator PunchScaleRoutine() { Vector3 orig = Vector3.one; Vector3 target = Vector3.one * punchScale; float half = punchDuration / 2f; float elaps = 0f; while (elaps < half) { elaps += Time.deltaTime; scoreText.transform.localScale = Vector3.Lerp(orig, target, elaps / half); yield return null; } elaps = 0f; while (elaps < half) { elaps += Time.deltaTime; scoreText.transform.localScale = Vector3.Lerp(target, orig, elaps / half); yield return null; } scoreText.transform.localScale = orig; }
    void GameOver() { isGameOver = true; isGameRunning = false; Time.timeScale = 0f; SaveHighScore(); if (losePanel != null) { losePanel.SetActive(true); if (finalScoreText != null) finalScoreText.text = " " + currentDistance.ToString("F0") + "m"; CalculateAndShowStars(); } }
    void CalculateAndShowStars() { if (starImages == null || starImages.Length < 3) return; foreach (Image img in starImages) if (img != null) img.color = starLockedColor; if (currentDistance >= dist1Star && starImages[0] != null) starImages[0].color = starUnlockedColor; if (currentDistance >= dist2Star && starImages[1] != null) starImages[1].color = starUnlockedColor; if (currentDistance >= dist3Star && starImages[2] != null) starImages[2].color = starUnlockedColor; }
    void SaveHighScore() { if (currentDistance > PlayerPrefs.GetFloat("HighScore", 0f)) { PlayerPrefs.SetFloat("HighScore", currentDistance); PlayerPrefs.Save(); } }
}