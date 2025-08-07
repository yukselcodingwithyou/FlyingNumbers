using UnityEngine;

/// <summary>
/// Collectible coin that moves leftward with floating animation.
/// When collected, adds to the coin count managed by CoinManager.
/// </summary>
public class Coin : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float floatAmplitude = 0.3f; // Up/down float amplitude
    [SerializeField] private float floatFrequency = 3f; // Up/down float frequency
    
    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 90f; // Degrees per second
    
    private Vector3 startPosition;
    private bool isCollected = false;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        if (isCollected) return;

        // Move leftward
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime, Space.World);
        
        // Add floating motion
        float newY = startPosition.y + Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        
        // Rotate the coin
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        
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
            CollectCoin();
        }
    }

    private void CollectCoin()
    {
        isCollected = true;
        
        // Notify coin manager
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.CollectCoin();
        }
        
        // Add collection effect
        if (TryGetComponent<Animator>(out var animator))
        {
            animator.SetTrigger("Collected");
        }
        
        // Destroy after a short delay to allow animation
        Destroy(gameObject, 0.5f);
    }
}