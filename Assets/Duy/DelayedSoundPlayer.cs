using UnityEngine;
using System.Collections;

public class DelayedSoundPlayer : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip audioClip;

    [Header("Settings")]
    [Range(0f, 1f)]
    public float volume = 1f;

    public float delayTime = 1f; // thời gian chờ trước khi phát (giây)

    void Start()
    {
        // Tự động phát khi bắt đầu game (nếu muốn)
        PlaySound();
    }

    public void PlaySound()
    {
        StartCoroutine(PlaySoundWithDelay());
    }

    IEnumerator PlaySoundWithDelay()
    {
        yield return new WaitForSeconds(delayTime);

        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();
    }
}
