using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    // Registry of all active spawners in the scene
    private static System.Collections.Generic.List<BallSpawner> _allSpawners = new System.Collections.Generic.List<BallSpawner>();

    public GameObject ballPrefab;
    public Transform spawnPoint;

    // True = right, False = left
    public bool shootRight = false;

    // Track currently active ball per spawner to avoid duplicate active balls
    private BallProjectile _currentBall;

    void Start()
    {
        SpawnBall();
    }
    void Awake()
    {
        if (!_allSpawners.Contains(this))
            _allSpawners.Add(this);
    }

    void OnDisable()
    {
        _allSpawners.Remove(this);
    }

    public void SpawnBall()
    {
        // Only spawn if this spawner doesn't already have an active ball
        if (_currentBall != null) return;

        UnityEngine.Debug.Log($"BallSpawner.SpawnBall called on {gameObject.name}");

        GameObject newBall = Instantiate(
            ballPrefab,
            spawnPoint.position,
            Quaternion.identity
        );

        BallProjectile ball = newBall.GetComponent<BallProjectile>();

        if (ball != null)
        {
            ball.SetSpawner(this);
            ball.SetDirection(shootRight);
            _currentBall = ball;
        }
    }

    // Keep API compatibility: called by a BallProjectile when it's destroyed.
    public void NotifyBallDestroyed(BallProjectile ball)
    {
        if (_currentBall == ball)
        {
            _currentBall = null;
        }
    }

    // Spawn balls on all spawners except the given one
    public static void SpawnAllExcept(BallSpawner exclude)
    {
        foreach (var sp in _allSpawners)
        {
            if (sp == null || sp == exclude) continue;
            // Clear any record of current ball and spawn immediately
            sp._currentBall = null;
            sp.SpawnBall();
        }
    }
    // Replace every spawner in the level with a fresh clone of itself.
    // This instantiates a copy of each spawner GameObject and destroys the original.
    public static void ReplaceAllSpawners()
    {
        // Work on a snapshot to avoid modifying the collection during iteration
        var spawners = _allSpawners.ToArray();

        foreach (var sp in spawners)
        {
            if (sp == null) continue;

            // Instantiate a clone at the same transform
            GameObject clone = Object.Instantiate(sp.gameObject, sp.transform.position, sp.transform.rotation);

            // If the clone contains any existing projectile children, remove them so it starts clean
            var childProjectiles = clone.GetComponentsInChildren<BallProjectile>(true);
            foreach (var p in childProjectiles)
            {
                if (p != null && p.gameObject != null)
                {
                    Object.Destroy(p.gameObject);
                }
            }

            // Destroy the original spawner GameObject
            Object.Destroy(sp.gameObject);
        }
    }

    // Replace this spawner with a new instance (clone) at the same position
    // and destroy the current spawner. The new spawner will initialize and
    // begin spawning as usual.
    // Replace this spawner with a new instance (clone) at the same position
    // and destroy the current spawner. Returns the new spawner component.
    public BallSpawner ReplaceSelf()
    {
        // Instantiate a clone of this spawner GameObject
        GameObject clone = Instantiate(this.gameObject, transform.position, transform.rotation);

        // Get the BallSpawner component on the clone
        BallSpawner newSpawner = clone.GetComponent<BallSpawner>();

        // Destroy the current spawner GameObject
        Destroy(this.gameObject);

        return newSpawner;
    }
    
}
