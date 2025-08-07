using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages in-app purchases including shield packs, coin doublers, and ad removal.
/// Simulates Unity IAP functionality as specified in the README ($0.05 for 5 shields).
/// </summary>
public class PurchaseManager : MonoBehaviour
{
    public static PurchaseManager Instance { get; private set; }

    [Header("Purchase Configuration")]
    [SerializeField] private bool purchasesEnabled = true;

    // Events
    public static System.Action<string> OnPurchaseSucceeded;
    public static System.Action<string> OnPurchaseFailed;
    public static System.Action OnPurchaseRestored;

    // Product definitions
    private Dictionary<string, Product> products = new Dictionary<string, Product>();

    // PlayerPrefs keys
    private const string CoinDoublerKey = "CoinDoubler";
    private const string AdsRemovedKey = "AdsRemoved";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializePurchases();
    }

    private void InitializePurchases()
    {
        if (!purchasesEnabled)
        {
            Debug.Log("Purchases disabled");
            return;
        }

        // Define products as specified in README
        products.Add("shield_pack_5", new Product("shield_pack_5", "5 Shields", "$0.05", 5, ProductType.Consumable));
        products.Add("coin_doubler", new Product("coin_doubler", "Coin Doubler", "$0.99", 0, ProductType.NonConsumable));
        products.Add("remove_ads", new Product("remove_ads", "Remove Ads", "$1.99", 0, ProductType.NonConsumable));
        
        // Additional shield packs
        products.Add("shield_pack_10", new Product("shield_pack_10", "10 Shields", "$0.09", 10, ProductType.Consumable));
        products.Add("shield_pack_25", new Product("shield_pack_25", "25 Shields", "$0.19", 25, ProductType.Consumable));
        
        // Coin packs
        products.Add("coin_pack_100", new Product("coin_pack_100", "100 Coins", "$0.99", 100, ProductType.Consumable));
        products.Add("coin_pack_500", new Product("coin_pack_500", "500 Coins", "$3.99", 500, ProductType.Consumable));

        Debug.Log("Purchase system initialized with " + products.Count + " products");
        
        // Restore non-consumable purchases
        RestorePurchases();
    }

    #region Purchase Methods

    public void PurchaseProduct(string productId)
    {
        if (!purchasesEnabled)
        {
            Debug.Log("Purchases are disabled");
            return;
        }

        if (!products.ContainsKey(productId))
        {
            Debug.LogError("Product not found: " + productId);
            OnPurchaseFailed?.Invoke(productId);
            return;
        }

        Product product = products[productId];
        Debug.Log($"Attempting to purchase: {product.title} ({product.price})");

        // Simulate purchase process
        StartCoroutine(SimulatePurchase(productId, product));
    }

    private System.Collections.IEnumerator SimulatePurchase(string productId, Product product)
    {
        // Simulate network delay
        yield return new WaitForSeconds(1f);

        // Simulate successful purchase (90% success rate for demo)
        bool purchaseSuccessful = UnityEngine.Random.value > 0.1f;

        if (purchaseSuccessful)
        {
            ProcessSuccessfulPurchase(productId, product);
            OnPurchaseSucceeded?.Invoke(productId);
            Debug.Log($"Purchase successful: {product.title}");
        }
        else
        {
            OnPurchaseFailed?.Invoke(productId);
            Debug.Log($"Purchase failed: {product.title}");
        }
    }

    private void ProcessSuccessfulPurchase(string productId, Product product)
    {
        switch (productId)
        {
            case "shield_pack_5":
            case "shield_pack_10":
            case "shield_pack_25":
                GiveShields(product.amount);
                break;
                
            case "coin_pack_100":
            case "coin_pack_500":
                GiveCoins(product.amount);
                break;
                
            case "coin_doubler":
                EnableCoinDoubler();
                break;
                
            case "remove_ads":
                RemoveAds();
                break;
        }

        // Save purchase state for non-consumables
        if (product.type == ProductType.NonConsumable)
        {
            PlayerPrefs.SetInt(productId, 1);
        }
    }

    #endregion

    #region Purchase Effects

    private void GiveShields(int amount)
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.AddShields(amount);
        }
        
        Debug.Log($"Gave {amount} shields to player");
    }

    private void GiveCoins(int amount)
    {
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.AddCoins(amount);
        }
        
        Debug.Log($"Gave {amount} coins to player");
    }

    private void EnableCoinDoubler()
    {
        PlayerPrefs.SetInt(CoinDoublerKey, 1);
        Debug.Log("Coin doubler enabled permanently");
        
        // Notify coin manager
        if (CoinManager.Instance != null)
        {
            // Would implement coin doubler logic in CoinManager
        }
    }

    private void RemoveAds()
    {
        PlayerPrefs.SetInt(AdsRemovedKey, 1);
        
        if (AdManager.Instance != null)
        {
            AdManager.Instance.RemoveAds();
        }
        
        Debug.Log("Ads removed permanently");
    }

    #endregion

    #region Restore Purchases

    public void RestorePurchases()
    {
        Debug.Log("Restoring purchases...");

        // Restore coin doubler
        if (PlayerPrefs.GetInt(CoinDoublerKey, 0) == 1)
        {
            EnableCoinDoubler();
        }

        // Restore ad removal
        if (PlayerPrefs.GetInt(AdsRemovedKey, 0) == 1)
        {
            RemoveAds();
        }

        // Restore other non-consumables
        foreach (var product in products.Values)
        {
            if (product.type == ProductType.NonConsumable)
            {
                if (PlayerPrefs.GetInt(product.id, 0) == 1)
                {
                    Debug.Log($"Restored purchase: {product.title}");
                }
            }
        }

        OnPurchaseRestored?.Invoke();
        Debug.Log("Purchase restoration complete");
    }

    #endregion

    #region Public Interface

    public Product GetProduct(string productId)
    {
        return products.ContainsKey(productId) ? products[productId] : null;
    }

    public List<Product> GetAllProducts()
    {
        return new List<Product>(products.Values);
    }

    public List<Product> GetProductsByType(ProductType type)
    {
        List<Product> filteredProducts = new List<Product>();
        foreach (var product in products.Values)
        {
            if (product.type == type)
            {
                filteredProducts.Add(product);
            }
        }
        return filteredProducts;
    }

    public bool IsPurchased(string productId)
    {
        return PlayerPrefs.GetInt(productId, 0) == 1;
    }

    public bool IsCoinDoublerActive()
    {
        return PlayerPrefs.GetInt(CoinDoublerKey, 0) == 1;
    }

    public bool AreAdsRemoved()
    {
        return PlayerPrefs.GetInt(AdsRemovedKey, 0) == 1;
    }

    // Quick purchase methods for UI
    public void PurchaseShieldPack5() => PurchaseProduct("shield_pack_5");
    public void PurchaseShieldPack10() => PurchaseProduct("shield_pack_10");
    public void PurchaseShieldPack25() => PurchaseProduct("shield_pack_25");
    public void PurchaseCoinDoubler() => PurchaseProduct("coin_doubler");
    public void PurchaseRemoveAds() => PurchaseProduct("remove_ads");
    public void PurchaseCoinPack100() => PurchaseProduct("coin_pack_100");
    public void PurchaseCoinPack500() => PurchaseProduct("coin_pack_500");

    #endregion

    private void OnDestroy()
    {
        // Clean up purchase system
    }
}

[System.Serializable]
public class Product
{
    public string id;
    public string title;
    public string price;
    public int amount;
    public ProductType type;

    public Product(string id, string title, string price, int amount, ProductType type)
    {
        this.id = id;
        this.title = title;
        this.price = price;
        this.amount = amount;
        this.type = type;
    }
}

public enum ProductType
{
    Consumable,    // Can be purchased multiple times
    NonConsumable  // Can only be purchased once
}