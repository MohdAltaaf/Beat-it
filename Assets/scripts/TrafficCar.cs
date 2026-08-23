using UnityEngine;

public class TrafficCar : MonoBehaviour
{
    private BikeController bike;
    private float speed, spawnZ, x, y, despawnTime;

    public void Init(float carSpeed, float startZ, float xPos, float yPos, float despawnAt)
    {
        speed = carSpeed; spawnZ = startZ; x = xPos; y = yPos; despawnTime = despawnAt;
    }

    void Start()
    {
        bike = FindAnyObjectByType<BikeController>();
    }

    void Update()
    {
        if (bike == null) return;

        if (bike.SmoothedElapsed >= despawnTime)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = new Vector3(x, y, spawnZ + speed * bike.SmoothedElapsed);
    }

   public GameObject hitParticlePrefab;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Bike")) return;

        if (hitParticlePrefab != null)
            Instantiate(hitParticlePrefab, transform.position, Quaternion.identity);

        FindAnyObjectByType<GameState>()?.RegisterCarHit();
        FindAnyObjectByType<ScoreManager>()?.BreakStreak();
        Destroy(gameObject); // poof - gone before the bike could visibly pass through it
    }
}