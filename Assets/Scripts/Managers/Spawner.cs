using UnityEngine;

/// <summary>
/// Enhanced spawner that dynamically spawns pipes, operators, collectibles, and power-ups.
/// Integrates with LevelManager for difficulty scaling and spawn rate adjustments.
/// Attach this to an empty GameObject positioned at the x spawn location.
/// </summary>
public class Spawner : MonoBehaviour
{
    [Header("Pipe Spawning")]
    [Tooltip("Prefab containing the pair of pipes with a gap")]
    public GameObject pipePrefab;
    [Tooltip("Minimum Y position for the gap center")] public float minY = -1f;
    [Tooltip("Maximum Y position for the gap center")] public float maxY = 1f;

    [Header("Obstacle Spawning")]
    [Tooltip("Mine prefab that moves up/down")]
    public GameObject minePrefab;
    [Tooltip("Chance to spawn a mine instead of pipes")] 
    public float mineSpawnChance = 0.3f;

    [Header("Operator Objects")]
    [Tooltip("Possible operator prefabs to spawn inside the gap")]
    public GameObject[] operatorPrefabs;
    [Tooltip("Special ×0 operator prefab for dangerous situations")]
    public GameObject dangerousOperatorPrefab;
    [Tooltip("Vertical offset of the operator from the gap center")]
    public float operatorYOffset = 0f;

    [Header("Collectibles")]
    [Tooltip("Coin prefab")]
    public GameObject coinPrefab;
    [Tooltip("Shield prefab")]
    public GameObject shieldPrefab;
    [Tooltip("Number of coins to spawn in a cluster")]
    public int coinsPerCluster = 3;
    [Tooltip("Spacing between coins in cluster")]
    public float coinSpacing = 1f;

    [Header("Power-ups")]
    [Tooltip("Available power-up prefabs")]
    public GameObject[] powerUpPrefabs;
    [Tooltip("Base chance to spawn power-ups")]
    public float powerUpSpawnChance = 0.1f;

    [Header("Spawn Timing")]
    [Tooltip("Base time between spawns")]
    public float baseSpawnInterval = 2f;

    private float currentSpawnInterval;
    private float lastSpawnTime;

    private void Start()
    {
        currentSpawnInterval = baseSpawnInterval;
        lastSpawnTime = Time.time;

        // Subscribe to level manager events
        if (LevelManager.Instance != null)
        {
            LevelManager.OnSpawnRateChanged += UpdateSpawnRate;
        }
    }

    private void Update()
    {
        if (Time.time - lastSpawnTime >= currentSpawnInterval)
        {
            SpawnNext();
            lastSpawnTime = Time.time;
        }
    }

    private void UpdateSpawnRate(float newSpawnRate)
    {
        currentSpawnInterval = newSpawnRate;
    }

    private void SpawnNext()
    {
        // Determine what to spawn based on level and chance
        bool shouldSpawnCollectible = LevelManager.Instance != null && LevelManager.Instance.ShouldSpawnCollectible();
        bool shouldSpawnDangerous = LevelManager.Instance != null && LevelManager.Instance.ShouldSpawnDangerousOperator();

        if (shouldSpawnCollectible && Random.value < 0.7f) // 70% chance for collectibles when allowed
        {
            SpawnCollectible();
        }
        else if (Random.value < mineSpawnChance)
        {
            SpawnMine();
        }
        else
        {
            SpawnPipeWithOperator(shouldSpawnDangerous);
        }

        // Small chance for power-ups
        if (Random.value < powerUpSpawnChance)
        {
            SpawnPowerUp();
        }
    }

    private void SpawnPipeWithOperator(bool forceDangerous = false)
    {
        if (pipePrefab == null) return;

        float gapY = Random.Range(minY, maxY);
        Vector3 spawnPos = new Vector3(transform.position.x, gapY, 0f);
        Instantiate(pipePrefab, spawnPos, Quaternion.identity);

        // Spawn operator in the gap
        SpawnOperator(gapY, forceDangerous);
    }

    private void SpawnOperator(float gapY, bool forceDangerous = false)
    {
        GameObject operatorToSpawn = null;

        if (forceDangerous && dangerousOperatorPrefab != null)
        {
            operatorToSpawn = dangerousOperatorPrefab;
        }
        else if (operatorPrefabs != null && operatorPrefabs.Length > 0)
        {
            operatorToSpawn = operatorPrefabs[Random.Range(0, operatorPrefabs.Length)];
        }

        if (operatorToSpawn != null)
        {
            Vector3 opPos = new Vector3(transform.position.x, gapY + operatorYOffset, 0f);
            Instantiate(operatorToSpawn, opPos, Quaternion.identity);
        }
    }

    private void SpawnMine()
    {
        if (minePrefab == null) return;

        float mineY = Random.Range(minY, maxY);
        Vector3 spawnPos = new Vector3(transform.position.x, mineY, 0f);
        Instantiate(minePrefab, spawnPos, Quaternion.identity);
    }

    private void SpawnCollectible()
    {
        // 60% chance for coins, 40% chance for shields
        if (Random.value < 0.6f)
        {
            SpawnCoinCluster();
        }
        else
        {
            SpawnShield();
        }
    }

    private void SpawnCoinCluster()
    {
        if (coinPrefab == null) return;

        float centerY = Random.Range(minY, maxY);
        
        for (int i = 0; i < coinsPerCluster; i++)
        {
            float coinY = centerY + (i - coinsPerCluster / 2f) * coinSpacing;
            Vector3 coinPos = new Vector3(transform.position.x + i * 0.5f, coinY, 0f);
            Instantiate(coinPrefab, coinPos, Quaternion.identity);
        }
    }

    private void SpawnShield()
    {
        if (shieldPrefab == null) return;

        float shieldY = Random.Range(minY, maxY);
        Vector3 spawnPos = new Vector3(transform.position.x, shieldY, 0f);
        Instantiate(shieldPrefab, spawnPos, Quaternion.identity);
    }

    private void SpawnPowerUp()
    {
        if (powerUpPrefabs == null || powerUpPrefabs.Length == 0) return;

        GameObject powerUpToSpawn = powerUpPrefabs[Random.Range(0, powerUpPrefabs.Length)];
        if (powerUpToSpawn != null)
        {
            float powerUpY = Random.Range(minY, maxY);
            Vector3 spawnPos = new Vector3(transform.position.x, powerUpY, 0f);
            Instantiate(powerUpToSpawn, spawnPos, Quaternion.identity);
        }
    }

    private void OnDestroy()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.OnSpawnRateChanged -= UpdateSpawnRate;
        }
    }
}
