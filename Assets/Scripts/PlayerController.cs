using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

/// <summary>
/// Handles player movement, wing animation, operator collisions and game over logic.
/// Attach this script to the player GameObject which should also have a Rigidbody2D
/// and Animator components. The Animator is expected to have a "Flap" trigger
/// that plays a wing flap animation.
/// Uses the Unity Input System with an <see cref="InputActionReference"/> for the
/// flap action.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float flapForce = 5f; // Upward force when flapping
    [SerializeField] private float gravityScale = 1f; // Gravity multiplier

    [Header("Input")]
    [SerializeField] private InputActionReference flapAction; // Reference to the input action that triggers a flap

    [Header("UI")]
    [SerializeField] private Text numberText; // UI text displaying the current number

    private Rigidbody2D rb;
    private Animator animator;
    private int currentNumber = 0;
    private bool isGameOver = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.gravityScale = gravityScale;
        UpdateNumberText();
    }

    private void OnEnable()
    {
        if (flapAction != null)
        {
            flapAction.action.performed += OnFlapPerformed;
            flapAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (flapAction != null)
        {
            flapAction.action.performed -= OnFlapPerformed;
            flapAction.action.Disable();
        }
    }

    private void Update()
    {
        // Only allow gravity and movement updates when not in game over state
        if (isGameOver)
            return;
    }

    private void OnFlapPerformed(InputAction.CallbackContext context)
    {
        if (!isGameOver)
            Flap();
    }

    /// <summary>
    /// Applies an upward force and triggers the wing flap animation.
    /// </summary>
    private void Flap()
    {
        rb.velocity = Vector2.up * flapForce;
        if (animator != null)
        {
            animator.SetTrigger("Flap");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isGameOver)
            return;

        if (collision.CompareTag("Pipe"))
        {
            GameOver();
            return;
        }

        // Check for operator objects like +1, -2, *3
        Operator operatorComponent = collision.GetComponent<Operator>();
        if (operatorComponent != null)
        {
            ApplyOperator(operatorComponent);
            if (ScoreManager.Instance != null)
                ScoreManager.Instance.AddScore(1);
            Destroy(collision.gameObject);
        }
    }

    /// <summary>
    /// Applies the operator effect to the current number.
    /// </summary>
    private void ApplyOperator(Operator op)
    {
        switch (op.type)
        {
            case Operator.Type.Add:
                currentNumber += op.value;
                break;
            case Operator.Type.Subtract:
                currentNumber -= op.value;
                break;
            case Operator.Type.Multiply:
                currentNumber *= op.value;
                break;
            case Operator.Type.Divide:
                if (op.value != 0)
                    currentNumber /= op.value;
                break;
        }

        UpdateNumberText();
    }

    /// <summary>
    /// Updates the on screen number.
    /// </summary>
    private void UpdateNumberText()
    {
        if (numberText != null)
        {
            numberText.text = currentNumber.ToString();
        }
    }

    /// <summary>
    /// Called when hitting a pipe collider.
    /// Stops movement and disables further input.
    /// </summary>
    private void GameOver()
    {
        isGameOver = true;
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.ShowGameOver();
    }
}
