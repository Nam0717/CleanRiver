using UnityEngine;
using UnityEngine.UI;

public class AxeInteraction : MonoBehaviour
{
    [Header("UI")]
    public GameObject interactUI;      // Group UI
    public Image progressImage;        // Image fill tiến độ

    [Header("Hold Settings")]
    public float holdDuration = 1.5f;  // Thời gian giữ E

    private bool isLooking;
    private float holdTimer;

    void Start()
    {
        if (interactUI != null)
            interactUI.SetActive(false);

        if (progressImage != null)
            progressImage.fillAmount = 0f;
    }

    void Update()
    {
        CheckRaycast();
        HandleHoldInput();
    }

    void HandleHoldInput()
    {
        if (!isLooking)
        {
            ResetProgress();
            return;
        }

        if (Input.GetKey(KeyCode.E))
        {
            holdTimer += Time.deltaTime;

            if (progressImage != null)
                progressImage.fillAmount = holdTimer / holdDuration;

            if (holdTimer >= holdDuration)
            {
                CompleteInteraction();
            }
        }
        else
        {
            ResetProgress();
        }
    }

    void ResetProgress()
    {
        holdTimer = 0f;

        if (progressImage != null)
            progressImage.fillAmount = 0f;
    }

    void CompleteInteraction()
    {
        ResetProgress();

        // 🔥 Thông báo cho cây dừng bị chặt
        TreeDestroyHandler tree =
            GetComponentInParent<TreeDestroyHandler>();

        if (tree != null)
        {
            tree.CancelDestroyCondition();
        }

        // 🟢 CỘNG ĐIỂM BẢO VỆ CÂY
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddProtectionScore(10);
        }

        gameObject.SetActive(false);
    }

    void CheckRaycast()
    {
        isLooking = false;

        if (InteractionRaycaster.Instance == null) return;
        if (!InteractionRaycaster.Instance.HasHit) return;

        RaycastHit hit = InteractionRaycaster.Instance.CurrentHit;

        if (hit.collider.gameObject == gameObject)
        {
            isLooking = true;
        }

        if (interactUI != null)
            interactUI.SetActive(isLooking);
    }
}
