using UnityEngine;

public class DogController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 100f;
    public float runSpeed = 200f;
    public float crouchSpeed = 1f;
    public float jumpForce = 5f;

    [Header("Rotation")]
    public float rotationSpeed = 10f;

    [Header("Gravity")]
    public float gravity = -9.81f;
    private float verticalVelocity;

    [Header("Mouse Look")]
    public float mouseSensitivity = 200f;
    public float minPitch = -40f;
    public float maxPitch = 60f;

    private float yaw;
    private float pitch;

    [Header("Camera")]
    public Transform cameraTransform;

    [Header("Animator")]
    public Animator animator;

    private CharacterController controller;
    private Vector3 moveDirection;

    private bool isSwimming = false;
    private bool isDigging = false;
    private bool isCrouching = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        yaw = cameraTransform.eulerAngles.y;
        pitch = cameraTransform.eulerAngles.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
        HandleActions();
        UpdateAnimator();
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        cameraTransform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    void HandleMovement()
    {
        if (isDigging) return;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        moveDirection = (forward * z + right * x).normalized;

        bool running = Input.GetKey(KeyCode.LeftShift);

        float currentSpeed = walkSpeed;

        if (isCrouching)
        {
            currentSpeed = crouchSpeed;
        }
        else if (running)
        {
            currentSpeed = runSpeed;
        }

        if (isSwimming)
        {
            currentSpeed *= 0.6f;
        }

        // Rotate dog toward movement direction
        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(moveDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        // Ground check
        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) &&
            controller.isGrounded &&
            !isSwimming)
        {
            verticalVelocity =
                Mathf.Sqrt(jumpForce * -2f * gravity);

            if (animator != null)
            {
                animator.SetTrigger("Jump");
            }
        }

        // Celebrate
        if (Input.GetKeyDown(KeyCode.C) &&
            controller.isGrounded &&
            !isSwimming)
        {
            if (animator != null)
            {
                animator.SetTrigger("Celebrate");
            }
        }

        // Gravity
        verticalVelocity += gravity * Time.deltaTime;

        Vector3 velocity = moveDirection * currentSpeed;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);
    }

    void HandleActions()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            isDigging = true;

            if (animator != null)
            {
                animator.SetBool("Digging", true);
            }
        }

        if (Input.GetKeyUp(KeyCode.F))
        {
            isDigging = false;

            if (animator != null)
            {
                animator.SetBool("Digging", false);
            }
        }
    }

    void UpdateAnimator()
    {
        if (animator == null) return;

        float speed = moveDirection.magnitude;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed *= 2f;
        }

        animator.SetFloat("Speed", speed);
        animator.SetBool("Swimming", isSwimming);
        animator.SetBool("Crouching", isCrouching);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            Debug.Log("Swimming dog");
            isSwimming = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isSwimming = false;
        }
    }
}