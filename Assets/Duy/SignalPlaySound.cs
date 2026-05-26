using UnityEngine;

public class SignalPlaySound : MonoBehaviour
{
    public AudioSource audioSource;

    // 👉 GỌI BẰNG SIGNAL
    public void PlaySound()
    {
        if (audioSource == null)
            return;

        audioSource.Play();
    }
}
