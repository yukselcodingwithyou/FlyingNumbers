#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class PlaceholderSpriteGenerator
{
    [MenuItem("Tools/FlyingNumbers/Generate Placeholder Sprites")] 
    public static void Generate()
    {
        // This produces simple colored squares so you can wire up sprites now and replace later.
        CreateSpriteTexture("Cat_Body", new Color(0.95f, 0.55f, 0.55f));
        CreateSpriteTexture("Dog_Body", new Color(0.8f, 0.6f, 0.4f));
        CreateSpriteTexture("Fox_Body", new Color(1.0f, 0.45f, 0.2f));

        CreateSpriteTexture("Cat_Wing_L", Color.white * 0.9f);
        CreateSpriteTexture("Cat_Wing_R", Color.white * 0.9f);
        CreateSpriteTexture("Cat_Foot_L", Color.black * 0.8f);
        CreateSpriteTexture("Cat_Foot_R", Color.black * 0.8f);

        CreateSpriteTexture("Dog_Wing_L", Color.white * 0.9f);
        CreateSpriteTexture("Dog_Wing_R", Color.white * 0.9f);
        CreateSpriteTexture("Dog_Foot_L", Color.black * 0.8f);
        CreateSpriteTexture("Dog_Foot_R", Color.black * 0.8f);

        CreateSpriteTexture("Fox_Wing_L", Color.white * 0.9f);
        CreateSpriteTexture("Fox_Wing_R", Color.white * 0.9f);
        CreateSpriteTexture("Fox_Foot_L", Color.black * 0.8f);
        CreateSpriteTexture("Fox_Foot_R", Color.black * 0.8f);

        CreateSpriteTexture("Coin", new Color(1.0f, 0.85f, 0.2f));
        CreateSpriteTexture("Shield", new Color(0.2f, 0.8f, 1.0f));
        CreateSpriteTexture("Slowdown", new Color(0.6f, 0.6f, 1.0f));
        CreateSpriteTexture("Pipe", new Color(0.2f, 0.8f, 0.2f));

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("Sprites", "Placeholder sprites generated in Assets/Sprites.", "OK");
    }

    private static void CreateSpriteTexture(string name, Color color)
    {
        const int size = 64;
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        var pixels = new Color[size * size];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
        tex.SetPixels(pixels);
        tex.Apply();

        var bytes = tex.EncodeToPNG();
        var folder = "Assets/Sprites";
        if (!AssetDatabase.IsValidFolder(folder)) AssetDatabase.CreateFolder("Assets", "Sprites");
        var path = $"{folder}/{name}.png";
        System.IO.File.WriteAllBytes(path, bytes);
        AssetDatabase.ImportAsset(path);

        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = TextureImporterType.Sprite;
        importer.spritePixelsPerUnit = 100;
        importer.SaveAndReimport();
    }
}
#endif
