using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the coin collection system and shield conversion.
/// Tracks coins collected and converts 3 coins to 1 shield as specified in the README.
/// </summary>
public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }

    [Header("Coin Settings")]
    [SerializeField] private int coinsPerShield = 3; // Coins needed for 1 shield
    
    [Header("UI References")]
    [SerializeField] private Text coinCountText; // Display current coins
    [SerializeField] private Text totalCoinsText; // Display total coins collected
    
    // Events for other systems
    public static System.Action<int> OnCoinsChanged;
    public static System.Action<int> OnTotalCoinsChanged;
    public static System.Action OnShieldEarned;

    private int currentCoins = 0; // Coins towards next shield
    private int totalCoins = 0; // Total coins collected (persistent)
    
    private const string TotalCoinsKey = "TotalCoins";

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Load persistent data
        totalCoins = PlayerPrefs.GetInt(TotalCoinsKey, 0);
        UpdateUI();
    }

    /// <summary>
    /// Called when a coin is collected
    /// </summary>
    public void CollectCoin()
    {
        currentCoins++;
        totalCoins++;
        
        // Save total coins persistently
        PlayerPrefs.SetInt(TotalCoinsKey, totalCoins);
        
        // Check if we have enough coins for a shield
        if (currentCoins >= coinsPerShield)
        {
            // Convert coins to shield
            currentCoins -= coinsPerShield;
            
            // Give shield to player
            PlayerController player = FindObjectOfType<PlayerController>();
            if (player != null)
            {
                player.AddShields(1);
            }
            
            OnShieldEarned?.Invoke();
        }
        
        UpdateUI();
        OnCoinsChanged?.Invoke(currentCoins);
        OnTotalCoinsChanged?.Invoke(totalCoins);
    }

    /// <summary>
    /// Updates the UI displays
    /// </summary>
    private void UpdateUI()
    {
        if (coinCountText != null)
        {
            coinCountText.text = $"{currentCoins}/{coinsPerShield}";
        }
        
        if (totalCoinsText != null)
        {
            totalCoinsText.text = totalCoins.ToString();
        }
    }

    /// <summary>
    /// Resets current coins (for new game)
    /// </summary>
    public void ResetCurrentCoins()
    {
        currentCoins = 0;
        UpdateUI();
        OnCoinsChanged?.Invoke(currentCoins);
    }

    /// <summary>
    /// Spend coins (for purchases, power-ups)
    /// </summary>
    public bool SpendCoins(int amount)
    {
        if (totalCoins >= amount)
        {
            totalCoins -= amount;
            PlayerPrefs.SetInt(TotalCoinsKey, totalCoins);
            UpdateUI();
            OnTotalCoinsChanged?.Invoke(totalCoins);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Get current coins towards next shield
    /// </summary>
    public int GetCurrentCoins()
    {
        return currentCoins;
    }

    /// <summary>
    /// Get total coins collected
    /// </summary>
    public int GetTotalCoins()
    {
        return totalCoins;
    }

    /// <summary>
    /// Add coins directly (for rewards, ads)
    /// </summary>
    public void AddCoins(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            CollectCoin();
        }
    }
}