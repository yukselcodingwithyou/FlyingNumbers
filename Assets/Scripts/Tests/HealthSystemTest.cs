using UnityEngine;

/// <summary>
/// Simple test script to verify health system integration.
/// Can be attached to a test GameObject to trigger damage tests.
/// </summary>
public class HealthSystemTest : MonoBehaviour
{
    [Header("Test Settings")]
    [SerializeField] private KeyCode testDamageKey = KeyCode.T;
    [SerializeField] private KeyCode testShieldKey = KeyCode.Y;
    [SerializeField] private int testDamageAmount = 1;
    
    private PlayerController player;
    
    private void Start()
    {
        // Find the player in the scene
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.GetComponent<PlayerController>();
        }
        
        if (player == null)
        {
            Debug.LogWarning("HealthSystemTest: No PlayerController found in scene!");
        }
        else
        {
            Debug.Log("HealthSystemTest: Press " + testDamageKey + " to deal damage, " + testShieldKey + " to add shield");
            Debug.Log($"Player Health: {player.GetCurrentHealth()}/{player.GetMaxHealth()}");
            Debug.Log($"Player Shields: {player.GetCurrentShields()}");
        }
    }
    
    private void Update()
    {
        if (player == null) return;
        
        // Test damage
        if (Input.GetKeyDown(testDamageKey))
        {
            Debug.Log($"Testing damage: {testDamageAmount}");
            player.TakeDamage(testDamageAmount);
            Debug.Log($"After damage - Health: {player.GetCurrentHealth()}/{player.GetMaxHealth()}, Shields: {player.GetCurrentShields()}");
        }
        
        // Test adding shields
        if (Input.GetKeyDown(testShieldKey))
        {
            Debug.Log("Adding shield");
            player.AddShields(1);
            Debug.Log($"After shield - Health: {player.GetCurrentHealth()}/{player.GetMaxHealth()}, Shields: {player.GetCurrentShields()}");
        }
    }
}