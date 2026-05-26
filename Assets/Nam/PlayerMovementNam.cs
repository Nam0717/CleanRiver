using UnityEngine;

public class PlayerMovementNam : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float mouseSensitivity = 200f;
    public Transform playerCamera;

    // --- Thêm biến cho trọng lực ---
    public float gravity = -9.81f; 
    Vector3 velocity;
    // ----------------------------

    private float xRotation = 0f;
    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // 1. Xử lý trọng lực (Phải đặt ở đầu Update)
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Giữ nhân vật bám sát mặt đất
        }

        // 2. Xoay Camera (Giữ nguyên code cũ)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // 3. Di chuyển
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * moveSpeed * Time.deltaTime);

        // 4. Áp dụng trọng lực vào chuyển động
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}