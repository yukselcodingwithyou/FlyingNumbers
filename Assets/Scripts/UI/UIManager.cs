using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Comprehensive UI manager handling all game UI elements and transitions.
/// Manages pause menu, settings, level up notifications, and game state UI.
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Game UI")]
    [SerializeField] private Text levelText;
    [SerializeField] private Text speedText;
    [SerializeField] private Slider shieldSlider;
    [SerializeField] private Text coinProgressText;
    
    [Header("Panels")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject levelUpPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject tutorialPanel;
    
    [Header("Level Up")]
    [SerializeField] private Text levelUpText;
    [SerializeField] private float levelUpDisplayTime = 2f;
    
    [Header("Settings")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Toggle fullscreenToggle;
    
    [Header("Pause")]
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button exitButton;
    
    private bool isPaused = false;
    private bool isGameOver = false;
    
    // Settings keys
    private const string MusicVolumeKey = "MusicVolume";
    private const string SFXVolumeKey = "SFXVolume";
    private const string FullscreenKey = "Fullscreen";
    private const string TutorialShownKey = "TutorialShown";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        InitializeUI();
        LoadSettings();
    }

    private void Start()
    {
        // Subscribe to events
        if (LevelManager.Instance != null)
        {
            LevelManager.OnLevelChanged += UpdateLevelDisplay;
            LevelManager.OnGameSpeedChanged += UpdateSpeedDisplay;
        }
        
        if (CoinManager.Instance != null)
        {
            CoinManager.OnCoinsChanged += UpdateCoinProgress;
        }
        
        PlayerController.OnShieldsChanged += UpdateShieldDisplay;
        PlayerController.OnPlayerDeath += OnPlayerDeath;
        
        // Show tutorial for first-time players
        ShowTutorialIfNeeded();
    }

    private void InitializeUI()
    {
        // Initialize all panels as inactive
        if (pausePanel != null) pausePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (levelUpPanel != null) levelUpPanel.SetActive(false);
        if (tutorialPanel != null) tutorialPanel.SetActive(false);
        
        // Setup button listeners
        if (pauseButton != null) pauseButton.onClick.AddListener(PauseGame);
        if (resumeButton != null) resumeButton.onClick.AddListener(ResumeGame);
        if (restartButton != null) restartButton.onClick.AddListener(RestartGame);
        if (settingsButton != null) settingsButton.onClick.AddListener(ShowSettings);
        if (exitButton != null) exitButton.onClick.AddListener(ExitGame);
        
        // Setup settings listeners
        if (musicVolumeSlider != null) musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        if (sfxVolumeSlider != null) sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        if (fullscreenToggle != null) fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
    }

    private void Update()
    {
        // Handle pause input
        if (Input.GetKeyDown(KeyCode.Escape) && !isGameOver)
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    #region Level and Game State UI

    public void UpdateLevelDisplay(int level)
    {
        if (levelText != null)
        {
            levelText.text = $"Level {level}";
        }
    }

    public void UpdateSpeedDisplay(float speed)
    {
        if (speedText != null)
        {
            speedText.text = $"Speed: {speed:F1}x";
        }
    }

    public void UpdateShieldDisplay(int shields)
    {
        if (shieldSlider != null)
        {
            shieldSlider.value = shields;
        }
    }

    public void UpdateCoinProgress(int coins)
    {
        if (coinProgressText != null)
        {
            coinProgressText.text = $"{coins}/3";
        }
    }

    public void ShowLevelUp(int level)
    {
        if (levelUpPanel != null && levelUpText != null)
        {
            levelUpText.text = $"Level {level}!";
            StartCoroutine(ShowLevelUpCoroutine());
        }
    }

    private IEnumerator ShowLevelUpCoroutine()
    {
        levelUpPanel.SetActive(true);
        
        // Animate the level up panel
        if (levelUpPanel.TryGetComponent<Animator>(out var animator))
        {
            animator.SetTrigger("Show");
        }
        
        yield return new WaitForSeconds(levelUpDisplayTime);
        
        levelUpPanel.SetActive(false);
    }

    #endregion

    #region Pause System

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        
        // Restore game speed from level manager
        if (LevelManager.Instance != null)
        {
            Time.timeScale = LevelManager.Instance.CurrentGameSpeed;
        }
        else
        {
            Time.timeScale = 1f;
        }
        
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        isGameOver = false;
        
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.Restart();
        }
    }

    #endregion

    #region Settings

    public void ShowSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    public void HideSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    private void LoadSettings()
    {
        float musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        float sfxVolume = PlayerPrefs.GetFloat(SFXVolumeKey, 1f);
        bool fullscreen = PlayerPrefs.GetInt(FullscreenKey, 1) == 1;

        if (musicVolumeSlider != null) musicVolumeSlider.value = musicVolume;
        if (sfxVolumeSlider != null) sfxVolumeSlider.value = sfxVolume;
        if (fullscreenToggle != null) fullscreenToggle.isOn = fullscreen;

        // Apply settings
        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);
        SetFullscreen(fullscreen);
    }

    public void SetMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat(MusicVolumeKey, volume);
        // Apply to audio system when available
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(volume);
        }
    }

    public void SetSFXVolume(float volume)
    {
        PlayerPrefs.SetFloat(SFXVolumeKey, volume);
        // Apply to audio system when available
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(volume);
        }
    }

    public void SetFullscreen(bool fullscreen)
    {
        PlayerPrefs.SetInt(FullscreenKey, fullscreen ? 1 : 0);
        Screen.fullScreen = fullscreen;
    }

    #endregion

    #region Tutorial

    private void ShowTutorialIfNeeded()
    {
        bool tutorialShown = PlayerPrefs.GetInt(TutorialShownKey, 0) == 1;
        
        if (!tutorialShown && tutorialPanel != null)
        {
            tutorialPanel.SetActive(true);
            PlayerPrefs.SetInt(TutorialShownKey, 1);
        }
    }

    public void CloseTutorial()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }
    }

    #endregion

    #region Game Over

    private void OnPlayerDeath()
    {
        isGameOver = true;
    }

    #endregion

    #region Utility

    public void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (LevelManager.Instance != null)
        {
            LevelManager.OnLevelChanged -= UpdateLevelDisplay;
            LevelManager.OnGameSpeedChanged -= UpdateSpeedDisplay;
        }
        
        if (CoinManager.Instance != null)
        {
            CoinManager.OnCoinsChanged -= UpdateCoinProgress;
        }
        
        PlayerController.OnShieldsChanged -= UpdateShieldDisplay;
        PlayerController.OnPlayerDeath -= OnPlayerDeath;
    }

    #endregion
}