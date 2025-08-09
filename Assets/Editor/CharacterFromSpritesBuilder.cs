#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FlyingNumbers.EditorTools
{
    public static class CharacterFromSpritesBuilder
    {
        private const string MenuPath = "Tools/FlyingNumbers/Build Characters From Sprites";

        // Naming: {Name}_Body.png, {Name}_Wing_L.png, {Name}_Wing_R.png, {Name}_Foot_L.png, {Name}_Foot_R.png
        [MenuItem(MenuPath)]
        public static void BuildFromSprites()
        {
            var sprites = FindAllSprites("Assets/Sprites");
            var groups = GroupByBaseName(sprites);

            EnsureDirs(new[]
            {
                "Assets/Prefabs",
                "Assets/Prefabs/Characters",
                "Assets/ScriptableObjects"
            });

            foreach (var kvp in groups)
            {
                string baseName = kvp.Key;
                var map = kvp.Value; // role -> Sprite

                if (!map.ContainsKey("Body") || !map.ContainsKey("Wing_L") || !map.ContainsKey("Wing_R"))
                {
                    Debug.LogWarning($"Skipping {baseName}: missing required sprites (Body/Wing_L/Wing_R).");
                    continue;
                }

                var root = new GameObject(baseName);
                try
                {
                    // Rigidbody2D on root (tune as needed)
                    var rb = root.AddComponent<Rigidbody2D>();
                    rb.gravityScale = 0.8f;
                    rb.mass = 1.0f;
                    rb.drag = 0.2f;
                    rb.angularDrag = 0.05f;
                    rb.interpolation = RigidbodyInterpolation2D.Interpolate;

                    // Attach optional runtime systems if present in project
                    AddIfExists(root, "CharacterAnimationManager");
                    AddIfExists(root, "PlayerController");

                    // Body
                    var body = new GameObject("Body");
                    body.transform.SetParent(root.transform, false);
                    var bodySr = body.AddComponent<SpriteRenderer>();
                    bodySr.sprite = map["Body"];
                    bodySr.sortingOrder = 0;

                    // Collider on body (Capsule2D default; falls back to Box2D)
                    var capsule = body.AddComponent<CapsuleCollider2D>();
                    if (capsule != null)
                    {
                        capsule.direction = CapsuleDirection2D.Vertical;
                        var bounds = bodySr.sprite != null ? bodySr.sprite.bounds.size : new Vector3(1, 1, 0);
                        capsule.size = new Vector2(bounds.x * 0.6f, bounds.y * 0.9f);
                        capsule.offset = Vector2.zero;
                        capsule.isTrigger = false;
                    }
                    else
                    {
                        var box = body.AddComponent<BoxCollider2D>();
                        var b = bodySr.sprite != null ? bodySr.sprite.bounds.size : new Vector3(1, 1, 0);
                        box.size = new Vector2(b.x * 0.6f, b.y * 0.9f);
                        box.isTrigger = false;
                    }

                    // Wings (parent rotates, child holds sprite; hinge simulated by offset)
                    CreateWing(root.transform, "LeftWing", map["Wing_L"], isRight: false);
                    CreateWing(root.transform, "RightWing", map["Wing_R"], isRight: true);

                    // Feet (optional)
                    if (map.TryGetValue("Foot_L", out var lFoot)) CreateFoot(root.transform, "LeftFoot", lFoot);
                    if (map.TryGetValue("Foot_R", out var rFoot)) CreateFoot(root.transform, "RightFoot", rFoot);

                    // Sorting
                    SetSortingOrder(root.transform);

                    // Assign animation controllers and profile if present
                    AssignAnimatorsAndProfiles(root);

                    // Save prefab
                    string prefabPath = $"Assets/Prefabs/Characters/{baseName}.prefab";
                    PrefabUtility.SaveAsPrefabAsset(root, prefabPath, out bool success);
                    if (success) Debug.Log($"Built character prefab: {prefabPath}");
                    else Debug.LogError($"Failed to save prefab: {prefabPath}");
                }
                finally
                {
                    Object.DestroyImmediate(root);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            // Optional: build/refresh Character Catalog after creating characters
            CharacterCatalogBuilder.BuildOrUpdateCatalogAsset();
        }

        private static Dictionary<string, Dictionary<string, Sprite>> GroupByBaseName(IEnumerable<Sprite> sprites)
        {
            var groups = new Dictionary<string, Dictionary<string, Sprite>>();
            foreach (var s in sprites)
            {
                var name = s.name; // e.g., Cat_Wing_L
                var parts = name.Split('_');
                if (parts.Length < 2) continue;

                // Handle {Name}_Body (only one suffix)
                if (parts[^1].Equals("Body", System.StringComparison.OrdinalIgnoreCase))
                {
                    var baseNameOnly = string.Join("_", parts.Take(parts.Length - 1));
                    if (!groups.TryGetValue(baseNameOnly, out var bodyMap))
                    {
                        bodyMap = new Dictionary<string, Sprite>();
                        groups[baseNameOnly] = bodyMap;
                    }
                    bodyMap["Body"] = s;
                    continue;
                }

                // General case: last two segments are role (Wing_L, Wing_R, Foot_L, Foot_R)
                if (parts.Length >= 2)
                {
                    string baseName = string.Join("_", parts.Take(parts.Length - 2));
                    string role = string.Join("_", parts.Skip(parts.Length - 2));

                    if (!groups.TryGetValue(baseName, out var map))
                    {
                        map = new Dictionary<string, Sprite>();
                        groups[baseName] = map;
                    }
                    map[role] = s;
                }
            }
            return groups;
        }

        private static List<Sprite> FindAllSprites(string rootFolder)
        {
            var guids = AssetDatabase.FindAssets("t:Sprite", new[] { rootFolder });
            var list = new List<Sprite>(guids.Length);
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var s = AssetDatabase.LoadAssetAtPath<Sprite>(path);
                if (s != null) list.Add(s);
            }
            return list;
        }

        private static void EnsureDirs(IEnumerable<string> dirs)
        {
            foreach (var d in dirs)
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

        private static void CreateWing(Transform parent, string name, Sprite sprite, bool isRight)
        {
            var wing = new GameObject(name);
            wing.transform.SetParent(parent, false);

            // Wing animation controller (reflection-safe)
            AddIfExists(wing, "WingAnimationController");
            AddAnimatorIfControllerExists(wing, "WingAnimator");

            // Graphic child
            var graphic = new GameObject("Graphic");
            graphic.transform.SetParent(wing.transform, false);
            var sr = graphic.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingOrder = -1;

            // Hinge offset (tweak to match your art)
            var hingeOffset = new Vector3(isRight ? 0.06f : -0.06f, 0.0f, 0);
            graphic.transform.localPosition = hingeOffset;

            // Tag left/right if script supports it
            SetFieldIfExists(wing, "WingAnimationController", "isRightWing", isRight);
        }

        private static void CreateFoot(Transform parent, string name, Sprite sprite)
        {
            var foot = new GameObject(name);
            foot.transform.SetParent(parent, false);
            AddIfExists(foot, "FeetAnimationController");
            AddAnimatorIfControllerExists(foot, "FeetAnimator");

            var graphic = new GameObject("Graphic");
            graphic.transform.SetParent(foot.transform, false);
            var sr = graphic.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingOrder = 1;
            graphic.transform.localPosition = Vector3.zero;
        }

        private static void SetSortingOrder(Transform root)
        {
            var body = root.Find("Body")?.GetComponent<SpriteRenderer>();
            if (body) body.sortingOrder = 0;

            var lw = root.Find("LeftWing/Graphic")?.GetComponent<SpriteRenderer>();
            var rw = root.Find("RightWing/Graphic")?.GetComponent<SpriteRenderer>();
            if (lw) lw.sortingOrder = -1;
            if (rw) rw.sortingOrder = -1;

            var lf = root.Find("LeftFoot/Graphic")?.GetComponent<SpriteRenderer>();
            var rf = root.Find("RightFoot/Graphic")?.GetComponent<SpriteRenderer>();
            if (lf) lf.sortingOrder = 1;
            if (rf) rf.sortingOrder = 1;
        }

        private static void AssignAnimatorsAndProfiles(GameObject root)
        {
            var flapProfile = FindAssetByName<ScriptableObject>("DefaultFlapProfile");
            if (flapProfile != null)
                SetFieldIfExists(root, "CharacterAnimationManager", "flapProfile", flapProfile);
        }

        private static void AddIfExists(GameObject go, string typeName)
        {
            var t = FindType(typeName);
            if (t != null && go.GetComponent(t) == null) go.AddComponent(t);
        }

        private static void AddAnimatorIfControllerExists(GameObject go, string controllerNameNoExt)
        {
            var ctrl = FindAssetByName<RuntimeAnimatorController>(controllerNameNoExt);
            if (ctrl != null)
            {
                var anim = go.GetComponent<Animator>() ?? go.AddComponent<Animator>();
                anim.runtimeAnimatorController = ctrl;
            }
        }

        private static void SetFieldIfExists(GameObject go, string typeName, string field, object value)
        {
            if (!go) return;
            var t = FindType(typeName);
            if (t == null) return;
            var c = go.GetComponent(t);
            if (!c) return;
            var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic;
            var f = t.GetField(field, flags);
            if (f != null && (value == null || f.FieldType.IsInstanceOfType(value)))
                f.SetValue(c, value);
        }

        private static System.Type FindType(string name)
        {
            return System.AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(x => x.Name == name);
        }

        private static T FindAssetByName<T>(string name) where T : Object
        {
            var guids = AssetDatabase.FindAssets($"{name} t:{typeof(T).Name}");
            foreach (var g in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(g);
                var asset = AssetDatabase.LoadAssetAtPath<T>(path);
                if (asset != null && Path.GetFileNameWithoutExtension(path) == name)
                    return asset;
            }
            return null;
        }
    }
}
#endif
