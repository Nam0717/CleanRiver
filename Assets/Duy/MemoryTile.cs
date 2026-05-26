using System.Collections;
using UnityEngine;

public class MemoryTile : MonoBehaviour
{
    public int tileID;
    public AudioSource audioSource;

    [Header("Highlight Color")]
    public Color highlightColor;

    private Renderer rend;
    private Color originalColor;

    void Start()
    {
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
    }

    void OnMouseDown()
    {
        if (!MemoryGameManager.Instance.playerTurn)
            return;

        MemoryGameManager.Instance.PlayerChoose(tileID);
        PlayEffect();
    }

    public void PlayEffect()
    {
        StartCoroutine(PlayVisualAndSound());
    }

    IEnumerator PlayVisualAndSound()
    {
        rend.material.color = highlightColor;
        audioSource.Play();

        yield return new WaitForSeconds(0.4f);

        rend.material.color = originalColor;
    }
}
