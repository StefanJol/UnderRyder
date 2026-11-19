using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    
   PlayerControls controls;          // the generated class
    Vector2 move;
    CharacterController controller;
    //Movement Values
    public float moveSpeed = 3f;
    public float JumpStr = 5f;
    public float gravity = -9.81f;
    public float runSpeed = 6f;
    Vector3 velocity;

void Awake() {
    controller = GetComponent<CharacterController>();
    controls = new PlayerControls();
    controls.Gameplay.Jump.performed += ctx => Jump();
}

void OnEnable()  { controls.Gameplay.Enable(); }
void OnDisable() { controls.Gameplay.Disable(); }

void Update() {
    // WASD Movement
    move = controls.Gameplay.Move.ReadValue<Vector2>();
    Vector3 moveDir = transform.right * move.x + transform.forward * move.y;

    // Gravity
        velocity.y += gravity * Time.deltaTime;
    // Applying Movement to Guy
     
    controller.Move( (moveDir * runSpeed + velocity) * Time.deltaTime);
}


void Jump(){

}
}
