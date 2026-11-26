using UnityEngine;
using System.Collections;
public class PlayerScript : MonoBehaviour
{
    
   PlayerControls controls;          // the generated class
    Vector2 move;
    CharacterController controller;
    //Movement Values
    public float moveSpeed = 3f;
    public float JumpStr = 5f;
    public float gravity = -20f;
    public float runSpeed = 6f;
    //Dash variables
    private int dashCounter = 3;
    public float dashRechargeTimer = 2f;
    private float dashTimer = 0f;
    private bool isRechargingDash = false;
    private Vector3 dashDir;
    public float dashStr = 20f;
    public float dashTime = 0.25f;
    Vector3 velocity;
    public int JumpsLeft = 1;
    private bool touchingWall = false;
    private string currentTouchedWall;
    private string lastTouchedWall;
    
void Awake() {
    controller = GetComponent<CharacterController>();
    controls = new PlayerControls();
    controls.Gameplay.Jump.performed += ctx => Jump();
    controls.Gameplay.Dash.performed += ctx => Dash();
}

void OnEnable()  { controls.Gameplay.Enable(); }
void OnDisable() { controls.Gameplay.Disable(); }

void Update() {
    touchingWall = false;
    // WASD Movement
    move = controls.Gameplay.Move.ReadValue<Vector2>();
    Vector3 moveDir = transform.right * move.x + transform.forward * move.y;

    // Applying Movement to Guy
     if (dashTimer > 0f)
        {
            dashTimer -= Time.deltaTime;
            controller.Move(dashDir * dashStr * Time.deltaTime);
        }
        else
        {
            
            // Gravity
            if (controller.isGrounded) {

            if (velocity.y < 0f) 
            {
                velocity.y = -2f; 
            }
    } 
    else {
        // Apply gravity when in the air
        velocity.y += gravity * Time.deltaTime;
    }
             // Normal Movement application
            controller.Move((moveDir * runSpeed + velocity) * Time.deltaTime);
        }
}


void Jump(){

    if (controller.isGrounded ){
        velocity.y = JumpStr;
        
        JumpsLeft = 1;
    }
    if (touchingWall && JumpsLeft > 0)
    {
        velocity.y = JumpStr;
        JumpsLeft -= 1;
        
    }
}

void Dash(){
       if (dashTimer <= 0f && dashCounter >=1)
        {
            dashDir = transform.right * move.x + transform.forward * move.y;
            if (dashDir == Vector3.zero) dashDir = transform.forward; // forward direction - may change this later
            dashDir.Normalize(); // so diagonal movement isnt faster than forward n back movement

            dashTimer = dashTime; // Start Timer
            dashCounter--;
        }
        if (dashCounter <3 && !isRechargingDash){
            StartCoroutine(RechargeDash(dashRechargeTimer));
        }


}

private IEnumerator RechargeDash(float timer){
    isRechargingDash = true;

        while (dashCounter < 3)
        {
            yield return new WaitForSeconds(dashRechargeTimer);
            dashCounter = Mathf.Min(dashCounter + 1, 3);
        }

        isRechargingDash = false;

}

void OnControllerColliderHit(ControllerColliderHit hit)
{
    // Check if the surface normal is roughly horizontal
    if (Mathf.Abs(hit.normal.y) < 0.3f)
        {  
            lastTouchedWall = hit.gameObject.name;
            touchingWall = true;
        }
}

}
