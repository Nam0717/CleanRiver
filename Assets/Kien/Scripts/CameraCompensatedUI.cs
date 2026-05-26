using UnityEngine;

public class CameraCompensatedUI : MonoBehaviour
{
    public Transform target;          // xe
    public Vector3 baseOffset = new Vector3(0, 1.2f, 0);
    public float cameraCompensation = 0.6f;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        if (cam == null || target == null) return;

        // Hướng camera nhìn xuống mặt đất
        Vector3 camForward = cam.transform.forward;
        camForward.y = 0;
        camForward.Normalize();

        // Bù vị trí UI theo hướng camera
        Vector3 compensatedOffset =
            baseOffset - camForward * cameraCompensation;

        transform.position = target.position + compensatedOffset;
    }
}
