using UnityEngine;
using System.Collections.Generic;
using TMPro; // Quản lý chữ UI TextMeshPro

public class CandySortingManager : MonoBehaviour
{
    [Header("Raycast & Selection Settings")]
    [SerializeField] private float liftHeight = 0.3f; // Độ cao nhấc lên khi chọn kẹo
    [SerializeField] private LayerMask candyLayer;       // Layer dành riêng cho kẹo/bí ngô

    [Header("Basket Targets")]
    [SerializeField] private Transform basket1Red;   // Giỏ 1 (Bên trái)
    [SerializeField] private Transform basket2Green; // Giỏ 2 (Bên phải)

    [Header("Throw Animation Settings")]
    [SerializeField] private float throwDuration = 0.5f; // Thời gian bay tới giỏ (giây)
    [SerializeField] private float arcHeight = 1.8f;     // Độ cao đỉnh vòng cung khi bay

    [Header("Basket Shake Settings (Khi làm sai)")]
    [Tooltip("Thời gian rung lắc của cái giỏ (giây).")]
    [SerializeField] private float basketShakeDuration = 0.3f;
    [Tooltip("Độ mạnh/Biên độ rung lắc của giỏ.")]
    [SerializeField] private float basketShakeMagnitude = 0.15f;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;   
    [SerializeField] private AudioClip correctSound;    
    [SerializeField] private AudioClip incorrectSound;  

    [Header("Game Rules & UI Panels")]
    [SerializeField] private int totalCandiesToWin = 6;  
    [SerializeField] private int maxAllowedMistakes = 3; 
    [SerializeField] private GameObject winPanel;        
    [SerializeField] private GameObject losePanel;       

    [Header("UI Counter Texts")]
    [SerializeField] private TextMeshProUGUI candyCounterText;  
    [SerializeField] private TextMeshProUGUI mistakeCounterText; 

    [Header("Visual Effects")]
    [Tooltip("Nếu để trống ô này, hệ thống tự sinh hiệu ứng hoa giấy bằng code!")]
    [SerializeField] private ParticleSystem confettiPrefab; 
    [Tooltip("Góc xoay hướng bắn (X, Y, Z) của hoa giấy tự sinh.")]
    [SerializeField] private Vector3 fallbackConfettiRotation = new Vector3(-90f, 0f, 0f);

    private int currentCorrectCount = 0;
    private int currentMistakeCount = 0;
    private bool isGameOver = false;
    private CandyItem selectedCandy = null; 

    void Start()
    {
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
        UpdateScoreUI();
    }

    void Update()
    {
        if (isGameOver) return; 

        HandleMouseSelection();
        HandleKeyboardSorting();
    }

    void HandleMouseSelection()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, candyLayer))
            {
                CandyItem clickedCandy = hit.collider.GetComponent<CandyItem>();
                if (clickedCandy != null)
                {
                    if (selectedCandy != null && selectedCandy != clickedCandy)
                    {
                        selectedCandy.Deselect(); 
                    }
                    selectedCandy = clickedCandy;
                    selectedCandy.Select(liftHeight);
                }
            }
        }
    }

    void HandleKeyboardSorting()
    {
        if (selectedCandy == null) return; 

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SortCandyToBasket(basket1Red, 1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SortCandyToBasket(basket2Green, 2);
        }
    }

    void SortCandyToBasket(Transform basketTarget, int basketNumber)
    {
        bool isCorrect = (basketNumber == 1 && selectedCandy.candyColor == CandyColor.Red) ||
                         (basketNumber == 2 && selectedCandy.candyColor == CandyColor.Green);

        if (isCorrect)
        {
            Debug.Log("<Color=Green>Chính xác!</Color>");
            PlaySFX(correctSound);

            currentCorrectCount++;
            UpdateScoreUI();

            // 🔥 ĐÃ SỬA: Không gọi SpawnConfetti ở đây nữa để tránh bị phun hoa giấy sớm
            StartCoroutine(ThrowArcRoutine(selectedCandy, basketTarget));

            if (currentCorrectCount >= totalCandiesToWin)
            {
                TriggerWin();
            }
        }
        else
        {
            Debug.Log("<Color=Red>Sai rồi!</Color>");
            PlaySFX(incorrectSound);

            currentMistakeCount++;
            UpdateScoreUI();

            StartCoroutine(ThrowBounceRoutine(selectedCandy, basketTarget));

            if (currentMistakeCount >= maxAllowedMistakes)
            {
                TriggerLose();
            }
        }

        selectedCandy = null; 
    }

    void UpdateScoreUI()
    {
        if (candyCounterText != null) candyCounterText.text = $"Số Lần Đúng: {currentCorrectCount} / {totalCandiesToWin}";
        if (mistakeCounterText != null) mistakeCounterText.text = $"Số lần sai: {currentMistakeCount} / {maxAllowedMistakes}";
    }

    // Coroutine làm rung lắc giỏ
    System.Collections.IEnumerator ShakeBasketRoutine(Transform basket)
    {
        if (basket == null) yield break;

        Vector3 originalPos = basket.position; 
        float elapsed = 0f;

        while (elapsed < basketShakeDuration)
        {
            elapsed += Time.deltaTime;
            Vector3 randomOffset = Random.insideUnitSphere * basketShakeMagnitude;
            randomOffset.y = 0f; 

            basket.position = originalPos + randomOffset;
            yield return null; 
        }

        basket.position = originalPos; 
    }

    void SpawnConfetti(Vector3 spawnPosition)
    {
        if (confettiPrefab != null)
        {
            ParticleSystem fx = Instantiate(confettiPrefab, spawnPosition + Vector3.up * 0.5f, Quaternion.Euler(fallbackConfettiRotation));
            fx.Play();
            Destroy(fx.gameObject, 2.0f);
        }
        else
        {
            GameObject confettiGO = new GameObject("ProceduralConfetti_Fallback");
            confettiGO.transform.position = spawnPosition + Vector3.up * 0.5f;
            confettiGO.transform.rotation = Quaternion.Euler(fallbackConfettiRotation);

            ParticleSystem ps = confettiGO.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.loop = false;
            main.startLifetime = 1.5f; 
            main.startSpeed = new ParticleSystem.MinMaxCurve(4f, 8f); main.startSize = new ParticleSystem.MinMaxCurve(0.12f, 0.25f); 
            main.gravityModifier = 1.3f; 

            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { 
                    new GradientColorKey(Color.red, 0.0f), 
                    new GradientColorKey(Color.yellow, 0.3f),
                    new GradientColorKey(new Color(0f, 0.6f, 1f), 0.6f), 
                    new GradientColorKey(Color.green, 1.0f) 
                },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
            );
            
            ParticleSystem.MinMaxGradient minMaxColor = new ParticleSystem.MinMaxGradient(gradient);
            minMaxColor.mode = ParticleSystemGradientMode.RandomColor; 
            main.startColor = minMaxColor; 

            var emission = ps.emission;
            emission.rateOverTime = 0; 
            var burst = new ParticleSystem.Burst(0f, 40); 
            emission.SetBursts(new ParticleSystem.Burst[] { burst });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 18f; 
            shape.radius = 0.1f;

            ParticleSystemRenderer psRenderer = confettiGO.GetComponent<ParticleSystemRenderer>();
            if (psRenderer != null)
            {
                Shader defaultShader = Shader.Find("Sprites/Default");
                if (defaultShader != null)
                {
                    psRenderer.material = new Material(defaultShader);
                }
            }

            ps.Play();
            Destroy(confettiGO, 2.5f); 
        }
    }

    void TriggerWin() { isGameOver = true; if (winPanel != null) winPanel.SetActive(true); }
    void TriggerLose() { isGameOver = true; if (losePanel != null) losePanel.SetActive(true); }
    void PlaySFX(AudioClip clip) { if (audioSource != null && clip != null) audioSource.PlayOneShot(clip); }

    // --- 🔥 COROUTINE NÉM ĐÚNG: ĐÃ ĐƯỢC ĐỒNG BỘ NHỊP TUNG HOA GIẤY ---
    System.Collections.IEnumerator ThrowArcRoutine(CandyItem candy, Transform target)
    {
        if (candy.TryGetComponent<Collider>(out Collider col)) col.enabled = false;
        Vector3 startPos = candy.transform.position;
        Vector3 targetPos = target.position;
        float elapsed = 0f;

        // Quả bí ngô bay hình vòng cung hướng về phía giỏ đúng
        while (elapsed < throwDuration)
        {
            if (candy == null) yield break;
            elapsed += Time.deltaTime;
            float t = elapsed / throwDuration;
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t);
            currentPos.y += Mathf.Sin(t * Mathf.PI) * arcHeight;
            candy.transform.position = currentPos;
            yield return null;
        }

        // 🔥 CHÍNH XÁC TẠI ĐÂY: Vừa ra khỏi vòng lặp (bí ngô lọt lòng giỏ), tung hoa giấy ngay lập tức!
        SpawnConfetti(targetPos);

        // Biến mất sau khi hoa giấy đã nổ
        if (candy != null) Destroy(candy.gameObject);
    }

    // --- Coroutine ném SAI: Giỏ rung khi chạm ---
    System.Collections.IEnumerator ThrowBounceRoutine(CandyItem candy, Transform target)
    {
        Collider col = candy.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Vector3 startPos = candy.transform.position; 
        Vector3 targetPos = target.position;         
        Vector3 tablePos = candy.originalPosition;   
        float elapsed = 0f;

        while (elapsed < throwDuration)
        {
            if (candy == null) yield break;
            elapsed += Time.deltaTime;
            float t = elapsed / throwDuration;
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t);
            currentPos.y += Mathf.Sin(t * Mathf.PI) * arcHeight;
            candy.transform.position = currentPos;
            yield return null;
        }

        // Bí ngô va trúng thành giỏ sai -> Kích hoạt giỏ rung bần bật
        StartCoroutine(ShakeBasketRoutine(target));

        elapsed = 0f;
        float bounceDuration = throwDuration * 0.8f; 

        while (elapsed < bounceDuration)
        {
            if (candy == null) yield break;
            elapsed += Time.deltaTime;
            float t = elapsed / bounceDuration;
            Vector3 currentPos = Vector3.Lerp(targetPos, tablePos, t);
            currentPos.y += Mathf.Sin(t * Mathf.PI) * (arcHeight * 0.6f); 
            candy.transform.position = currentPos;
            yield return null;
        }

        if (candy != null)
        {
            candy.Deselect(); 
            if (col != null) col.enabled = true; 
        }
    }
}