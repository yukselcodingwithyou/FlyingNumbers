using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Manages local and online leaderboards for score sharing and competition.
/// Provides functionality for name input, score submission, and leaderboard display.
/// </summary>
public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance { get; private set; }

    [Header("Leaderboard Settings")]
    [SerializeField] private int maxLocalEntries = 10;
    [SerializeField] private int maxGlobalEntries = 100;

    [Header("UI References")]
    [SerializeField] private GameObject leaderboardPanel;
    [SerializeField] private Transform localLeaderboardParent;
    [SerializeField] private Transform globalLeaderboardParent;
    [SerializeField] private GameObject leaderboardEntryPrefab;

    // Events
    public static System.Action<LeaderboardEntry> OnNewHighScore;
    public static System.Action OnLeaderboardUpdated;

    private List<LeaderboardEntry> localLeaderboard = new List<LeaderboardEntry>();
    private List<LeaderboardEntry> globalLeaderboard = new List<LeaderboardEntry>();
    private string playerName = "Anonymous";

    private const string PlayerNameKey = "PlayerName";
    private const string LocalLeaderboardKey = "LocalLeaderboard";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadLeaderboardData();
    }

    private void Start()
    {
        // Subscribe to game events
        ScoreManager.Instance.OnScoreUpdated += CheckForNewRecord;
        PlayerController.OnPlayerDeath += SubmitScore;
    }

    private void LoadLeaderboardData()
    {
        // Load player name
        playerName = PlayerPrefs.GetString(PlayerNameKey, "Anonymous");

        // Load local leaderboard
        string localData = PlayerPrefs.GetString(LocalLeaderboardKey, "");
        if (!string.IsNullOrEmpty(localData))
        {
            try
            {
                string[] entries = localData.Split('|');
                foreach (string entry in entries)
                {
                    if (!string.IsNullOrEmpty(entry))
                    {
                        string[] parts = entry.Split(',');
                        if (parts.Length >= 3)
                        {
                            LeaderboardEntry leaderboardEntry = new LeaderboardEntry
                            {
                                playerName = parts[0],
                                score = int.Parse(parts[1]),
                                date = parts[2]
                            };
                            localLeaderboard.Add(leaderboardEntry);
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error loading leaderboard data: " + e.Message);
                localLeaderboard.Clear();
            }
        }

        SortLocalLeaderboard();
    }

    private void SaveLeaderboardData()
    {
        // Save player name
        PlayerPrefs.SetString(PlayerNameKey, playerName);

        // Save local leaderboard
        List<string> entries = new List<string>();
        foreach (var entry in localLeaderboard)
        {
            entries.Add($"{entry.playerName},{entry.score},{entry.date}");
        }
        PlayerPrefs.SetString(LocalLeaderboardKey, string.Join("|", entries));
    }

    #region Score Submission

    private void CheckForNewRecord(int currentScore)
    {
        // Check if current score would make it to leaderboard
        if (localLeaderboard.Count < maxLocalEntries || currentScore > localLeaderboard.Last().score)
        {
            // This is a potential new record
        }
    }

    private void SubmitScore()
    {
        if (ScoreManager.Instance == null) return;

        int finalScore = ScoreManager.Instance.GetScore();
        if (finalScore <= 0) return;

        LeaderboardEntry newEntry = new LeaderboardEntry
        {
            playerName = playerName,
            score = finalScore,
            date = System.DateTime.Now.ToString("yyyy-MM-dd")
        };

        AddToLocalLeaderboard(newEntry);
        
        // Submit to global leaderboard if online features are available
        SubmitToGlobalLeaderboard(newEntry);
    }

    private void AddToLocalLeaderboard(LeaderboardEntry entry)
    {
        localLeaderboard.Add(entry);
        SortLocalLeaderboard();

        // Remove excess entries
        if (localLeaderboard.Count > maxLocalEntries)
        {
            localLeaderboard.RemoveRange(maxLocalEntries, localLeaderboard.Count - maxLocalEntries);
        }

        SaveLeaderboardData();
        OnLeaderboardUpdated?.Invoke();

        // Check if this is a new high score
        if (localLeaderboard.Count > 0 && localLeaderboard[0].score == entry.score)
        {
            OnNewHighScore?.Invoke(entry);
            Debug.Log($"New high score: {entry.score} by {entry.playerName}!");
        }
    }

    private void SortLocalLeaderboard()
    {
        localLeaderboard = localLeaderboard.OrderByDescending(entry => entry.score).ToList();
    }

    private void SubmitToGlobalLeaderboard(LeaderboardEntry entry)
    {
        // This would integrate with online services like Unity Cloud, Firebase, etc.
        // For now, we'll simulate it
        Debug.Log($"Submitting to global leaderboard: {entry.playerName} - {entry.score}");
        
        // Add to simulated global leaderboard
        globalLeaderboard.Add(entry);
        globalLeaderboard = globalLeaderboard.OrderByDescending(e => e.score).Take(maxGlobalEntries).ToList();
    }

    #endregion

    #region UI Management

    public void ShowLeaderboard()
    {
        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(true);
            RefreshLeaderboardDisplay();
        }
    }

    public void HideLeaderboard()
    {
        if (leaderboardPanel != null)
        {
            leaderboardPanel.SetActive(false);
        }
    }

    private void RefreshLeaderboardDisplay()
    {
        RefreshLocalLeaderboard();
        RefreshGlobalLeaderboard();
    }

    private void RefreshLocalLeaderboard()
    {
        if (localLeaderboardParent == null || leaderboardEntryPrefab == null) return;

        // Clear existing entries
        foreach (Transform child in localLeaderboardParent)
        {
            Destroy(child.gameObject);
        }

        // Create new entries
        for (int i = 0; i < localLeaderboard.Count && i < maxLocalEntries; i++)
        {
            var entry = localLeaderboard[i];
            GameObject entryObj = Instantiate(leaderboardEntryPrefab, localLeaderboardParent);
            
            // Configure entry (assuming it has specific components)
            var entryScript = entryObj.GetComponent<LeaderboardEntryUI>();
            if (entryScript != null)
            {
                entryScript.SetEntry(i + 1, entry.playerName, entry.score, entry.date);
            }
        }
    }

    private void RefreshGlobalLeaderboard()
    {
        if (globalLeaderboardParent == null || leaderboardEntryPrefab == null) return;

        // Clear existing entries
        foreach (Transform child in globalLeaderboardParent)
        {
            Destroy(child.gameObject);
        }

        // Create new entries
        for (int i = 0; i < globalLeaderboard.Count && i < maxGlobalEntries; i++)
        {
            var entry = globalLeaderboard[i];
            GameObject entryObj = Instantiate(leaderboardEntryPrefab, globalLeaderboardParent);
            
            var entryScript = entryObj.GetComponent<LeaderboardEntryUI>();
            if (entryScript != null)
            {
                entryScript.SetEntry(i + 1, entry.playerName, entry.score, entry.date);
            }
        }
    }

    #endregion

    #region Social Sharing

    public void ShareScore(int score)
    {
        string shareText = $"I just scored {score} points in Flying Numbers! Can you beat my score?";
        
        #if UNITY_ANDROID
        ShareOnAndroid(shareText);
        #elif UNITY_IOS
        ShareOnIOS(shareText);
        #else
        Debug.Log($"Share: {shareText}");
        #endif
    }

    #if UNITY_ANDROID
    private void ShareOnAndroid(string text)
    {
        AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
        AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
        intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
        intentObject.Call<AndroidJavaObject>("setType", "text/plain");
        intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), text);
        
        AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
        currentActivity.Call("startActivity", intentObject);
    }
    #endif

    #if UNITY_IOS
    private void ShareOnIOS(string text)
    {
        // iOS sharing implementation would go here
        Debug.Log($"iOS Share: {text}");
    }
    #endif

    #endregion

    #region Public Interface

    public void SetPlayerName(string name)
    {
        playerName = string.IsNullOrEmpty(name) ? "Anonymous" : name;
        SaveLeaderboardData();
    }

    public string GetPlayerName()
    {
        return playerName;
    }

    public List<LeaderboardEntry> GetLocalLeaderboard()
    {
        return new List<LeaderboardEntry>(localLeaderboard);
    }

    public List<LeaderboardEntry> GetGlobalLeaderboard()
    {
        return new List<LeaderboardEntry>(globalLeaderboard);
    }

    public int GetPlayerRank(int score)
    {
        int rank = 1;
        foreach (var entry in localLeaderboard)
        {
            if (score > entry.score)
                break;
            rank++;
        }
        return rank;
    }

    public void ClearLocalLeaderboard()
    {
        localLeaderboard.Clear();
        SaveLeaderboardData();
        OnLeaderboardUpdated?.Invoke();
    }

    #endregion

    private void OnDestroy()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnScoreUpdated -= CheckForNewRecord;
        PlayerController.OnPlayerDeath -= SubmitScore;
    }
}

[System.Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public int score;
    public string date;
    public bool isGlobal;
}

/// <summary>
/// UI component for individual leaderboard entries
/// </summary>
public class LeaderboardEntryUI : MonoBehaviour
{
    [Header("UI Elements")]
    public UnityEngine.UI.Text rankText;
    public UnityEngine.UI.Text nameText;
    public UnityEngine.UI.Text scoreText;
    public UnityEngine.UI.Text dateText;

    public void SetEntry(int rank, string playerName, int score, string date)
    {
        if (rankText != null) rankText.text = rank.ToString();
        if (nameText != null) nameText.text = playerName;
        if (scoreText != null) scoreText.text = score.ToString();
        if (dateText != null) dateText.text = date;
    }
}