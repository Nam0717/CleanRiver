using UnityEngine;

public class SignalSetActive : MonoBehaviour
{
    [Header("Target")]
    public GameObject targetObject;

    // 👉 GỌI BẰNG SIGNAL
    public void SetActiveTrue()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(true);
        }
    }

    // (Tuỳ chọn) nếu cần tắt
    public void SetActiveFalse()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(false);
        }
    }
}
