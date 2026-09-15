using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Movement tuning
    public float movementSpeed = 5f;
    private float _moveInput;
    private PlayerControls _controls;
    private Rigidbody2D _rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _controls = new PlayerControls();
    }

    
    //Called when player becomes enabled or active
    void OnEnable() {
        _controls.Player.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        // Read the 2D vector from the MoveAction
       _moveInput = _controls.Player.Move.ReadValue<float>();

       transform.position += Vector3.right * _moveInput * movementSpeed * Time.deltaTime;
    }
}
