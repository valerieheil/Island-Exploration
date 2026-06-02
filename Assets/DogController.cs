// using System.Diagnostics;
using UnityEngine;

public class DogController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float crouchSpeed = 1f;
    public float jumpForce = 5f;

    [Header("Rotation")]
    public float rotationSpeed = 10f;

    [Header("Gravity")]
    public float gravity = -9.81f;
    private float verticalVelocity;

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

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleMovement();
        HandleActions();
        UpdateAnimator();
    }

    void HandleMovement()
    {
        if (isDigging) return;

        // INPUT
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // CAMERA RELATIVE MOVEMENT
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // Ignore camera vertical angle
        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        // Movement relative to camera
        moveDirection = (forward * z + right * x).normalized;

        // SPEED
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

        // Swimming slows movement
        if (isSwimming)
        {
            currentSpeed *= 0.6f;
        }


        // ROTATE DOG TOWARD MOVEMENT
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

        // GROUND CHECK
        if (controller.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }

        // JUMP
        if (Input.GetKeyDown(KeyCode.Space) &&
            controller.isGrounded &&
            !isSwimming)
        {
            verticalVelocity =
                Mathf.Sqrt(jumpForce * -2f * gravity);

            animator.SetTrigger("Jump");
        }

        if (Input.GetKeyDown(KeyCode.C) &&
            controller.isGrounded &&
            !isSwimming)
        {
        

            animator.SetTrigger("Celebrate");
        }

        // GRAVITY
        verticalVelocity += gravity * Time.deltaTime;

        // FINAL MOVEMENT
        Vector3 velocity = moveDirection * currentSpeed;
        velocity.y = verticalVelocity;

        controller.Move(velocity * Time.deltaTime);
    }

    void HandleActions()
    {
        // Digging
        if (Input.GetKeyDown(KeyCode.F))
        {
            isDigging = true;
            animator.SetBool("Digging", true);
        }

        if (Input.GetKeyUp(KeyCode.F))
        {
            isDigging = false;
            animator.SetBool("Digging", false);
        }
    }

    void UpdateAnimator()
    {
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