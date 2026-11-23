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
    public float landDipAmount = 0.12f;
    public float landDipSpeed = 12f;  
    // 
    private bool wasGrounded = true;
    private float currentLandDip = 0f;
    // Weapon Bobbing
    public Transform weaponTransform;     // select weapon
    public float weaponBobSpeed = 14f;    // Speed of the walking bob cycle
    public float weaponBobX = 0.04f;      // Left and right sway amount
    public float weaponBobY = 0.04f;      // Up and down dip amount
    public float weaponSmooth = 10f;      // How smoothly the weapon moves/returns
    private float weaponBobTimer = 0f;
    private Vector3 weaponInitialPos;
    void Start()
    {
        // Lock Cursor temp
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        // default y position of player head
        defaultYPos = transform.localPosition.y;
        // default y position of weapon
        weaponInitialPos = weaponTransform.localPosition;

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
        float verticalInput = 0f;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed) horizontalInput -= 1f;
            if (Keyboard.current.dKey.isPressed) horizontalInput += 1f;
            if (Keyboard.current.wKey.isPressed) verticalInput += 1f;
            if (Keyboard.current.sKey.isPressed) verticalInput -= 1f;
        
        }
        bool isMoving = Mathf.Abs(horizontalInput) > 0.1f || Mathf.Abs(verticalInput) > 0.1f; // math.abs so negative values get turned into absolute
        // Negative so pressing D (right) tilts the camera slightly clockwise
        float targetTilt = -horizontalInput * tiltAmount;
        currentTilt = Mathf.Lerp(currentTilt, targetTilt, tiltSpeed * Time.deltaTime);
    
        // Getting the keyboard presses and calling Bob method

           
        HandleHeadBob(horizontalInput, verticalInput);
        HandleWeaponBob(isMoving);
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
        bool isGrounded = controller != null ? controller.isGrounded : true;

        
        if (!wasGrounded && isGrounded)
        {
            
            currentLandDip = -landDipAmount;
        }
        wasGrounded = isGrounded;

        
        currentLandDip = Mathf.Lerp(currentLandDip, 0f, landDipSpeed * Time.deltaTime);

        float targetY = defaultYPos + currentLandDip;

        if (isMoving && isGrounded)
        {
            bobTimer += Time.deltaTime * bobSpeed;
            targetY += Mathf.Sin(bobTimer) * bobAmount;

            transform.localPosition = new Vector3(transform.localPosition.x, targetY, transform.localPosition.z);
        }
        else
        {
            bobTimer = 0f;
            float smoothY = Mathf.Lerp(transform.localPosition.y, targetY, Time.deltaTime * bobReturnSpeed);
            transform.localPosition = new Vector3(transform.localPosition.x, smoothY, transform.localPosition.z);
        }

}
void HandleWeaponBob(bool isMoving)
    {
       Vector3 targetPos = weaponInitialPos;

    if (isMoving)
    {
        weaponBobTimer += Time.deltaTime * weaponBobSpeed;
        // actual fucking wizardry
        float bobX = Mathf.Cos(weaponBobTimer * 0.5f) * weaponBobX;
        float bobY = Mathf.Abs(Mathf.Sin(weaponBobTimer)) * weaponBobY;

        targetPos += new Vector3(bobX, bobY, 0f);
    }
    else
    {
        weaponBobTimer = 0f;
    }

    weaponTransform.localPosition = Vector3.Lerp(weaponTransform.localPosition, targetPos, weaponSmooth * Time.deltaTime);
    }
}