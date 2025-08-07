using UnityEngine;
using System.Collections;

/// <summary>
/// Manages all advertisement functionality including banner, interstitial, and rewarded ads.
/// Integrates with Unity Ads or other ad networks as specified in the README.
/// </summary>
public class AdManager : MonoBehaviour
{
    public static AdManager Instance { get; private set; }

    [Header("Ad Configuration")]
    [SerializeField] private bool adsEnabled = true;
    [SerializeField] private string gameId = "your_game_id";
    [SerializeField] private bool testMode = true;

    [Header("Ad Timing")]
    [SerializeField] private int interstitialFrequency = 3; // Show after every 3-4 restarts
    [SerializeField] private float bannerRefreshRate = 30f; // Refresh banner every 30 seconds

    [Header("Reward Amounts")]
    [SerializeField] private int rewardedAdCoins = 10;
    [SerializeField] private int rewardedAdShields = 1;

    // Events
    public static System.Action OnInterstitialShown;
    public static System.Action OnInterstitialClosed;
    public static System.Action OnRewardedAdRewardGiven;
    public static System.Action OnBannerShown;

    private int gameRestartCount = 0;
    private bool isInterstitialReady = false;
    private bool isRewardedAdReady = false;
    private bool isBannerShown = false;
    private bool adsRemoved = false;

    // Ad unit IDs (would be set from ad network)
    private const string InterstitialAdUnitId = "interstitial_placement";
    private const string RewardedAdUnitId = "rewarded_placement";
    private const string BannerAdUnitId = "banner_placement";

    // PlayerPrefs keys
    private const string AdsRemovedKey = "AdsRemoved";
    private const string RestartCountKey = "RestartCount";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadAdSettings();
        InitializeAds();
    }

    private void Start()
    {
        // Subscribe to game events
        GameManager.OnGameEnded += OnGameEnded;
        GameManager.OnGameStarted += OnGameStarted;
    }

    private void LoadAdSettings()
    {
        adsRemoved = PlayerPrefs.GetInt(AdsRemovedKey, 0) == 1;
        gameRestartCount = PlayerPrefs.GetInt(RestartCountKey, 0);
    }

    private void SaveAdSettings()
    {
        PlayerPrefs.SetInt(AdsRemovedKey, adsRemoved ? 1 : 0);
        PlayerPrefs.SetInt(RestartCountKey, gameRestartCount);
    }

    #region Ad Initialization

    private void InitializeAds()
    {
        if (!adsEnabled || adsRemoved)
        {
            Debug.Log("Ads disabled or removed");
            return;
        }

        // Initialize ad network (Unity Ads example)
        // Advertisement.Initialize(gameId, testMode, this);
        
        // Simulate ad initialization
        StartCoroutine(SimulateAdInitialization());
    }

    private IEnumerator SimulateAdInitialization()
    {
        yield return new WaitForSeconds(1f);
        
        isInterstitialReady = true;
        isRewardedAdReady = true;
        
        Debug.Log("Ads initialized (simulated)");
        
        // Show banner ad if not removed
        if (!adsRemoved)
        {
            ShowBannerAd();
        }
    }

    #endregion

    #region Banner Ads

    public void ShowBannerAd()
    {
        if (!adsEnabled || adsRemoved || isBannerShown)
            return;

        // Show banner ad at bottom of screen
        // Advertisement.Banner.Show(BannerAdUnitId);
        
        isBannerShown = true;
        OnBannerShown?.Invoke();
        
        Debug.Log("Banner ad shown (simulated)");
        
        // Start refresh timer
        StartCoroutine(RefreshBannerAd());
    }

    public void HideBannerAd()
    {
        if (!isBannerShown)
            return;

        // Advertisement.Banner.Hide();
        
        isBannerShown = false;
        Debug.Log("Banner ad hidden");
    }

    private IEnumerator RefreshBannerAd()
    {
        while (isBannerShown && !adsRemoved)
        {
            yield return new WaitForSeconds(bannerRefreshRate);
            
            if (isBannerShown)
            {
                // Refresh banner
                Debug.Log("Banner ad refreshed (simulated)");
            }
        }
    }

    #endregion

    #region Interstitial Ads

    public void ShowInterstitialAd()
    {
        if (!adsEnabled || adsRemoved || !isInterstitialReady)
            return;

        // Show interstitial ad
        // Advertisement.Show(InterstitialAdUnitId);
        
        StartCoroutine(SimulateInterstitialAd());
    }

    private IEnumerator SimulateInterstitialAd()
    {
        OnInterstitialShown?.Invoke();
        
        // Pause game during ad
        if (GameManager.Instance != null)
            GameManager.Instance.PauseGame();
        
        Debug.Log("Interstitial ad shown (simulated)");
        
        // Simulate ad duration
        yield return new WaitForSecondsRealtime(3f);
        
        // Resume game after ad
        if (GameManager.Instance != null)
            GameManager.Instance.ResumeGame();
        
        OnInterstitialClosed?.Invoke();
        Debug.Log("Interstitial ad closed");
        
        // Reload interstitial for next use
        LoadInterstitialAd();
    }

    private void LoadInterstitialAd()
    {
        if (!adsEnabled || adsRemoved)
            return;

        // Load next interstitial
        // Advertisement.Load(InterstitialAdUnitId, this);
        
        StartCoroutine(SimulateAdLoad());
    }

    private IEnumerator SimulateAdLoad()
    {
        isInterstitialReady = false;
        yield return new WaitForSeconds(1f);
        isInterstitialReady = true;
        Debug.Log("Interstitial ad loaded (simulated)");
    }

    #endregion

    #region Rewarded Ads

    public void ShowRewardedAd(RewardType rewardType)
    {
        if (!adsEnabled || !isRewardedAdReady)
        {
            Debug.Log("Rewarded ad not ready");
            return;
        }

        // Show rewarded ad
        // Advertisement.Show(RewardedAdUnitId);
        
        StartCoroutine(SimulateRewardedAd(rewardType));
    }

    private IEnumerator SimulateRewardedAd(RewardType rewardType)
    {
        Debug.Log("Rewarded ad shown (simulated)");
        
        // Pause game during ad
        if (GameManager.Instance != null)
            GameManager.Instance.PauseGame();
        
        // Simulate ad duration
        yield return new WaitForSecondsRealtime(5f);
        
        // Give reward
        GiveReward(rewardType);
        
        // Resume game after ad
        if (GameManager.Instance != null)
            GameManager.Instance.ResumeGame();
        
        OnRewardedAdRewardGiven?.Invoke();
        Debug.Log("Rewarded ad completed, reward given");
        
        // Reload rewarded ad
        LoadRewardedAd();
    }

    private void GiveReward(RewardType rewardType)
    {
        switch (rewardType)
        {
            case RewardType.Coins:
                if (CoinManager.Instance != null)
                    CoinManager.Instance.AddCoins(rewardedAdCoins);
                break;
            case RewardType.Shield:
                PlayerController player = FindObjectOfType<PlayerController>();
                if (player != null)
                    player.AddShields(rewardedAdShields);
                break;
            case RewardType.Revive:
                // Implement revive functionality
                RevivePlayer();
                break;
        }
    }

    private void RevivePlayer()
    {
        // Give player a shield and continue game
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.AddShields(1);
            // Reset player position and continue game
            if (GameManager.Instance != null)
                GameManager.Instance.ResumeGame();
        }
    }

    private void LoadRewardedAd()
    {
        if (!adsEnabled)
            return;

        // Load next rewarded ad
        // Advertisement.Load(RewardedAdUnitId, this);
        
        StartCoroutine(SimulateRewardedAdLoad());
    }

    private IEnumerator SimulateRewardedAdLoad()
    {
        isRewardedAdReady = false;
        yield return new WaitForSeconds(1f);
        isRewardedAdReady = true;
        Debug.Log("Rewarded ad loaded (simulated)");
    }

    #endregion

    #region Event Handlers

    private void OnGameEnded()
    {
        gameRestartCount++;
        SaveAdSettings();
        
        // Show interstitial ad every few restarts
        if (gameRestartCount % interstitialFrequency == 0)
        {
            ShowInterstitialAd();
        }
    }

    private void OnGameStarted()
    {
        // Ensure banner is shown during gameplay
        if (!adsRemoved)
        {
            ShowBannerAd();
        }
    }

    #endregion

    #region Public Interface

    public bool IsRewardedAdReady()
    {
        return isRewardedAdReady && adsEnabled;
    }

    public bool IsInterstitialReady()
    {
        return isInterstitialReady && adsEnabled && !adsRemoved;
    }

    public void RemoveAds()
    {
        adsRemoved = true;
        SaveAdSettings();
        
        HideBannerAd();
        Debug.Log("Ads removed via purchase");
    }

    public bool AreAdsRemoved()
    {
        return adsRemoved;
    }

    public void ShowRewardedAdForCoins()
    {
        ShowRewardedAd(RewardType.Coins);
    }

    public void ShowRewardedAdForShield()
    {
        ShowRewardedAd(RewardType.Shield);
    }

    public void ShowRewardedAdForRevive()
    {
        ShowRewardedAd(RewardType.Revive);
    }

    #endregion

    private void OnDestroy()
    {
        GameManager.OnGameEnded -= OnGameEnded;
        GameManager.OnGameStarted -= OnGameStarted;
    }
}

public enum AdRewardType
{
    Coins,
    Shield,
    Revive
}