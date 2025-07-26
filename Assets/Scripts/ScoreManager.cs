using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Tracks the player's score and high score using PlayerPrefs and updates the UI.
/// Also exposes a restart method for the Game Over UI button.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [Header("UI References")]
    [Tooltip("Text displaying the current score during play")]
    public Text scoreText;
    [Tooltip("Game over panel shown when the player loses")] public GameObject gameOverPanel;
    [Tooltip("Text displaying the final score in the game over panel")]
    public Text gameOverScoreText;
    [Tooltip("Text displaying the high score in the game over panel")]
    public Text gameOverHighScoreText;

    private int score;
    private int highScore;

    private const string HighScoreKey = "HighScore";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        UpdateScoreUI();
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    /// <summary>
    /// Adds to the current score and updates the persistent high score if needed.
    /// </summary>
    public void AddScore(int amount)
    {
        score += amount;
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt(HighScoreKey, highScore);
        }
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = score.ToString();
        if (gameOverScoreText != null)
            gameOverScoreText.text = $"Score: {score}";
        if (gameOverHighScoreText != null)
            gameOverHighScoreText.text = $"Best: {highScore}";
    }

    /// <summary>
    /// Displays the game over panel.
    /// </summary>
    public void ShowGameOver()
    {
        UpdateScoreUI();
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    /// <summary>
    /// Resets the score and hides the game over panel.
    /// Call this when restarting the scene.
    /// </summary>
    public void ResetScore()
    {
        score = 0;
        UpdateScoreUI();
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    /// <summary>
    /// Reloads the current scene. Hook this to the restart button.
    /// </summary>
    public void Restart()
    {
        ResetScore();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
