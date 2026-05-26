using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Camera Settings")]
    public float distance = 5f;
    public float height = 2f;
    public float rotationSpeed = 3f;

    [Header("Vertical Clamp")]
    public float minY = -30f;
    public float maxY = 60f;

    [Header("Collision")]
    public float collisionOffset = 0.2f;
    public LayerMask collisionMask;

    [Header("Shoulder Camera")]
    public float sideOffset = 1.1f;
    public float lookOffset = -0.4f;

    [Header("Input")]
    public bool rightMouseToggleLook = true;

    [Header("Fail-Safe Cursor Unlock")]
    public bool autoUnlockWhenPaused = true;

    [Header("Fake Cursor UI (optional)")]
    public GameObject fakeCursorUI;

    private float currentX;
    private float currentY;

    private bool isLooking;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        currentX = angles.y;
        currentY = angles.x;

        SetLooking(false);
    }

    void Update()
    {
        // ===== FAIL SAFE =====
        if (autoUnlockWhenPaused && Time.timeScale == 0f)
        {
            SetLooking(false);
            return;
        }

        // ===== RMB TOGGLE =====
        if (rightMouseToggleLook && Input.GetMouseButtonDown(1))
        {
            SetLooking(!isLooking);
        }

        // ===== ROTATE CAMERA =====
        if (isLooking)
        {
            currentX += Input.GetAxis("Mouse X") * rotationSpeed * 100f * Time.unscaledDeltaTime;
            currentY -= Input.GetAxis("Mouse Y") * rotationSpeed * 100f * Time.unscaledDeltaTime;
            currentY = Mathf.Clamp(currentY, minY, maxY);
        }
    }

    void LateUpdate()
    {
        if (!target) return;

        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);

        Vector3 backDir = rotation * Vector3.back;
        Vector3 rightDir = rotation * Vector3.right;

        Vector3 targetPos = target.position + Vector3.up * height;

        float desiredDistance = distance;

        // Collision
        if (Physics.Raycast(targetPos, backDir, out RaycastHit hit, distance, collisionMask))
        {
            desiredDistance = hit.distance - collisionOffset;
            desiredDistance = Mathf.Clamp(desiredDistance, 0.5f, distance);
        }

        Vector3 cameraPos =
            targetPos
            + backDir * desiredDistance
            + rightDir * sideOffset;

        transform.position = cameraPos;

        Vector3 lookTarget =
            targetPos
            + rightDir * lookOffset;

        transform.LookAt(lookTarget);
    }

    void OnDisable()
    {
        SetLooking(false);
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            SetLooking(false);
    }

    void SetLooking(bool looking)
    {
        isLooking = looking;

        if (isLooking)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (fakeCursorUI != null)
                fakeCursorUI.SetActive(true);
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (fakeCursorUI != null)
                fakeCursorUI.SetActive(false);
        }
    }

    // 👇 Nếu bạn muốn PauseManager gọi trực tiếp
    public void ForceUnlock()
    {
        SetLooking(false);
    }
}
