using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Mission and challenge system that tracks player progress and provides rewards.
/// Supports daily challenges and various mission types as specified in the README.
/// </summary>
public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance { get; private set; }

    [Header("Mission Settings")]
    [SerializeField] private int maxActiveMissions = 3;
    [SerializeField] private int maxDailyChallenges = 1;

    // Events
    public static System.Action<Mission> OnMissionCompleted;
    public static System.Action<Mission> OnMissionProgress;
    public static System.Action OnMissionsUpdated;

    private List<Mission> activeMissions = new List<Mission>();
    private List<Mission> dailyChallenges = new List<Mission>();
    private DateTime lastDailyReset;

    private const string LastDailyResetKey = "LastDailyReset";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadMissionData();
        InitializeMissions();
    }

    private void Start()
    {
        // Subscribe to game events
        PlayerController.OnNumberChanged += OnNumberChanged;
        CoinManager.OnCoinsChanged += OnCoinsCollected;
        ScoreManager.Instance.OnScoreUpdated += OnScoreUpdated;
        LevelManager.OnLevelChanged += OnLevelChanged;

        CheckDailyReset();
    }

    private void InitializeMissions()
    {
        if (activeMissions.Count == 0)
        {
            GenerateNewMissions();
        }

        if (dailyChallenges.Count == 0)
        {
            GenerateDailyChallenges();
        }
    }

    private void CheckDailyReset()
    {
        DateTime now = DateTime.Now;
        if (now.Date > lastDailyReset.Date)
        {
            ResetDailyChallenges();
            lastDailyReset = now;
            SaveMissionData();
        }
    }

    private void GenerateNewMissions()
    {
        activeMissions.Clear();

        // Generate random missions
        List<Mission> possibleMissions = new List<Mission>
        {
            new Mission("Coin Collector", "Collect 10 coins", MissionType.CollectCoins, 10, new Reward(RewardType.Coins, 20)),
            new Mission("High Scorer", "Reach a score of 50", MissionType.ReachScore, 50, new Reward(RewardType.Shield, 1)),
            new Mission("Shield Master", "Use 3 shields", MissionType.UseShields, 3, new Reward(RewardType.Coins, 15)),
            new Mission("Level Climber", "Reach level 5", MissionType.ReachLevel, 5, new Reward(RewardType.Coins, 25)),
            new Mission("Number Wizard", "Reach number 20", MissionType.ReachNumber, 20, new Reward(RewardType.Shield, 2)),
            new Mission("Survivor", "Survive for 60 seconds", MissionType.SurviveTime, 60, new Reward(RewardType.Coins, 30)),
            new Mission("Operator Expert", "Collect 15 operators", MissionType.CollectOperators, 15, new Reward(RewardType.Shield, 1))
        };

        // Select random missions
        while (activeMissions.Count < maxActiveMissions && possibleMissions.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, possibleMissions.Count);
            activeMissions.Add(possibleMissions[index]);
            possibleMissions.RemoveAt(index);
        }

        OnMissionsUpdated?.Invoke();
    }

    private void GenerateDailyChallenges()
    {
        dailyChallenges.Clear();

        // Generate more challenging daily missions
        List<Mission> possibleChallenges = new List<Mission>
        {
            new Mission("Daily Champion", "Score 100 points in one game", MissionType.ReachScore, 100, new Reward(RewardType.Coins, 50)),
            new Mission("Speed Demon", "Reach level 8", MissionType.ReachLevel, 8, new Reward(RewardType.Shield, 3)),
            new Mission("Coin Hoarder", "Collect 25 coins", MissionType.CollectCoins, 25, new Reward(RewardType.Coins, 40)),
            new Mission("Number Master", "Reach number 50", MissionType.ReachNumber, 50, new Reward(RewardType.Shield, 2)),
            new Mission("Marathon Runner", "Survive for 120 seconds", MissionType.SurviveTime, 120, new Reward(RewardType.Coins, 60))
        };

        // Select one daily challenge
        if (possibleChallenges.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, possibleChallenges.Count);
            Mission dailyChallenge = possibleChallenges[index];
            dailyChallenge.isDaily = true;
            dailyChallenges.Add(dailyChallenge);
        }

        OnMissionsUpdated?.Invoke();
    }

    private void ResetDailyChallenges()
    {
        dailyChallenges.Clear();
        GenerateDailyChallenges();
    }

    #region Event Handlers

    private void OnNumberChanged(int newNumber)
    {
        CheckMissionProgress(MissionType.ReachNumber, newNumber);
    }

    private void OnCoinsCollected(int totalCoins)
    {
        CheckMissionProgress(MissionType.CollectCoins, 1, true); // Increment by 1 each collection
    }

    private void OnScoreUpdated(int newScore)
    {
        CheckMissionProgress(MissionType.ReachScore, newScore);
    }

    private void OnLevelChanged(int newLevel)
    {
        CheckMissionProgress(MissionType.ReachLevel, newLevel);
    }

    public void OnShieldUsed()
    {
        CheckMissionProgress(MissionType.UseShields, 1, true);
    }

    public void OnOperatorCollected()
    {
        CheckMissionProgress(MissionType.CollectOperators, 1, true);
    }

    public void OnTimeSurvived(float seconds)
    {
        CheckMissionProgress(MissionType.SurviveTime, (int)seconds);
    }

    #endregion

    private void CheckMissionProgress(MissionType type, int value, bool isIncrement = false)
    {
        // Check active missions
        foreach (var mission in activeMissions)
        {
            if (mission.type == type && !mission.isCompleted)
            {
                UpdateMissionProgress(mission, value, isIncrement);
            }
        }

        // Check daily challenges
        foreach (var challenge in dailyChallenges)
        {
            if (challenge.type == type && !challenge.isCompleted)
            {
                UpdateMissionProgress(challenge, value, isIncrement);
            }
        }
    }

    private void UpdateMissionProgress(Mission mission, int value, bool isIncrement)
    {
        if (isIncrement)
        {
            mission.currentProgress += value;
        }
        else
        {
            mission.currentProgress = Mathf.Max(mission.currentProgress, value);
        }

        OnMissionProgress?.Invoke(mission);

        if (mission.currentProgress >= mission.targetValue && !mission.isCompleted)
        {
            CompleteMission(mission);
        }
    }

    private void CompleteMission(Mission mission)
    {
        mission.isCompleted = true;
        mission.completionDate = DateTime.Now;

        // Give reward
        GiveReward(mission.reward);

        OnMissionCompleted?.Invoke(mission);
        Debug.Log($"Mission completed: {mission.title}");

        // Generate new mission if it's not a daily challenge
        if (!mission.isDaily && activeMissions.Contains(mission))
        {
            activeMissions.Remove(mission);
            
            // Add a new random mission
            if (activeMissions.Count < maxActiveMissions)
            {
                GenerateNewMissions();
            }
        }
    }

    private void GiveReward(Reward reward)
    {
        switch (reward.type)
        {
            case RewardType.Coins:
                if (CoinManager.Instance != null)
                    CoinManager.Instance.AddCoins(reward.amount);
                break;
            case RewardType.Shield:
                PlayerController player = FindObjectOfType<PlayerController>();
                if (player != null)
                    player.AddShields(reward.amount);
                break;
        }
    }

    #region Public Interface

    public List<Mission> GetActiveMissions()
    {
        return new List<Mission>(activeMissions);
    }

    public List<Mission> GetDailyChallenges()
    {
        return new List<Mission>(dailyChallenges);
    }

    public void ResetAllMissions()
    {
        activeMissions.Clear();
        dailyChallenges.Clear();
        GenerateNewMissions();
        GenerateDailyChallenges();
    }

    #endregion

    #region Save/Load

    private void LoadMissionData()
    {
        string lastResetStr = PlayerPrefs.GetString(LastDailyResetKey, DateTime.Now.ToString());
        if (DateTime.TryParse(lastResetStr, out DateTime parsedDate))
        {
            lastDailyReset = parsedDate;
        }
        else
        {
            lastDailyReset = DateTime.Now;
        }
    }

    private void SaveMissionData()
    {
        PlayerPrefs.SetString(LastDailyResetKey, lastDailyReset.ToString());
    }

    #endregion

    private void OnDestroy()
    {
        PlayerController.OnNumberChanged -= OnNumberChanged;
        CoinManager.OnCoinsChanged -= OnCoinsCollected;
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnScoreUpdated -= OnScoreUpdated;
        LevelManager.OnLevelChanged -= OnLevelChanged;
    }
}

[System.Serializable]
public class Mission
{
    public string title;
    public string description;
    public MissionType type;
    public int targetValue;
    public int currentProgress;
    public Reward reward;
    public bool isCompleted;
    public bool isDaily;
    public DateTime completionDate;

    public Mission(string title, string description, MissionType type, int targetValue, Reward reward)
    {
        this.title = title;
        this.description = description;
        this.type = type;
        this.targetValue = targetValue;
        this.reward = reward;
        this.currentProgress = 0;
        this.isCompleted = false;
        this.isDaily = false;
    }
}

[System.Serializable]
public class Reward
{
    public RewardType type;
    public int amount;

    public Reward(RewardType type, int amount)
    {
        this.type = type;
        this.amount = amount;
    }
}

public enum MissionType
{
    CollectCoins,
    ReachScore,
    UseShields,
    ReachLevel,
    ReachNumber,
    SurviveTime,
    CollectOperators
}

public enum RewardType
{
    Coins,
    Shield
}