using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShipMoveToPoint : MonoBehaviour
{
    public Transform pointB;
    public float speed = 5f;
    public string sceneToLoad;

    private bool isMoving = false;

    public CanvasGroup TGKM;
    public CanvasGroup Button1;
    public CanvasGroup Button2;
    public CanvasGroup Button3;

    public float fadeDuration = 1f;

    bool isFading = false;

    void Update()
    {
        if (isMoving)
        {
            MoveToPoint();
        }
    }

    void MoveToPoint()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            pointB.position,
            speed * Time.deltaTime
        );

        // Chỉ fade 1 lần
        if (!isFading)
        {
            isFading = true;
            StartCoroutine(FadeUI());
        }

        if (Vector3.Distance(transform.position, pointB.position) < 0.1f)
        {
            isMoving = false;
            FindAnyObjectByType<FadeManager>().FadeAndLoadScene(sceneToLoad);
        }
    }

    IEnumerator FadeUI()
    {
        float timer = 0;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            float alpha = Mathf.Lerp(1, 0, timer / fadeDuration);

            TGKM.alpha = alpha;
            Button1.alpha = alpha;
            Button2.alpha = alpha;
            Button3.alpha = alpha;

            yield return null;
        }

        TGKM.gameObject.SetActive(false);
        Button1.gameObject.SetActive(false);
        Button2.gameObject.SetActive(false);
        Button3.gameObject.SetActive(false);
    }

    public void StartMoving()
    {
        isMoving = true;
    }
}