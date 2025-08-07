using UnityEngine;

/// <summary>
/// Base class for all power-ups in the game.
/// Handles common power-up behavior and provides virtual methods for specific effects.
/// </summary>
public abstract class PowerUp : MonoBehaviour
{
    [Header("Power-up Settings")]
    [SerializeField] protected float duration = 5f; // How long the power-up lasts
    [SerializeField] protected float moveSpeed = 2f; // Movement speed
    [SerializeField] protected float floatAmplitude = 0.5f; // Up/down float amplitude
    [SerializeField] protected float floatFrequency = 2f; // Up/down float frequency
    
    protected Vector3 startPosition;
    protected bool isCollected = false;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        if (!isCollected)
        {
            MovePowerUp();
        }
    }

    /// <summary>
    /// Handles the movement of the power-up
    /// </summary>
    protected virtual void MovePowerUp()
    {
        // Move leftward
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        
        // Add floating motion
        float newY = startPosition.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        
        // Destroy if off screen
        if (transform.position.x < -10f)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Called when the power-up is collected by the player
    /// </summary>
    public virtual void ApplyPowerUp(PlayerController player)
    {
        if (isCollected) return;
        
        isCollected = true;
        OnPowerUpCollected(player);
        
        // Add visual/audio feedback here
        if (TryGetComponent<Animator>(out var animator))
        {
            animator.SetTrigger("Collected");
        }
    }

    /// <summary>
    /// Override this method in derived classes to implement specific power-up effects
    /// </summary>
    protected abstract void OnPowerUpCollected(PlayerController player);
}

/// <summary>
/// Magnet power-up that attracts nearby coins and shields
/// </summary>
public class MagnetPowerUp : PowerUp
{
    [Header("Magnet Settings")]
    [SerializeField] private float magnetRange = 5f;
    [SerializeField] private float attractionForce = 10f;

    protected override void OnPowerUpCollected(PlayerController player)
    {
        // Start magnet coroutine
        StartCoroutine(MagnetEffect(player));
    }

    private System.Collections.IEnumerator MagnetEffect(PlayerController player)
    {
        float timer = 0f;
        
        while (timer < duration)
        {
            // Find nearby coins and shields
            Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(player.transform.position, magnetRange);
            
            foreach (var obj in nearbyObjects)
            {
                if (obj.CompareTag("Coin") || obj.CompareTag("Shield"))
                {
                    // Attract object to player
                    Vector2 direction = (player.transform.position - obj.transform.position).normalized;
                    obj.transform.Translate(direction * attractionForce * Time.deltaTime);
                }
            }
            
            timer += Time.deltaTime;
            yield return null;
        }
    }
}

/// <summary>
/// Slow motion power-up that reduces game speed temporarily
/// </summary>
public class SlowMotionPowerUp : PowerUp
{
    [Header("Slow Motion Settings")]
    [SerializeField] private float slowMotionScale = 0.5f;

    protected override void OnPowerUpCollected(PlayerController player)
    {
        StartCoroutine(SlowMotionEffect());
    }

    private System.Collections.IEnumerator SlowMotionEffect()
    {
        // Store original time scale
        float originalTimeScale = Time.timeScale;
        
        // Apply slow motion
        Time.timeScale = slowMotionScale;
        
        // Wait for duration (real time)
        yield return new WaitForSecondsRealtime(duration);
        
        // Restore original time scale
        Time.timeScale = originalTimeScale;
    }
}

/// <summary>
/// Shrink power-up that reduces player size temporarily
/// </summary>
public class ShrinkPowerUp : PowerUp
{
    [Header("Shrink Settings")]
    [SerializeField] private float shrinkScale = 0.5f;

    protected override void OnPowerUpCollected(PlayerController player)
    {
        StartCoroutine(ShrinkEffect(player));
    }

    private System.Collections.IEnumerator ShrinkEffect(PlayerController player)
    {
        // Store original scale
        Vector3 originalScale = player.transform.localScale;
        
        // Apply shrink
        player.SetScale(shrinkScale);
        
        // Wait for duration
        yield return new WaitForSeconds(duration);
        
        // Restore original scale
        player.transform.localScale = originalScale;
    }
}

/// <summary>
/// Invincibility power-up that makes player immune to obstacles
/// </summary>
public class InvincibilityPowerUp : PowerUp
{
    protected override void OnPowerUpCollected(PlayerController player)
    {
        StartCoroutine(InvincibilityEffect(player));
    }

    private System.Collections.IEnumerator InvincibilityEffect(PlayerController player)
    {
        // Apply invincibility
        player.SetInvincible(true);
        
        // Add visual feedback (blinking, glow, etc.)
        StartCoroutine(BlinkEffect(player));
        
        // Wait for duration
        yield return new WaitForSeconds(duration);
        
        // Remove invincibility
        player.SetInvincible(false);
    }

    private System.Collections.IEnumerator BlinkEffect(PlayerController player)
    {
        SpriteRenderer playerSprite = player.GetComponent<SpriteRenderer>();
        if (playerSprite == null) yield break;

        float blinkTimer = 0f;
        bool isVisible = true;

        while (blinkTimer < duration)
        {
            isVisible = !isVisible;
            playerSprite.color = new Color(1f, 1f, 1f, isVisible ? 1f : 0.5f);
            
            yield return new WaitForSeconds(0.1f);
            blinkTimer += 0.1f;
        }

        // Ensure player is visible at the end
        playerSprite.color = Color.white;
    }
}