using UnityEngine;
using System.Collections;

public class TreeDestroyHandler : MonoBehaviour
{
    [Header("Destroy Settings")]
    public GameObject axeObject;
    public int requiredHits = 15;

    public int currentHits;
    private bool isActive;

    [Header("Hit Feedback")]
    public AudioSource audioSource;
    public AudioClip chopSound;

    [Header("Shake Settings")]
    public float shakeDuration = 0.1f;
    public float shakeStrength = 0.05f;

    private Vector3 originalPosition;
    private Coroutine shakeCoroutine;

    void Start()
    {
        if (axeObject != null)
            axeObject.SetActive(false);

        originalPosition = transform.localPosition;
    }

    public void StartDestroyCondition()
    {
        isActive = true;
        currentHits = 0;

        if (axeObject != null)
        {
            axeObject.SetActive(false); // 👈 TẮT TRƯỚC
            axeObject.SetActive(true);  // 👈 BẬT LẠI → CÓ PHẢN HỒI
        }
    }

    public void CancelDestroyCondition()
    {
        isActive = false;
        currentHits = 0;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;

        if (other.CompareTag("Axe"))
        {
            currentHits++;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.PlayChopSoundAt(transform.position);
            }

            ShakeTree();

            if (currentHits >= requiredHits)
            {
                DestroyTree();
            }
        }
    }

    void PlayChopSound()
    {
        if (audioSource != null && chopSound != null)
        {
            audioSource.PlayOneShot(chopSound);
        }
    }

    void ShakeTree()
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(ShakeCoroutine());
    }

    IEnumerator ShakeCoroutine()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            Vector3 randomOffset = Random.insideUnitSphere * shakeStrength;
            randomOffset.y = 0f; // rung ngang cho tự nhiên

            transform.localPosition = originalPosition + randomOffset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
    }

    void DestroyTree()
    {
        Destroy(gameObject);
    }
}
