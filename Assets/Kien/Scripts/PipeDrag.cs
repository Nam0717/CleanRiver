using UnityEngine;
using UnityEngine.EventSystems;

public class PipeDrag : MonoBehaviour
{
    [Header("Rotation")]
    public float rotateStep = 90f;

    [Header("Snap Settings")]
    public float snapDistance = 0.5f;
    [Range(0.7f, 1f)]
    public float snapAngleThreshold = 0.9f;

    [Header("Inventory")]
    public bool isInInventory = true;
    public Vector3 inventoryScale = new Vector3(0.4f, 0.4f, 0.4f);

    private Vector3 originalScale;
    private Vector3 originalPosition;

    private Camera cam;
    private bool isDragging;
    private Vector3 offset;
    private Connector[] connectors;

    void Start()
    {
        cam = Camera.main;
        connectors = GetComponentsInChildren<Connector>();

        originalScale = transform.localScale;
        originalPosition = transform.position;

        if (isInInventory)
        {
            transform.localScale = inventoryScale;
        }
    }

    void Update()
    {
        if (isDragging && Input.GetKeyDown(KeyCode.R))
        {
            RotatePipe();
        }
    }

    void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        isDragging = true;

        AudioManager.Instance.PlayDrag();

        Vector3 mouseWorld = GetMouseWorld();
        offset = transform.position - mouseWorld;

        // Nếu đang ở inventory → phóng to lại
        if (isInInventory)
        {
            isInInventory = false;
            transform.localScale = originalScale;
        }

        foreach (var c in connectors)
            c.ForceDisconnect();
    }

    void OnMouseDrag()
    {
        if (!isDragging) return;

        Vector3 mouseWorld = GetMouseWorld();
        transform.position = mouseWorld + offset;
    }

    void OnMouseUp()
    {
        isDragging = false;

        TrySnap();

        // Nếu thả không nằm trên board → quay lại inventory
        if (!IsOnBoard())
        {
            ReturnToInventory();
        }
    }

    Vector3 GetMouseWorld()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);

        if (plane.Raycast(ray, out float distance))
            return ray.GetPoint(distance);

        return Vector3.zero;
    }

    void TrySnap()
    {
        bool snappedSomething;

        do
        {
            snappedSomething = false;

            foreach (var my in connectors)
            {
                if (my.IsConnected) continue;

                Connector nearest = FindBestMatch(my);

                if (nearest != null)
                {
                    Snap(my, nearest);
                    snappedSomething = true;
                }
            }

        } while (snappedSomething);
    }

    Connector FindBestMatch(Connector my)
    {
        Connector best = null;
        float minDist = snapDistance;

        foreach (var other in Connector.All)
        {
            if (other == my) continue;
            if (other.transform.root == transform) continue;
            if (other.IsConnected) continue;

            float dist = Vector3.Distance(
                my.transform.position,
                other.transform.position);

            if (dist > minDist) continue;

            float dot = Vector3.Dot(
                my.WorldDirection,
                -other.WorldDirection);

            if (dot < snapAngleThreshold) continue;

            minDist = dist;
            best = other;
        }

        return best;
    }

    void Snap(Connector my, Connector other)
    {
        Quaternion rotationNeeded =
            Quaternion.FromToRotation(
                my.WorldDirection,
                -other.WorldDirection);

        transform.rotation = rotationNeeded * transform.rotation;

        Vector3 offsetFromRoot =
            my.transform.position - transform.position;

        transform.position =
            other.transform.position - offsetFromRoot;

        my.Connect(other);

        AudioManager.Instance.PlaySnap();
    }

    void RotatePipe()
    {
        transform.Rotate(0f, rotateStep, 0f);
        AudioManager.Instance.PlayRotate();
    }

    // =========================
    // INVENTORY LOGIC
    // =========================

    bool IsOnBoard()
    {
        // Bạn chỉnh theo vị trí board của bạn
        // Ví dụ: board bắt đầu từ x > -2

        return transform.position.x > -2f;
    }

    void ReturnToInventory()
    {
        isInInventory = true;

        transform.localScale = inventoryScale;
        transform.position = originalPosition;

        foreach (var c in connectors)
            c.ForceDisconnect();
    }
}