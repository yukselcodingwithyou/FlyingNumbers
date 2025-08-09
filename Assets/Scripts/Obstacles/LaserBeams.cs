using UnityEngine;

/// <summary>
/// Laser beams obstacle that moves leftward and fires periodic laser bursts.
/// Deals the highest damage among obstacles and has charging/firing phases.
/// </summary>
public class LaserBeams : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private int damageAmount = 5; // Damage dealt to player on collision
    
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2.5f; // Leftward movement speed (slower due to high damage)
    
    [Header("Laser Behavior")]
    [SerializeField] private float chargeDuration = 1.5f; // Time to charge before firing
    [SerializeField] private float fireDuration = 0.8f; // Time laser stays active
    [SerializeField] private float cooldownDuration = 2f; // Time between laser cycles
    [SerializeField] private GameObject laserBeam; // Child object representing the laser beam
    [SerializeField] private GameObject chargingEffect; // Child object for charging effect
    
    [Header("Animation")]
    [SerializeField] private float chargeIntensity = 2f; // How much the charging effect pulses
    [SerializeField] private float chargeSpeed = 10f; // Speed of charging animation
    
    private enum LaserState
    {
        Cooldown,
        Charging,
        Firing
    }
    
    private LaserState currentState = LaserState.Cooldown;
    private float stateTimer = 0f;
    private Vector3 baseScale;
    private bool canDamagePlayer = false;

    private void Start()
    {
        baseScale = transform.localScale;
        
        // Initialize laser components
        if (laserBeam != null)
            laserBeam.SetActive(false);
            
        if (chargingEffect != null)
            chargingEffect.SetActive(false);
            
        // Start in cooldown state
        stateTimer = cooldownDuration;
    }

    private void Update()
    {
        // Move leftward
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime, Space.World);
        
        // Update laser state machine
        UpdateLaserState();
        
        // Update visual effects based on state
        UpdateVisualEffects();
        
        // Destroy if off screen
        if (transform.position.x < -10f)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Updates the laser state machine
    /// </summary>
    private void UpdateLaserState()
    {
        stateTimer -= Time.deltaTime;
        
        switch (currentState)
        {
            case LaserState.Cooldown:
                if (stateTimer <= 0f)
                {
                    StartCharging();
                }
                break;
                
            case LaserState.Charging:
                if (stateTimer <= 0f)
                {
                    StartFiring();
                }
                break;
                
            case LaserState.Firing:
                if (stateTimer <= 0f)
                {
                    StartCooldown();
                }
                break;
        }
    }
    
    /// <summary>
    /// Updates visual effects based on current state
    /// </summary>
    private void UpdateVisualEffects()
    {
        switch (currentState)
        {
            case LaserState.Cooldown:
                // Normal appearance
                transform.localScale = baseScale;
                break;
                
            case LaserState.Charging:
                // Pulsing charging effect
                float chargeProgress = 1f - (stateTimer / chargeDuration);
                float pulseValue = 1f + Mathf.Sin(Time.time * chargeSpeed) * chargeIntensity * chargeProgress;
                transform.localScale = baseScale * pulseValue;
                break;
                
            case LaserState.Firing:
                // Steady intense glow
                transform.localScale = baseScale * 1.2f;
                break;
        }
    }

    /// <summary>
    /// Start the charging phase
    /// </summary>
    private void StartCharging()
    {
        currentState = LaserState.Charging;
        stateTimer = chargeDuration;
        canDamagePlayer = false;
        
        if (chargingEffect != null)
            chargingEffect.SetActive(true);
            
        if (laserBeam != null)
            laserBeam.SetActive(false);
    }
    
    /// <summary>
    /// Start the firing phase
    /// </summary>
    private void StartFiring()
    {
        currentState = LaserState.Firing;
        stateTimer = fireDuration;
        canDamagePlayer = true;
        
        if (chargingEffect != null)
            chargingEffect.SetActive(false);
            
        if (laserBeam != null)
            laserBeam.SetActive(true);
    }
    
    /// <summary>
    /// Start the cooldown phase
    /// </summary>
    private void StartCooldown()
    {
        currentState = LaserState.Cooldown;
        stateTimer = cooldownDuration;
        canDamagePlayer = false;
        
        if (chargingEffect != null)
            chargingEffect.SetActive(false);
            
        if (laserBeam != null)
            laserBeam.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && canDamagePlayer)
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage(damageAmount);
            }
            
            // Add laser hit effect
            CreateLaserHitEffect();
        }
    }

    /// <summary>
    /// Creates a laser hit effect when the laser damages the player
    /// </summary>
    private void CreateLaserHitEffect()
    {
        // Trigger laser hit animation if available
        if (TryGetComponent<Animator>(out var animator))
        {
            animator.SetTrigger("LaserHit");
        }
        
        // Intense flash effect
        if (TryGetComponent<SpriteRenderer>(out var spriteRenderer))
        {
            Color originalColor = spriteRenderer.color;
            LeanTween.color(gameObject, Color.cyan, 0.05f)
                .setOnComplete(() => {
                    LeanTween.color(gameObject, Color.white, 0.05f)
                        .setOnComplete(() => {
                            LeanTween.color(gameObject, originalColor, 0.1f);
                        });
                });
        }
        
        // Scale effect
        LeanTween.scale(gameObject, baseScale * 1.5f, 0.1f).setEase(LeanTweenType.easeOutBack)
            .setOnComplete(() => {
                LeanTween.scale(gameObject, baseScale, 0.2f);
            });
    }
}