using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 6f;
    public float mouseSensitivity = 0.1f;
    public float gravity = -9.81f;
    public Transform cameraTransform;

    CharacterController controller;

    float xRotation;
    Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // ===== INPUT (simple polling) =====
        Vector2 moveInput = Vector2.zero;
        Vector2 lookInput = Vector2.zero;

        var kb = Keyboard.current;
        var mouse = Mouse.current;

        if (kb != null)
        {
            if (kb.wKey.isPressed) moveInput.y += 1;
            if (kb.sKey.isPressed) moveInput.y -= 1;
            if (kb.dKey.isPressed) moveInput.x += 1;
            if (kb.aKey.isPressed) moveInput.x -= 1;
        }

        if (mouse != null)
        {
            lookInput = mouse.delta.ReadValue();
        }

        // ===== LOOK =====
        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // ===== MOVE =====
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * speed * Time.deltaTime);

        // ===== GRAVITY =====
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}