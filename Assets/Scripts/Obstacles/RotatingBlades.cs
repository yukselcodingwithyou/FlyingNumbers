using UnityEngine;

/// <summary>
/// Rotating blades obstacle that moves leftward while spinning rapidly.
/// Deals significant damage and has an intimidating rotating animation.
/// </summary>
public class RotatingBlades : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private int damageAmount = 3; // Damage dealt to player on collision
    
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3.5f; // Leftward movement speed
    [SerializeField] private float verticalDrift = 0.5f; // Slight vertical drift
    [SerializeField] private float driftFrequency = 1f; // Drift frequency
    
    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 360f; // Degrees per second
    [SerializeField] private float rotationAcceleration = 50f; // Acceleration over time
    
    [Header("Animation")]
    [SerializeField] private float wobbleAmount = 5f; // Wobble angle
    [SerializeField] private float wobbleSpeed = 8f; // Wobble frequency
    
    private Vector3 startPosition;
    private float currentRotationSpeed;
    private float randomOffset;
    private bool hasHitPlayer = false;

    private void Start()
    {
        startPosition = transform.position;
        currentRotationSpeed = rotationSpeed;
        randomOffset = Random.Range(0f, 2f * Mathf.PI);
    }

    private void Update()
    {
        // Move leftward
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime, Space.World);
        
        // Add subtle vertical drift
        float time = Time.time + randomOffset;
        float newY = startPosition.y + Mathf.Sin(time * driftFrequency) * verticalDrift;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        
        // Accelerate rotation over time for increasing danger
        currentRotationSpeed += rotationAcceleration * Time.deltaTime;
        
        // Apply rotation with wobble effect
        float wobble = Mathf.Sin(time * wobbleSpeed) * wobbleAmount;
        float totalRotation = currentRotationSpeed + wobble;
        transform.Rotate(0, 0, totalRotation * Time.deltaTime);
        
        // Destroy if off screen
        if (transform.position.x < -10f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasHitPlayer)
        {
            hasHitPlayer = true; // Prevent multiple hits in quick succession
            
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage(damageAmount);
            }
            
            // Add slash effect
            CreateSlashEffect();
            
            // Reset hit flag after a short delay
            Invoke(nameof(ResetHitFlag), 0.5f);
        }
    }

    /// <summary>
    /// Creates a slash effect when the blades hit the player
    /// </summary>
    private void CreateSlashEffect()
    {
        // Trigger slash animation if available
        if (TryGetComponent<Animator>(out var animator))
        {
            animator.SetTrigger("Slash");
        }
        
        // Speed up rotation temporarily
        currentRotationSpeed *= 2f;
        
        // Flash effect with spark-like color
        if (TryGetComponent<SpriteRenderer>(out var spriteRenderer))
        {
            Color originalColor = spriteRenderer.color;
            LeanTween.color(gameObject, Color.yellow, 0.05f)
                .setOnComplete(() => {
                    LeanTween.color(gameObject, Color.white, 0.05f)
                        .setOnComplete(() => {
                            LeanTween.color(gameObject, originalColor, 0.1f);
                        });
                });
        }
        
        // Screen shake effect (if camera shake system exists)
        // This would require a camera shake manager, which might not exist yet
        // CameraShake.Instance?.Shake(0.2f, 0.1f);
    }
    
    /// <summary>
    /// Resets the hit flag to allow multiple hits
    /// </summary>
    private void ResetHitFlag()
    {
        hasHitPlayer = false;
    }
}