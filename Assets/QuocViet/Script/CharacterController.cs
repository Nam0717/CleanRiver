using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 4f;
    public float runSpeed = 7f;
    public float rotationSpeed = 10f;

    private bool isWalking;
    private bool isRunning;


    [Header("Jump & Gravity")]
    public bool canJump = true;
    public float jumpForce = 1.5f;
    public float gravity = -9.81f;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float staminaDrainRate = 20f;   // trừ khi chạy
    public float staminaRegenRate = 15f;   // hồi khi không chạy
    public Slider staminaSlider;

    [Header("Stamina Recovery")]
    public float recoverThreshold = 30f; // mốc hồi để chạy lại
    private bool isExhausted;
    private float currentStamina;
    private CharacterController controller;
    private Animator animator;
    private Transform cam;

    private Vector3 velocity;
    private bool isGrounded;
    private float currentSpeed;

    [Header("Ground Check")]
    public Transform groundCheckPoint;   // Empty object dưới chân
    public float groundCheckRadius = 0.25f;
    public LayerMask groundMask;
    [Header("Ground Buffer")]
    public float groundBufferTime = 0.1f; // 100ms
    private float groundBufferCounter;
    private bool isGroundedStable;



    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        cam = Camera.main.transform;

        currentStamina = maxStamina;

        if (staminaSlider != null)
        {
            staminaSlider.maxValue = maxStamina;
            staminaSlider.value = currentStamina;
        }
    }

    void Update()
    {
        CheckGround();
        Move();
        Jump(); 
        ApplyGravity();
        UpdateAnimator();
        UpdateStaminaUI();
    }

    void CheckGround()
    {
        bool rawGrounded = Physics.CheckSphere(
            groundCheckPoint.position,
            groundCheckRadius,
            groundMask
        );

        if (rawGrounded)
        {
            groundBufferCounter = groundBufferTime;
        }
        else
        {
            groundBufferCounter -= Time.deltaTime;
        }

        isGroundedStable = groundBufferCounter > 0f;
        isGrounded = isGroundedStable;

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }



    void Jump()
    {
        if (!canJump) return;

        if (Input.GetButtonDown("Jump") && isGroundedStable && !isExhausted)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
            animator.SetTrigger("Jump");
        }
    }



    void Move()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 inputDir = new Vector3(h, 0, v).normalized;

        bool hasInput = inputDir.magnitude > 0.1f;
        bool wantsToRun = Input.GetKey(KeyCode.LeftShift);

        // ===== EXHAUST LOGIC =====
        if (currentStamina <= 0f)
        {
            isExhausted = true;
            currentStamina = 0f;
        }

        if (isExhausted && currentStamina >= recoverThreshold)
        {
            isExhausted = false;
        }

        bool canRun = !isExhausted && currentStamina > 0f;

        // ===== WALK / RUN STATE =====
        isRunning = hasInput && wantsToRun && canRun && isGroundedStable;
        isWalking = hasInput && !isRunning && isGroundedStable;

        currentSpeed = isRunning ? runSpeed : walkSpeed;

        // ===== STAMINA UPDATE =====
        if (isRunning)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
        }
        else
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
        }

        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

        // ===== MOVE =====
        if (hasInput)
        {
            float targetAngle = Mathf.Atan2(inputDir.x, inputDir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, rotationSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);

            Vector3 moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;
            controller.Move(moveDir * currentSpeed * Time.deltaTime);
        }
    }



    void ApplyGravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void UpdateAnimator()
    {
        animator.SetBool("Walk", isWalking);
        animator.SetBool("Run", isRunning);
        animator.SetBool("Grounded", isGroundedStable);
        animator.SetBool("Exhausted", isExhausted);
    }


    void UpdateStaminaUI()
    {
        if (staminaSlider != null)
        {
            staminaSlider.value = currentStamina;
        }
    }
}
