using UnityEngine;

public class UIAnimationButton : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator;

    [Header("Trigger Names")]
    public string triggerIn = "in";
    public string triggerOut = "out";

    private bool isOpen;

    void Reset()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayIn()
    {
        if (animator == null) return;

        animator.ResetTrigger(triggerOut);
        animator.SetTrigger(triggerIn);
        isOpen = true;
    }

    public void PlayOut()
    {
        if (animator == null) return;

        animator.ResetTrigger(triggerIn);
        animator.SetTrigger(triggerOut);
        isOpen = false;
    }

    // Nếu bạn muốn 1 nút toggle
    public void Toggle()
    {
        if (isOpen) PlayOut();
        else PlayIn();
    }
}
