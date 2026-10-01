using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    public GameObject ballPrefab;
    public Transform spawnPoint;

    // True = right, False = left
    public bool shootRight = false;

    void Start()
    {
        SpawnBall();
    }

    public void SpawnBall()
    {
        GameObject newBall = Instantiate(
            ballPrefab,
            spawnPoint.position,
            Quaternion.identity
        );

        BallProjectile ball = newBall.GetComponent<BallProjectile>();

        ball.SetSpawner(this);
        ball.SetDirection(shootRight);
    }
}