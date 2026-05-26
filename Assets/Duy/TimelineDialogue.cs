using TMPro;
using UnityEngine;
using System.Collections;


public class TimelineDialogue : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text dialogueText;

    [Header("Dialogue")]
    [TextArea(2, 5)]
    public string[] dialogues;

    [Header("Typing")]
    public float typingSpeed = 0.04f;

    int currentIndex = -1;
    bool isTyping = false;
    Coroutine typingCoroutine;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }
    }

    // Được Timeline gọi bằng Signal
    public void PlayDialogue(int index)
    {
        currentIndex = index;
        StartTyping(dialogues[index]);
    }

    void StartTyping(string text)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(text));
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    void HandleClick()
    {
        // Click khi đang typing → chạy hết chữ
        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.text = dialogues[currentIndex];
            isTyping = false;
        }
        // Click khi đã hiện đủ chữ → sang đoạn tiếp theo
        else
        {
            GoNextDialogue();
        }
    }

    void GoNextDialogue()
    {
        // Timeline sẽ lo chuyển đoạn bằng Signal
        // Ở đây chỉ để trống hoặc thêm hiệu ứng click
    }
}
