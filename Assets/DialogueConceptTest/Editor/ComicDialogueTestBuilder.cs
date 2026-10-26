using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DialogueConceptTest.Editor
{
    public static class ComicDialogueTestBuilder
    {
        private const string Root = "Assets/DialogueConceptTest";
        public const string ScenePath = Root + "/Test.unity";
        private static Font font;

        [InitializeOnLoadMethod]
        private static void ScheduleBuild()
        {
            if (Application.isBatchMode) return;
            EditorApplication.delayCall += () =>
            {
                if (!File.Exists(ScenePath) && !EditorApplication.isPlayingOrWillChangePlaymode)
                    Build();
            };
        }

        [MenuItem("Tools/Test Setup/Open Comic Dialogue Test")]
        public static void Open()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            if (!File.Exists(ScenePath)) Build();
            EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        }

        [MenuItem("Tools/Test Setup/Open Comic Dialogue Test", true)]
        private static bool CanOpen() => !EditorApplication.isPlayingOrWillChangePlaymode;

        private static Sprite Slice(Texture2D atlas, string name, int x, int y, int w, int h)
        {
            string path = Root + "/Art/" + name + ".png";
            var texture = new Texture2D(w, h, TextureFormat.RGBA32, false);
            var pixels = atlas.GetPixels(x, atlas.height - y - h, w, h);
            // Extract each icon's connected silhouette; atlas neighbours may enter
            // a rectangular crop at the slanted corners of parallelogram buttons.
            if (y == 740)
            {
                var visited = new bool[pixels.Length];
                var largest = new List<int>();
                for (int seed = 0; seed < pixels.Length; seed++)
                {
                    if (visited[seed] || pixels[seed].a < .1f) continue;
                    var component = new List<int>();
                    var queue = new Queue<int>();
                    queue.Enqueue(seed);
                    visited[seed] = true;
                    while (queue.Count > 0)
                    {
                        int p = queue.Dequeue();
                        component.Add(p);
                        int px = p % w;
                        int py = p / w;
                        for (int dy = -1; dy <= 1; dy++)
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            int nx = px + dx, ny = py + dy;
                            if (nx < 0 || nx >= w || ny < 0 || ny >= h) continue;
                            int next = ny * w + nx;
                            if (visited[next] || pixels[next].a < .1f) continue;
                            visited[next] = true;
                            queue.Enqueue(next);
                        }
                    }
                    if (component.Count > largest.Count) largest = component;
                }
                var keep = new bool[pixels.Length];
                foreach (int p in largest)
                {
                    int px = p % w, py = p / w;
                    for (int dy = -1; dy <= 1; dy++)
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        int nx = px + dx, ny = py + dy;
                        if (nx >= 0 && nx < w && ny >= 0 && ny < h) keep[ny * w + nx] = true;
                    }
                }
                for (int p = 0; p < pixels.Length; p++)
                    if (!keep[p]) pixels[p] = Color.clear;
            }
            texture.SetPixels(pixels);
            texture.Apply();
            File.WriteAllBytes(path, texture.EncodeToPNG());
            Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(path);
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.maxTextureSize = 2048;
            importer.SaveAndReimport();
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        private static RectTransform Rect(string name, Transform parent, float x, float y, float w, float h)
        {
            var obj = new GameObject(name, typeof(RectTransform));
            var rect = (RectTransform)obj.transform;
            rect.SetParent(parent, false);
            rect.anchorMin = rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(x, -y);
            rect.sizeDelta = new Vector2(w, h);
            return rect;
        }

        private static Image Picture(string name, Transform parent, Sprite sprite, float x, float y, float w, float h)
        {
            var image = Rect(name, parent, x, y, w, h).gameObject.AddComponent<Image>();
            image.sprite = sprite;
            image.raycastTarget = false;
            return image;
        }

        private static Text Label(string name, Transform parent, string text, float x, float y, float w, float h,
            int size, Color color, TextAnchor alignment = TextAnchor.MiddleLeft)
        {
            var label = Rect(name, parent, x, y, w, h).gameObject.AddComponent<Text>();
            label.font = font;
            label.text = text;
            label.fontSize = size;
            label.color = color;
            label.alignment = alignment;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Truncate;
            label.raycastTarget = false;
            return label;
        }

        private static Button Clickable(Image image)
        {
            image.raycastTarget = true;
            var button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(.82f, .82f, .82f);
            colors.pressedColor = new Color(.55f, .55f, .55f);
            colors.selectedColor = new Color(.9f, .9f, .9f);
            colors.disabledColor = new Color(.46f, .46f, .46f, .7f);
            button.colors = colors;
            return button;
        }

        private static void SavePart(GameObject obj, string name)
        {
            PrefabUtility.SaveAsPrefabAsset(obj, Root + "/Prefabs/" + name + ".prefab");
        }

        public static void Build()
        {
            if (File.Exists(ScenePath)) return;
            Directory.CreateDirectory(Root + "/Prefabs");
            font = AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/Pretendard-ExtraBold.otf");
            var importer = (TextureImporter)AssetImporter.GetAtPath(Root + "/Art/ComicUIAtlas.png");
            if (importer == null || font == null) throw new System.InvalidOperationException("Comic test source assets missing");
            importer.isReadable = true;
            importer.maxTextureSize = 2048;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
            var atlas = AssetDatabase.LoadAssetAtPath<Texture2D>(Root + "/Art/ComicUIAtlas.png");
            var panelSprite = Slice(atlas, "DialoguePanel", 10, 40, 1520, 405);
            var choiceSprite = Slice(atlas, "ChoiceButton", 20, 495, 1015, 166);
            var nameSprite = Slice(atlas, "SpeakerPlate", 1040, 495, 485, 166);
            string[] names = { "Talk", "Branch", "History", "Skip", "Back", "Continue" };
            int[] xs = { 20, 268, 510, 758, 1002, 1250 };
            var icons = names.Select((name, i) => Slice(atlas, name, xs[i], 740, 265, 196)).ToArray();

            var previous = SceneManager.GetActiveScene();
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,
                Application.isBatchMode ? NewSceneMode.Single : NewSceneMode.Additive);
            SceneManager.SetActiveScene(scene);
            try
            {
                var camera = new GameObject("Preview Camera").AddComponent<Camera>();
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = new Color(.16f, .16f, .17f);
                camera.orthographic = true;
                camera.transform.position = new Vector3(0, 0, -10);
                var canvasObject = new GameObject("Comic Dialogue Preview", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
                var canvas = canvasObject.GetComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                var scaler = canvasObject.GetComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.matchWidthOrHeight = .5f;
                var content = Rect("1920x1080 Layout", canvas.transform, 0, 0, 1920, 1080);
                content.anchorMin = content.anchorMax = new Vector2(.5f, .5f);
                content.pivot = new Vector2(.5f, .5f);
                var preview = canvasObject.AddComponent<ComicDialoguePreview>();
                Label("Test Label", content, "만화 컷형 / 대화 UI 테스트", 60, 45, 1000, 60, 30, Color.white);
                Label("Test Hint", content, "배경 없는 독립 시안 · 선택지와 기능 버튼을 눌러보세요", 60, 108, 1200, 50, 24, new Color(.65f, .65f, .65f));

                var choices = Rect("Choices", content, 1200, 245, 660, 390);
                preview.choices = choices.gameObject;
                preview.choiceButtons = new Button[3];
                string[] options = { "기사를 공개한다", "증거를 더 찾는다", "침묵한다" };
                for (int i = 0; i < 3; i++)
                {
                    var image = Picture("Choice_" + (i + 1), choices, choiceSprite, 0, i * 130, 660, 110);
                    preview.choiceButtons[i] = Clickable(image);
                    var badge = Picture("Number Badge", image.transform, nameSprite, 8, 8, 100, 90);
                    Label("Number", badge.transform, (i + 1).ToString("00"), 0, 0, 100, 90, 30, Color.white, TextAnchor.MiddleCenter);
                    Label("Choice Text", image.transform, options[i], 132, 10, 460, 82, 32, Color.black);
                    preview.choiceButtons[i].interactable = i != 2;
                    SavePart(image.gameObject, "Choice_" + (i + 1));
                }

                var panel = Picture("Dialogue Panel", content, panelSprite, 35, 720, 1550, 325);
                preview.dialogue = Label("Dialogue Text", panel.transform,
                    "진실을 쓰는 데에도 대가가 따르지.\n하지만 우리가 침묵하면,\n이 사건은 영원히 묻히고 말 거야.",
                    105, 88, 1320, 205, 38, Color.black, TextAnchor.UpperLeft);
                preview.dialogue.lineSpacing = 1.12f;
                var plate = Picture("Speaker Plate", panel.transform, nameSprite, 65, 2, 250, 79);
                Label("Speaker Name", plate.transform, "편집장", 15, 0, 220, 69, 32, Color.white, TextAnchor.MiddleCenter);
                Label("Continue Marker", panel.transform, "▼", 1430, 248, 45, 45, 30, Color.black);
                SavePart(panel.gameObject, "DialoguePanel");

                var controls = Rect("Function Controls - 2 Columns", content, 1620, 720, 260, 330);
                string[] captions = { "대화", "분기", "기록", "넘기기", "이전", "계속" };
                preview.controls = new Button[6];
                for (int i = 0; i < 6; i++)
                {
                    var image = Picture(names[i], controls, icons[i], (i % 2) * 135, (i / 2) * 110, 125, 82);
                    preview.controls[i] = Clickable(image);
                    Label("Caption", image.transform, captions[i], 0, 80, 125, 25, 20, Color.white, TextAnchor.MiddleCenter);
                    SavePart(image.gameObject, names[i] + "Button");
                }

                var history = Picture("History Overlay", content, panelSprite, 70, 225, 1070, 420);
                Label("History Text", history.transform,
                    "대화 기록\n\n편집장 : 진실을 쓰는 데에도 대가가 따르지.\n편집장 : 하지만 우리가 침묵하면,\n이 사건은 영원히 묻히고 말 거야.",
                    85, 105, 900, 285, 28, Color.black, TextAnchor.UpperLeft);
                preview.history = history.gameObject;
                history.gameObject.SetActive(false);
                new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
                SavePart(canvasObject, "ComicDialoguePreview");
                EditorSceneManager.SaveScene(scene, ScenePath);
                AssetDatabase.SaveAssets();
                Debug.Log("Comic dialogue test created: " + ScenePath);
            }
            finally
            {
                if (!Application.isBatchMode)
                {
                    if (previous.IsValid()) SceneManager.SetActiveScene(previous);
                    EditorSceneManager.CloseScene(scene, true);
                }
            }
        }

        // Used by the isolated batch validation project; never runs during a normal import.
        public static void ValidateAndRender()
        {
            Build();
            var scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            var preview = Object.FindObjectOfType<ComicDialoguePreview>();
            if (preview == null || preview.choiceButtons.Length != 3 || preview.controls.Length != 6)
                throw new System.Exception("Preview UI references are incomplete");
            preview.SendMessage("Awake");
            string initial = preview.dialogue.text;
            preview.controls[5].onClick.Invoke();
            if (preview.dialogue.text == initial) throw new System.Exception("Continue button did not advance");
            preview.controls[4].onClick.Invoke();
            if (preview.dialogue.text != initial) throw new System.Exception("Back button did not restore dialogue");
            preview.choiceButtons[0].onClick.Invoke();
            if (!preview.dialogue.text.Contains("마지막까지")) throw new System.Exception("Choice button did not respond");
            preview.controls[2].onClick.Invoke();
            if (!preview.history.activeSelf) throw new System.Exception("History button did not open history");
            preview.controls[2].onClick.Invoke();
            preview.controls[1].onClick.Invoke();
            if (preview.choices.activeSelf) throw new System.Exception("Branch toggle did not hide choices");
            preview.controls[1].onClick.Invoke();
            preview.controls[3].onClick.Invoke();
            if (!preview.dialogue.text.Contains("오늘의 취재")) throw new System.Exception("Skip did not advance to last page");
            preview.controls[0].onClick.Invoke();
            if (preview.choiceButtons[2].interactable) throw new System.Exception("Locked choice should be disabled");

            var canvas = preview.GetComponent<Canvas>();
            var camera = Object.FindObjectOfType<Camera>();
            var target = new RenderTexture(1920, 1080, 24);
            camera.targetTexture = target;
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera;
            canvas.planeDistance = 10;
            Canvas.ForceUpdateCanvases();
            camera.Render();
            RenderTexture.active = target;
            var screenshot = new Texture2D(1920, 1080, TextureFormat.RGB24, false);
            screenshot.ReadPixels(new UnityEngine.Rect(0, 0, 1920, 1080), 0, 0);
            screenshot.Apply();
            File.WriteAllBytes(Root + "/Preview.png", screenshot.EncodeToPNG());
            RenderTexture.active = null;
            camera.targetTexture = null;
            Object.DestroyImmediate(screenshot);
            Object.DestroyImmediate(target);
            Debug.Log("COMIC_PREVIEW_VALIDATED: 3 choices, 6 controls, interactions and 1920x1080 render passed.");
        }
    }
}
