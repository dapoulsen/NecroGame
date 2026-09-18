using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Movement tuning
    public float movementSpeed = 5f;
    public float jumpForce = 8f;
    
    // Make sure player only can jump when on ground
    public Transform groundCheck;
    public Vector2 groundCheckSize = new Vector2(0.4f, 0.1f);
    public LayerMask groundLayer;

    // The point where the player should respawn
    public Transform respawnPoint;
    //The sprite to be spawned when you die
    public GameObject deadBodyPrefab;

    // Sprite to flip it when moving left
    public SpriteRenderer spriteRenderer;

    // Player controls and fields that are private
    private float _moveInput;
    private PlayerControls _controls;
    private Rigidbody2D _rb;
    private bool _isGrounded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _controls = new PlayerControls();
        spriteRenderer = GetComponent<SpriteRenderer>();
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

        // Set field to if player is on ground or not
       _isGrounded = Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0f, groundLayer);

       //Flip sprite when moving left
       if (_moveInput > 0f)
            spriteRenderer.flipX = false;
        else if (_moveInput < 0f)
            spriteRenderer.flipX = true;
    }

    void OnJump(InputAction.CallbackContext ctx)
    {
        if (_isGrounded)
        {
            _rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Spike"))
        {
            Die();
        }   
    }

    void Die()
    {
        //Spawn body at death location, facing the same way
        Instantiate(deadBodyPrefab, transform.position, transform.rotation);

        _rb.linearVelocity = Vector2.zero; // Stop any falling jumping momentum
        transform.position = respawnPoint.position;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawCube(groundCheck.position, groundCheckSize);
    }
}
