using UnityEngine;
using UnityEngine.InputSystem;
public class FirstPersonCam : MonoBehaviour
{
    
    public Transform playerBody;
    public CharacterController controller;
    // Settings
    public float sensitivityX = 15f;
    public float sensitivityY = 15f;
    public float maxVerticalAngle = 85f;
    // Clamping
    private float xRotation = 0f;
    private float currentTilt = 0f;
    // Camera Sway
    public float tiltAmount = 1.8f;      // How many degrees the camera leans
    public float tiltSpeed = 10f;      // How fast it snaps into and out of the lean
    // Camera Bob
    public float bobSpeed = 14f;
    public float bobAmount = 0.05f;
    public float bobReturnSpeed = 10f;
    private float defaultYPos = 0f;
    private float bobTimer = 0f;
    void Start()
    {
        // Lock Cursor temp
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // default y position of player head
        defaultYPos = transform.localPosition.y;
    }

    void Update()
    {
        // get mouse input
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        float mouseX = mouseDelta.x * sensitivityX * Time.deltaTime;
        float mouseY = mouseDelta.y * sensitivityY * Time.deltaTime;
        // clamp
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxVerticalAngle, maxVerticalAngle);
        // Strafe Tilt Calculation
        float horizontalInput = 0f;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed) horizontalInput -= 1f;
            if (Keyboard.current.dKey.isPressed) horizontalInput += 1f;
        }
        // Negative so pressing D (right) tilts the camera slightly clockwise
        float targetTilt = -horizontalInput * tiltAmount;
        currentTilt = Mathf.Lerp(currentTilt, targetTilt, tiltSpeed * Time.deltaTime);
        float verticalInput = 0f;
        // Getting the keyboard presses and calling Bob method
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) verticalInput += 1f;
            if (Keyboard.current.sKey.isPressed) verticalInput -= 1f;
        }
        HandleHeadBob(horizontalInput, verticalInput);

        // Apply rotation
        transform.localRotation = Quaternion.Euler(xRotation, 0f, currentTilt);

        // Rotate the player body left and right
        if (playerBody != null)
        {
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }

    void HandleHeadBob(float moveX, float moveZ)
{
    bool isMoving = Mathf.Abs(moveX) > 0.1f || Mathf.Abs(moveZ) > 0.1f;
    bool isGrounded = controller.isGrounded;

    if (isMoving && isGrounded)
    {
        bobTimer += Time.deltaTime * bobSpeed;
        float newY = defaultYPos + Mathf.Sin(bobTimer) * bobAmount;
        transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.z);
    }
    else
    {
        bobTimer = 0f;
        float smoothY = Mathf.Lerp(transform.localPosition.y, defaultYPos, Time.deltaTime * bobReturnSpeed);
        transform.localPosition = new Vector3(transform.localPosition.x, smoothY, transform.localPosition.z);
    }
}
}