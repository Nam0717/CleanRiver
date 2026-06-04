using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeManager : MonoBehaviour
{
    [Header("UI")]
    public Image fadeImage;

    [Header("Settings")]
    public float fadeDuration = 1f;
    public float FadeinDuration = 1.5f;

    [Header("Colors")]
    public Color fadeInColor = Color.cyan;     // Màu khi vào scene
    public Color fadeOutColor = Color.yellow;  // Màu khi chuyển scene

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    public void FadeAndLoadScene(string sceneName)
    {
        StartCoroutine(FadeOut(sceneName));
    }

    IEnumerator FadeIn()
    {
        float t = FadeinDuration;

        while (t > 0)
        {
            t -= Time.deltaTime;
            float alpha = t / FadeinDuration;

            fadeImage.color = new Color(
                fadeInColor.r,
                fadeInColor.g,
                fadeInColor.b,
                alpha
            );

            yield return null;
        }

        fadeImage.color = new Color(
            fadeInColor.r,
            fadeInColor.g,
            fadeInColor.b,
            0
        );
    }

    IEnumerator FadeOut(string sceneName)
    {
        float t = 0;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = t / fadeDuration;

            fadeImage.color = new Color(
                fadeOutColor.r,
                fadeOutColor.g,
                fadeOutColor.b,
                alpha
            );

            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }
}