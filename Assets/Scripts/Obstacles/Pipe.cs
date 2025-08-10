using UnityEngine;

/// <summary>
/// Detects when the player has passed this pipe and awards score.
/// Also handles collision damage to the player.
/// Attach this to the pipe prefab root object.
/// </summary>
public class Pipe : MonoBehaviour
{
    [Header("Damage")]
    [SerializeField] private int damageAmount = 1; // Damage dealt to player on collision
    
    private bool scored;
    private Transform player;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    private void Update()
    {
        if (!scored && player != null && player.position.x > transform.position.x)
        {
            scored = true;
            if (ScoreManager.Instance != null)
                ScoreManager.Instance.AddScore(1);
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
        }
    }
}
