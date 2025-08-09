using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Binds scene UI elements to the singleton ScoreManager at runtime.
/// Attach to any object in the Game scene and assign UI references in inspector or via scaffolder.
/// </summary>
public class ScoreUIBinder : MonoBehaviour
{
    [Header("In-Game")]
    public Text scoreText;

    [Header("Game Over Panel")]
    public GameObject gameOverPanel;
    public Text gameOverScoreText;
    public Text gameOverHighScoreText;

    private void Start()
    {
        if (ScoreManager.Instance == null) return;

        if (scoreText != null) ScoreManager.Instance.scoreText = scoreText;
        if (gameOverPanel != null) ScoreManager.Instance.gameOverPanel = gameOverPanel;
        if (gameOverScoreText != null) ScoreManager.Instance.gameOverScoreText = gameOverScoreText;
        if (gameOverHighScoreText != null) ScoreManager.Instance.gameOverHighScoreText = gameOverHighScoreText;

        // Ensure UI reflects current values
        // Trigger a tiny update
        ScoreManager.Instance.AddScore(0);
        if (ScoreManager.Instance.gameOverPanel != null)
            ScoreManager.Instance.gameOverPanel.SetActive(false);
    }
}
