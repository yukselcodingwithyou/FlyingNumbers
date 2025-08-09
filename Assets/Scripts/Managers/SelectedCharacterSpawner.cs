using UnityEngine;

namespace FlyingNumbers
{
    public class SelectedCharacterSpawner : MonoBehaviour
    {
        public CharacterCatalog catalog;
        public Transform spawnPoint;
        public bool alignToSpawnPointRotation = false;

        private void Start()
        {
            if (catalog == null || catalog.entries.Count == 0)
            {
                Debug.LogWarning("SelectedCharacterSpawner: No catalog assigned.");
                return;
            }

            var name = CharacterSelectionService.GetOrDefault(catalog.FirstNameOrNull());
            var prefab = catalog.GetByName(name);
            if (prefab == null)
            {
                Debug.LogWarning($"SelectedCharacterSpawner: Prefab for '{name}' not found in catalog.");
                return;
            }

            var pos = spawnPoint ? spawnPoint.position : transform.position;
            var rot = alignToSpawnPointRotation && spawnPoint ? spawnPoint.rotation : Quaternion.identity;
            Instantiate(prefab, pos, rot);
        }
    }
}
