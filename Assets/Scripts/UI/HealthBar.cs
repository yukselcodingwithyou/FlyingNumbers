using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Health bar UI component that displays player health in real time.
/// Can be implemented as either a Slider or Image fill.
/// Listens to PlayerController health change events.
/// </summary>
public class HealthBar : MonoBehaviour
{
    [Header("Health Bar Display")]
    [SerializeField] private Slider healthSlider; // Optional: Use slider for health bar
    [SerializeField] private Image healthFillImage; // Optional: Use image fill for health bar
    [SerializeField] private Image[] healthHeartImages; // Optional: Use heart icons for health display
    
    [Header("Visual Settings")]
    [SerializeField] private Color fullHealthColor = Color.green;
    [SerializeField] private Color lowHealthColor = Color.red;
    [SerializeField] private bool useColorGradient = true;
    [SerializeField] private float updateSpeed = 5f; // Speed of health bar animation
    
    private PlayerController player;
    private int maxHealth;
    private int currentHealth;
    private float targetHealthPercentage;
    private float currentHealthPercentage;

    private void Start()
    {
        // Find the player in the scene
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.GetComponent<PlayerController>();
            if (player != null)
            {
                maxHealth = player.GetMaxHealth();
                currentHealth = player.GetCurrentHealth();
                targetHealthPercentage = (float)currentHealth / maxHealth;
                currentHealthPercentage = targetHealthPercentage;
                
                InitializeHealthBar();
            }
        }
        
        // Subscribe to health change events
        PlayerController.OnHealthChanged += UpdateHealth;
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        PlayerController.OnHealthChanged -= UpdateHealth;
    }
    
    private void Update()
    {
        // Smoothly animate health bar changes
        if (Mathf.Abs(currentHealthPercentage - targetHealthPercentage) > 0.01f)
        {
            currentHealthPercentage = Mathf.MoveTowards(currentHealthPercentage, targetHealthPercentage, updateSpeed * Time.deltaTime);
            UpdateHealthBarVisual();
        }
    }
    
    /// <summary>
    /// Initialize the health bar based on available UI components
    /// </summary>
    private void InitializeHealthBar()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = 1f;
            healthSlider.value = currentHealthPercentage;
        }
        
        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = currentHealthPercentage;
        }
        
        if (healthHeartImages != null && healthHeartImages.Length > 0)
        {
            // Adjust heart array to match max health
            for (int i = 0; i < healthHeartImages.Length; i++)
            {
                if (healthHeartImages[i] != null)
                {
                    healthHeartImages[i].gameObject.SetActive(i < maxHealth);
                    if (i < currentHealth)
                    {
                        healthHeartImages[i].color = Color.white; // Full heart
                    }
                    else
                    {
                        healthHeartImages[i].color = Color.gray; // Empty heart
                    }
                }
            }
        }
        
        UpdateHealthBarColor();
    }
    
    /// <summary>
    /// Called when player health changes
    /// </summary>
    private void UpdateHealth(int newHealth)
    {
        currentHealth = newHealth;
        targetHealthPercentage = (float)currentHealth / maxHealth;
        
        // Update heart display immediately if using hearts
        if (healthHeartImages != null && healthHeartImages.Length > 0)
        {
            for (int i = 0; i < healthHeartImages.Length && i < maxHealth; i++)
            {
                if (healthHeartImages[i] != null)
                {
                    if (i < currentHealth)
                    {
                        healthHeartImages[i].color = Color.white; // Full heart
                    }
                    else
                    {
                        healthHeartImages[i].color = Color.gray; // Empty heart
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Updates the visual appearance of the health bar
    /// </summary>
    private void UpdateHealthBarVisual()
    {
        if (healthSlider != null)
        {
            healthSlider.value = currentHealthPercentage;
        }
        
        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = currentHealthPercentage;
        }
        
        UpdateHealthBarColor();
    }
    
    /// <summary>
    /// Updates the color of the health bar based on current health
    /// </summary>
    private void UpdateHealthBarColor()
    {
        if (!useColorGradient)
            return;
            
        Color targetColor = Color.Lerp(lowHealthColor, fullHealthColor, currentHealthPercentage);
        
        if (healthSlider != null && healthSlider.fillRect != null)
        {
            Image sliderFill = healthSlider.fillRect.GetComponent<Image>();
            if (sliderFill != null)
                sliderFill.color = targetColor;
        }
        
        if (healthFillImage != null)
        {
            healthFillImage.color = targetColor;
        }
    }
}