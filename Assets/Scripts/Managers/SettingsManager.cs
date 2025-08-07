using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Comprehensive settings manager handling all game preferences and configurations.
/// Manages audio, graphics, controls, language, and accessibility settings.
/// </summary>
public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    [Header("Audio Settings")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Toggle musicToggle;
    [SerializeField] private Toggle sfxToggle;

    [Header("Graphics Settings")]
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Dropdown qualityDropdown;
    [SerializeField] private Dropdown resolutionDropdown;
    [SerializeField] private Toggle vsyncToggle;

    [Header("Gameplay Settings")]
    [SerializeField] private Dropdown difficultyDropdown;
    [SerializeField] private Toggle tutorialToggle;
    [SerializeField] private Toggle hintsToggle;
    [SerializeField] private Slider sensitivitySlider;

    [Header("Accessibility")]
    [SerializeField] private Toggle colorBlindModeToggle;
    [SerializeField] private Toggle reducedMotionToggle;
    [SerializeField] private Dropdown languageDropdown;
    [SerializeField] private Slider textSizeSlider;

    [Header("Privacy")]
    [SerializeField] private Toggle analyticsToggle;
    [SerializeField] private Toggle crashReportingToggle;

    // Events
    public static System.Action OnSettingsChanged;
    public static System.Action<GameSettings> OnSettingsApplied;

    private GameSettings currentSettings;

    // PlayerPrefs keys
    private const string MusicVolumeKey = "MusicVolume";
    private const string SFXVolumeKey = "SFXVolume";
    private const string MusicEnabledKey = "MusicEnabled";
    private const string SFXEnabledKey = "SFXEnabled";
    private const string FullscreenKey = "Fullscreen";
    private const string QualityKey = "Quality";
    private const string VSyncKey = "VSync";
    private const string DifficultyKey = "Difficulty";
    private const string TutorialEnabledKey = "TutorialEnabled";
    private const string HintsEnabledKey = "HintsEnabled";
    private const string SensitivityKey = "Sensitivity";
    private const string ColorBlindModeKey = "ColorBlindMode";
    private const string ReducedMotionKey = "ReducedMotion";
    private const string LanguageKey = "Language";
    private const string TextSizeKey = "TextSize";
    private const string AnalyticsKey = "Analytics";
    private const string CrashReportingKey = "CrashReporting";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeSettings();
    }

    private void Start()
    {
        SetupUIListeners();
        LoadSettings();
        ApplySettings();
    }

    private void InitializeSettings()
    {
        currentSettings = new GameSettings();
        LoadDefaultSettings();
    }

    private void LoadDefaultSettings()
    {
        currentSettings.musicVolume = 1f;
        currentSettings.sfxVolume = 1f;
        currentSettings.musicEnabled = true;
        currentSettings.sfxEnabled = true;
        currentSettings.fullscreen = true;
        currentSettings.qualityLevel = QualitySettings.GetQualityLevel();
        currentSettings.vsync = true;
        currentSettings.difficulty = GameDifficulty.Normal;
        currentSettings.tutorialEnabled = true;
        currentSettings.hintsEnabled = true;
        currentSettings.sensitivity = 1f;
        currentSettings.colorBlindMode = false;
        currentSettings.reducedMotion = false;
        currentSettings.language = "English";
        currentSettings.textSize = 1f;
        currentSettings.analytics = true;
        currentSettings.crashReporting = true;
    }

    private void SetupUIListeners()
    {
        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        
        if (musicToggle != null)
            musicToggle.onValueChanged.AddListener(SetMusicEnabled);
        
        if (sfxToggle != null)
            sfxToggle.onValueChanged.AddListener(SetSFXEnabled);
        
        if (fullscreenToggle != null)
            fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        
        if (qualityDropdown != null)
            qualityDropdown.onValueChanged.AddListener(SetQuality);
        
        if (vsyncToggle != null)
            vsyncToggle.onValueChanged.AddListener(SetVSync);
        
        if (difficultyDropdown != null)
            difficultyDropdown.onValueChanged.AddListener(SetDifficulty);
        
        if (tutorialToggle != null)
            tutorialToggle.onValueChanged.AddListener(SetTutorialEnabled);
        
        if (hintsToggle != null)
            hintsToggle.onValueChanged.AddListener(SetHintsEnabled);
        
        if (sensitivitySlider != null)
            sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
        
        if (colorBlindModeToggle != null)
            colorBlindModeToggle.onValueChanged.AddListener(SetColorBlindMode);
        
        if (reducedMotionToggle != null)
            reducedMotionToggle.onValueChanged.AddListener(SetReducedMotion);
        
        if (languageDropdown != null)
            languageDropdown.onValueChanged.AddListener(SetLanguage);
        
        if (textSizeSlider != null)
            textSizeSlider.onValueChanged.AddListener(SetTextSize);
        
        if (analyticsToggle != null)
            analyticsToggle.onValueChanged.AddListener(SetAnalytics);
        
        if (crashReportingToggle != null)
            crashReportingToggle.onValueChanged.AddListener(SetCrashReporting);
    }

    #region Load/Save Settings

    public void LoadSettings()
    {
        currentSettings.musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, currentSettings.musicVolume);
        currentSettings.sfxVolume = PlayerPrefs.GetFloat(SFXVolumeKey, currentSettings.sfxVolume);
        currentSettings.musicEnabled = PlayerPrefs.GetInt(MusicEnabledKey, currentSettings.musicEnabled ? 1 : 0) == 1;
        currentSettings.sfxEnabled = PlayerPrefs.GetInt(SFXEnabledKey, currentSettings.sfxEnabled ? 1 : 0) == 1;
        currentSettings.fullscreen = PlayerPrefs.GetInt(FullscreenKey, currentSettings.fullscreen ? 1 : 0) == 1;
        currentSettings.qualityLevel = PlayerPrefs.GetInt(QualityKey, currentSettings.qualityLevel);
        currentSettings.vsync = PlayerPrefs.GetInt(VSyncKey, currentSettings.vsync ? 1 : 0) == 1;
        currentSettings.difficulty = (GameDifficulty)PlayerPrefs.GetInt(DifficultyKey, (int)currentSettings.difficulty);
        currentSettings.tutorialEnabled = PlayerPrefs.GetInt(TutorialEnabledKey, currentSettings.tutorialEnabled ? 1 : 0) == 1;
        currentSettings.hintsEnabled = PlayerPrefs.GetInt(HintsEnabledKey, currentSettings.hintsEnabled ? 1 : 0) == 1;
        currentSettings.sensitivity = PlayerPrefs.GetFloat(SensitivityKey, currentSettings.sensitivity);
        currentSettings.colorBlindMode = PlayerPrefs.GetInt(ColorBlindModeKey, currentSettings.colorBlindMode ? 1 : 0) == 1;
        currentSettings.reducedMotion = PlayerPrefs.GetInt(ReducedMotionKey, currentSettings.reducedMotion ? 1 : 0) == 1;
        currentSettings.language = PlayerPrefs.GetString(LanguageKey, currentSettings.language);
        currentSettings.textSize = PlayerPrefs.GetFloat(TextSizeKey, currentSettings.textSize);
        currentSettings.analytics = PlayerPrefs.GetInt(AnalyticsKey, currentSettings.analytics ? 1 : 0) == 1;
        currentSettings.crashReporting = PlayerPrefs.GetInt(CrashReportingKey, currentSettings.crashReporting ? 1 : 0) == 1;

        UpdateUI();
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetFloat(MusicVolumeKey, currentSettings.musicVolume);
        PlayerPrefs.SetFloat(SFXVolumeKey, currentSettings.sfxVolume);
        PlayerPrefs.SetInt(MusicEnabledKey, currentSettings.musicEnabled ? 1 : 0);
        PlayerPrefs.SetInt(SFXEnabledKey, currentSettings.sfxEnabled ? 1 : 0);
        PlayerPrefs.SetInt(FullscreenKey, currentSettings.fullscreen ? 1 : 0);
        PlayerPrefs.SetInt(QualityKey, currentSettings.qualityLevel);
        PlayerPrefs.SetInt(VSyncKey, currentSettings.vsync ? 1 : 0);
        PlayerPrefs.SetInt(DifficultyKey, (int)currentSettings.difficulty);
        PlayerPrefs.SetInt(TutorialEnabledKey, currentSettings.tutorialEnabled ? 1 : 0);
        PlayerPrefs.SetInt(HintsEnabledKey, currentSettings.hintsEnabled ? 1 : 0);
        PlayerPrefs.SetFloat(SensitivityKey, currentSettings.sensitivity);
        PlayerPrefs.SetInt(ColorBlindModeKey, currentSettings.colorBlindMode ? 1 : 0);
        PlayerPrefs.SetInt(ReducedMotionKey, currentSettings.reducedMotion ? 1 : 0);
        PlayerPrefs.SetString(LanguageKey, currentSettings.language);
        PlayerPrefs.SetFloat(TextSizeKey, currentSettings.textSize);
        PlayerPrefs.SetInt(AnalyticsKey, currentSettings.analytics ? 1 : 0);
        PlayerPrefs.SetInt(CrashReportingKey, currentSettings.crashReporting ? 1 : 0);

        PlayerPrefs.Save();
    }

    #endregion

    #region Setting Setters

    public void SetMusicVolume(float volume)
    {
        currentSettings.musicVolume = Mathf.Clamp01(volume);
        SaveSettings();
        ApplyAudioSettings();
    }

    public void SetSFXVolume(float volume)
    {
        currentSettings.sfxVolume = Mathf.Clamp01(volume);
        SaveSettings();
        ApplyAudioSettings();
    }

    public void SetMusicEnabled(bool enabled)
    {
        currentSettings.musicEnabled = enabled;
        SaveSettings();
        ApplyAudioSettings();
    }

    public void SetSFXEnabled(bool enabled)
    {
        currentSettings.sfxEnabled = enabled;
        SaveSettings();
        ApplyAudioSettings();
    }

    public void SetFullscreen(bool fullscreen)
    {
        currentSettings.fullscreen = fullscreen;
        SaveSettings();
        ApplyGraphicsSettings();
    }

    public void SetQuality(int qualityIndex)
    {
        currentSettings.qualityLevel = qualityIndex;
        SaveSettings();
        ApplyGraphicsSettings();
    }

    public void SetVSync(bool vsync)
    {
        currentSettings.vsync = vsync;
        SaveSettings();
        ApplyGraphicsSettings();
    }

    public void SetDifficulty(int difficultyIndex)
    {
        currentSettings.difficulty = (GameDifficulty)difficultyIndex;
        SaveSettings();
        OnSettingsChanged?.Invoke();
    }

    public void SetTutorialEnabled(bool enabled)
    {
        currentSettings.tutorialEnabled = enabled;
        SaveSettings();
        OnSettingsChanged?.Invoke();
    }

    public void SetHintsEnabled(bool enabled)
    {
        currentSettings.hintsEnabled = enabled;
        SaveSettings();
        OnSettingsChanged?.Invoke();
    }

    public void SetSensitivity(float sensitivity)
    {
        currentSettings.sensitivity = sensitivity;
        SaveSettings();
        OnSettingsChanged?.Invoke();
    }

    public void SetColorBlindMode(bool enabled)
    {
        currentSettings.colorBlindMode = enabled;
        SaveSettings();
        ApplyAccessibilitySettings();
    }

    public void SetReducedMotion(bool enabled)
    {
        currentSettings.reducedMotion = enabled;
        SaveSettings();
        ApplyAccessibilitySettings();
    }

    public void SetLanguage(int languageIndex)
    {
        string[] languages = { "English", "Spanish", "French", "German", "Italian", "Portuguese", "Japanese", "Korean", "Chinese" };
        if (languageIndex >= 0 && languageIndex < languages.Length)
        {
            currentSettings.language = languages[languageIndex];
            SaveSettings();
            ApplyLanguageSettings();
        }
    }

    public void SetTextSize(float size)
    {
        currentSettings.textSize = Mathf.Clamp(size, 0.5f, 2f);
        SaveSettings();
        ApplyAccessibilitySettings();
    }

    public void SetAnalytics(bool enabled)
    {
        currentSettings.analytics = enabled;
        SaveSettings();
        OnSettingsChanged?.Invoke();
    }

    public void SetCrashReporting(bool enabled)
    {
        currentSettings.crashReporting = enabled;
        SaveSettings();
        OnSettingsChanged?.Invoke();
    }

    #endregion

    #region Apply Settings

    public void ApplySettings()
    {
        ApplyAudioSettings();
        ApplyGraphicsSettings();
        ApplyAccessibilitySettings();
        ApplyLanguageSettings();
        OnSettingsApplied?.Invoke(currentSettings);
    }

    private void ApplyAudioSettings()
    {
        if (AudioManager.Instance != null)
        {
            float musicVolume = currentSettings.musicEnabled ? currentSettings.musicVolume : 0f;
            float sfxVolume = currentSettings.sfxEnabled ? currentSettings.sfxVolume : 0f;
            
            AudioManager.Instance.SetMusicVolume(musicVolume);
            AudioManager.Instance.SetSFXVolume(sfxVolume);
        }
    }

    private void ApplyGraphicsSettings()
    {
        Screen.fullScreen = currentSettings.fullscreen;
        QualitySettings.SetQualityLevel(currentSettings.qualityLevel);
        QualitySettings.vSyncCount = currentSettings.vsync ? 1 : 0;
    }

    private void ApplyAccessibilitySettings()
    {
        // Apply color blind mode
        if (currentSettings.colorBlindMode)
        {
            // Would apply color blind friendly palette
        }

        // Apply reduced motion
        if (currentSettings.reducedMotion)
        {
            // Would reduce or disable animations
        }

        // Apply text size scaling
        // Would scale UI text elements based on textSize multiplier
    }

    private void ApplyLanguageSettings()
    {
        // Would apply localization based on selected language
        Debug.Log($"Language set to: {currentSettings.language}");
    }

    #endregion

    #region UI Update

    private void UpdateUI()
    {
        if (musicVolumeSlider != null) musicVolumeSlider.value = currentSettings.musicVolume;
        if (sfxVolumeSlider != null) sfxVolumeSlider.value = currentSettings.sfxVolume;
        if (musicToggle != null) musicToggle.isOn = currentSettings.musicEnabled;
        if (sfxToggle != null) sfxToggle.isOn = currentSettings.sfxEnabled;
        if (fullscreenToggle != null) fullscreenToggle.isOn = currentSettings.fullscreen;
        if (qualityDropdown != null) qualityDropdown.value = currentSettings.qualityLevel;
        if (vsyncToggle != null) vsyncToggle.isOn = currentSettings.vsync;
        if (difficultyDropdown != null) difficultyDropdown.value = (int)currentSettings.difficulty;
        if (tutorialToggle != null) tutorialToggle.isOn = currentSettings.tutorialEnabled;
        if (hintsToggle != null) hintsToggle.isOn = currentSettings.hintsEnabled;
        if (sensitivitySlider != null) sensitivitySlider.value = currentSettings.sensitivity;
        if (colorBlindModeToggle != null) colorBlindModeToggle.isOn = currentSettings.colorBlindMode;
        if (reducedMotionToggle != null) reducedMotionToggle.isOn = currentSettings.reducedMotion;
        if (textSizeSlider != null) textSizeSlider.value = currentSettings.textSize;
        if (analyticsToggle != null) analyticsToggle.isOn = currentSettings.analytics;
        if (crashReportingToggle != null) crashReportingToggle.isOn = currentSettings.crashReporting;
    }

    #endregion

    #region Public Interface

    public GameSettings GetCurrentSettings()
    {
        return currentSettings;
    }

    public void ResetToDefaults()
    {
        LoadDefaultSettings();
        SaveSettings();
        UpdateUI();
        ApplySettings();
    }

    #endregion
}

[System.Serializable]
public class GameSettings
{
    [Header("Audio")]
    public float musicVolume = 1f;
    public float sfxVolume = 1f;
    public bool musicEnabled = true;
    public bool sfxEnabled = true;

    [Header("Graphics")]
    public bool fullscreen = true;
    public int qualityLevel = 3;
    public bool vsync = true;

    [Header("Gameplay")]
    public GameDifficulty difficulty = GameDifficulty.Normal;
    public bool tutorialEnabled = true;
    public bool hintsEnabled = true;
    public float sensitivity = 1f;

    [Header("Accessibility")]
    public bool colorBlindMode = false;
    public bool reducedMotion = false;
    public string language = "English";
    public float textSize = 1f;

    [Header("Privacy")]
    public bool analytics = true;
    public bool crashReporting = true;
}

public enum GameDifficulty
{
    Easy,
    Normal,
    Hard,
    Expert
}