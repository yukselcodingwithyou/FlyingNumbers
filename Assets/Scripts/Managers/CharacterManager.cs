using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages unlockable characters (numbers 1, 2, 3, 4, etc.) with different wings and properties.
/// Characters can be unlocked through gameplay achievements or purchases.
/// </summary>
public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance { get; private set; }

    [Header("Character Configuration")]
    [SerializeField] private List<CharacterData> characters = new List<CharacterData>();
    [SerializeField] private int defaultCharacterIndex = 0;

    // Events
    public static System.Action<int> OnCharacterUnlocked;
    public static System.Action<int> OnCharacterSelected;
    public static System.Action OnCharacterDataUpdated;

    private int currentSelectedCharacter = 0;
    private const string SelectedCharacterKey = "SelectedCharacter";
    private const string UnlockedCharactersKey = "UnlockedCharacters";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeCharacters();
        LoadCharacterData();
    }

    private void Start()
    {
        // Subscribe to game events for unlocking characters
        ScoreManager.Instance.OnScoreUpdated += CheckScoreUnlocks;
        CoinManager.OnTotalCoinsChanged += CheckCoinUnlocks;
        LevelManager.OnLevelChanged += CheckLevelUnlocks;
    }

    private void InitializeCharacters()
    {
        if (characters.Count == 0)
        {
            // Create default character set if none configured
            characters.Add(new CharacterData(1, "One", "The original flying number", CharacterRarity.Common, 
                0, 0, 0, true, false)); // Starting character - always unlocked
            
            characters.Add(new CharacterData(2, "Two", "Twice as nice", CharacterRarity.Common, 
                50, 0, 0, false, false)); // Unlock at score 50
            
            characters.Add(new CharacterData(3, "Three", "Third time's the charm", CharacterRarity.Common, 
                100, 0, 0, false, false)); // Unlock at score 100
            
            characters.Add(new CharacterData(4, "Four", "Four-leaf lucky", CharacterRarity.Uncommon, 
                0, 50, 0, false, false)); // Unlock with 50 total coins
            
            characters.Add(new CharacterData(5, "Five", "High five!", CharacterRarity.Uncommon, 
                200, 0, 0, false, false)); // Unlock at score 200
            
            characters.Add(new CharacterData(6, "Six", "Six-shooter", CharacterRarity.Uncommon, 
                0, 100, 0, false, false)); // Unlock with 100 total coins
            
            characters.Add(new CharacterData(7, "Seven", "Lucky seven", CharacterRarity.Rare, 
                500, 0, 5, false, false)); // Unlock at score 500 and level 5
            
            characters.Add(new CharacterData(8, "Eight", "Infinity symbol", CharacterRarity.Rare, 
                0, 200, 0, false, false)); // Unlock with 200 total coins
            
            characters.Add(new CharacterData(9, "Nine", "Cloud nine", CharacterRarity.Rare, 
                1000, 0, 10, false, false)); // Unlock at score 1000 and level 10
            
            characters.Add(new CharacterData(0, "Zero", "The void master", CharacterRarity.Legendary, 
                0, 0, 0, false, true)); // Premium character - purchase only
        }
    }

    private void LoadCharacterData()
    {
        currentSelectedCharacter = PlayerPrefs.GetInt(SelectedCharacterKey, defaultCharacterIndex);
        
        // Load unlocked characters
        string unlockedData = PlayerPrefs.GetString(UnlockedCharactersKey, "0"); // Character 1 (index 0) is unlocked by default
        string[] unlockedIndices = unlockedData.Split(',');
        
        foreach (string indexStr in unlockedIndices)
        {
            if (int.TryParse(indexStr, out int index) && index >= 0 && index < characters.Count)
            {
                characters[index].isUnlocked = true;
            }
        }
        
        OnCharacterDataUpdated?.Invoke();
    }

    private void SaveCharacterData()
    {
        PlayerPrefs.SetInt(SelectedCharacterKey, currentSelectedCharacter);
        
        // Save unlocked characters
        List<string> unlockedIndices = new List<string>();
        for (int i = 0; i < characters.Count; i++)
        {
            if (characters[i].isUnlocked)
            {
                unlockedIndices.Add(i.ToString());
            }
        }
        
        PlayerPrefs.SetString(UnlockedCharactersKey, string.Join(",", unlockedIndices));
    }

    #region Character Unlocking

    private void CheckScoreUnlocks(int score)
    {
        for (int i = 0; i < characters.Count; i++)
        {
            var character = characters[i];
            if (!character.isUnlocked && !character.isPremium && 
                character.unlockScore > 0 && score >= character.unlockScore &&
                (character.unlockLevel == 0 || GetCurrentLevel() >= character.unlockLevel) &&
                (character.unlockCoins == 0 || GetTotalCoins() >= character.unlockCoins))
            {
                UnlockCharacter(i);
            }
        }
    }

    private void CheckCoinUnlocks(int totalCoins)
    {
        for (int i = 0; i < characters.Count; i++)
        {
            var character = characters[i];
            if (!character.isUnlocked && !character.isPremium && 
                character.unlockCoins > 0 && totalCoins >= character.unlockCoins &&
                (character.unlockScore == 0 || GetCurrentScore() >= character.unlockScore) &&
                (character.unlockLevel == 0 || GetCurrentLevel() >= character.unlockLevel))
            {
                UnlockCharacter(i);
            }
        }
    }

    private void CheckLevelUnlocks(int level)
    {
        for (int i = 0; i < characters.Count; i++)
        {
            var character = characters[i];
            if (!character.isUnlocked && !character.isPremium && 
                character.unlockLevel > 0 && level >= character.unlockLevel &&
                (character.unlockScore == 0 || GetCurrentScore() >= character.unlockScore) &&
                (character.unlockCoins == 0 || GetTotalCoins() >= character.unlockCoins))
            {
                UnlockCharacter(i);
            }
        }
    }

    public void UnlockCharacter(int characterIndex)
    {
        if (characterIndex < 0 || characterIndex >= characters.Count)
            return;

        if (characters[characterIndex].isUnlocked)
            return;

        characters[characterIndex].isUnlocked = true;
        SaveCharacterData();
        
        OnCharacterUnlocked?.Invoke(characterIndex);
        OnCharacterDataUpdated?.Invoke();
        
        Debug.Log($"Character unlocked: {characters[characterIndex].name}");
        
        // Show unlock notification
        if (UIManager.Instance != null)
        {
            // Would show character unlock popup
        }
    }

    public void UnlockPremiumCharacter(int characterIndex)
    {
        if (characterIndex < 0 || characterIndex >= characters.Count)
            return;

        var character = characters[characterIndex];
        if (!character.isPremium)
            return;

        UnlockCharacter(characterIndex);
    }

    #endregion

    #region Character Selection

    public void SelectCharacter(int characterIndex)
    {
        if (characterIndex < 0 || characterIndex >= characters.Count)
            return;

        if (!characters[characterIndex].isUnlocked)
        {
            Debug.Log("Cannot select locked character");
            return;
        }

        currentSelectedCharacter = characterIndex;
        SaveCharacterData();
        
        OnCharacterSelected?.Invoke(characterIndex);
        Debug.Log($"Character selected: {characters[characterIndex].name}");
    }

    #endregion

    #region Public Interface

    public CharacterData GetCurrentCharacter()
    {
        if (currentSelectedCharacter >= 0 && currentSelectedCharacter < characters.Count)
        {
            return characters[currentSelectedCharacter];
        }
        return characters[defaultCharacterIndex];
    }

    public CharacterData GetCharacter(int index)
    {
        if (index >= 0 && index < characters.Count)
        {
            return characters[index];
        }
        return null;
    }

    public List<CharacterData> GetAllCharacters()
    {
        return new List<CharacterData>(characters);
    }

    public List<CharacterData> GetUnlockedCharacters()
    {
        List<CharacterData> unlockedChars = new List<CharacterData>();
        foreach (var character in characters)
        {
            if (character.isUnlocked)
            {
                unlockedChars.Add(character);
            }
        }
        return unlockedChars;
    }

    public int GetSelectedCharacterIndex()
    {
        return currentSelectedCharacter;
    }

    public bool IsCharacterUnlocked(int index)
    {
        if (index >= 0 && index < characters.Count)
        {
            return characters[index].isUnlocked;
        }
        return false;
    }

    public int GetUnlockedCharacterCount()
    {
        int count = 0;
        foreach (var character in characters)
        {
            if (character.isUnlocked)
                count++;
        }
        return count;
    }

    #endregion

    #region Helper Methods

    private int GetCurrentScore()
    {
        return ScoreManager.Instance != null ? ScoreManager.Instance.GetScore() : 0;
    }

    private int GetTotalCoins()
    {
        return CoinManager.Instance != null ? CoinManager.Instance.GetTotalCoins() : 0;
    }

    private int GetCurrentLevel()
    {
        return LevelManager.Instance != null ? LevelManager.Instance.CurrentLevel : 1;
    }

    #endregion

    private void OnDestroy()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.OnScoreUpdated -= CheckScoreUnlocks;
        CoinManager.OnTotalCoinsChanged -= CheckCoinUnlocks;
        LevelManager.OnLevelChanged -= CheckLevelUnlocks;
    }
}

[System.Serializable]
public class CharacterData
{
    public int number;
    public string name;
    public string description;
    public CharacterRarity rarity;
    public int unlockScore;
    public int unlockCoins;
    public int unlockLevel;
    public bool isUnlocked;
    public bool isPremium;
    
    // Visual properties (would reference sprite/prefab assets)
    public string spritePath;
    public Color characterColor = Color.white;

    public CharacterData(int number, string name, string description, CharacterRarity rarity,
                        int unlockScore, int unlockCoins, int unlockLevel, bool isUnlocked, bool isPremium)
    {
        this.number = number;
        this.name = name;
        this.description = description;
        this.rarity = rarity;
        this.unlockScore = unlockScore;
        this.unlockCoins = unlockCoins;
        this.unlockLevel = unlockLevel;
        this.isUnlocked = isUnlocked;
        this.isPremium = isPremium;
    }
}

public enum CharacterRarity
{
    Common,
    Uncommon,
    Rare,
    Legendary
}