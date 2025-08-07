using UnityEngine;
using System.Collections;

/// <summary>
/// Main game manager that coordinates all game systems and handles game state.
/// Manages initialization, game flow, and communication between different managers.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    [SerializeField] private bool autoStartGame = true;
    [SerializeField] private float gameStartDelay = 1f;

    [Header("Managers")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform playerSpawnPoint;

    public enum GameState
    {
        Menu,
        Playing,
        Paused,
        GameOver,
        Tutorial
    }

    private GameState currentState = GameState.Menu;
    private PlayerController currentPlayer;

    // Events
    public static System.Action<GameState> OnGameStateChanged;
    public static System.Action OnGameStarted;
    public static System.Action OnGameEnded;

    public GameState CurrentState => currentState;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeGame();
    }

    private void Start()
    {
        if (autoStartGame)
        {
            StartCoroutine(AutoStartGameCoroutine());
        }
    }

    private void InitializeGame()
    {
        // Ensure all managers are initialized
        EnsureManagersExist();

        // Subscribe to player death
        PlayerController.OnPlayerDeath += OnPlayerDeath;

        Debug.Log("Game Manager initialized");
    }

    private void EnsureManagersExist()
    {
        // Check for essential managers and create them if they don't exist
        if (ScoreManager.Instance == null)
        {
            GameObject scoreManager = new GameObject("ScoreManager");
            scoreManager.AddComponent<ScoreManager>();
        }

        if (CoinManager.Instance == null)
        {
            GameObject coinManager = new GameObject("CoinManager");
            coinManager.AddComponent<CoinManager>();
        }

        if (LevelManager.Instance == null)
        {
            GameObject levelManager = new GameObject("LevelManager");
            levelManager.AddComponent<LevelManager>();
        }

        if (UIManager.Instance == null)
        {
            GameObject uiManager = new GameObject("UIManager");
            uiManager.AddComponent<UIManager>();
        }

        if (AudioManager.Instance == null)
        {
            GameObject audioManager = new GameObject("AudioManager");
            audioManager.AddComponent<AudioManager>();
        }
    }

    private IEnumerator AutoStartGameCoroutine()
    {
        yield return new WaitForSeconds(gameStartDelay);
        StartGame();
    }

    public void StartGame()
    {
        if (currentState == GameState.Playing)
            return;

        ChangeGameState(GameState.Playing);

        // Reset all systems
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.ResetScore();

        if (CoinManager.Instance != null)
            CoinManager.Instance.ResetCurrentCoins();

        if (LevelManager.Instance != null)
            LevelManager.Instance.ResetLevel();

        // Spawn player if needed
        SpawnPlayer();

        // Start gameplay audio
        if (AudioManager.Instance != null)
            AudioManager.Instance.StartGameplayAudio();

        OnGameStarted?.Invoke();
        Debug.Log("Game Started");
    }

    public void PauseGame()
    {
        if (currentState == GameState.Playing)
        {
            ChangeGameState(GameState.Paused);
            
            if (AudioManager.Instance != null)
                AudioManager.Instance.PauseAudio();
        }
    }

    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            ChangeGameState(GameState.Playing);
            
            if (AudioManager.Instance != null)
                AudioManager.Instance.ResumeAudio();
        }
    }

    public void EndGame()
    {
        if (currentState == GameState.GameOver)
            return;

        ChangeGameState(GameState.GameOver);
        OnGameEnded?.Invoke();
        
        Debug.Log("Game Ended");
    }

    public void RestartGame()
    {
        // Reset time scale
        Time.timeScale = 1f;
        
        StartGame();
    }

    public void ReturnToMenu()
    {
        ChangeGameState(GameState.Menu);
        
        // Reset time scale
        Time.timeScale = 1f;
        
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMenuMusic();
    }

    private void SpawnPlayer()
    {
        if (playerPrefab != null && playerSpawnPoint != null)
        {
            if (currentPlayer != null)
            {
                Destroy(currentPlayer.gameObject);
            }

            GameObject playerObj = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
            currentPlayer = playerObj.GetComponent<PlayerController>();
        }
        else
        {
            // Find existing player in scene
            currentPlayer = FindObjectOfType<PlayerController>();
        }
    }

    private void OnPlayerDeath()
    {
        StartCoroutine(HandlePlayerDeathCoroutine());
    }

    private IEnumerator HandlePlayerDeathCoroutine()
    {
        // Small delay before ending game
        yield return new WaitForSeconds(1f);
        EndGame();
    }

    private void ChangeGameState(GameState newState)
    {
        if (currentState == newState)
            return;

        GameState previousState = currentState;
        currentState = newState;

        OnGameStateChanged?.Invoke(currentState);
        Debug.Log($"Game State changed from {previousState} to {currentState}");
    }

    // Public getters for other systems
    public PlayerController GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public bool IsGamePlaying()
    {
        return currentState == GameState.Playing;
    }

    public bool IsGamePaused()
    {
        return currentState == GameState.Paused;
    }

    public bool IsGameOver()
    {
        return currentState == GameState.GameOver;
    }

    private void OnDestroy()
    {
        PlayerController.OnPlayerDeath -= OnPlayerDeath;
    }

    // Debug methods for testing
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void DebugAddScore(int amount)
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.AddScore(amount);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void DebugAddCoins(int amount)
    {
        if (CoinManager.Instance != null)
            CoinManager.Instance.AddCoins(amount);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void DebugAdvanceLevel()
    {
        if (LevelManager.Instance != null)
        {
            // Force level advance by adding score
            DebugAddScore(10);
        }
    }
}