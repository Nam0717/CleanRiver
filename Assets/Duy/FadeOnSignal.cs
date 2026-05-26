using UnityEngine;
using System.Collections;

public class FadeOnSignal : MonoBehaviour
{
    [Header("Fade Target")]
    public CanvasGroup canvasGroup;

    [Header("Fade Settings")]
    public float fadeDuration = 1f;

    bool isFading = false;

    // 👉 GỌI BẰNG SIGNAL 4
    public void FadeOut()
    {
        if (isFading) return;
        StartCoroutine(FadeCoroutine());
    }

    IEnumerator FadeCoroutine()
    {
        isFading = true;

        float startAlpha = canvasGroup.alpha;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = 0f;
        isFading = false;
    }
}
