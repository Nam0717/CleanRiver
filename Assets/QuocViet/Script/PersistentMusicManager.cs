using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PersistentMusicManager : MonoBehaviour
{
    public static PersistentMusicManager Instance;

    [Header("Fade Settings")]
    public float fadeDuration = 2f;
    [Range(0f, 1f)]
    public float targetVolume = 1f;

    private AudioSource audioSource;
    private Coroutine fadeCoroutine;

    private void Awake()
    {
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

        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Nếu đang fade thì dừng lại
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        // Reset nhạc về đầu
        audioSource.Stop();
        audioSource.time = 0f;

        fadeCoroutine = StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        audioSource.volume = 0f;
        audioSource.Play();

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime; // Không bị ảnh hưởng bởi pause
            audioSource.volume = Mathf.Lerp(0f, targetVolume, timer / fadeDuration);
            yield return null;
        }

        audioSource.volume = targetVolume;
    }
}