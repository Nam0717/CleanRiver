using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource audioSource;

    [Header("Pipe Sounds")]
    public AudioClip dragClip;
    public AudioClip snapClip;
    public AudioClip rotateClip;

    void Awake()
    {
        Instance = this;
    }

    public void PlayDrag()
    {
        audioSource.PlayOneShot(dragClip);
    }

    public void PlaySnap()
    {
        audioSource.PlayOneShot(snapClip);
    }

    public void PlayRotate()
    {
        audioSource.PlayOneShot(rotateClip);
    }
}