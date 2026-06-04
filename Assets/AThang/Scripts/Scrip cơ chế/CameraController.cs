using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Pan Settings")]
    public float panSensitivity = 0.005f;   // chỉnh trong Inspector

    [Header("Zoom Settings")]
    public float zoomSpeed = 10f;
    public float minZoom = 6f;
    public float maxZoom = 20f;

    [Header("Camera Limits")]
    public float minX = -1f;
    public float maxX = 5f;
    public float minZ = -1f;
    public float maxZ = 5f;
    private bool isDraggingCube;

    Vector3 lastMouse;

    void Update()
    {
        HandleZoom();
        HandlePan();
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll == 0) return;

        Vector3 pos = transform.position;

        pos.y -= scroll * zoomSpeed;
        pos.y = Mathf.Clamp(pos.y, minZoom, maxZoom);

        transform.position = pos;
    }

    void HandlePan()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastMouse = Input.mousePosition;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform.GetComponent<CubeController>() != null)
                {
                    isDraggingCube = true;
                    return;
                }
            }

            isDraggingCube = false;
        }

        if (Input.GetMouseButton(0) && !isDraggingCube)
        {
            Vector3 delta = Input.mousePosition - lastMouse;

            Vector3 move = new Vector3(
                -delta.x * panSensitivity,
                0,
                -delta.y * panSensitivity
            );

            transform.position += move;

            ClampPosition();

            lastMouse = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
            isDraggingCube = false;
    }

    void ClampPosition()
    {
        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.z = Mathf.Clamp(pos.z, minZ, maxZ);

        transform.position = pos;
    }
}