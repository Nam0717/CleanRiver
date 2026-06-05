using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject settingsPanel;
    public GameObject pauseButton;
    public GameObject helpPanel;

    [Header("Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Cấu hình Âm Thanh (Phân biệt rõ ràng)")]
    // 1. Nguồn phát NHẠC NỀN (Music)
    public AudioSource musicSource; 
    public AudioClip musicFile;

    // 2. Nguồn phát HIỆU ỨNG (SFX - Ví dụ: Tiếng nút bấm UI)
    // Cái này dùng để test hoặc phát tiếng click trong Menu
    public AudioSource sfxSource; 

    // --- BIẾN TOÀN CỤC (STATIC) ĐỂ CÁC SCRIPT KHÁC GỌI ---
    public static bool IsGamePaused = false;
    
    // Biến lưu trữ volume hiện tại để các script khác (Player, RunnerStats) đọc được
    public static float MasterMusicVolume = 1f; 
    public static float MasterSFXVolume = 1f;

    void Start()
    {
        IsGamePaused = false; 
        Time.timeScale = 1;

        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (helpPanel != null) helpPanel.SetActive(false);

        // --- 1. SETUP NHẠC NỀN ---
        if (musicSource != null && musicFile != null)
        {
            musicSource.clip = musicFile;
            musicSource.loop = true;
            musicSource.playOnAwake = false; // Tắt cái này để code tự quản lý
            
            if (!musicSource.isPlaying) musicSource.Play();
        }

        // --- 2. SETUP SFX SOURCE (Để tránh lỗi null) ---
        if (sfxSource == null)
        {
            // Nếu chưa kéo vào, tự lấy AudioSource thứ 2 hoặc thêm mới
            // (Thường ta nên kéo tay vào Inspector cho chuẩn)
            sfxSource = gameObject.AddComponent<AudioSource>();
        }

        // --- 3. LOAD DỮ LIỆU ĐÃ LƯU ---
        float savedMusicVol = PlayerPrefs.GetFloat("MusicVol", 1f);
        float savedSFXVol = PlayerPrefs.GetFloat("SFXVol", 1f);

        // Cập nhật Slider
        if (musicSlider != null) musicSlider.value = savedMusicVol;
        if (sfxSlider != null) sfxSlider.value = savedSFXVol;

        // Áp dụng âm lượng ngay lập tức
        ApplyMusicVolume(savedMusicVol);
        ApplySFXVolume(savedSFXVol);

        // Lắng nghe sự kiện kéo thanh trượt
        if (musicSlider != null) musicSlider.onValueChanged.AddListener(ApplyMusicVolume);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(ApplySFXVolume);
    }

    // --- CÁC HÀM XỬ LÝ UI & PAUSE (GIỮ NGUYÊN) ---
    public void ToggleSettings()
    {
        bool currentState = IsGamePaused;
        SetPause(!currentState);
    }

    public void CloseSettingsButton()
    {
        SetPause(false);
    }

    void SetPause(bool shouldPause)
    {
        IsGamePaused = shouldPause;

        if (shouldPause)
        {
            if (settingsPanel != null) settingsPanel.SetActive(true);
            if (pauseButton != null) pauseButton.SetActive(false);
            Time.timeScale = 0;
        }
        else
        {
            CloseAllPanels();
            if (pauseButton != null) pauseButton.SetActive(true);
            Time.timeScale = 1;
        }
    }

    void CloseAllPanels()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (helpPanel != null) helpPanel.SetActive(false);
    }

    public void OpenHelp()
    {
        if (helpPanel != null) helpPanel.SetActive(true);
    }

    public void CloseHelp()
    {
        if (helpPanel != null) helpPanel.SetActive(false);
    }

    // --- PHẦN QUAN TRỌNG: CẬP NHẬT ÂM THANH ---

    public void ApplyMusicVolume(float volume)
    {
        // 1. Lưu vào biến tĩnh
        MasterMusicVolume = volume;
        
        // 2. Cập nhật ngay lập tức cho cái loa nhạc nền
        if (musicSource != null)
        {
            musicSource.volume = MasterMusicVolume;
        }

        // 3. Lưu xuống máy
        PlayerPrefs.SetFloat("MusicVol", volume);
    }

    public void ApplySFXVolume(float volume)
    {
        // 1. Lưu vào biến tĩnh (Để PlayerController và RunnerStats đọc)
        MasterSFXVolume = volume;

        // 2. Cập nhật cho cái loa SFX (nếu dùng để test tiếng click UI)
        if (sfxSource != null)
        {
            sfxSource.volume = MasterSFXVolume;
        }

        // 3. Lưu xuống máy
        PlayerPrefs.SetFloat("SFXVol", volume);
    }
    
    // Hàm phụ: Dùng để phát tiếng click nút (Gán vào Button OnClick)
    public void PlayClickSound(AudioClip clickClip)
    {
        if (sfxSource != null && clickClip != null)
        {
            sfxSource.PlayOneShot(clickClip, MasterSFXVolume);
        }
    }
}