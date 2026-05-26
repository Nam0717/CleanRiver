using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CutsceneController : MonoBehaviour
{
    public static CutsceneController Instance;

    [Header("Camera")]
    public Camera mainCamera;
    public Camera cutsceneCamera;

    [Header("Canvas")]
    public GameObject mainCanvas;
    public GameObject cutsceneCanvas;

    [Header("Win UI")]
    public GameObject winPanel;

    [Header("Smoke")]
    public ParticleSystem blackSmoke;
    public ParticleSystem whiteSmoke;

    [Header("Timing")]
    public float smokeDelay = 3f;
    public float smokeTransitionDuration = 2f;
    public float winPanelDelay = 10f;

    void Awake()
    {
        Instance = this;
    }

    public void PlayWinCutscene()
    {
        StartCoroutine(WinSequence());
    }

    IEnumerator WinSequence()
    {
        // 🎥 Switch camera
        mainCamera.gameObject.SetActive(false);
        mainCanvas.SetActive(false);

        cutsceneCamera.gameObject.SetActive(true);
        cutsceneCanvas.SetActive(true);

        winPanel.SetActive(false);

        // 💨 Đợi 3 giây trước khi chuyển khói
        yield return new WaitForSeconds(smokeDelay);

        // 🤍 Bắt đầu bật khói trắng
        whiteSmoke.Play();

        float transitionTimer = 0f;

        var blackMain = blackSmoke.main;
        var whiteMain = whiteSmoke.main;

        Color blackStart = blackMain.startColor.color;
        Color whiteStart = whiteMain.startColor.color;

        while (transitionTimer < smokeTransitionDuration)
        {
            transitionTimer += Time.deltaTime;
            float t = transitionTimer / smokeTransitionDuration;

            Color newBlack = blackStart;
            newBlack.a = Mathf.Lerp(1f, 0f, t);
            blackMain.startColor = newBlack;

            Color newWhite = whiteStart;
            newWhite.a = Mathf.Lerp(0f, 1f, t);
            whiteMain.startColor = newWhite;

            yield return null;
        }

        blackSmoke.Stop();

        // ⏳ Đợi đến đủ 10 giây tổng thời gian
        float remainingTime = winPanelDelay - smokeDelay;
        yield return new WaitForSeconds(remainingTime);

        // 🏆 Hiện WinPanel
        winPanel.SetActive(true);
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