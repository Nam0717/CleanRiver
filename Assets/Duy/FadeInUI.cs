using UnityEngine;
using System.Collections;

public class FadeInUI : MonoBehaviour
{
    [Header("Target")]
    public GameObject targetObject;     // Image (cha)
    public CanvasGroup canvasGroup;      // CanvasGroup của Image

    [Header("Fade Settings")]
    public float fadeDuration = 1f;

    Coroutine fadeCoroutine;

    // 👉 GỌI HÀM NÀY KHI CẦN HIỆN UI
    public void Show()
    {
        // Active object
        targetObject.SetActive(true);

        // Reset alpha
        canvasGroup.alpha = 0f;

        // Bật tương tác
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        // Start fade
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }
}
