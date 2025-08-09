using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/// <summary>
/// Enhanced player controller with shield system, rotation, and improved mechanics.
/// Handles player movement, wing animation, operator collisions, shields, and game over logic.
/// Attach this script to the player GameObject which should also have a Rigidbody2D
/// and Animator components. The Animator is expected to have a "Flap" trigger
/// that plays a wing flap animation.
/// Uses the Unity Input System with an <see cref="InputActionReference"/> for the
/// flap action.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float flapForce = 5f; // Upward force when flapping
    [SerializeField] private float gravityScale = 1f; // Gravity multiplier
    [SerializeField] private float maxRotationAngle = 30f; // Maximum rotation angle
    [SerializeField] private float rotationSpeed = 4f; // Speed of rotation

    [Header("Input")]
    [SerializeField] private InputActionReference flapAction; // Reference to the input action that triggers a flap

    [Header("UI")]
    [SerializeField] private Text numberText; // UI text displaying the current number
    [SerializeField] private GameObject shieldVisual; // Visual shield effect

    [Header("Shield System")]
    [SerializeField] private int maxShields = 3; // Maximum shields the player can have
    
    [Header("Health System")]
    [SerializeField] private int maxHealth = 3; // Maximum health the player can have
    [SerializeField] private int currentHealth = 3; // Current health
    
    [Header("Animation")]
    [SerializeField] private CharacterAnimationManager characterAnimationManager; // Manages wing and feet animations
    
    // Events for other systems to listen to
    public static System.Action<int> OnNumberChanged;
    public static System.Action<int> OnShieldsChanged;
    public static System.Action<int> OnHealthChanged;
    public static System.Action OnPlayerDeath;

    private Rigidbody2D rb;
    private Animator animator;
    private int currentNumber = 1; // Start with number 1 as specified
    private int currentShields = 0;
    private bool isGameOver = false;
    private bool isInvincible = false; // For power-ups

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.gravityScale = gravityScale;
        
        // Initialize with starting number and no shields
        currentNumber = 1;
        currentShields = 0;
        currentHealth = maxHealth;
        
        UpdateNumberText();
        UpdateShieldVisual();
        
        // Notify other systems of initial values
        OnNumberChanged?.Invoke(currentNumber);
        OnShieldsChanged?.Invoke(currentShields);
        OnHealthChanged?.Invoke(currentHealth);
    }

    private void OnEnable()
    {
        if (flapAction != null)
        {
            flapAction.action.performed += OnFlapPerformed;
            flapAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (flapAction != null)
        {
            flapAction.action.performed -= OnFlapPerformed;
            flapAction.action.Disable();
        }
    }

    private void Update()
    {
        // Only allow gravity and movement updates when not in game over state
        if (isGameOver)
            return;
            
        // Handle rotation based on velocity
        HandleRotation();
    }
    
    /// <summary>
    /// Handles the rotation of the player based on vertical velocity
    /// </summary>
    private void HandleRotation()
    {
        float angle = Mathf.Lerp(-maxRotationAngle, maxRotationAngle, 
            (rb.velocity.y + 10f) / 20f); // Normalize velocity to rotation
        angle = Mathf.Clamp(angle, -maxRotationAngle, maxRotationAngle);
        
        transform.rotation = Quaternion.Lerp(transform.rotation, 
            Quaternion.Euler(0, 0, angle), rotationSpeed * Time.deltaTime);
    }

    private void OnFlapPerformed(InputAction.CallbackContext context)
    {
        if (!isGameOver)
            Flap();
    }

    /// <summary>
    /// Applies an upward force and triggers the wing flap animation.
    /// </summary>
    private void Flap()
    {
        rb.velocity = Vector2.up * flapForce;
        
        // Use new animation system if available, fallback to old system
        if (characterAnimationManager != null)
        {
            characterAnimationManager.TriggerFlapAndKick();
        }
        else if (animator != null)
        {
            animator.SetTrigger("Flap");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isGameOver)
            return;

        // Handle different collision types
        if (collision.CompareTag("Pipe") || collision.CompareTag("Mine"))
        {
            HandleObstacleCollision();
            return;
        }

        // Handle collectibles
        if (collision.CompareTag("Shield"))
        {
            CollectShield();
            Destroy(collision.gameObject);
            return;
        }
        
        if (collision.CompareTag("Coin"))
        {
            CollectCoin();
            Destroy(collision.gameObject);
            return;
        }
        
        // Handle power-ups
        PowerUp powerUp = collision.GetComponent<PowerUp>();
        if (powerUp != null)
        {
            powerUp.ApplyPowerUp(this);
            Destroy(collision.gameObject);
            return;
        }

        // Check for operator objects like +1, -2, *3
        Operator operatorComponent = collision.GetComponent<Operator>();
        if (operatorComponent != null)
        {
            ApplyOperator(operatorComponent);
            if (ScoreManager.Instance != null)
                ScoreManager.Instance.AddScore(1);
            Destroy(collision.gameObject);
        }
    }
    
    /// <summary>
    /// Takes damage from obstacles or other sources
    /// </summary>
    public void TakeDamage(int amount)
    {
        if (isInvincible || isGameOver)
            return;
            
        if (currentShields > 0)
        {
            // Use shield to absorb damage
            currentShields--;
            UpdateShieldVisual();
            OnShieldsChanged?.Invoke(currentShields);
            
            // Add visual feedback for shield use
            if (animator != null)
                animator.SetTrigger("ShieldHit");
        }
        else
        {
            // Take health damage
            currentHealth = Mathf.Max(0, currentHealth - amount);
            OnHealthChanged?.Invoke(currentHealth);
            
            // Add visual feedback for health damage
            if (animator != null)
                animator.SetTrigger("TakeDamage");
            
            if (currentHealth <= 0)
            {
                GameOver();
            }
        }
    }
    
    /// <summary>
    /// Handles collision with obstacles (pipes, mines) - Legacy method maintained for compatibility
    /// </summary>
    private void HandleObstacleCollision()
    {
        TakeDamage(1); // Default damage for legacy obstacles
    }
    
    /// <summary>
    /// Collects a shield item
    /// </summary>
    private void CollectShield()
    {
        if (currentShields < maxShields)
        {
            currentShields++;
            UpdateShieldVisual();
            OnShieldsChanged?.Invoke(currentShields);
            
            if (animator != null)
                animator.SetTrigger("ShieldCollect");
        }
    }
    
    /// <summary>
    /// Collects a coin
    /// </summary>
    private void CollectCoin()
    {
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.CollectCoin();
        }
        
        if (animator != null)
            animator.SetTrigger("CoinCollect");
    }

    /// <summary>
    /// Applies the operator effect to the current number.
    /// </summary>
    private void ApplyOperator(Operator op)
    {
        int previousNumber = currentNumber;
        
        switch (op.type)
        {
            case Operator.Type.Add:
                currentNumber += op.value;
                break;
            case Operator.Type.Subtract:
                currentNumber -= op.value;
                break;
            case Operator.Type.Multiply:
                // Special case: ×0 acts like an obstacle unless protected
                if (op.value == 0)
                {
                    if (isInvincible)
                        return;
                        
                    if (currentShields > 0)
                    {
                        currentShields--;
                        UpdateShieldVisual();
                        OnShieldsChanged?.Invoke(currentShields);
                        return; // Don't apply the multiplication
                    }
                    else
                    {
                        GameOver();
                        return;
                    }
                }
                currentNumber *= op.value;
                break;
            case Operator.Type.Divide:
                if (op.value != 0)
                    currentNumber /= op.value;
                break;
        }

        UpdateNumberText();
        OnNumberChanged?.Invoke(currentNumber);
        
        // Trigger number change animation
        if (animator != null && currentNumber != previousNumber)
            animator.SetTrigger("NumberChange");
    }

    /// <summary>
    /// Updates the on screen number.
    /// </summary>
    private void UpdateNumberText()
    {
        if (numberText != null)
        {
            numberText.text = currentNumber.ToString();
        }
    }
    
    /// <summary>
    /// Updates the shield visual effect
    /// </summary>
    private void UpdateShieldVisual()
    {
        if (shieldVisual != null)
        {
            shieldVisual.SetActive(currentShields > 0);
        }
    }

    /// <summary>
    /// Called when hitting a pipe collider.
    /// Stops movement and disables further input.
    /// </summary>
    private void GameOver()
    {
        isGameOver = true;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        
        // Reset rotation
        transform.rotation = Quaternion.identity;
        
        // Notify other systems
        OnPlayerDeath?.Invoke();
        
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.ShowGameOver();
    }
    
    // Public methods for power-ups and external systems
    
    /// <summary>
    /// Adds shields to the player (used by coin system)
    /// </summary>
    public void AddShields(int amount)
    {
        currentShields = Mathf.Min(currentShields + amount, maxShields);
        UpdateShieldVisual();
        OnShieldsChanged?.Invoke(currentShields);
    }
    
    /// <summary>
    /// Sets invincibility state (for power-ups)
    /// </summary>
    public void SetInvincible(bool invincible)
    {
        isInvincible = invincible;
    }
    
    /// <summary>
    /// Gets current number value
    /// </summary>
    public int GetCurrentNumber()
    {
        return currentNumber;
    }
    
    /// <summary>
    /// Gets current shield count
    /// </summary>
    public int GetCurrentShields()
    {
        return currentShields;
    }
    
    /// <summary>
    /// Sets the player scale (for shrink power-up)
    /// </summary>
    public void SetScale(float scale)
    {
        transform.localScale = Vector3.one * scale;
    }
    
    /// <summary>
    /// Gets the character animation manager
    /// </summary>
    public CharacterAnimationManager GetAnimationManager()
    {
        return characterAnimationManager;
    }
    
    /// <summary>
    /// Gets current health value
    /// </summary>
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
    
    /// <summary>
    /// Gets maximum health value
    /// </summary>
    public int GetMaxHealth()
    {
        return maxHealth;
    }
}
