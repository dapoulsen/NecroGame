using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
    
    // Temporary invulnerability after respawn to avoid instant re-death
    public float invulnerabilityDuration = 1.0f;
    private bool _invulnerable = false;
    
    // Coyote time
    public float coyoteTime = 0.1f;
    private float _coyoteTimer;
    
    // Jump buffering
    public float jumpBufferTime = 0.1f;
    private float _jumpBufferTimer;
    private bool _jumpHeld;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _controls = new PlayerControls();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable() {
        _controls.Player.Enable();
        _controls.Player.Jump.performed += OnJump;
        _controls.Player.Jump.canceled += OnJumpCancelled;
        _controls.Player.ResetGame.performed += OnGameReset;
    }

    void OnDisable()
    {
        _controls.Player.ResetGame.performed -= OnGameReset;
        _controls.Player.Jump.performed -= OnJump;
        _controls.Player.Jump.canceled -= OnJumpCancelled;
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
        //Moving
        _rb.linearVelocity = new Vector2(_moveInput * movementSpeed, _rb.linearVelocity.y);
        
        
        //Jumping
        Vector2 checkPosition = (Vector2)transform.position + Vector2.up * groundCheckOffset;
        _isGrounded = Physics2D.OverlapBox(checkPosition, groundCheckSize, 0f, groundLayer);

        //Coyote time
        if (_isGrounded && _rb.linearVelocity.y < coyoteTime)
            _coyoteTimer = coyoteTime;
        else 
            _coyoteTimer -= Time.fixedDeltaTime;
        
        //Jump buffering
        _jumpBufferTimer -= Time.fixedDeltaTime;

        if (_jumpBufferTimer > 0f && _coyoteTimer > 0f)
        {
            //If the button is already released before landing, do a short jump
            float jumpSpeed = _jumpHeld ? jumpForce : jumpForce * 0.4f;
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpSpeed);

            _jumpBufferTimer = 0f;
            _coyoteTimer = 0f;
        }
    }

    void OnJump(InputAction.CallbackContext ctx)
    {
        _jumpBufferTimer = jumpBufferTime;
        _jumpHeld = true;
    }

    void OnJumpCancelled(InputAction.CallbackContext ctx)
    {
        _jumpHeld = false;
        if (_rb.linearVelocity.y > 0f)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _rb.linearVelocityY * 0.5f);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Spike"))
        {
            Die();
        }
        if (other.CompareTag("Arrow"))
        {
            if (_invulnerable) return;
            Die();

            // Make sure we only destroy the actual projectile instance
            // and not a spawner or parent object that might share the collider/tag.
            BallProjectile ball = other.GetComponent<BallProjectile>();
            if (ball == null)
            {
                ball = other.GetComponentInParent<BallProjectile>();
            }

            if (ball != null)
            {
                ball.DestroyByPlayer();
            }
        }
    }

    void Die()
    {
        GameObject bodyToSpawn = deadBodyPrefab;
        if (LevelSettings.Instance != null && LevelSettings.Instance.deadBodyOverride != null)
        {
            bodyToSpawn = LevelSettings.Instance.deadBodyOverride;
        }
        
        Instantiate(bodyToSpawn, transform.position, transform.rotation);
        DeathCounter.Instance.RegisterDeath();

        _rb.linearVelocity = Vector2.zero;
        transform.position = respawnPoint.position;

        // Start temporary invulnerability so the player doesn't immediately die again
        StartCoroutine(TemporaryInvulnerability());
    }

    private IEnumerator TemporaryInvulnerability()
    {
        _invulnerable = true;
        yield return new WaitForSeconds(invulnerabilityDuration);
        _invulnerable = false;
    }

    void OnGameReset(InputAction.CallbackContext ctx)
    {
        DeathCounter.Instance.ResetAll();
        SceneManager.LoadScene("Level0");
    }

    void OnDrawGizmosSelected()
    {
        Vector2 checkPosition = (Vector2)transform.position + Vector2.up * groundCheckOffset;
        Gizmos.color = Color.red;
        Gizmos.DrawCube(checkPosition, groundCheckSize);
    }
}
