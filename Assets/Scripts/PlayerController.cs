using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Movement tuning
    public float movementSpeed = 5f;
    public float jumpForce = 8f;
    
    public Transform groundCheck;
    public Vector2 groundCheckSize = new Vector2(0.4f, 0.1f);
    public LayerMask groundLayer;

    private float _moveInput;
    private PlayerControls _controls;
    private Rigidbody2D _rb;
    private bool _isGrounded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _controls = new PlayerControls();
    }

    
    //Called when player becomes enabled or active
    void OnEnable() {
        _controls.Player.Enable();
        _controls.Player.Jump.performed += OnJump;
    }

    void OnDisable()
    {
        _controls.Player.Jump.performed -= OnJump;
        _controls.Player.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        // Read the 2D vector from the MoveAction
       _moveInput = _controls.Player.Move.ReadValue<float>();

       transform.position += Vector3.right * _moveInput * movementSpeed * Time.deltaTime;

       _isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);
    }

    void OnJump(InputAction.CallbackContext ctx)
    {
        if (_isGrounded)
        {
            _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
}
