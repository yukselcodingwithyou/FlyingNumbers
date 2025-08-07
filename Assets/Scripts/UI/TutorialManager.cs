using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Tutorial system for first-time players to learn game mechanics.
/// Provides step-by-step guidance through core gameplay features.
/// </summary>
public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("Tutorial UI")]
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private Text tutorialText;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button skipButton;
    [SerializeField] private GameObject tutorialArrow;
    [SerializeField] private GameObject tutorialHighlight;

    [Header("Tutorial Steps")]
    [SerializeField] private List<TutorialStep> tutorialSteps = new List<TutorialStep>();

    private int currentStepIndex = 0;
    private bool isTutorialActive = false;
    private bool tutorialCompleted = false;
    private Coroutine currentStepCoroutine;

    // Events
    public static System.Action OnTutorialStarted;
    public static System.Action OnTutorialCompleted;
    public static System.Action<int> OnTutorialStepChanged;

    private const string TutorialCompletedKey = "TutorialCompleted";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeTutorial();
    }

    private void Start()
    {
        CheckTutorialStatus();
    }

    private void InitializeTutorial()
    {
        // Create default tutorial steps if none configured
        if (tutorialSteps.Count == 0)
        {
            CreateDefaultTutorialSteps();
        }

        // Setup UI
        if (nextButton != null)
            nextButton.onClick.AddListener(NextStep);
        
        if (skipButton != null)
            skipButton.onClick.AddListener(SkipTutorial);

        // Hide tutorial elements initially
        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);
        
        if (tutorialArrow != null)
            tutorialArrow.SetActive(false);
        
        if (tutorialHighlight != null)
            tutorialHighlight.SetActive(false);
    }

    private void CreateDefaultTutorialSteps()
    {
        tutorialSteps.Add(new TutorialStep(
            "Welcome to Flying Numbers!",
            "Tap or press SPACE to flap your wings and stay airborne!",
            TutorialStepType.Text,
            3f
        ));

        tutorialSteps.Add(new TutorialStep(
            "Collect Operators",
            "Fly through math operators to change your number. Avoid ×0 unless you have a shield!",
            TutorialStepType.WaitForOperator,
            0f
        ));

        tutorialSteps.Add(new TutorialStep(
            "Gather Coins",
            "Collect coins! Every 3 coins gives you 1 shield for protection.",
            TutorialStepType.WaitForCoin,
            0f
        ));

        tutorialSteps.Add(new TutorialStep(
            "Shield Protection",
            "Shields protect you from obstacles and dangerous operators like ×0.",
            TutorialStepType.Text,
            3f
        ));

        tutorialSteps.Add(new TutorialStep(
            "Avoid Obstacles",
            "Dodge pipes and mines! They'll end your game unless you have a shield.",
            TutorialStepType.Text,
            3f
        ));

        tutorialSteps.Add(new TutorialStep(
            "Level Up",
            "As your score increases, you'll level up! Higher levels mean faster gameplay.",
            TutorialStepType.Text,
            3f
        ));

        tutorialSteps.Add(new TutorialStep(
            "Power-ups",
            "Look out for special power-ups that give you temporary abilities!",
            TutorialStepType.Text,
            3f
        ));

        tutorialSteps.Add(new TutorialStep(
            "Ready to Fly!",
            "You're all set! Compete for high scores and unlock new characters. Good luck!",
            TutorialStepType.Text,
            4f
        ));
    }

    private void CheckTutorialStatus()
    {
        tutorialCompleted = PlayerPrefs.GetInt(TutorialCompletedKey, 0) == 1;

        if (!tutorialCompleted)
        {
            StartTutorial();
        }
    }

    public void StartTutorial()
    {
        if (isTutorialActive || tutorialCompleted)
            return;

        isTutorialActive = true;
        currentStepIndex = 0;

        // Pause the game
        Time.timeScale = 0f;

        OnTutorialStarted?.Invoke();
        ShowCurrentStep();

        Debug.Log("Tutorial started");
    }

    public void NextStep()
    {
        if (!isTutorialActive)
            return;

        currentStepIndex++;

        if (currentStepIndex >= tutorialSteps.Count)
        {
            CompleteTutorial();
        }
        else
        {
            ShowCurrentStep();
        }
    }

    public void SkipTutorial()
    {
        if (!isTutorialActive)
            return;

        CompleteTutorial();
    }

    private void ShowCurrentStep()
    {
        if (currentStepIndex < 0 || currentStepIndex >= tutorialSteps.Count)
            return;

        TutorialStep step = tutorialSteps[currentStepIndex];

        // Show tutorial panel
        if (tutorialPanel != null)
            tutorialPanel.SetActive(true);

        // Update text
        if (tutorialText != null)
        {
            tutorialText.text = $"{step.title}\n\n{step.description}";
        }

        // Handle different step types
        if (currentStepCoroutine != null)
        {
            StopCoroutine(currentStepCoroutine);
        }

        switch (step.stepType)
        {
            case TutorialStepType.Text:
                currentStepCoroutine = StartCoroutine(HandleTextStep(step));
                break;
            case TutorialStepType.WaitForInput:
                currentStepCoroutine = StartCoroutine(HandleInputStep(step));
                break;
            case TutorialStepType.WaitForOperator:
                currentStepCoroutine = StartCoroutine(HandleOperatorStep(step));
                break;
            case TutorialStepType.WaitForCoin:
                currentStepCoroutine = StartCoroutine(HandleCoinStep(step));
                break;
        }

        OnTutorialStepChanged?.Invoke(currentStepIndex);
    }

    private IEnumerator HandleTextStep(TutorialStep step)
    {
        // Show next button after delay or immediately
        if (nextButton != null)
            nextButton.gameObject.SetActive(true);

        if (step.duration > 0)
        {
            yield return new WaitForSecondsRealtime(step.duration);
            NextStep();
        }
    }

    private IEnumerator HandleInputStep(TutorialStep step)
    {
        if (nextButton != null)
            nextButton.gameObject.SetActive(false);

        // Wait for player input
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                NextStep();
                break;
            }
            yield return null;
        }
    }

    private IEnumerator HandleOperatorStep(TutorialStep step)
    {
        if (nextButton != null)
            nextButton.gameObject.SetActive(false);

        // Resume game temporarily to allow interaction
        Time.timeScale = 1f;

        // Subscribe to operator collection
        bool operatorCollected = false;
        System.Action<int> onNumberChanged = (number) => { operatorCollected = true; };
        PlayerController.OnNumberChanged += onNumberChanged;

        // Wait for operator collection
        while (!operatorCollected)
        {
            yield return null;
        }

        PlayerController.OnNumberChanged -= onNumberChanged;

        // Pause game again
        Time.timeScale = 0f;
        NextStep();
    }

    private IEnumerator HandleCoinStep(TutorialStep step)
    {
        if (nextButton != null)
            nextButton.gameObject.SetActive(false);

        // Resume game temporarily
        Time.timeScale = 1f;

        // Subscribe to coin collection
        bool coinCollected = false;
        System.Action<int> onCoinsChanged = (coins) => { coinCollected = true; };
        CoinManager.OnCoinsChanged += onCoinsChanged;

        // Wait for coin collection
        while (!coinCollected)
        {
            yield return null;
        }

        CoinManager.OnCoinsChanged -= onCoinsChanged;

        // Pause game again
        Time.timeScale = 0f;
        NextStep();
    }

    private void CompleteTutorial()
    {
        isTutorialActive = false;
        tutorialCompleted = true;

        // Save completion status
        PlayerPrefs.SetInt(TutorialCompletedKey, 1);

        // Hide tutorial UI
        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);
        
        if (tutorialArrow != null)
            tutorialArrow.SetActive(false);
        
        if (tutorialHighlight != null)
            tutorialHighlight.SetActive(false);

        // Resume game
        Time.timeScale = 1f;

        OnTutorialCompleted?.Invoke();
        Debug.Log("Tutorial completed");
    }

    public void PositionArrow(Vector3 worldPosition)
    {
        if (tutorialArrow != null)
        {
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
            tutorialArrow.transform.position = screenPosition;
            tutorialArrow.SetActive(true);
        }
    }

    public void HighlightObject(GameObject target)
    {
        if (tutorialHighlight != null && target != null)
        {
            tutorialHighlight.transform.position = target.transform.position;
            tutorialHighlight.transform.localScale = target.transform.localScale * 1.2f;
            tutorialHighlight.SetActive(true);
        }
    }

    public void HideHighlights()
    {
        if (tutorialArrow != null)
            tutorialArrow.SetActive(false);
        
        if (tutorialHighlight != null)
            tutorialHighlight.SetActive(false);
    }

    // Public interface
    public bool IsTutorialActive => isTutorialActive;
    public bool IsTutorialCompleted => tutorialCompleted;
    public int CurrentStepIndex => currentStepIndex;

    public void ResetTutorial()
    {
        PlayerPrefs.DeleteKey(TutorialCompletedKey);
        tutorialCompleted = false;
        Debug.Log("Tutorial reset");
    }

    private void OnDestroy()
    {
        if (currentStepCoroutine != null)
        {
            StopCoroutine(currentStepCoroutine);
        }
    }
}

[System.Serializable]
public class TutorialStep
{
    public string title;
    public string description;
    public TutorialStepType stepType;
    public float duration; // For timed steps

    public TutorialStep(string title, string description, TutorialStepType stepType, float duration = 0f)
    {
        this.title = title;
        this.description = description;
        this.stepType = stepType;
        this.duration = duration;
    }
}

public enum TutorialStepType
{
    Text,           // Show text and auto-advance or wait for next button
    WaitForInput,   // Wait for player input (tap/space)
    WaitForOperator,// Wait for player to collect an operator
    WaitForCoin,    // Wait for player to collect a coin
    WaitForShield,  // Wait for player to use/collect a shield
    WaitForLevel    // Wait for player to level up
}