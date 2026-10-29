#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Builds GameObjects from a scene a vision model described (VisionTools' ask_model.py writes these .scene.json files):
/// Tools → Vision → Build Scene From JSON. Every object becomes a UI element on a Canvas, in the JSON's drawing order:
/// rects and sprites as Images, texts as Text, buttons as Buttons. With "Sprites as GameObjects", sprites and rects
/// become SpriteRenderers in the world instead (a game scene), with texts and buttons on a Canvas above them.
/// Sprites are looked up by name in the sprite folder you give: the same PNGs the model was trained with.
/// </summary>
public class SceneFromJson : EditorWindow
{
    [Serializable]
    public class SceneFile
    {
        public int width;
        public int height;
        public string background;
        public SceneObject[] objects;
    }

    [Serializable]
    public class SceneObject
    {
        public string type;
        public int[] box;          // left, top, right, bottom: 0..1000 across and down the picture
        public string name;        // sprite
        public string text;        // text, button
        public string colour;
        public string textColour;  // button
        public string flip;        // sprite: "x", "y" or "xy"
    }

    string jsonPath = "";
    string pastedJson = "";
    DefaultAsset spriteFolder;
    bool worldSprites;
    float pixelsPerUnit = 16f;
    Vector2 scroll;

    [MenuItem("Tools/Vision/Build Scene From JSON")]
    static void Open()
    {
        GetWindow<SceneFromJson>("Scene From JSON");
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Scene JSON", EditorStyles.boldLabel);
        using (new EditorGUILayout.HorizontalScope())
        {
            jsonPath = EditorGUILayout.TextField("File", jsonPath);
            if (GUILayout.Button("Browse", GUILayout.Width(70)))
            {
                string picked = EditorUtility.OpenFilePanel("Scene JSON", Path.GetDirectoryName(jsonPath) ?? "", "json");
                if (!string.IsNullOrEmpty(picked)) jsonPath = picked;
            }
        }
        EditorGUILayout.LabelField("... or paste it here (used when File is empty):");
        scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(120));
        pastedJson = EditorGUILayout.TextArea(pastedJson, GUILayout.ExpandHeight(true));
        EditorGUILayout.EndScrollView();

        EditorGUILayout.Space();
        spriteFolder = (DefaultAsset)EditorGUILayout.ObjectField("Sprite folder", spriteFolder, typeof(DefaultAsset), false);
        worldSprites = EditorGUILayout.Toggle(new GUIContent("Sprites as GameObjects",
            "Sprites and rects become SpriteRenderers in the world (a game scene); texts and buttons go on a Canvas " +
            "drawn above them. Off: everything is UI on one Canvas, in exactly the JSON's drawing order."), worldSprites);
        if (worldSprites) pixelsPerUnit = EditorGUILayout.FloatField("Pixels per unit", pixelsPerUnit);

        EditorGUILayout.Space();
        if (GUILayout.Button("Build", GUILayout.Height(30))) BuildFromWindow();
    }

    void BuildFromWindow()
    {
        string json = !string.IsNullOrEmpty(jsonPath) ? File.ReadAllText(jsonPath) : pastedJson;
        if (string.IsNullOrWhiteSpace(json))
        {
            EditorUtility.DisplayDialog("Scene From JSON", "Choose a .scene.json file or paste the JSON first.", "OK");
            return;
        }
        SceneFile scene;
        try
        {
            scene = JsonUtility.FromJson<SceneFile>(json);
        }
        catch (Exception e)
        {
            EditorUtility.DisplayDialog("Scene From JSON", "That isn't valid scene JSON:\n" + e.Message, "OK");
            return;
        }
        if (scene == null || scene.objects == null)
        {
            EditorUtility.DisplayDialog("Scene From JSON", "The JSON has no \"objects\" list.", "OK");
            return;
        }
        string folder = spriteFolder != null ? AssetDatabase.GetAssetPath(spriteFolder) : null;
        string title = !string.IsNullOrEmpty(jsonPath) ? Path.GetFileNameWithoutExtension(jsonPath) : "pasted";
        GameObject root = Build(scene, LoadSprites(folder), worldSprites, pixelsPerUnit, "Scene " + title);
        Selection.activeGameObject = root;
    }

    /// <summary>Every sprite in a folder (and its subfolders) by name. A sprite sheet's slices go by their own names.</summary>
    public static Dictionary<string, Sprite> LoadSprites(string folder)
    {
        var found = new Dictionary<string, Sprite>();
        if (string.IsNullOrEmpty(folder)) return found;
        foreach (string guid in AssetDatabase.FindAssets("t:Sprite", new[] { folder }))
        {
            foreach (UnityEngine.Object asset in AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GUIDToAssetPath(guid)))
            {
                var sprite = asset as Sprite;
                if (sprite != null && !found.ContainsKey(sprite.name)) found[sprite.name] = sprite;
            }
        }
        return found;
    }

    /// <summary>Builds the scene under a new root GameObject (one Undo step) and returns the root.</summary>
    public static GameObject Build(SceneFile scene, Dictionary<string, Sprite> sprites, bool worldSprites,
                                   float pixelsPerUnit, string rootName)
    {
        // Sizes in pixels: the picture's own, or a 16:9 stand-in if the JSON doesn't say (boxes are 0..1000 anyway).
        float w = scene.width > 0 ? scene.width : 640, h = scene.height > 0 ? scene.height : 360;
        var root = new GameObject(rootName);
        Undo.RegisterCreatedObjectUndo(root, "Build scene from JSON");

        // A camera that shows exactly the picture: world (0,0) is its top-left corner, y going down is negative.
        var camObject = new GameObject("Camera");
        camObject.transform.SetParent(root.transform, false);
        var cam = camObject.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = h / 2f / pixelsPerUnit;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = ParseColour(scene.background, Color.black);
        cam.depth = 10; // in front of any camera the scene already has
        camObject.transform.position = new Vector3(w / 2f / pixelsPerUnit, -h / 2f / pixelsPerUnit, -10f);

        var canvasObject = new GameObject("Canvas", typeof(RectTransform));
        canvasObject.transform.SetParent(root.transform, false);
        var canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = cam;
        canvas.planeDistance = 5f;
        canvas.sortingOrder = 100; // above the world sprites
        var scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(w, h);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 1f; // scale with the height, like the orthographic camera
        canvasObject.AddComponent<GraphicRaycaster>();

        // The picture's area, centred on the screen; objects are placed from its top-left corner.
        var picture = NewUi("Picture", canvasObject.transform);
        picture.anchorMin = picture.anchorMax = new Vector2(0.5f, 0.5f);
        picture.pivot = new Vector2(0.5f, 0.5f);
        picture.anchoredPosition = Vector2.zero;
        picture.sizeDelta = new Vector2(w, h);

        Transform world = null;
        if (worldSprites)
        {
            world = new GameObject("World").transform;
            world.SetParent(root.transform, false);
        }

        bool anyButton = false;
        for (int i = 0; i < scene.objects.Length; i++)
        {
            SceneObject o = scene.objects[i];
            if (o == null || o.box == null || o.box.Length != 4)
            {
                Debug.LogWarning($"Scene From JSON: object {i} has no box [left, top, right, bottom]; skipped.");
                continue;
            }
            // The box in pixels.
            float x1 = o.box[0] * w / 1000f, y1 = o.box[1] * h / 1000f, x2 = o.box[2] * w / 1000f, y2 = o.box[3] * h / 1000f;
            var centre = new Vector2((x1 + x2) / 2f, (y1 + y2) / 2f);
            var size = new Vector2(Mathf.Max(1f, x2 - x1), Mathf.Max(1f, y2 - y1));

            switch (o.type)
            {
                case "rect":
                    if (worldSprites) WorldSprite("Rect", world, WhiteSprite(), ParseColour(o.colour, Color.white), centre, size, "", i, pixelsPerUnit);
                    else UiImage("Rect", picture, null, ParseColour(o.colour, Color.white), centre, size, "");
                    break;
                case "sprite":
                    Sprite sprite = FindSprite(sprites, o.name);
                    if (sprite == null)
                    {
                        Debug.LogWarning($"Scene From JSON: no sprite called '{o.name}' in the sprite folder; drawn as a magenta box.");
                        UiImage("Missing sprite " + o.name, picture, null, new Color(1f, 0f, 1f, 0.5f), centre, size, "");
                    }
                    else if (worldSprites) WorldSprite(o.name, world, sprite, Color.white, centre, size, o.flip ?? "", i, pixelsPerUnit);
                    else UiImage(o.name, picture, sprite, Color.white, centre, size, o.flip ?? "");
                    break;
                case "text":
                    UiText("Text " + o.text, picture, o.text ?? "", ParseColour(o.colour, Color.white), centre, size, 1f);
                    break;
                case "button":
                    var image = UiImage("Button " + o.text, picture, null, ParseColour(o.colour, Color.grey), centre, size, "");
                    image.gameObject.AddComponent<Button>().targetGraphic = image;
                    // The label is about half the button's height (as the training pictures draw it), centred.
                    var label = UiText("Label", image.rectTransform, o.text ?? "", ParseColour(o.textColour, Color.black),
                                       Vector2.zero, size, 0.5f);
                    label.rectTransform.anchorMin = label.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                    label.rectTransform.anchoredPosition = Vector2.zero;
                    anyButton = true;
                    break;
                default:
                    Debug.LogWarning($"Scene From JSON: object {i} has an unknown type '{o.type}'; skipped.");
                    break;
            }
        }
        if (anyButton) EnsureEventSystem();
        EditorSceneManager.MarkSceneDirty(root.scene);
        return root;
    }

    static Sprite FindSprite(Dictionary<string, Sprite> sprites, string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        // A PNG imported as a sprite sheet (Multiple) names its first slice name_0.
        return sprites.TryGetValue(name, out Sprite s) ? s : sprites.TryGetValue(name + "_0", out s) ? s : null;
    }

    static RectTransform NewUi(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return (RectTransform)go.transform;
    }

    /// <summary>A UI element at a pixel box measured from the picture's top-left corner.</summary>
    static RectTransform Place(string name, Transform parent, Vector2 centre, Vector2 size)
    {
        RectTransform rt = NewUi(name, parent);
        rt.anchorMin = rt.anchorMax = new Vector2(0f, 1f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(centre.x, -centre.y);
        rt.sizeDelta = size;
        return rt;
    }

    static Image UiImage(string name, Transform parent, Sprite sprite, Color colour, Vector2 centre, Vector2 size, string flip)
    {
        RectTransform rt = Place(name, parent, centre, size);
        var image = rt.gameObject.AddComponent<Image>();
        image.sprite = sprite;
        image.color = colour;
        rt.localScale = new Vector3(flip.Contains("x") ? -1f : 1f, flip.Contains("y") ? -1f : 1f, 1f);
        return image;
    }

    /// <summary>Text whose letters fill `fill` of the box's height (the box is the height of the letters' ink, so the
    /// font size is bigger than the box: capitals and descenders together are about 0.93 of the font size).</summary>
    static Text UiText(string name, Transform parent, string text, Color colour, Vector2 centre, Vector2 size, float fill)
    {
        RectTransform rt = Place(name, parent, centre, size);
        var t = rt.gameObject.AddComponent<Text>();
        t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        t.text = text;
        t.color = colour;
        t.alignment = TextAnchor.MiddleCenter;
        t.horizontalOverflow = HorizontalWrapMode.Overflow;
        t.verticalOverflow = VerticalWrapMode.Overflow;
        t.raycastTarget = false;
        float ink = 0.72f + (text.IndexOfAny("gjpqy,;".ToCharArray()) >= 0 ? 0.21f : 0f);
        t.fontSize = Mathf.Max(1, Mathf.RoundToInt(size.y * fill / ink));
        // If that makes it wider than its box, shrink it to fit.
        float preferred = t.preferredWidth;
        float room = size.x * (fill < 1f ? 0.8f : 1.05f);
        if (preferred > room && preferred > 0f) t.fontSize = Mathf.Max(1, Mathf.FloorToInt(t.fontSize * room / preferred));
        return t;
    }

    /// <summary>A SpriteRenderer stretched over a pixel box (world units = pixels / pixelsPerUnit, y down negative).</summary>
    static void WorldSprite(string name, Transform parent, Sprite sprite, Color colour, Vector2 centre, Vector2 size,
                            string flip, int order, float pixelsPerUnit)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = colour;
        sr.sortingOrder = order;
        sr.flipX = flip.Contains("x");
        sr.flipY = flip.Contains("y");
        Bounds b = sprite.bounds; // in the sprite's own units, around its pivot
        var scale = new Vector3(size.x / pixelsPerUnit / b.size.x, size.y / pixelsPerUnit / b.size.y, 1f);
        go.transform.localScale = scale;
        // Flipping mirrors the sprite around its pivot, so the offset from pivot to centre flips too.
        var offset = new Vector3(b.center.x * (sr.flipX ? -1f : 1f) * scale.x, b.center.y * (sr.flipY ? -1f : 1f) * scale.y, 0f);
        go.transform.localPosition = new Vector3(centre.x / pixelsPerUnit, -centre.y / pixelsPerUnit, 0f) - offset;
    }

    /// <summary>A plain white sprite for coloured rects in the world, saved once in Assets/VisionTools.</summary>
    static Sprite WhiteSprite()
    {
        const string path = "Assets/VisionTools/White.png";
        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        if (sprite != null) return sprite;
        Directory.CreateDirectory("Assets/VisionTools");
        var tex = new Texture2D(4, 4);
        var pixels = new Color[16];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.white;
        tex.SetPixels(pixels);
        File.WriteAllBytes(path, tex.EncodeToPNG());
        DestroyImmediate(tex);
        AssetDatabase.ImportAsset(path);
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = 4;
        importer.filterMode = FilterMode.Point;
        importer.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    /// <summary>Buttons need an EventSystem to be clicked: add one (with the Input System's module if the project uses
    /// it, as Unity's own GameObject → UI → Event System does) unless the scene has one.</summary>
    static void EnsureEventSystem()
    {
        if (FindFirstObjectByType<EventSystem>() != null) return;
        var go = new GameObject("EventSystem", typeof(EventSystem));
        Type inputSystemModule = Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
        if (inputSystemModule != null) go.AddComponent(inputSystemModule);
        else go.AddComponent<StandaloneInputModule>();
        Undo.RegisterCreatedObjectUndo(go, "Build scene from JSON");
    }

    static Color ParseColour(string colour, Color fallback)
    {
        return !string.IsNullOrEmpty(colour) && ColorUtility.TryParseHtmlString(colour, out Color c) ? c : fallback;
    }
}
#endif
