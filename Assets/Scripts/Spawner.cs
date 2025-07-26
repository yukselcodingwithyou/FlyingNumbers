using UnityEngine;

/// <summary>
/// Periodically spawns pipe obstacles with a vertical gap and optionally
/// places operator-number prefabs inside the gap.
/// Attach this to an empty GameObject positioned at the x spawn location.
/// </summary>
public class Spawner : MonoBehaviour
{
    [Header("Pipe Spawning")]
    [Tooltip("Prefab containing the pair of pipes with a gap")]
    public GameObject pipePrefab;

    [Tooltip("Time between pipe spawns")] public float spawnInterval = 2f;
    [Tooltip("Minimum Y position for the gap center")] public float minY = -1f;
    [Tooltip("Maximum Y position for the gap center")] public float maxY = 1f;

    [Header("Operator Objects")]
    [Tooltip("Possible operator prefabs to spawn inside the gap")]
    public GameObject[] operatorPrefabs;
    [Tooltip("Chance from 0-1 to spawn an operator each pipe")] public float operatorSpawnChance = 0.5f;
    [Tooltip("Vertical offset of the operator from the gap center")]
    public float operatorYOffset = 0f;

    private void Start()
    {
        InvokeRepeating(nameof(Spawn), spawnInterval, spawnInterval);
    }

    private void Spawn()
    {
        if (pipePrefab == null)
            return;

        float gapY = Random.Range(minY, maxY);
        Vector3 spawnPos = new Vector3(transform.position.x, gapY, 0f);
        Instantiate(pipePrefab, spawnPos, Quaternion.identity);

        if (operatorPrefabs != null && operatorPrefabs.Length > 0 && Random.value < operatorSpawnChance)
        {
            GameObject opPrefab = operatorPrefabs[Random.Range(0, operatorPrefabs.Length)];
            if (opPrefab != null)
            {
                Vector3 opPos = new Vector3(transform.position.x, gapY + operatorYOffset, 0f);
                Instantiate(opPrefab, opPos, Quaternion.identity);
            }
        }
    }
}
