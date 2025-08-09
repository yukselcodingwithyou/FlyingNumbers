using UnityEngine;
using System.Collections;

/// <summary>
/// Controls wing animations for characters and winged objects.
/// Handles both impulse flaps (for player characters) and continuous looping (for power-ups/obstacles).
/// </summary>
public class WingAnimationController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FlapProfile flapProfile;
    [SerializeField] private Animator animator;
    
    [Header("Settings")]
    [SerializeField] private bool isLooping = false;
    [SerializeField] private float desyncOffset = 0f; // For left/right wing variation
    
    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
            
        // Apply random desync offset if profile is available
        if (flapProfile != null)
        {
            desyncOffset = Random.Range(flapProfile.wingDesyncRange.x, flapProfile.wingDesyncRange.y);
        }
    }
    
    private void Start()
    {
        if (isLooping)
        {
            StartLoopingFlap();
        }
    }
    
    /// <summary>
    /// Triggers a single flap impulse (for player characters)
    /// </summary>
    public void TriggerFlap()
    {
        if (animator != null && !isLooping)
        {
            StartCoroutine(DelayedFlap());
        }
    }
    
    /// <summary>
    /// Starts continuous looping flap animation (for power-ups and obstacles)
    /// </summary>
    public void StartLoopingFlap()
    {
        if (animator != null)
        {
            isLooping = true;
            animator.SetBool("Looping", true);
            
            if (flapProfile != null)
            {
                animator.SetFloat("FlapSpeed", flapProfile.flapFrequency);
            }
            else
            {
                animator.SetFloat("FlapSpeed", 6f); // Default frequency
            }
        }
    }
    
    /// <summary>
    /// Stops looping flap animation
    /// </summary>
    public void StopLoopingFlap()
    {
        if (animator != null)
        {
            isLooping = false;
            animator.SetBool("Looping", false);
        }
    }
    
    /// <summary>
    /// Adds small delay for wing desync (organic feel)
    /// </summary>
    private IEnumerator DelayedFlap()
    {
        if (desyncOffset > 0f)
        {
            yield return new WaitForSeconds(desyncOffset);
        }
        
        if (animator != null)
        {
            animator.SetTrigger("Flap");
        }
    }
    
    /// <summary>
    /// Sets the flap profile for this wing controller
    /// </summary>
    public void SetFlapProfile(FlapProfile profile)
    {
        flapProfile = profile;
        
        if (isLooping && animator != null && profile != null)
        {
            animator.SetFloat("FlapSpeed", profile.flapFrequency);
        }
    }
}