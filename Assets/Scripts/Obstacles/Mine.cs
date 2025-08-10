using UnityEngine;

/// <summary>
/// Mine obstacle that moves leftward and up/down in a sine wave pattern.
/// Animated and deals damage to the player on collision unless they have a shield.
/// </summary>
public class Mine : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private int damageAmount = 1; // Damage dealt to player on collision
    
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float verticalAmplitude = 2f; // Up/down movement range
    [SerializeField] private float verticalFrequency = 1.5f; // Up/down movement speed
    [SerializeField] private float rotationSpeed = 45f; // Rotation for menacing look
    
    [Header("Animation")]
    [SerializeField] private float pulseSpeed = 3f; // Pulsing animation speed
    [SerializeField] private float pulseScale = 0.1f; // How much it pulses
    
    private Vector3 startPosition;
    private Vector3 baseScale;
    private float randomOffset;

    private void Start()
    {
        startPosition = transform.position;
        baseScale = transform.localScale;
        randomOffset = Random.Range(0f, 2f * Mathf.PI); // Random phase for variation
    }

    private void Update()
    {
        // Move leftward
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime, Space.World);
        
        // Add vertical sine wave movement
        float time = Time.time + randomOffset;
        float newY = startPosition.y + Mathf.Sin(time * verticalFrequency) * verticalAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        
        // Rotate the mine for a menacing effect
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        
        // Add pulsing scale animation
        float pulseValue = 1f + Mathf.Sin(time * pulseSpeed) * pulseScale;
        transform.localScale = baseScale * pulseValue;
        
        // Destroy if off screen
        if (transform.position.x < -10f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage(damageAmount);
            }
            
            // Add explosion effect
            CreateExplosionEffect();
        }
    }

    /// <summary>
    /// Creates an explosion effect when the mine is hit
    /// </summary>
    private void CreateExplosionEffect()
    {
        // Trigger explosion animation if available
        if (TryGetComponent<Animator>(out var animator))
        {
            animator.SetTrigger("Explode");
        }
        
        // Scale up quickly then destroy
        LeanTween.scale(gameObject, baseScale * 2f, 0.2f).setEase(LeanTweenType.easeOutBack);
        
        // Fade out
        if (TryGetComponent<SpriteRenderer>(out var spriteRenderer))
        {
            LeanTween.alpha(gameObject, 0f, 0.3f);
        }
        
        // Destroy after effect
        Destroy(gameObject, 0.5f);
    }
}