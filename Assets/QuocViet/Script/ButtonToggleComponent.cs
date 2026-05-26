using UnityEngine;
using System.Collections;

public class ButtonToggleComponent : MonoBehaviour
{
    [Header("Target Component")]
    public Behaviour targetComponent;

    [Header("Animator (Optional)")]
    public Animator animator;
    public string triggerIn = "in";
    public string triggerOut = "out";

    [Header("Timing")]
    public float animationDelay = 0.3f; // thời gian chờ để animation chạy trước khi tắt/bật

    private bool isBusy;

    // ================= BUTTON EVENTS =================

    public void Toggle()
    {
        if (targetComponent == null) return;
        if (isBusy) return;

        if (targetComponent.enabled)
            StartCoroutine(DisableWithAnim());
        else
            StartCoroutine(EnableWithAnim());
    }

    public void Enable()
    {
        if (targetComponent == null) return;
        if (isBusy) return;

        StartCoroutine(EnableWithAnim());
    }

    public void Disable()
    {
        if (targetComponent == null) return;
        if (isBusy) return;

        StartCoroutine(DisableWithAnim());
    }

    // ================= CORE =================

    IEnumerator DisableWithAnim()
    {
        isBusy = true;

        // 1) Trigger "in" trước khi tắt
        PlayTrigger(triggerIn);

        // 2) Chờ animation chạy
        if (animationDelay > 0f)
            yield return new WaitForSecondsRealtime(animationDelay);

        // 3) Tắt component
        if (targetComponent != null)
            targetComponent.enabled = false;

        isBusy = false;
    }

    IEnumerator EnableWithAnim()
    {
        isBusy = true;

        // 1) Bật component trước
        if (targetComponent != null)
            targetComponent.enabled = true;

        // 2) Trigger "out" sau khi bật
        PlayTrigger(triggerOut);

        // 3) Chờ một chút để tránh spam
        if (animationDelay > 0f)
            yield return new WaitForSecondsRealtime(animationDelay);

        isBusy = false;
    }

    void PlayTrigger(string triggerName)
    {
        if (animator == null) return;
        if (string.IsNullOrEmpty(triggerName)) return;

        animator.ResetTrigger(triggerIn);
        animator.ResetTrigger(triggerOut);

        animator.SetTrigger(triggerName);
    }
}
