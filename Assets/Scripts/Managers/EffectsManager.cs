using UnityEngine;
using System.Collections;

/// <summary>
/// Manages visual effects and juice for enhanced game feedback.
/// Includes screen shake, particle effects, combo bonuses, and visual polish.
/// </summary>
public class EffectsManager : MonoBehaviour
{
    public static EffectsManager Instance { get; private set; }

    [Header("Screen Shake")]
    [SerializeField] private float shakeIntensity = 0.5f;
    [SerializeField] private float shakeDuration = 0.3f;
    [SerializeField] private AnimationCurve shakeDecay = AnimationCurve.EaseInOut(0, 1, 1, 0);

    [Header("Particle Effects")]
    [SerializeField] private ParticleSystem coinCollectEffect;
    [SerializeField] private ParticleSystem shieldCollectEffect;
    [SerializeField] private ParticleSystem explosionEffect;
    [SerializeField] private ParticleSystem levelUpEffect;
    [SerializeField] private ParticleSystem powerUpEffect;

    [Header("Score Effects")]
    [SerializeField] private GameObject scorePopupPrefab;
    [SerializeField] private GameObject comboTextPrefab;
    [SerializeField] private Transform effectsParent;

    [Header("Flash Effects")]
    [SerializeField] private GameObject flashOverlay;
    [SerializeField] private Color damageFlashColor = Color.red;
    [SerializeField] private Color collectFlashColor = Color.yellow;
    [SerializeField] private float flashDuration = 0.1f;

    private Camera mainCamera;
    private Vector3 originalCameraPosition;
    private bool isShaking = false;
    private int currentCombo = 0;
    private float lastCollectionTime = 0f;
    private float comboTimeWindow = 2f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeEffects();
    }

    private void Start()
    {
        // Subscribe to game events
        PlayerController.OnNumberChanged += OnNumberChanged;
        CoinManager.OnCoinsChanged += OnCoinCollected;
        PlayerController.OnShieldsChanged += OnShieldChanged;
        PlayerController.OnPlayerDeath += OnPlayerDeath;
        LevelManager.OnLevelChanged += OnLevelUp;
        MissionManager.OnMissionCompleted += OnMissionCompleted;
    }

    private void InitializeEffects()
    {
        mainCamera = Camera.main;
        if (mainCamera != null)
        {
            originalCameraPosition = mainCamera.transform.position;
        }

        // Create effects parent if not assigned
        if (effectsParent == null)
        {
            GameObject effectsObj = new GameObject("Effects");
            effectsParent = effectsObj.transform;
            effectsObj.transform.SetParent(transform);
        }

        // Initialize flash overlay
        if (flashOverlay != null)
        {
            flashOverlay.SetActive(false);
        }
    }

    #region Screen Shake

    public void ShakeScreen(float intensity = -1f, float duration = -1f)
    {
        if (mainCamera == null) return;

        float shakeStrength = intensity > 0 ? intensity : shakeIntensity;
        float shakeDur = duration > 0 ? duration : shakeDuration;

        if (!isShaking)
        {
            StartCoroutine(ScreenShakeCoroutine(shakeStrength, shakeDur));
        }
    }

    private IEnumerator ScreenShakeCoroutine(float intensity, float duration)
    {
        isShaking = true;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float normalizedTime = elapsed / duration;
            float strength = intensity * shakeDecay.Evaluate(normalizedTime);

            Vector3 randomOffset = Random.insideUnitSphere * strength;
            randomOffset.z = 0f; // Keep camera on same Z plane

            mainCamera.transform.position = originalCameraPosition + randomOffset;

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        mainCamera.transform.position = originalCameraPosition;
        isShaking = false;
    }

    #endregion

    #region Particle Effects

    public void PlayCoinCollectEffect(Vector3 position)
    {
        if (coinCollectEffect != null)
        {
            coinCollectEffect.transform.position = position;
            coinCollectEffect.Play();
        }
    }

    public void PlayShieldCollectEffect(Vector3 position)
    {
        if (shieldCollectEffect != null)
        {
            shieldCollectEffect.transform.position = position;
            shieldCollectEffect.Play();
        }
    }

    public void PlayExplosionEffect(Vector3 position)
    {
        if (explosionEffect != null)
        {
            explosionEffect.transform.position = position;
            explosionEffect.Play();
        }
        
        // Add screen shake for explosions
        ShakeScreen(0.8f, 0.4f);
    }

    public void PlayLevelUpEffect(Vector3 position)
    {
        if (levelUpEffect != null)
        {
            levelUpEffect.transform.position = position;
            levelUpEffect.Play();
        }
    }

    public void PlayPowerUpEffect(Vector3 position)
    {
        if (powerUpEffect != null)
        {
            powerUpEffect.transform.position = position;
            powerUpEffect.Play();
        }
    }

    #endregion

    #region Score and Combo Effects

    public void ShowScorePopup(Vector3 position, int score, Color color)
    {
        if (scorePopupPrefab != null && effectsParent != null)
        {
            GameObject popup = Instantiate(scorePopupPrefab, position, Quaternion.identity, effectsParent);
            
            // Configure popup
            var text = popup.GetComponent<UnityEngine.UI.Text>();
            if (text != null)
            {
                text.text = "+" + score.ToString();
                text.color = color;
            }

            // Animate popup
            StartCoroutine(AnimateScorePopup(popup));
        }
    }

    private IEnumerator AnimateScorePopup(GameObject popup)
    {
        Vector3 startPos = popup.transform.position;
        Vector3 endPos = startPos + Vector3.up * 2f;
        float duration = 1f;
        float elapsed = 0f;

        while (elapsed < duration && popup != null)
        {
            float t = elapsed / duration;
            popup.transform.position = Vector3.Lerp(startPos, endPos, t);
            
            // Fade out
            var text = popup.GetComponent<UnityEngine.UI.Text>();
            if (text != null)
            {
                Color color = text.color;
                color.a = 1f - t;
                text.color = color;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (popup != null)
        {
            Destroy(popup);
        }
    }

    public void ShowComboText(Vector3 position, int comboCount)
    {
        if (comboTextPrefab != null && effectsParent != null)
        {
            GameObject comboText = Instantiate(comboTextPrefab, position, Quaternion.identity, effectsParent);
            
            var text = comboText.GetComponent<UnityEngine.UI.Text>();
            if (text != null)
            {
                text.text = $"COMBO x{comboCount}!";
                text.color = Color.yellow;
            }

            StartCoroutine(AnimateComboText(comboText));
        }
    }

    private IEnumerator AnimateComboText(GameObject comboText)
    {
        Vector3 originalScale = comboText.transform.localScale;
        
        // Scale up animation
        float scaleUpDuration = 0.3f;
        float elapsed = 0f;
        
        while (elapsed < scaleUpDuration && comboText != null)
        {
            float t = elapsed / scaleUpDuration;
            comboText.transform.localScale = Vector3.Lerp(Vector3.zero, originalScale * 1.2f, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Hold
        yield return new WaitForSeconds(0.5f);

        // Scale down and fade
        elapsed = 0f;
        float fadeOutDuration = 0.5f;
        
        while (elapsed < fadeOutDuration && comboText != null)
        {
            float t = elapsed / fadeOutDuration;
            comboText.transform.localScale = Vector3.Lerp(originalScale * 1.2f, Vector3.zero, t);
            
            var text = comboText.GetComponent<UnityEngine.UI.Text>();
            if (text != null)
            {
                Color color = text.color;
                color.a = 1f - t;
                text.color = color;
            }
            
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (comboText != null)
        {
            Destroy(comboText);
        }
    }

    #endregion

    #region Flash Effects

    public void FlashScreen(Color color, float duration = -1f)
    {
        if (flashOverlay != null)
        {
            float flashDur = duration > 0 ? duration : flashDuration;
            StartCoroutine(FlashCoroutine(color, flashDur));
        }
    }

    private IEnumerator FlashCoroutine(Color color, float duration)
    {
        if (flashOverlay == null) yield break;

        var image = flashOverlay.GetComponent<UnityEngine.UI.Image>();
        if (image == null) yield break;

        flashOverlay.SetActive(true);
        image.color = color;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(color.a, 0f, elapsed / duration);
            Color currentColor = color;
            currentColor.a = alpha;
            image.color = currentColor;

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        flashOverlay.SetActive(false);
    }

    #endregion

    #region Event Handlers

    private void OnNumberChanged(int newNumber)
    {
        // Show score popup for number changes
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            ShowScorePopup(player.transform.position + Vector3.up, 1, Color.green);
        }
    }

    private void OnCoinCollected(int coins)
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            PlayCoinCollectEffect(player.transform.position);
            FlashScreen(collectFlashColor, 0.05f);
            
            // Check for combo
            if (Time.time - lastCollectionTime < comboTimeWindow)
            {
                currentCombo++;
                if (currentCombo >= 3)
                {
                    ShowComboText(player.transform.position + Vector3.up * 1.5f, currentCombo);
                }
            }
            else
            {
                currentCombo = 1;
            }
            
            lastCollectionTime = Time.time;
        }
    }

    private void OnShieldChanged(int shields)
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            PlayShieldCollectEffect(player.transform.position);
            FlashScreen(Color.blue, 0.1f);
        }
    }

    private void OnPlayerDeath()
    {
        FlashScreen(damageFlashColor, 0.3f);
        ShakeScreen(1f, 0.5f);
        
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            PlayExplosionEffect(player.transform.position);
        }
        
        currentCombo = 0;
    }

    private void OnLevelUp(int level)
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            PlayLevelUpEffect(player.transform.position);
            ShowScorePopup(player.transform.position + Vector3.up * 2f, level * 10, Color.yellow);
        }
        
        FlashScreen(Color.white, 0.2f);
    }

    private void OnMissionCompleted(Mission mission)
    {
        // Could show special effects for mission completion
        FlashScreen(Color.green, 0.15f);
    }

    #endregion

    #region Public Interface

    public void CreateNumberChangeEffect(Vector3 position, int oldNumber, int newNumber)
    {
        // Create visual effect showing number transformation
        ShowScorePopup(position, newNumber - oldNumber, newNumber > oldNumber ? Color.green : Color.red);
    }

    public void CreatePowerUpActivationEffect(Vector3 position, string powerUpName)
    {
        PlayPowerUpEffect(position);
        // Could show power-up name text
    }

    #endregion

    private void OnDestroy()
    {
        PlayerController.OnNumberChanged -= OnNumberChanged;
        CoinManager.OnCoinsChanged -= OnCoinCollected;
        PlayerController.OnShieldsChanged -= OnShieldChanged;
        PlayerController.OnPlayerDeath -= OnPlayerDeath;
        LevelManager.OnLevelChanged -= OnLevelUp;
        MissionManager.OnMissionCompleted -= OnMissionCompleted;
    }
}