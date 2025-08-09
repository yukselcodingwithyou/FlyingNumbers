using UnityEngine;

/// <summary>
/// Controls feet animations for characters. 
/// Triggers foot kick animations synchronized with wing flaps.
/// </summary>
public class FeetAnimationController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FlapProfile flapProfile;
    [SerializeField] private Animator animator;
    
    [Header("Settings")]
    [SerializeField] private float kickIntensity = 1f;
    
    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }
    
    /// <summary>
    /// Triggers a foot kick animation (synchronized with wing flaps)
    /// </summary>
    public void TriggerKick()
    {
        if (animator != null)
        {
            animator.SetTrigger("Kick");
            
            // Apply kick intensity if profile is available
            if (flapProfile != null)
            {
                // You can use this to modify animation speed or other parameters
                animator.speed = flapProfile.feetKickMultiplier * kickIntensity;
            }
        }
    }
    
    /// <summary>
    /// Sets the flap profile for this feet controller
    /// </summary>
    public void SetFlapProfile(FlapProfile profile)
    {
        flapProfile = profile;
    }
    
    /// <summary>
    /// Sets the kick intensity multiplier
    /// </summary>
    public void SetKickIntensity(float intensity)
    {
        kickIntensity = intensity;
    }
}