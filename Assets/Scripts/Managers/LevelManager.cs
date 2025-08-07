using UnityEngine;

/// <summary>
/// Manages game level progression, difficulty scaling, and dynamic game speed.
/// Increases difficulty based on score and time as specified in the README.
/// </summary>
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [Header("Level Progression")]
    [SerializeField] private float baseGameSpeed = 1f;
    [SerializeField] private float speedIncreasePerLevel = 0.2f;
    [SerializeField] private float maxGameSpeed = 3f;
    [SerializeField] private int scorePerLevel = 10; // Score needed to advance level
    
    [Header("Spawn Rate Changes")]
    [SerializeField] private float baseSpawnRate = 2f;
    [SerializeField] private float spawnRateIncrease = 0.1f; // Faster spawning per level
    [SerializeField] private float minSpawnRate = 0.8f;
    
    [Header("Difficulty Scaling")]
    [SerializeField] private float baseDangerChance = 0.1f; // Chance of ×0 operators
    [SerializeField] private float dangerChanceIncrease = 0.05f; // Increase per level
    [SerializeField] private float maxDangerChance = 0.4f;
    
    [SerializeField] private float baseCollectibleChance = 0.8f; // Chance of coins/shields
    [SerializeField] private float collectibleChanceDecrease = 0.05f; // Decrease per level
    [SerializeField] private float minCollectibleChance = 0.3f;

    // Events for other systems
    public static System.Action<int> OnLevelChanged;
    public static System.Action<float> OnGameSpeedChanged;
    public static System.Action<float> OnSpawnRateChanged;

    private int currentLevel = 1;
    private float currentGameSpeed;
    private float currentSpawnRate;
    private float gameStartTime;
    private int lastLevelScore = 0;

    public int CurrentLevel => currentLevel;
    public float CurrentGameSpeed => currentGameSpeed;
    public float CurrentSpawnRate => currentSpawnRate;
    public float DangerChance => Mathf.Min(baseDangerChance + (currentLevel - 1) * dangerChanceIncrease, maxDangerChance);
    public float CollectibleChance => Mathf.Max(baseCollectibleChance - (currentLevel - 1) * collectibleChanceDecrease, minCollectibleChance);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        InitializeLevel();
    }

    private void Start()
    {
        gameStartTime = Time.time;
        
        // Subscribe to score changes
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreUpdated += CheckLevelProgression;
        }
    }

    private void InitializeLevel()
    {
        currentLevel = 1;
        UpdateGameSpeed();
        UpdateSpawnRate();
        
        OnLevelChanged?.Invoke(currentLevel);
        OnGameSpeedChanged?.Invoke(currentGameSpeed);
        OnSpawnRateChanged?.Invoke(currentSpawnRate);
    }

    /// <summary>
    /// Checks if the player should advance to the next level
    /// </summary>
    private void CheckLevelProgression(int currentScore)
    {
        int targetScore = lastLevelScore + scorePerLevel;
        
        if (currentScore >= targetScore)
        {
            AdvanceLevel();
            lastLevelScore = targetScore;
        }
    }

    /// <summary>
    /// Advances to the next level and updates difficulty
    /// </summary>
    private void AdvanceLevel()
    {
        currentLevel++;
        
        UpdateGameSpeed();
        UpdateSpawnRate();
        
        // Notify other systems
        OnLevelChanged?.Invoke(currentLevel);
        OnGameSpeedChanged?.Invoke(currentGameSpeed);
        OnSpawnRateChanged?.Invoke(currentSpawnRate);
        
        // Visual feedback for level up
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowLevelUp(currentLevel);
        }
        
        Debug.Log($"Level Up! Now at Level {currentLevel}");
    }

    /// <summary>
    /// Updates the game speed based on current level
    /// </summary>
    private void UpdateGameSpeed()
    {
        currentGameSpeed = Mathf.Min(baseGameSpeed + (currentLevel - 1) * speedIncreasePerLevel, maxGameSpeed);
        
        // Apply time scale for global speed increase
        Time.timeScale = currentGameSpeed;
    }

    /// <summary>
    /// Updates spawn rates for obstacles and collectibles
    /// </summary>
    private void UpdateSpawnRate()
    {
        currentSpawnRate = Mathf.Max(baseSpawnRate - (currentLevel - 1) * spawnRateIncrease, minSpawnRate);
    }

    /// <summary>
    /// Resets level for new game
    /// </summary>
    public void ResetLevel()
    {
        Time.timeScale = 1f; // Reset time scale
        lastLevelScore = 0;
        gameStartTime = Time.time;
        InitializeLevel();
    }

    /// <summary>
    /// Gets the current difficulty multiplier (for other systems)
    /// </summary>
    public float GetDifficultyMultiplier()
    {
        return 1f + (currentLevel - 1) * 0.1f;
    }

    /// <summary>
    /// Determines if a dangerous operator (×0) should spawn
    /// </summary>
    public bool ShouldSpawnDangerousOperator()
    {
        return Random.value < DangerChance;
    }

    /// <summary>
    /// Determines if a collectible should spawn
    /// </summary>
    public bool ShouldSpawnCollectible()
    {
        return Random.value < CollectibleChance;
    }

    private void OnDestroy()
    {
        // Ensure time scale is reset
        Time.timeScale = 1f;
    }
}