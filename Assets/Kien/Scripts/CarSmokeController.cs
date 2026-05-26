using UnityEngine;
using UnityEngine.UI;

public class CarSmokeController : MonoBehaviour
{
    [Header("Smoke")]
    public ParticleSystem blackSmoke;
    public ParticleSystem whiteSmoke;

    [Header("Progress UI")]
    public Canvas progressCanvas;
    public Slider progressSlider;

    [Header("Clean Settings")]
    public float cleanPerClick = 1f;   // KHÔNG đổi theo độ khó
    public int baseRequiredClicks = 3; // số click ban đầu

    private float currentProgress = 0;
    private float requiredProgress;
    public bool isCleaned { get; private set; } = false;

    void Start()
    {
        InitDirtyState();
    }

    public void InitDirtyState()
    {
        isCleaned = false;
        currentProgress = 0;

        int difficultyLevel = CarGameManager.Instance.CurrentDifficultyLevel;
        requiredProgress = baseRequiredClicks + difficultyLevel * 2;

        progressSlider.maxValue = requiredProgress;
        progressSlider.value = 0;

        progressCanvas.gameObject.SetActive(false);

        blackSmoke.Play();
        whiteSmoke.Stop();
    }

    void OnMouseDown()
    {
        if (isCleaned) return;

        progressCanvas.gameObject.SetActive(true);
        Clean();
    }

    void Clean()
    {
        currentProgress += cleanPerClick;
        progressSlider.value = currentProgress;

        if (currentProgress >= requiredProgress)
        {
            CompleteClean();
        }
    }

    void CompleteClean()
    {
        isCleaned = true;

        blackSmoke.Stop();
        whiteSmoke.Play();

        progressCanvas.gameObject.SetActive(false);

        CarGameManager.Instance.OnCarCleaned();
    }

    public bool IsCleaned()
    {
        return isCleaned;
    }
}
