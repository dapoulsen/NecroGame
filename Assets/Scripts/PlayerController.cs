using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Movement tuning
    public float movementSpeed = 0.01f;
    private Vector2 _moveInput;
    public InputAction MoveAction; 

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
       _moveInput = MoveAction.ReadValue<Vector2>(); 

       Vector2 position = (Vector2)transform.position + _moveInput * movementSpeed;

       transform.position = position;
    }
}
