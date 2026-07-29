using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameEditor;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Reflection;

public static class DayEditorSceneBuilder
{
    private const string ScenePath = "Assets/Editor/DayEditor.unity";
    private const string FontPath = "Assets/Fonts/GyeongiCheonnyeon SDF.asset";

    [InitializeOnLoadMethod]
    private static void RebuildOutdatedSceneOnce()
    {
        EditorApplication.delayCall += () =>
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling) return;
            SceneAsset sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath);
            if (sceneAsset == null) return;
            string marker = $"DayEditorScene.v2:{AssetDatabase.AssetPathToGUID(ScenePath)}";
            if (SessionState.GetString("DayEditorSceneBuilder.Version", "") == marker) return;
            SessionState.SetString("DayEditorSceneBuilder.Version", marker);
            Rebuild();
        };
    }

    [MenuItem("Tools/Day Editor/Rebuild Scene")]
    public static void Rebuild()
    {
        TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
        if (font == null) throw new InvalidOperationException($"경기천년 폰트를 찾을 수 없습니다: {FontPath}");

        List<AudioClip> bgm = ReadWorldBgm();
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "DayEditor";

        var cameraObject = new GameObject("Main Camera");
        var camera = cameraObject.AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(.04f, .05f, .07f);
        cameraObject.tag = "MainCamera";

        var eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<StandaloneInputModule>();

        var root = new GameObject("DayEditor");
        DayEditorController controller = root.AddComponent<DayEditorController>();
        SerializedObject serialized = new(controller);
        serialized.FindProperty("koreanFont").objectReferenceValue = font;
        SetBackgrounds(serialized.FindProperty("backgrounds"));
        SetNpcVisuals(serialized.FindProperty("npcVisuals"));
        SetObjectList(serialized.FindProperty("bgmClips"), bgm.Cast<UnityEngine.Object>().ToList());
        SetStringList(serialized.FindProperty("miniGameScenes"), EditorBuildSettings.scenes
            .Where(x => x.path.Contains("/Scenes/Minigame/", StringComparison.OrdinalIgnoreCase))
            .Select(x => Path.GetFileNameWithoutExtension(x.path)).Distinct().OrderBy(x => x).ToList());
        serialized.ApplyModifiedPropertiesWithoutUndo();

        EditorSceneManager.SaveScene(scene, ScenePath);
        AssetDatabase.SaveAssets();
        Debug.Log($"DayEditor 씬 재구성 완료: {ScenePath}");
    }

    public static void SmokeTest()
    {
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        DayEditorController controller = scene.GetRootGameObjects()
            .SelectMany(x => x.GetComponentsInChildren<DayEditorController>(true)).Single();
        MethodInfo awake = typeof(DayEditorController).GetMethod("Awake",
            BindingFlags.Instance | BindingFlags.NonPublic);
        awake?.Invoke(controller, null);

        TMP_FontAsset expected = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(FontPath);
        TMP_Text[] texts = controller.GetComponentsInChildren<TMP_Text>(true);
        if (texts.Length == 0) throw new InvalidOperationException("DayEditor 동적 UI가 생성되지 않았습니다.");
        if (texts.Any(x => x.font != expected))
            throw new InvalidOperationException("경기천년 폰트를 사용하지 않는 TMP UI가 있습니다.");
        Canvas canvas = controller.GetComponentInChildren<Canvas>(true);
        if (canvas == null || !canvas.pixelPerfect)
            throw new InvalidOperationException("DayEditor Canvas가 픽셀 퍼펙트로 설정되지 않았습니다.");
        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        if (scaler == null || scaler.referenceResolution != new Vector2(1920, 1080))
            throw new InvalidOperationException("DayEditor 기준 해상도가 1920x1080이 아닙니다.");
        RectTransform[] rects = controller.GetComponentsInChildren<RectTransform>(true);
        if (rects.Any(x => x.anchorMin.x < 0 || x.anchorMin.y < 0 ||
                           x.anchorMax.x > 1 || x.anchorMax.y > 1 ||
                           x.anchorMin.x > x.anchorMax.x || x.anchorMin.y > x.anchorMax.y ||
                           x.pivot.x < 0 || x.pivot.x > 1 || x.pivot.y < 0 || x.pivot.y > 1 ||
                           x.localScale != Vector3.one))
            throw new InvalidOperationException("유효하지 않은 Anchor/Pivot/Scale을 가진 UI가 있습니다.");

        TMP_InputField newName = (TMP_InputField)typeof(DayEditorController)
            .GetField("newNameField", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(controller);
        newName.SetTextWithoutNotify("__dayeditor_smoke__.json");
        typeof(DayEditorController).GetMethod("CreateNew",
            BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(controller, null);
        DailyData uiSample = (DailyData)typeof(DayEditorController)
            .GetField("currentData", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(controller);
        uiSample.dayTimes[0].npc.Add(new ChatObjectData
        {
            name = "LayoutTest",
            objectType = "Rex",
            positions = new List<WorldVector> { new(World.Street, 0) },
            anchor = new List<Anchor> { new() },
            chat = new List<string> { "layout-test.json" },
            onAwake = new List<bool> { false }
        });
        typeof(DayEditorController).GetMethod("RebuildEditor",
            BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(controller, null);
        Canvas.ForceUpdateCanvases();

        RectTransform[] npcLayoutRects = controller.GetComponentsInChildren<RectTransform>(true);
        RectTransform identityColumn = npcLayoutRects.FirstOrDefault(x => x.name == "NPC 및 ChatData");
        RectTransform anchorColumn = npcLayoutRects.FirstOrDefault(x => x.name == "위치 및 Anchor");
        if (identityColumn == null || anchorColumn == null || identityColumn.parent != anchorColumn.parent)
            throw new InvalidOperationException("NPC 정보와 위치/Anchor가 두 열로 배치되지 않았습니다.");
        if (Mathf.Abs(identityColumn.position.y - anchorColumn.position.y) > 1f)
            throw new InvalidOperationException("NPC 두 열의 상단 정렬이 맞지 않습니다.");

        TMP_InputField[] inputs = controller.GetComponentsInChildren<TMP_InputField>(true);
        if (inputs.Length == 0 || inputs.Any(x => !x.customCaretColor || x.caretWidth < 2))
            throw new InvalidOperationException("입력 필드의 포커스 캐럿 설정이 올바르지 않습니다.");
        TMP_Dropdown[] dropdowns = controller.GetComponentsInChildren<TMP_Dropdown>(true);
        if (dropdowns.Length == 0)
            throw new InvalidOperationException("테스트할 TMP Dropdown이 없습니다.");
        TMP_Dropdown dropdown = dropdowns[0];
        // EditMode에서 컨트롤러 Awake를 직접 호출했으므로 Unity가 런타임에 호출하는
        // TMP_Dropdown.Awake도 명시적으로 실행해 실제 Show 경로를 검증한다.
        typeof(TMP_Dropdown).GetMethod("Awake",
            BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(dropdown, null);
        typeof(TMP_Dropdown).GetMethod("Start",
            BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(dropdown, null);
        dropdown.Show();
        Canvas.ForceUpdateCanvases();
        Transform dropdownList = dropdown.transform.Find("Dropdown List");
        if (dropdownList == null || !dropdownList.gameObject.activeInHierarchy ||
            dropdownList.GetComponentsInChildren<Toggle>(true).Length == 0)
            throw new InvalidOperationException("TMP Dropdown이 실제 선택 목록을 생성하지 못했습니다.");
        Canvas.ForceUpdateCanvases();
        RectTransform dropdownListRect = dropdownList as RectTransform;
        Toggle[] optionToggles = dropdownList.GetComponentsInChildren<Toggle>(true);
        if (dropdownListRect.rect.height < 1f ||
            optionToggles.Any(x => ((RectTransform)x.transform).rect.height < 1f))
            throw new InvalidOperationException("TMP Dropdown 선택 목록 또는 항목의 표시 높이가 0입니다.");
        dropdown.Hide();

        DailyData sample = DayEditorDataService.CreateDefault();
        if (DayEditorValidator.Validate(sample, null, null, false)
            .Any(x => x.severity == DayEditorIssueSeverity.Error))
            throw new InvalidOperationException("기본 DailyData가 구조 검증을 통과하지 못했습니다.");
        sample.dayTimes = new TimeData[3];
        if (!DayEditorValidator.Validate(sample, null, null, false)
            .Any(x => x.severity == DayEditorIssueSeverity.Error && x.path == "dayTimes"))
            throw new InvalidOperationException("TimeData 개수 오류 검증에 실패했습니다.");

        sample = DayEditorDataService.CreateDefault();
        sample.dayTimes[0].npc.Add(new ChatObjectData
        {
            positions = new List<WorldVector> { new(World.Street, 0) },
            anchor = new List<Anchor> { new() { size = 0 } },
            chat = new List<string> { "hidden-awake.json" },
            onAwake = new List<bool> { true }
        });
        if (DayEditorValidator.Validate(sample, null, null, false)
            .Any(x => x.severity == DayEditorIssueSeverity.Error))
            throw new InvalidOperationException("size=0/onAwake=true 숨김 NPC가 검증을 통과하지 못했습니다.");
        sample.dayTimes[0].npc[0].onAwake[0] = false;
        if (!DayEditorValidator.Validate(sample, null, null, false)
            .Any(x => x.severity == DayEditorIssueSeverity.Error &&
                      x.path.EndsWith(".anchor[0].size", StringComparison.Ordinal)))
            throw new InvalidOperationException("size=0/onAwake=false 오류 검증에 실패했습니다.");

        Debug.Log($"DayEditor 스모크 테스트 통과: TMP {texts.Length}개, RectTransform {rects.Length}개, " +
                  $"Input {inputs.Length}개, Dropdown {dropdowns.Length}개, " +
                  "픽셀 퍼펙트/Anchor/Pivot/캐럿/Dropdown/기본값/구조 검증 정상");
    }

    private static List<AudioClip> ReadWorldBgm()
    {
        var clips = new List<AudioClip>();
        Scene scene = EditorSceneManager.OpenScene("Assets/Scenes/MainWorld.unity", OpenSceneMode.Additive);
        try
        {
            SoundManager manager = scene.GetRootGameObjects()
                .SelectMany(x => x.GetComponentsInChildren<SoundManager>(true))
                .FirstOrDefault(x => x.clips != null && x.clips.Count >= (int)World.Max);
            if (manager != null && manager.clips != null) clips.AddRange(manager.clips);
        }
        finally { EditorSceneManager.CloseScene(scene, true); }
        return clips;
    }

    private static void SetBackgrounds(SerializedProperty list)
    {
        var entries = new List<(World, int, Sprite)>();
        string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { "Assets/Images/Background" });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            if (sprite == null || !TryMapBackground(Path.GetFileNameWithoutExtension(path), out World world, out int position))
                continue;
            if (entries.All(x => x.Item1 != world || x.Item2 != position)) entries.Add((world, position, sprite));
        }
        list.arraySize = entries.Count;
        for (int i = 0; i < entries.Count; i++)
        {
            SerializedProperty item = list.GetArrayElementAtIndex(i);
            item.FindPropertyRelative("world").enumValueIndex = (int)entries[i].Item1;
            item.FindPropertyRelative("position").intValue = entries[i].Item2;
            item.FindPropertyRelative("sprite").objectReferenceValue = entries[i].Item3;
        }
    }

    private static bool TryMapBackground(string name, out World world, out int position)
    {
        world = World.Street; position = 0;
        string lower = name.ToLowerInvariant();
        // 실제 월드 연결은 이미지 파일명의 분류와 일부 다릅니다.
        if (lower == "square")
        {
            world = World.Street; position = 2; return true;
        }
        if (lower == "temple_0")
        {
            world = World.Street; position = 3; return true;
        }
        if (lower == "temple_1")
        {
            world = World.Temple; position = 0; return true;
        }
        if (lower.StartsWith("street")) world = World.Street;
        else if (lower.StartsWith("bar")) world = World.Bar;
        else if (lower.StartsWith("cafe")) world = World.Cafe;
        else if (lower.StartsWith("restaurant")) world = World.Restaurant;
        else if (lower.StartsWith("temple")) world = World.Temple;
        else if (lower.StartsWith("hallway")) world = World.Hallway;
        else if (lower.StartsWith("office-day") || lower.StartsWith("office-night")) world = World.Office;
        else if (lower.StartsWith("desk")) world = World.Office3;
        else if (lower.StartsWith("interrog")) world = World.Interrogate;
        else return false;
        for (int i = name.Length - 1; i >= 0; i--)
            if (char.IsDigit(name[i])) { position = name[i] - '0'; break; }
        return true;
    }

    private static void SetNpcVisuals(SerializedProperty list)
    {
        string[] types = { "none", "Rex", "Clover", "Henderson", "Kennedy", "King", "Klayton",
            "Price", "Walter", "Mechanic", "Monk", "Reporter", "Nametag" };
        list.arraySize = types.Length;
        for (int i = 0; i < types.Length; i++)
        {
            SerializedProperty item = list.GetArrayElementAtIndex(i);
            item.FindPropertyRelative("type").stringValue = types[i];
            string[] guids = AssetDatabase.FindAssets($"PF_NPC_{types[i]} t:Prefab", new[] { "Assets/Prefabs/MainWorld" });
            if (types[i] == "none") guids = AssetDatabase.FindAssets("PF_NPC t:Prefab", new[] { "Assets/Prefabs/MainWorld" });
            Sprite sprite = null;
            if (guids.Length > 0)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guids[0]));
                sprite = prefab == null ? null : prefab.GetComponentsInChildren<SpriteRenderer>(true)
                    .Select(x => x.sprite).FirstOrDefault(x => x != null);
            }
            item.FindPropertyRelative("sprite").objectReferenceValue = sprite;
        }
    }

    private static void SetObjectList(SerializedProperty list, IReadOnlyList<UnityEngine.Object> values)
    {
        list.arraySize = values.Count;
        for (int i = 0; i < values.Count; i++) list.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
    }

    private static void SetStringList(SerializedProperty list, IReadOnlyList<string> values)
    {
        list.arraySize = values.Count;
        for (int i = 0; i < values.Count; i++) list.GetArrayElementAtIndex(i).stringValue = values[i];
    }
}
