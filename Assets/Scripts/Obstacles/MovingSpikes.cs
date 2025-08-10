using UnityEngine;

/// <summary>
/// Moving spikes obstacle that moves leftward with vertical oscillation and rotation.
/// Deals more damage than basic obstacles and has a threatening appearance.
/// </summary>
public class MovingSpikes : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private int damageAmount = 2; // Damage dealt to player on collision
    
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f; // Leftward movement speed
    [SerializeField] private float verticalAmplitude = 1.5f; // Up/down movement range
    [SerializeField] private float verticalFrequency = 2f; // Up/down movement speed
    [SerializeField] private float rotationSpeed = 90f; // Rotation speed for menacing effect
    
    [Header("Animation")]
    [SerializeField] private float scaleOscillationSpeed = 4f; // Scale pulsing speed
    [SerializeField] private float scaleOscillationAmount = 0.15f; // How much the scale changes
    
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
        
        // Rotate the spikes for a threatening effect
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        
        // Add scale oscillation for threatening animation
        float scaleValue = 1f + Mathf.Sin(time * scaleOscillationSpeed) * scaleOscillationAmount;
        transform.localScale = baseScale * scaleValue;
        
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
            
            // Add impact effect
            CreateImpactEffect();
        }
    }

    /// <summary>
    /// Creates an impact effect when the spikes hit the player
    /// </summary>
    private void CreateImpactEffect()
    {
        // Trigger impact animation if available
        if (TryGetComponent<Animator>(out var animator))
        {
            animator.SetTrigger("Impact");
        }
        
        // Quick scale effect
        LeanTween.scale(gameObject, baseScale * 1.3f, 0.1f).setEase(LeanTweenType.easeOutBack)
            .setOnComplete(() => {
                LeanTween.scale(gameObject, baseScale, 0.1f);
            });
        
        // Flash effect
        if (TryGetComponent<SpriteRenderer>(out var spriteRenderer))
        {
            Color originalColor = spriteRenderer.color;
            LeanTween.color(gameObject, Color.red, 0.1f)
                .setOnComplete(() => {
                    LeanTween.color(gameObject, originalColor, 0.1f);
                });
        }
    }
}