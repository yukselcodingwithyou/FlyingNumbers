using UnityEngine;

/// <summary>
/// Detects when the player has passed this pipe and awards score.
/// Attach this to the pipe prefab root object.
/// </summary>
public class Pipe : MonoBehaviour
{
    private bool scored;
    private Transform player;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    private void Update()
    {
        if (!scored && player != null && player.position.x > transform.position.x)
        {
            scored = true;
            if (ScoreManager.Instance != null)
                ScoreManager.Instance.AddScore(1);
        }
    }
}
