using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Cài đặt Di chuyển")]
    private int currentLane = 1;
    public float laneDistance = 3.0f;
    public float moveSpeed = 10f;

    [Header("Cài đặt Cảm ứng (Mobile)")]
    public float minSwipeDistance = 50f;
    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

    [Header("Cài đặt Hiệu ứng Va chạm")]
    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 0.3f;
    public Color damageColor = Color.red;
    private Color[] originalColors;

    [Header("Cấu hình Collider")]
    public Collider playerCollider;

    [Header("Tham chiếu")]
    public RunnerStats runnerStats;

    [Tooltip("Kéo tất cả các bộ phận của xe vào đây")]
    public MeshRenderer[] carMeshes;
    public CameraShake cameraShake;

    // --- CÀI ĐẶT ÂM THANH ---
    [Header("Cài đặt Âm thanh")]
    public AudioSource sfxAudioSource; 
    public AudioClip damageSound;      

    private bool isBlinking = false;
    private bool isDamaged = false;

    void Start()
    {
        // Tự tìm AudioSource
        if (sfxAudioSource == null) sfxAudioSource = GetComponent<AudioSource>();

        // Lưu màu gốc
        if (carMeshes != null && carMeshes.Length > 0)
        {
            originalColors = new Color[carMeshes.Length];
            for (int i = 0; i < carMeshes.Length; i++)
            {
                if (carMeshes[i] != null) originalColors[i] = carMeshes[i].material.color;
            }
        }

        if (playerCollider == null) playerCollider = GetComponent<Collider>();
        if (runnerStats == null) Debug.LogError("LỖI: Chưa kéo RunnerStats vào PlayerController!");
    }

    void Update()
    {
        if (GameSettings.IsGamePaused) return;

        // --- XỬ LÝ INPUT ---
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) MoveLane(-1);
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) MoveLane(1);

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) startTouchPosition = touch.position;
            else if (touch.phase == TouchPhase.Ended)
            {
                endTouchPosition = touch.position;
                HandleSwipe();
            }
        }

        // --- DI CHUYỂN XE ---
        float targetX = (currentLane - 1) * laneDistance;
        Vector3 targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    void HandleSwipe()
    {
        float distanceX = endTouchPosition.x - startTouchPosition.x;
        float distanceY = endTouchPosition.y - startTouchPosition.y;

        if (Mathf.Abs(distanceX) > minSwipeDistance && Mathf.Abs(distanceX) > Mathf.Abs(distanceY))
        {
            if (distanceX > 0) MoveLane(1);
            else MoveLane(-1);
        }
    }

    void MoveLane(int direction)
    {
        int targetLane = currentLane + direction;
        if (targetLane >= 0 && targetLane <= 2) currentLane = targetLane;
        else HandleBoundaryPenalty();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            if (!isDamaged)
            {
                if (runnerStats != null) runnerStats.TakeDamage(1);
                
                PlayDamageSound(); // Gọi hàm phát âm thanh
                
                HitObstacleEffect();
            }
        }
    }

    // --- HÀM ĐÃ SỬA: TÁCH BIỆT ÂM THANH ---
    void PlayDamageSound()
    {
        if (sfxAudioSource != null && damageSound != null)
        {
            // Lấy volume SFX hiện tại từ Setting
            float currentSFXVolume = GameSettings.MasterSFXVolume;

            // QUAN TRỌNG: Dùng tham số thứ 2 của PlayOneShot để chỉnh âm lượng RIÊNG cho âm thanh này
            // Không được dùng sfxAudioSource.volume = ... vì nó sẽ ảnh hưởng toàn bộ AudioSource
            sfxAudioSource.PlayOneShot(damageSound, currentSFXVolume);
        }
    }

    public void HitObstacleEffect()
    {
        if (cameraShake != null) cameraShake.Shake(shakeDuration, shakeMagnitude);
        StartCoroutine(BecomeInvincible());
    }

    IEnumerator BecomeInvincible()
    {
        if (isDamaged) yield break;
        isDamaged = true;
        if (playerCollider != null) playerCollider.enabled = false;

        SetCarColor(damageColor);

        for (int i = 0; i < 5; i++)
        {
            SetCarVisibility(false); 
            yield return new WaitForSeconds(0.1f);
            SetCarVisibility(true);  
            yield return new WaitForSeconds(0.1f);
        }

        ResetCarColor();
        if (playerCollider != null) playerCollider.enabled = true;
        isDamaged = false;
    }

    void HandleBoundaryPenalty()
    {
        if (runnerStats != null && !isBlinking)
        {
            runnerStats.TakeDamage(1);
            PlayDamageSound(); // Gọi hàm phát âm thanh khi đâm biên
            StartCoroutine(BoundaryPenaltyEffect());
        }
    }

    IEnumerator BoundaryPenaltyEffect()
    {
        isBlinking = true;
        SetCarColor(damageColor);

        for (int i = 0; i < 3; i++)
        {
            SetCarVisibility(false);
            yield return new WaitForSeconds(0.1f);
            SetCarVisibility(true);
            yield return new WaitForSeconds(0.1f);
        }

        ResetCarColor();
        isBlinking = false;
    }

    // --- Các hàm hỗ trợ Mesh (Giữ nguyên) ---
    void SetCarVisibility(bool isVisible)
    {
        if (carMeshes == null) return;
        foreach (var mesh in carMeshes) if (mesh != null) mesh.enabled = isVisible;
    }

    void SetCarColor(Color color)
    {
        if (carMeshes == null) return;
        foreach (var mesh in carMeshes) if (mesh != null) mesh.material.color = color;
    }

    void ResetCarColor()
    {
        if (carMeshes == null || originalColors == null) return;
        for (int i = 0; i < carMeshes.Length; i++)
        {
            if (carMeshes[i] != null && i < originalColors.Length)
            {
                carMeshes[i].enabled = true;
                carMeshes[i].material.color = originalColors[i];
            }
        }
    }
}