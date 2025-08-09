#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

public static class PrefabAutoBuilder
{
    private const string PrefabsFolder = "Assets/Prefabs";
    private const string SpritesFolder = "Assets/Sprites"; // Adjust if different

    [MenuItem("Tools/FlyingNumbers/Build Missing Prefabs")]
    public static void BuildMissingPrefabs()
    {
        EnsureFolder(PrefabsFolder);

        // Character prefabs from Number1 template
        DuplicateFromTemplate("Characters/Number1", "Characters/Cat");
        DuplicateFromTemplate("Characters/Number1", "Characters/Dog");
        DuplicateFromTemplate("Characters/Number1", "Characters/Fox");

        // Powerups from Magnet template
        DuplicateFromTemplate("Powerups/Magnet", "Powerups/Coin");
        DuplicateFromTemplate("Powerups/Magnet", "Powerups/Shield");
        DuplicateFromTemplate("Powerups/Magnet", "Powerups/Slowdown");

        // Obstacle from Mine template
        DuplicateFromTemplate("Obstacles/Mine", "Obstacles/Pipe");

        EditorUtility.DisplayDialog("Prefab Builder", "Prefab creation done. You may need to assign sprites.", "OK");
    }

    private static void DuplicateFromTemplate(string templateRelative, string newRelative)
    {
        var templatePath = FindAssetAtPath(PrefabsFolder, Path.GetFileName(templateRelative), "t:Prefab", templateRelative);
        if (string.IsNullOrEmpty(templatePath))
        {
            Debug.LogWarning($"Template prefab not found: {templateRelative}");
            return;
        }

        var newPath = $"{PrefabsFolder}/{newRelative}.prefab";
        var newDir = Path.GetDirectoryName(newPath);
        EnsureFolder(newDir.Replace('\\', '/'));

        if (!File.Exists(newPath))
        {
            AssetDatabase.CopyAsset(templatePath, newPath);
        }

        // Try to swap sprites if available
        var root = PrefabUtility.LoadPrefabContents(newPath);
        TrySwapSprites(root, Path.GetFileName(newRelative));
        PrefabUtility.SaveAsPrefabAsset(root, newPath);
        PrefabUtility.UnloadPrefabContents(root);
        Debug.Log($"Prepared prefab: {newRelative}");
    }

    private static void TrySwapSprites(GameObject root, string baseName)
    {
        // Expected sprite naming conventions:
        // Characters: {Name}_Body, {Name}_Wing_L, {Name}_Wing_R, {Name}_Foot_L, {Name}_Foot_R
        // Singles: {Name}
        var renderers = root.GetComponentsInChildren<SpriteRenderer>(true);
        if (renderers == null || renderers.Length == 0) return;

        foreach (var sr in renderers)
        {
            string targetKey;

            var lower = sr.gameObject.name.ToLower();
            if (lower.Contains("leftwing") || lower.Contains("lwing"))
                targetKey = $"{baseName}_Wing_L";
            else if (lower.Contains("rightwing") || lower.Contains("rwing"))
                targetKey = $"{baseName}_Wing_R";
            else if (lower.Contains("leftfoot") || lower.Contains("lfoot"))
                targetKey = $"{baseName}_Foot_L";
            else if (lower.Contains("rightfoot") || lower.Contains("rfoot"))
                targetKey = $"{baseName}_Foot_R";
            else
                targetKey = $"{baseName}_Body"; // body or single sprite

            var sprite = FindSprite(targetKey) ?? FindSprite(baseName);
            if (sprite != null)
                sr.sprite = sprite;
        }
    }

    private static string FindAssetAtPath(string folder, string name, string filter, string subfolderHint = null)
    {
        string[] searchInFolders = subfolderHint != null ? new[] {$"{folder}/{Path.GetDirectoryName(subfolderHint)}"} : new[] { folder };
        var guids = AssetDatabase.FindAssets($"{filter} {name}", searchInFolders);
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (Path.GetFileNameWithoutExtension(path).Equals(name, System.StringComparison.OrdinalIgnoreCase))
                return path;
        }
        return null;
    }

    private static Sprite FindSprite(string name)
    {
        var guids = AssetDatabase.FindAssets($"t:Sprite {name}", new[] { SpritesFolder });
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            if (Path.GetFileNameWithoutExtension(path).Equals(name, System.StringComparison.OrdinalIgnoreCase))
                return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }
        return null;
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;
        var parts = path.Split('/');
        var cur = parts[0];
        for (int i = 1; i < parts.Length; i++)
        {
            var next = $"{cur}/{parts[i]}";
            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(cur, parts[i]);
            cur = next;
        }
    }
}
#endif
