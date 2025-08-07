using UnityEngine;
using System.Collections;

/// <summary>
/// Manages all audio in the game including music, sound effects, and dynamic soundtrack.
/// Supports volume control and dynamic music that speeds up with level progression.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Music Tracks")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip gameplayMusic;
    [SerializeField] private AudioClip gameOverMusic;

    [Header("Sound Effects")]
    [SerializeField] private AudioClip flapSound;
    [SerializeField] private AudioClip coinCollectSound;
    [SerializeField] private AudioClip shieldCollectSound;
    [SerializeField] private AudioClip shieldHitSound;
    [SerializeField] private AudioClip operatorSound;
    [SerializeField] private AudioClip powerUpSound;
    [SerializeField] private AudioClip explosionSound;
    [SerializeField] private AudioClip levelUpSound;
    [SerializeField] private AudioClip gameOverSound;

    [Header("Dynamic Music")]
    [SerializeField] private float baseMusicPitch = 1f;
    [SerializeField] private float maxMusicPitch = 1.5f;
    [SerializeField] private float pitchTransitionSpeed = 2f;

    private float currentMusicVolume = 1f;
    private float currentSFXVolume = 1f;
    private float targetPitch = 1f;
    private bool isGameplayMusicPlaying = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeAudioSources();
    }

    private void Start()
    {
        // Subscribe to game events
        if (LevelManager.Instance != null)
        {
            LevelManager.OnLevelChanged += OnLevelChanged;
            LevelManager.OnGameSpeedChanged += OnGameSpeedChanged;
        }

        PlayerController.OnPlayerDeath += OnPlayerDeath;
        CoinManager.OnShieldEarned += OnShieldEarned;

        // Start with menu music
        PlayMenuMusic();
    }

    private void Update()
    {
        // Smoothly adjust music pitch based on game speed
        if (isGameplayMusicPlaying && musicSource.pitch != targetPitch)
        {
            musicSource.pitch = Mathf.Lerp(musicSource.pitch, targetPitch, pitchTransitionSpeed * Time.unscaledDeltaTime);
        }
    }

    private void InitializeAudioSources()
    {
        if (musicSource == null)
        {
            GameObject musicObj = new GameObject("Music Source");
            musicObj.transform.SetParent(transform);
            musicSource = musicObj.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }

        if (sfxSource == null)
        {
            GameObject sfxObj = new GameObject("SFX Source");
            sfxObj.transform.SetParent(transform);
            sfxSource = sfxObj.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }
    }

    #region Music Control

    public void PlayMenuMusic()
    {
        if (menuMusic != null)
        {
            PlayMusic(menuMusic);
            musicSource.pitch = baseMusicPitch;
            isGameplayMusicPlaying = false;
        }
    }

    public void PlayGameplayMusic()
    {
        if (gameplayMusic != null)
        {
            PlayMusic(gameplayMusic);
            isGameplayMusicPlaying = true;
            targetPitch = baseMusicPitch;
        }
    }

    public void PlayGameOverMusic()
    {
        if (gameOverMusic != null)
        {
            PlayMusic(gameOverMusic);
            musicSource.pitch = baseMusicPitch;
            isGameplayMusicPlaying = false;
        }
    }

    private void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
        isGameplayMusicPlaying = false;
    }

    #endregion

    #region Sound Effects

    public void PlayFlapSound()
    {
        PlaySFX(flapSound);
    }

    public void PlayCoinCollectSound()
    {
        PlaySFX(coinCollectSound);
    }

    public void PlayShieldCollectSound()
    {
        PlaySFX(shieldCollectSound);
    }

    public void PlayShieldHitSound()
    {
        PlaySFX(shieldHitSound);
    }

    public void PlayOperatorSound()
    {
        PlaySFX(operatorSound);
    }

    public void PlayPowerUpSound()
    {
        PlaySFX(powerUpSound);
    }

    public void PlayExplosionSound()
    {
        PlaySFX(explosionSound);
    }

    public void PlayLevelUpSound()
    {
        PlaySFX(levelUpSound);
    }

    public void PlayGameOverSound()
    {
        PlaySFX(gameOverSound);
    }

    private void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    #endregion

    #region Volume Control

    public void SetMusicVolume(float volume)
    {
        currentMusicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
        {
            musicSource.volume = currentMusicVolume;
        }
    }

    public void SetSFXVolume(float volume)
    {
        currentSFXVolume = Mathf.Clamp01(volume);
        if (sfxSource != null)
        {
            sfxSource.volume = currentSFXVolume;
        }
    }

    public float GetMusicVolume()
    {
        return currentMusicVolume;
    }

    public float GetSFXVolume()
    {
        return currentSFXVolume;
    }

    #endregion

    #region Event Handlers

    private void OnLevelChanged(int level)
    {
        PlayLevelUpSound();
    }

    private void OnGameSpeedChanged(float speed)
    {
        // Adjust music pitch based on game speed
        targetPitch = Mathf.Lerp(baseMusicPitch, maxMusicPitch, (speed - 1f) / 2f);
        targetPitch = Mathf.Clamp(targetPitch, baseMusicPitch, maxMusicPitch);
    }

    private void OnPlayerDeath()
    {
        PlayGameOverSound();
        StartCoroutine(TransitionToGameOverMusic());
    }

    private void OnShieldEarned()
    {
        PlayShieldCollectSound();
    }

    private IEnumerator TransitionToGameOverMusic()
    {
        // Wait a moment before changing music
        yield return new WaitForSecondsRealtime(1f);
        PlayGameOverMusic();
    }

    #endregion

    #region Public Interface

    public void StartGameplayAudio()
    {
        PlayGameplayMusic();
    }

    public void PauseAudio()
    {
        if (musicSource != null)
            musicSource.Pause();
    }

    public void ResumeAudio()
    {
        if (musicSource != null)
            musicSource.UnPause();
    }

    public void MuteAll()
    {
        SetMusicVolume(0f);
        SetSFXVolume(0f);
    }

    public void UnmuteAll()
    {
        SetMusicVolume(1f);
        SetSFXVolume(1f);
    }

    #endregion

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (LevelManager.Instance != null)
        {
            LevelManager.OnLevelChanged -= OnLevelChanged;
            LevelManager.OnGameSpeedChanged -= OnGameSpeedChanged;
        }

        PlayerController.OnPlayerDeath -= OnPlayerDeath;
        CoinManager.OnShieldEarned -= OnShieldEarned;
    }
}