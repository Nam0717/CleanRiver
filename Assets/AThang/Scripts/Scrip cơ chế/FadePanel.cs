using UnityEngine;
using System.Collections;

public class FadePanel : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float fadeDuration = 1.5f; // thời gian fade

    public void FadeIn()
    {
        StartCoroutine(Fade(0f, 1f));
    }

    IEnumerator Fade(float start, float end)
    {
        float time = 0f;

        canvasGroup.alpha = start;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, end, time / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = end;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }
}
