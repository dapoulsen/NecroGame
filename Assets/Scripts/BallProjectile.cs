using UnityEngine;

public class BallProjectile : MonoBehaviour
{
    public float speed = 5f;

    private BallSpawner spawner;
    private Camera mainCamera;
    private bool hasLeftScreen = false;

    public void SetSpawner(BallSpawner ballSpawner)
    {
        spawner = ballSpawner;
    }

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Shoot/move to the left
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        // Convert the ball's position into viewport coordinates
        Vector3 viewportPosition = mainCamera.WorldToViewportPoint(transform.position);

        // Check if the ball has completely gone off the left side
        if (!hasLeftScreen && viewportPosition.x < 0f)
        {
            hasLeftScreen = true;

            // Spawn the next ball
            spawner.SpawnBall();

            // Destroy this ball
            Destroy(gameObject);
        }
    }
}