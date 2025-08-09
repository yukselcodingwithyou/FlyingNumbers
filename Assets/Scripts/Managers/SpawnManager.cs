using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Spawn Areas (World Space)")]
    public Vector2 spawnXRange = new Vector2(8f, 10f);
    public Vector2 spawnYRange = new Vector2(-3.5f, 3.5f);

    [Header("Powerups")]
    public List<GameObject> powerupPrefabs = new List<GameObject>();
    public float powerupSpawnInterval = 2.5f;

    [Header("Obstacles")]
    public List<GameObject> obstaclePrefabs = new List<GameObject>();
    public float obstacleSpawnInterval = 2.0f;

    [Header("Movement")]
    public Vector2 initialVelocity = new Vector2(-2.5f, 0f);
    public float randomYJitter = 0.75f;
    [Tooltip("Multiply initialVelocity by LevelManager game speed when spawning")]
    public bool scaleVelocityWithGameSpeed = true;

    private Coroutine powerupRoutine;
    private Coroutine obstacleRoutine;
    private bool isActive;
    private float currentPowerupInterval;
    private float currentObstacleInterval;

    void OnEnable()
    {
        // Initialize intervals from fields
        currentPowerupInterval = powerupSpawnInterval;
        currentObstacleInterval = obstacleSpawnInterval;

        // Subscribe to difficulty changes
        LevelManager.OnSpawnRateChanged += OnSpawnRateChanged;
        LevelManager.OnGameSpeedChanged += OnGameSpeedChanged;
        GameManager.OnGameStarted += OnGameStarted;
        GameManager.OnGameEnded += OnGameEnded;

        // Apply current level settings if available
        if (LevelManager.Instance != null)
        {
            OnSpawnRateChanged(LevelManager.Instance.CurrentSpawnRate);
        }

        StartSpawning();
    }

    void OnDisable()
    {
        StopSpawning();
        LevelManager.OnSpawnRateChanged -= OnSpawnRateChanged;
        LevelManager.OnGameSpeedChanged -= OnGameSpeedChanged;
        GameManager.OnGameStarted -= OnGameStarted;
        GameManager.OnGameEnded -= OnGameEnded;
    }

    public void StartSpawning()
    {
        isActive = true;
        if (powerupRoutine == null && powerupPrefabs.Count > 0)
            powerupRoutine = StartCoroutine(SpawnLoop(powerupPrefabs, isPowerup:true));

        if (obstacleRoutine == null && obstaclePrefabs.Count > 0)
            obstacleRoutine = StartCoroutine(SpawnLoop(obstaclePrefabs, isPowerup:false));
    }

    public void StopSpawning()
    {
        isActive = false;
        if (powerupRoutine != null) StopCoroutine(powerupRoutine);
        if (obstacleRoutine != null) StopCoroutine(obstacleRoutine);
        powerupRoutine = obstacleRoutine = null;
    }

    private IEnumerator SpawnLoop(List<GameObject> prefabs, bool isPowerup)
    {
        while (isActive)
        {
            SpawnRandom(prefabs);
            // Use current interval each iteration to reflect dynamic changes
            float interval = isPowerup ? currentPowerupInterval : currentObstacleInterval;
            if (interval <= 0f) interval = 0.1f;
            yield return new WaitForSeconds(interval);
        }
    }

    private void SpawnRandom(List<GameObject> prefabs)
    {
        if (prefabs == null || prefabs.Count == 0) return;
        var prefab = prefabs[Random.Range(0, prefabs.Count)];
        var x = Random.Range(spawnXRange.x, spawnXRange.y);
        var y = Mathf.Clamp(Random.Range(spawnYRange.x, spawnYRange.y) + Random.Range(-randomYJitter, randomYJitter), spawnYRange.x, spawnYRange.y);
        var go = Instantiate(prefab, new Vector3(x, y, 0f), Quaternion.identity);

        var rb = go.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = GetScaledVelocity();
        }
        else
        {
            // Simple drift if no Rigidbody2D is present
            rb = go.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.velocity = GetScaledVelocity();
        }
    }

    private Vector2 GetScaledVelocity()
    {
        if (!scaleVelocityWithGameSpeed || LevelManager.Instance == null)
            return initialVelocity;
        return initialVelocity * LevelManager.Instance.CurrentGameSpeed;
    }

    private void OnSpawnRateChanged(float newRate)
    {
        // Apply a simple mapping: obstacles at base rate, powerups a bit slower
        currentObstacleInterval = Mathf.Max(newRate, 0.1f);
        currentPowerupInterval = Mathf.Max(newRate * 1.25f, 0.15f);
    }

    private void OnGameSpeedChanged(float newSpeed)
    {
        // Velocity is recomputed on spawn via GetScaledVelocity; nothing to do here for existing entities
    }

    private void OnGameStarted()
    {
        StartSpawning();
    }

    private void OnGameEnded()
    {
        StopSpawning();
    }
}
