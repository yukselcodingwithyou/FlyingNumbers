using UnityEngine;

/// <summary>
/// ScriptableObject that contains tuning parameters for wing and feet animations.
/// Used to centralize animation settings and make them easily adjustable in the Inspector.
/// </summary>
[CreateAssetMenu(fileName = "DefaultFlapProfile", menuName = "Animation/Flap Profile")]
public class FlapProfile : ScriptableObject
{
    [Header("Wing Animation Settings")]
    [Tooltip("Rest angle of the wings when not flapping")]
    public float restAngle = 0f;
    
    [Tooltip("Minimum angle during looping animation (lower bound)")]
    public float minAngle = -12f;
    
    [Tooltip("Maximum angle during looping animation (upper bound)")]
    public float maxAngle = 12f;
    
    [Tooltip("Frequency in Hz for looping wing animations (power-ups, mines)")]
    public float flapFrequency = 6f;
    
    [Tooltip("Total time for player flap impulse (in + out)")]
    public float flapImpulseTime = 0.18f;
    
    [Header("Feet Animation Settings")]
    [Tooltip("Multiplier for feet kick animation intensity")]
    public float feetKickMultiplier = 1f;
    
    [Header("Timing Settings")]
    [Tooltip("Duration for wing flap in phase")]
    public float flapInDuration = 0.08f;
    
    [Tooltip("Duration for wing flap out phase")]
    public float flapOutDuration = 0.10f;
    
    [Header("Animation Offsets")]
    [Tooltip("Random offset range for left/right wing desync (organic feel)")]
    public Vector2 wingDesyncRange = new Vector2(0.01f, 0.03f);
}