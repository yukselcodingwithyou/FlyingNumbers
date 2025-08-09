#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace FlyingNumbers.EditorTools
{
    public static class CharacterCatalogBuilder
    {
        private const string AssetDir = "Assets/ScriptableObjects";
        private const string AssetPath = AssetDir + "/CharacterCatalog.asset";

        [MenuItem("Tools/FlyingNumbers/Build Character Catalog")]
        public static void BuildMenu()
        {
            BuildOrUpdateCatalogAsset();
        }

        public static void BuildOrUpdateCatalogAsset()
        {
            EnsureDir(AssetDir);
            var catalog = AssetDatabase.LoadAssetAtPath<FlyingNumbers.CharacterCatalog>(AssetPath);
            if (catalog == null)
            {
                catalog = ScriptableObject.CreateInstance<FlyingNumbers.CharacterCatalog>();
                AssetDatabase.CreateAsset(catalog, AssetPath);
            }

            var characterGuids = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Prefabs/Characters" });
            var found = new List<FlyingNumbers.CharacterCatalog.Entry>();

            foreach (var guid in characterGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab == null) continue;

                var entry = new FlyingNumbers.CharacterCatalog.Entry
                {
                    displayName = Path.GetFileNameWithoutExtension(path),
                    prefab = prefab
                };
                found.Add(entry);
            }

            catalog.entries = found;
            EditorUtility.SetDirty(catalog);
            AssetDatabase.SaveAssets();
            Debug.Log($"Character Catalog updated with {found.Count} entries at {AssetPath}");
        }

        private static void EnsureDir(string d)
        {
            if (!AssetDatabase.IsValidFolder(d))
            {
                var parent = Path.GetDirectoryName(d)?.Replace("\\", "/");
                var folder = Path.GetFileName(d);
                if (!string.IsNullOrEmpty(parent) && !string.IsNullOrEmpty(folder))
                    AssetDatabase.CreateFolder(parent, folder);
            }
        }
    }
}
#endif
