using UnityEngine;
using System.Collections;

public class UIPanelFade : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public float fadeTime = 0.3f;

    Coroutine currentRoutine;

    void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Show()
    {
        gameObject.SetActive(true);

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(Fade(0, 1, true));
    }

    public void Hide()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(Fade(1, 0, false));
    }

    IEnumerator Fade(float start, float end, bool enable)
    {
        float time = 0;

        canvasGroup.alpha = start;
        canvasGroup.interactable = enable;
        canvasGroup.blocksRaycasts = enable;

        while (time < fadeTime)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(start, end, time / fadeTime);
            yield return null;
        }

        canvasGroup.alpha = end;

        if (!enable)
            gameObject.SetActive(false);
    }
    
}