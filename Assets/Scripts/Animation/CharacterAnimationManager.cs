using UnityEngine;

/// <summary>
/// Master controller that manages both wing and feet animations for character prefabs.
/// Coordinates animation triggers and handles the synchronization between wings and feet.
/// </summary>
public class CharacterAnimationManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FlapProfile flapProfile;
    [SerializeField] private WingAnimationController leftWing;
    [SerializeField] private WingAnimationController rightWing;
    [SerializeField] private FeetAnimationController leftFoot;
    [SerializeField] private FeetAnimationController rightFoot;
    
    [Header("Settings")]
    [SerializeField] private bool hasWings = true;
    [SerializeField] private bool hasFeet = true;
    [SerializeField] private bool enableIdleFlutter = false;
    [SerializeField] private float idleFlutterRate = 0.1f; // Chance per second
    
    private float idleTimer = 0f;
    
    private void Start()
    {
        InitializeAnimationControllers();
    }
    
    private void Update()
    {
        // Handle idle wing flutter for characters
        if (enableIdleFlutter && hasWings && !IsLooping())
        {
            HandleIdleFlutter();
        }
    }
    
    /// <summary>
    /// Initialize all animation controllers with the flap profile
    /// </summary>
    private void InitializeAnimationControllers()
    {
        if (flapProfile != null)
        {
            if (leftWing != null) leftWing.SetFlapProfile(flapProfile);
            if (rightWing != null) rightWing.SetFlapProfile(flapProfile);
            if (leftFoot != null) leftFoot.SetFlapProfile(flapProfile);
            if (rightFoot != null) rightFoot.SetFlapProfile(flapProfile);
        }
    }
    
    /// <summary>
    /// Triggers a coordinated flap (wings) and kick (feet) animation
    /// Called when player flaps or when other systems need animation
    /// </summary>
    public void TriggerFlapAndKick()
    {
        if (hasWings)
        {
            if (leftWing != null) leftWing.TriggerFlap();
            if (rightWing != null) rightWing.TriggerFlap();
        }
        
        if (hasFeet)
        {
            if (leftFoot != null) leftFoot.TriggerKick();
            if (rightFoot != null) rightFoot.TriggerKick();
        }
    }
    
    /// <summary>
    /// Starts continuous looping animation for wings (used by power-ups and obstacles)
    /// </summary>
    public void StartLoopingAnimation()
    {
        if (hasWings)
        {
            if (leftWing != null) leftWing.StartLoopingFlap();
            if (rightWing != null) rightWing.StartLoopingFlap();
        }
    }
    
    /// <summary>
    /// Stops continuous looping animation
    /// </summary>
    public void StopLoopingAnimation()
    {
        if (hasWings)
        {
            if (leftWing != null) leftWing.StopLoopingFlap();
            if (rightWing != null) rightWing.StopLoopingFlap();
        }
    }
    
    /// <summary>
    /// Sets whether this character should use looping wing animation
    /// </summary>
    public void SetLooping(bool looping)
    {
        if (looping)
        {
            StartLoopingAnimation();
        }
        else
        {
            StopLoopingAnimation();
        }
    }
    
    /// <summary>
    /// Check if wings are currently in looping mode
    /// </summary>
    private bool IsLooping()
    {
        if (leftWing != null)
        {
            // Assuming we can check if the wing is in looping mode
            // This would need to be implemented in WingAnimationController if needed
            return false; // Placeholder
        }
        return false;
    }
    
    /// <summary>
    /// Handles occasional idle wing flutter for characters
    /// </summary>
    private void HandleIdleFlutter()
    {
        idleTimer += Time.deltaTime;
        
        if (idleTimer >= 1f) // Check every second
        {
            if (Random.Range(0f, 1f) < idleFlutterRate)
            {
                TriggerFlapAndKick();
            }
            idleTimer = 0f;
        }
    }
    
    /// <summary>
    /// Sets the flap profile for all animation controllers
    /// </summary>
    public void SetFlapProfile(FlapProfile profile)
    {
        flapProfile = profile;
        InitializeAnimationControllers();
    }
}