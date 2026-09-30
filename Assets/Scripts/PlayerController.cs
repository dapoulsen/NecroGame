using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Movement tuning
    public float movementSpeed = 5f;
    public float jumpForce = 8f;
    
    // Make sure player only can jump when on ground
    public Vector2 groundCheckSize = new Vector2(0.4f, 0.1f);
    public float groundCheckOffset = 0.02f; // Hvor højt over transform.position boxen sidder
    public LayerMask groundLayer;

    // The point where the player should respawn
    public Transform respawnPoint;
    //The sprite to be spawned when you die
    public GameObject deadBodyPrefab;

    // Sprite to flip it when moving left
    private SpriteRenderer spriteRenderer;

    // Player controls and fields that are private
    private float _moveInput;
    private PlayerControls _controls;
    private Rigidbody2D _rb;
    private bool _isGrounded;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _controls = new PlayerControls();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable() {
        _controls.Player.Enable();
        _controls.Player.Jump.performed += OnJump;
    }

    void OnDisable()
    {
        _controls.Player.Jump.performed -= OnJump;
        _controls.Player.Disable();
    }

    void Update()
    {
        _moveInput = _controls.Player.Move.ReadValue<float>();

        if (_moveInput > 0f)
            spriteRenderer.flipX = false;
        else if (_moveInput < 0f)
            spriteRenderer.flipX = true;
    }

    void FixedUpdate()
    {
        _rb.linearVelocity = new Vector2(_moveInput * movementSpeed, _rb.linearVelocity.y);

        Vector2 checkPosition = (Vector2)transform.position + Vector2.up * groundCheckOffset;
        _isGrounded = Physics2D.OverlapBox(checkPosition, groundCheckSize, 0f, groundLayer);
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
        Instantiate(deadBodyPrefab, transform.position, transform.rotation);

        _rb.linearVelocity = Vector2.zero;
        transform.position = respawnPoint.position;
    }

    void OnDrawGizmosSelected()
    {
        Vector2 checkPosition = (Vector2)transform.position + Vector2.up * groundCheckOffset;
        Gizmos.color = Color.red;
        Gizmos.DrawCube(checkPosition, groundCheckSize);
    }
}