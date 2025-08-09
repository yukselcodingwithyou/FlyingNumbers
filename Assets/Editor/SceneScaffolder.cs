#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public static class SceneScaffolder
{
    private const string ScenesFolder = "Assets/Scenes";

    [MenuItem("Tools/FlyingNumbers/Scaffold Scenes")]
    public static void ScaffoldScenes()
    {
        EnsureFolder(ScenesFolder);

        CreateStartScene();
        CreateGameScene();
        CreateDemoScene();

        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog("Scenes", "Start, Game, Demo scenes scaffolded.", "OK");
    }

    private static void CreateStartScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var loader = new GameObject("SceneLoader").AddComponent<SceneLoader>();

        CreateEventSystem();
        var canvas = CreateCanvas("StartCanvas");
    // Header
    var title = CreateText(canvas.transform, "Title", "Flying Numbers");
    title.alignment = TextAnchor.UpperCenter;
    title.fontSize = 32;
    title.rectTransform.anchorMin = new Vector2(0.5f, 1);
    title.rectTransform.anchorMax = new Vector2(0.5f, 1);
    title.rectTransform.pivot = new Vector2(0.5f, 1);
    title.rectTransform.anchoredPosition = new Vector2(0, -20);

    // Character carousel area
    var carouselGO = new GameObject("CharacterCarousel");
    carouselGO.transform.SetParent(canvas.transform, false);
    var carouselRT = carouselGO.AddComponent<RectTransform>();
    carouselRT.sizeDelta = new Vector2(420, 200);
    carouselRT.anchorMin = carouselRT.anchorMax = new Vector2(0.5f, 0.5f);
    carouselRT.anchoredPosition = new Vector2(0, 40);

    var nameText = CreateText(carouselGO.transform, "Name", "One");
    nameText.alignment = TextAnchor.MiddleCenter;
    nameText.rectTransform.anchoredPosition = new Vector2(0, 60);

    var previewGO = new GameObject("Preview");
    previewGO.transform.SetParent(carouselGO.transform, false);
    var previewImg = previewGO.AddComponent<Image>();
    previewImg.color = new Color(1, 1, 1, 0.8f);
    var previewRT = previewGO.GetComponent<RectTransform>();
    previewRT.sizeDelta = new Vector2(120, 120);
    previewRT.anchorMin = previewRT.anchorMax = new Vector2(0.5f, 0.5f);
    previewRT.anchoredPosition = Vector2.zero;

    var prevBtn = CreateButton(carouselGO.transform, "PrevButton", "<");
    prevBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(-150, 0);
    var nextBtn = CreateButton(carouselGO.transform, "NextButton", ">");
    nextBtn.GetComponent<RectTransform>().anchoredPosition = new Vector2(150, 0);

    var carousel = carouselGO.AddComponent<CharacterCarousel>();
    carousel.nameText = nameText;
    carousel.previewImage = previewImg;
    carousel.prevButton = prevBtn;
    carousel.nextButton = nextBtn;

    var startBtn = CreateButton(canvas.transform, "StartButton", "Start Game");
        startBtn.onClick.AddListener(loader.LoadGame);

        var demoBtn = CreateButton(canvas.transform, "DemoButton", "Demo");
    demoBtn.transform.localPosition = new Vector3(0, -60, 0);
        demoBtn.onClick.AddListener(loader.LoadDemo);

        var quitBtn = CreateButton(canvas.transform, "QuitButton", "Quit");
    quitBtn.transform.localPosition = new Vector3(0, -120, 0);
        quitBtn.onClick.AddListener(loader.Quit);

        EditorSceneManager.SaveScene(scene, $"{ScenesFolder}/Start.unity");
    }

    private static void CreateGameScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var loader = new GameObject("SceneLoader").AddComponent<SceneLoader>();

        // Spawners
        var spawners = new GameObject("Spawners").AddComponent<SpawnManager>();
        spawners.spawnXRange = new Vector2(8f, 10f);
        spawners.spawnYRange = new Vector2(-3.5f, 3.5f);

        // UI
        CreateEventSystem();
        var canvas = CreateCanvas("GameCanvas");

    // HUD
    var hud = new GameObject("HUD");
        hud.transform.SetParent(canvas.transform, false);
    var score = CreateText(hud.transform, "ScoreText", "0");
        score.alignment = TextAnchor.UpperLeft;
        score.rectTransform.anchorMin = new Vector2(0, 1);
        score.rectTransform.anchorMax = new Vector2(0, 1);
        score.rectTransform.pivot = new Vector2(0, 1);
        score.rectTransform.anchoredPosition = new Vector2(16, -16);

        // GameOver Panel
    var panelGO = new GameObject("GameOverPanel");
        panelGO.transform.SetParent(canvas.transform, false);
        var panel = panelGO.AddComponent<Image>();
        panel.color = new Color(0, 0, 0, 0.5f);
        var rt = panel.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
        panelGO.SetActive(false);

    var overText = CreateText(panelGO.transform, "GameOverText", "Game Over");
        overText.alignment = TextAnchor.MiddleCenter;
        overText.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        overText.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        overText.rectTransform.anchoredPosition = new Vector2(0, 40);
        overText.fontSize = 36;

    var finalScore = CreateText(panelGO.transform, "FinalScoreText", "Score: 0");
    finalScore.alignment = TextAnchor.MiddleCenter;
    finalScore.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
    finalScore.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
    finalScore.rectTransform.anchoredPosition = new Vector2(0, 0);

    var bestScore = CreateText(panelGO.transform, "BestScoreText", "Best: 0");
    bestScore.alignment = TextAnchor.MiddleCenter;
    bestScore.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
    bestScore.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
    bestScore.rectTransform.anchoredPosition = new Vector2(0, -30);

        var restartBtn = CreateButton(panelGO.transform, "RestartButton", "Restart");
        restartBtn.transform.localPosition = new Vector3(0, -10, 0);
        restartBtn.onClick.AddListener(loader.Restart);

        var homeBtn = CreateButton(panelGO.transform, "HomeButton", "Home");
        homeBtn.transform.localPosition = new Vector3(0, -70, 0);
        homeBtn.onClick.AddListener(loader.LoadStart);

        // Bind ScoreManager UI
        var binder = new GameObject("ScoreUIBinder").AddComponent<ScoreUIBinder>();
        binder.transform.SetParent(canvas.transform, false);
        binder.scoreText = score;
        binder.gameOverPanel = panelGO;
        binder.gameOverScoreText = finalScore;
        binder.gameOverHighScoreText = bestScore;

        var saved = EditorSceneManager.SaveScene(scene, $"{ScenesFolder}/Game.unity");
        if (saved)
        {
            // Add scenes to build settings if missing
            AddScenesToBuildSettings();
        }
    }

    private static void CreateDemoScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var loader = new GameObject("SceneLoader").AddComponent<SceneLoader>();
        CreateEventSystem();
        var canvas = CreateCanvas("DemoCanvas");
        var t = CreateText(canvas.transform, "Hint", "Demo playground\nAdd prefabs and test animations.");
        t.alignment = TextAnchor.UpperCenter;
        t.rectTransform.anchorMin = new Vector2(0.5f, 1);
        t.rectTransform.anchorMax = new Vector2(0.5f, 1);
        t.rectTransform.pivot = new Vector2(0.5f, 1);
        t.rectTransform.anchoredPosition = new Vector2(0, -16);

        EditorSceneManager.SaveScene(scene, $"{ScenesFolder}/Demo.unity");
    }

    // Helpers
    private static Canvas CreateCanvas(string name)
    {
        var canvasGO = new GameObject(name);
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGO.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasGO.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    private static void CreateEventSystem()
    {
        if (Object.FindObjectOfType<EventSystem>() != null) return;
        var es = new GameObject("EventSystem");
        es.AddComponent<EventSystem>();
        es.AddComponent<StandaloneInputModule>();
    }

    private static Button CreateButton(Transform parent, string name, string label)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);

        var img = go.AddComponent<Image>();
        img.color = new Color(0.2f, 0.6f, 1f, 1f);
        var btn = go.AddComponent<Button>();

        var rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(200, 44);
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);

        var text = CreateText(go.transform, "Text", label);
        text.alignment = TextAnchor.MiddleCenter;
        text.rectTransform.anchorMin = Vector2.zero;
        text.rectTransform.anchorMax = Vector2.one;
        text.rectTransform.offsetMin = text.rectTransform.offsetMax = Vector2.zero;

        return btn;
    }

    private static Text CreateText(Transform parent, string name, string content)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var txt = go.AddComponent<Text>();
        txt.text = content;
        txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        txt.color = Color.white;
        var rt = go.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(300, 60);
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        return txt;
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

    private static void AddScenesToBuildSettings()
    {
        var start = $"{ScenesFolder}/Start.unity";
        var game = $"{ScenesFolder}/Game.unity";
        var demo = $"{ScenesFolder}/Demo.unity";

        var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>();
        foreach (var s in EditorBuildSettings.scenes)
        {
            scenes.Add(s);
        }

        void Ensure(string path)
        {
            bool exists = scenes.Exists(x => x.path == path);
            if (!exists)
            {
                scenes.Add(new EditorBuildSettingsScene(path, true));
            }
        }

        Ensure(start);
        Ensure(game);
        Ensure(demo);

        EditorBuildSettings.scenes = scenes.ToArray();
    }
}
#endif
