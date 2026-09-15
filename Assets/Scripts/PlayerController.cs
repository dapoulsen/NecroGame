using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Movement tuning
    public float movementSpeed = 0.01f;
    //Input System action exposed in Inspector for binding (WASD keys)
    public InputAction MoveAction;
    private Vector2 moveInput;

    //Called when player becomes enabled or active
    void OnEnable() {
        //Enable the MoveAction so it starts reading input
        MoveAction.Enable();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Read the 2D vector from the MoveAction
       moveInput = MoveAction.ReadValue<Vector2>(); 

       Vector2 position = (Vector2)transform.position + moveInput * movementSpeed;

       transform.position = position;
    }
}
