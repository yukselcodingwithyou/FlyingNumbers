using UnityEngine;

/// <summary>
/// Collectible shield that provides protection from obstacles and ×0 operators.
/// Moves leftward with random floating motion as specified in the README.
/// </summary>
public class Shield : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float floatAmplitude = 0.5f; // Up/down float amplitude
    [SerializeField] private float floatFrequency = 2f; // Up/down float frequency
    [SerializeField] private float randomOffset = 1f; // Random offset for variation
    
    [Header("Visual Effects")]
    [SerializeField] private float glowPulseSpeed = 2f;
    [SerializeField] private float glowIntensity = 0.3f;
    
    private Vector3 startPosition;
    private float randomTime; // Random offset for each shield
    private bool isCollected = false;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        startPosition = transform.position;
        randomTime = Random.Range(0f, 2f * Mathf.PI); // Random phase offset
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (isCollected) return;

        // Move leftward
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime, Space.World);
        
        // Add random floating motion
        float time = Time.time + randomTime;
        float newY = startPosition.y + 
                    Mathf.Sin(time * floatFrequency) * floatAmplitude +
                    Mathf.Sin(time * floatFrequency * 1.3f) * randomOffset * 0.3f;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        
        // Add glowing effect
        if (spriteRenderer != null)
        {
            float alpha = 1f + Mathf.Sin(time * glowPulseSpeed) * glowIntensity;
            Color color = spriteRenderer.color;
            color.a = alpha;
            spriteRenderer.color = color;
        }
        
        // Destroy if off screen
        if (transform.position.x < -10f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected) return;
        
        if (other.CompareTag("Player"))
        {
            CollectShield();
        }
    }

    private void CollectShield()
    {
        isCollected = true;
        
        // Give shield to player
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.AddShields(1);
        }
        
        // Add collection effect
        if (TryGetComponent<Animator>(out var animator))
        {
            animator.SetTrigger("Collected");
        }
        
        // Visual feedback
        if (spriteRenderer != null)
        {
            // Quick flash effect
            spriteRenderer.color = Color.white;
        }
        
        // Destroy after a short delay to allow animation
        Destroy(gameObject, 0.5f);
    }
}