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
    public Color starUnlockedColor = new Color(1f, 1f, 1f, 1f); 
    public Color starLockedColor = new Color(0.3f, 0.3f, 0.3f, 1f); 

    [Header("Hiệu ứng Visual")]
    public float punchScale = 1.5f;
    public float punchDuration = 0.2f;
    private Coroutine punchCoroutine;

    // ================= HỆ THỐNG ĐÈN GIAO THÔNG ĐƯỜNG RAY =================
    [Header("UI Đèn Giao Thông Chắn Tàu")]
    public GameObject trafficLightPanel; 
    public Image yellowLightImg;
    public Image redLightImg;
    public Image greenLightImg;
    public GameObject spacePromptUI;     

    [Header("Cấu hình thời gian phản xạ (Chỉ dùng cho Đèn Đỏ)")]
    [Tooltip("Thời gian an toàn (giây) chừa cho người chơi kịp bấm phanh khi ĐÈN ĐỎ xuất hiện trước khi tính phạt.")]
    public float gracePeriod = 2.0f; 

    [Header("UI Ký Hiệu Người Đi Bộ (Tách Biệt Đỏ/Xanh)")]
    public Image pedestrianStopImg;       
    public Image pedestrianWalkImg;       

    private float baseRunSpeed;           
    private enum LightState { None, Warning, Red, Green }
    private LightState currentLightState = LightState.None;
    private bool hasProcessedDamage = false; 
    private bool hasStoppedCorrectly = false; 

    private float lightStateTimer = 0f;    
    [HideInInspector] public int trafficMapCount = 0; 

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
        trafficMapCount = 0; 
        
        baseRunSpeed = runSpeed; 
        nextSoundMilestone = soundInterval; 
        
        if (sfxSource == null) sfxSource = GetComponent<AudioSource>(); 

        if (trafficLightPanel != null) trafficLightPanel.SetActive(false);
        if (spacePromptUI != null) spacePromptUI.SetActive(false);
        
        SetPedestrianVisuals(false, false); 

        UpdateUI(); 

        if (autoStart) StartGameLogic();
        else ShowMenuLogic();
    }

    void Update()
    {
        if (!isGameRunning || isGameOver) return;

        // --- CƠ CHẾ ĐÈ PHANH XE BẰNG SPACE + LOGIC PHẠT MỚI ---
        if (currentLightState != LightState.None)
        {
            if (currentLightState == LightState.Red || currentLightState == LightState.Green)
            {
                lightStateTimer += Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.Space)) // Người chơi đang ĐÈ PHANH
            {
                runSpeed = 0f; 

                // ĐÈN ĐỎ: Ghi nhận đã dừng đúng quy định, kích hoạt cờ an toàn
                if (currentLightState == LightState.Red)
                {
                    hasStoppedCorrectly = true;
                }

                // 🔥 ĐÈN XANH SỬA TẠI ĐÂY: Nhấn phanh là PHẠT LUÔN lập tức, không chờ giây ân hạn
                if (currentLightState == LightState.Green && !hasProcessedDamage)
                {
                    TakeDamage(1);
                    hasProcessedDamage = true;
                    Debug.Log("Giao thông: Đèn xanh tự nhiên phanh gấp! Bị xe phía sau tông trúng ngay lập tức.");
                }
            }
            else // Người chơi KHÔNG PHANH (Thả tự do hoặc đang chạy)
            {
                runSpeed = baseRunSpeed; 

                // ĐÈN ĐỎ: Chỉ phạt khi quá thời gian ân hạn (gracePeriod) VÀ người chơi chưa từng dừng đúng trước đó
                if (currentLightState == LightState.Red && lightStateTimer > gracePeriod && !hasProcessedDamage && !hasStoppedCorrectly)
                {
                    TakeDamage(1);
                    hasProcessedDamage = true;
                    Debug.Log($"Giao thông: Đèn đỏ quá {gracePeriod} giây không chịu dừng! Phạt trừ máu.");
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
        hasStoppedCorrectly = false; 
        lightStateTimer = 0f; 
        if (trafficLightPanel != null) trafficLightPanel.SetActive(true);

        // 1. GIAI ĐOẠN NHẤP NHÁY CẢNH BÁO CHUẨN ĐỜI THỰC (3 LẦN)
        currentLightState = LightState.Warning; 
        if (spacePromptUI != null) spacePromptUI.SetActive(false);

        for (int i = 0; i < 3; i++)
        {
            if (isRedLightMap)
            {
                SetLightVisuals(false, true, false); 
                SetPedestrianVisuals(true, false); 
            }
            else
            {
                SetLightVisuals(true, false, false); 
                SetPedestrianVisuals(true, false); 
            }
            yield return new WaitForSeconds(0.4f);
            
            SetLightVisuals(false, false, false); 
            SetPedestrianVisuals(false, false); 
            yield return new WaitForSeconds(0.4f);
        }

        // 2. GIAI ĐOẠN ĐÈN SÁNG ĐỨNG YÊN CHÍNH THỨC (Duy trì đúng 3 giây, KHÔNG NHÁY)
        currentLightState = isRedLightMap ? LightState.Red : LightState.Green;
        lightStateTimer = 0f; 
        if (spacePromptUI != null) spacePromptUI.SetActive(true);

        if (currentLightState == LightState.Red)
        {
            SetLightVisuals(true, false, false); 
            SetPedestrianVisuals(true, false); 
            
            if (activeMap != null) activeMap.StartTrainMovement(); 
            yield return new WaitForSeconds(3f); 
        }
        else // LightState.Green
        {
            SetLightVisuals(false, false, true); 
            SetPedestrianVisuals(false, true); 
            yield return new WaitForSeconds(3f); 
        }

        // 3. KẾT THÚC PHÂN ĐOẠN -> TẮT TOÀN BỘ CÁC ĐÈN UI
        currentLightState = LightState.None;
        hasStoppedCorrectly = false; 
        lightStateTimer = 0f;
        if (trafficLightPanel != null) trafficLightPanel.SetActive(false);
        if (spacePromptUI != null) spacePromptUI.SetActive(false);
        SetPedestrianVisuals(false, false); 
        runSpeed = baseRunSpeed;
    }

    private void SetLightVisuals(bool red, bool yellow, bool green)
    {
        if (redLightImg != null) redLightImg.gameObject.SetActive(red);
        if (yellowLightImg != null) yellowLightImg.gameObject.SetActive(yellow);
        if (greenLightImg != null) greenLightImg.gameObject.SetActive(green);
    }

    private void SetPedestrianVisuals(bool stopActive, bool walkActive)
    {
        if (pedestrianStopImg != null) pedestrianStopImg.gameObject.SetActive(stopActive);
        if (pedestrianWalkImg != null) pedestrianWalkImg.gameObject.SetActive(walkActive);
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