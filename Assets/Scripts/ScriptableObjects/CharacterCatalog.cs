using System.Collections.Generic;
using UnityEngine;

namespace FlyingNumbers
{
    [CreateAssetMenu(fileName = "CharacterCatalog", menuName = "FlyingNumbers/Character Catalog")]
    public class CharacterCatalog : ScriptableObject
    {
        [System.Serializable]
        public class Entry
        {
            public string displayName;
            public GameObject prefab;
        }

        public List<Entry> entries = new List<Entry>();

        public GameObject GetByName(string name)
        {
            foreach (var e in entries)
                if (!string.IsNullOrEmpty(e.displayName) && e.displayName == name)
                    return e.prefab;
            return entries.Count > 0 ? entries[0].prefab : null;
        }

        public string FirstNameOrNull()
        {
            return entries.Count > 0 ? entries[0].displayName : null;
        }
    }
}
