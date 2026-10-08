using UnityEngine;

public class BallProjectile : MonoBehaviour
{
    public float speed = 5f;

    private float direction = -1f;

    private BallSpawner spawner;
    private Camera mainCamera;
    private SpriteRenderer spriteRenderer;
    private bool hasLeftScreen = false;
    private bool hasHandledDestruction = false;

    public void SetSpawner(BallSpawner ballSpawner)
    {
        spawner = ballSpawner;
    }

    public BallSpawner GetSpawner()
    {
        return spawner;
    }

    public void SetDirection(bool shootRight)
    {
        if (shootRight)
        {
            direction = 1f;
        }
        else
        {
            direction = -1f;
        }

        // Get the arrow's SpriteRenderer
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Flip the sprite depending on direction
        if (shootRight)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }

    void Start()
    {
        mainCamera = Camera.main;

        // In case the SpriteRenderer wasn't found yet
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    void Update()
    {
        // Move left or right
        transform.Translate(
            Vector2.right * direction * speed * Time.deltaTime
        );

        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);

        // Check if the arrow has left the screen
        if (!hasLeftScreen &&
            (viewportPosition.x < 0f ||
             viewportPosition.x > 1f))
        {
            hasLeftScreen = true;

            HandleDestruction();
            Destroy(gameObject);
        }
    }

    // Called when the player collides with the ball.
    public void DestroyByPlayer()
    {
        HandleDestruction();
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        HandleDestruction();
    }

    private void HandleDestruction()
    {
        if (hasHandledDestruction)
        {
            return;
        }

        hasHandledDestruction = true;

        if (spawner != null)
        {
            spawner.NotifyBallDestroyed(this);
        }
    }
}
