using UnityEngine;

/// <summary>
/// Represents an operator that modifies the player's number upon collision.
/// Add this component to operator objects like +1 or *3 and set the type and value.
/// </summary>
public class Operator : MonoBehaviour
{
    public enum Type { Add, Subtract, Multiply, Divide }

    [Tooltip("Type of operator to apply")] public Type type = Type.Add;
    [Tooltip("Value used with the operator")] public int value = 1;
}
