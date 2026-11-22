using UnityEngine;

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
    private float dashTimer = 0f;
    private Vector3 dashDir;
    public float dashStr = 20f;
    public float dashTime = 0.25f;
    Vector3 velocity;

void Awake() {
    controller = GetComponent<CharacterController>();
    controls = new PlayerControls();
    controls.Gameplay.Jump.performed += ctx => Jump();
    controls.Gameplay.Dash.performed += ctx => Dash();;
}

void OnEnable()  { controls.Gameplay.Enable(); }
void OnDisable() { controls.Gameplay.Disable(); }

void Update() {
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

    if (controller.isGrounded){
        velocity.y = JumpStr;
        
        
    }
}

void Dash(){
       if (dashTimer <= 0f)
        {
            dashDir = transform.right * move.x + transform.forward * move.y;
            if (dashDir == Vector3.zero) dashDir = transform.forward; // forward direction - may change this later
            dashDir.Normalize(); // so diagonal movement isnt faster than forward n back movement

            dashTimer = dashTime; // Start Timer
        }


}

}
