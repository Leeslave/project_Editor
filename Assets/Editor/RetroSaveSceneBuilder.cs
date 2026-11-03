using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using U = RetroSaveUi;

/// <summary>Creates the editable desktop prefab and applies it to the existing SaveSelect scene.</summary>
public static class RetroSaveSceneBuilder
{
    private const string SmokeKey = "RetroSaveSceneBuilder.PlaySmoke";
    public const string ScenePath = "Assets/Scenes/Lobby/SaveSelect.unity";
    public const string PrefabPath = "Assets/Prefabs/SaveSelect/RetroSaveDesktop.prefab";
    public const string SlotPath = "Assets/Prefabs/SaveSelect/RetroSaveSlot.prefab";
    private static Font font;
    private static Sprite folder;
    private static Sprite saveIcon;
    private static Sprite office;

    [InitializeOnLoadMethod]
    private static void ResumePlaySmoke()
    {
        if (!SessionState.GetBool(SmokeKey, false) && !SessionState.GetBool(SmokeKey + ".exit", false)) return;
        EditorApplication.update += PollPlaySmoke;
    }

    /// <summary>Read-only runtime check using the real MainController and player's saved records.</summary>
    public static void RunPlayModeSmokeTest()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/MainController.unity", OpenSceneMode.Single);
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Additive);
        SceneManager.SetActiveScene(scene);
        SessionState.SetBool(SmokeKey, true);
        SessionState.SetBool(SmokeKey + ".exit", false);
        SessionState.SetInt(SmokeKey + ".frames", 0);
        SessionState.SetInt(SmokeKey + ".result", 0);
        EditorApplication.update -= PollPlaySmoke;
        EditorApplication.update += PollPlaySmoke;
        EditorApplication.isPlaying = true;
    }

    private static void PollPlaySmoke()
    {
        if (SessionState.GetBool(SmokeKey + ".exit", false))
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            SessionState.SetBool(SmokeKey + ".exit", false);
            EditorApplication.update -= PollPlaySmoke;
            EditorApplication.Exit(SessionState.GetInt(SmokeKey + ".result", 1));
            return;
        }
        if (!EditorApplication.isPlaying || EditorApplication.isCompiling) return;
        int frames = SessionState.GetInt(SmokeKey + ".frames", 0) + 1;
        SessionState.SetInt(SmokeKey + ".frames", frames);
        if (frames < 30) return;
        try
        {
            RetroSaveDesktop controller = UnityEngine.Object.FindObjectOfType<RetroSaveDesktop>();
            Require(controller != null && GameSystem.Instance != null && GameSystem.SaveService != null,
                "SaveSelect did not initialize with MainController.");
            Require(controller.Records.Count > 0 && controller.loadButton.interactable,
                "Player records did not initialize or load is disabled.");
            Require(controller.slotContent.GetComponentsInChildren<RetroSaveSlot>().Length > 0,
                "No live save slots were created.");
            controller.OpenMap();
            Require(controller.graphContent.GetComponentsInChildren<Button>().Length == controller.Records.Count,
                "Runtime map contains missing or invented save nodes.");
            int id = controller.SelectedSaveId;
            controller.SelectMapRecord(id);
            controller.UseMapRecord();
            Require(controller.SelectedSaveId == id && !controller.mapWindow.activeSelf,
                "Runtime map selection failed.");
            Capture(controller, "output/save-select-runtime.png");
            controller.OpenMap();
            Capture(controller, "output/save-select-runtime-map.png");
            Debug.Log("WIN98_SAVE_PLAY_OK: MainController, actual records, live slots, load enabled, map selection; no save writes.");
        }
        catch (Exception exception)
        {
            SessionState.SetInt(SmokeKey + ".result", 1);
            Debug.LogException(exception);
        }
        SessionState.SetBool(SmokeKey, false);
        SessionState.SetBool(SmokeKey + ".exit", true);
        EditorApplication.isPlaying = false;
    }

    [MenuItem("Tools/Save Explorer/Rebuild Win98 SaveSelect")]
    public static void RebuildFromMenu()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        BuildAndVerify();
    }

    [MenuItem("Tools/Save Explorer/Open with MainController")]
    public static void OpenWithController()
    {
        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        EditorSceneManager.OpenScene("Assets/Scenes/MainController.unity", OpenSceneMode.Single);
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Additive);
        SceneManager.SetActiveScene(scene);
    }

    public static void BuildAndVerify()
    {
        font = AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/neodgm.ttf");
        folder = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Images/Documents/Folder.png");
        saveIcon = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Images/News/Save.png");
        office = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Images/Background/Office-Night_1.png");
        if (!font || !folder || !saveIcon || !office) throw new InvalidOperationException("Save explorer resources are missing.");

        // The scene GUID and existing build-settings entry remain unchanged.
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var cameraObject = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
        cameraObject.tag = "MainCamera";
        Camera camera = cameraObject.GetComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color32(66, 100, 100, 255);
        camera.orthographic = true;
        cameraObject.transform.position = new Vector3(0, 0, -10);
        var events = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        events.GetComponent<EventSystem>().sendNavigationEvents = true;

        RetroSaveSlot slot = BuildSlot();
        var root = new GameObject("SaveDesktop", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        root.layer = 5;
        Canvas canvas = root.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.pixelPerfect = true;
        CanvasScaler scaler = root.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = .5f;
        RetroSaveDesktop controller = root.AddComponent<RetroSaveDesktop>();
        controller.font = font;
        controller.folderIcon = folder;
        controller.defaultThumbnail = office;
        controller.slotPrefab = slot;

        Image desktop = U.Fill("DesktopBackground", root.transform, 0, 0, 1920, 1080, new Color32(66, 100, 100, 255));
        U.Stretch((RectTransform)desktop.transform);
        Shortcut(root.transform, folder, "내 기록", 28, 45);
        Shortcut(root.transform, saveIcon, "이어하기", 28, 160);
        RectTransform explorer = Window("SaveExplorerWindow", root.transform, 130, 70, 1300, 840,
            "저장 기록 - One Day Dream", controller.ReturnToTitle);
        Menu(explorer);
        U.Picture("FolderIcon", explorer, folder, 32, 110, 62, 56);
        U.Label("Heading", explorer, font, "어느 하루로 돌아갈까요?", 118, 101, 760, 42, 28);
        U.Label("Description", explorer, font, "저장된 날짜와 분기를 선택하세요.", 118, 146, 800, 30, 18);
        Wordmark(explorer, 1080, 108, "기록 보관함");
        U.Label("AddressCaption", explorer, font, "위치(D)", 22, 190, 100, 40, 18);
        RectTransform addressFrame = U.Frame("AddressBar", explorer, 130, 188, 1145, 42, true, Color.white);
        U.Picture("Folder", addressFrame, folder, 12, 10, 27, 22);
        controller.address = U.Label("Path", addressFrame, font, "내 기록", 52, 2, 1020, 38, 18);
        U.Label("DropdownMark", addressFrame, font, "▼", 1105, 2, 30, 38, 16, null, TextAnchor.MiddleCenter);

        controller.dayContent = ScrollArea("DayFolders", explorer, 20, 248, 245, 423, 229, 410);
        U.Label("RecordListHeading", explorer, font, "저장된 기록", 286, 246, 700, 31, 18);
        controller.itemCount = U.Label("ItemCount", explorer, font, "0개 항목", 1090, 246, 180, 31, 18,
            new Color(.3f,.3f,.3f), TextAnchor.MiddleRight);
        controller.slotContent = ScrollArea("SaveFiles", explorer, 282, 283, 992, 285, 976, 265);
        RectTransform properties = U.Frame("SelectedRecord", explorer, 282, 591, 992, 147, true);
        U.Label("Caption", properties, font, "선택한 기록", 14, 7, 250, 26, 18);
        U.Frame("ThumbnailInset", properties, 15, 41, 184, 94, true);
        controller.recordPreview = U.Picture("Thumbnail", properties, office, 18, 44, 178, 88, false);
        controller.recordTitle = U.Label("RecordTitle", properties, font, "저장된 기록 없음", 225, 38, 730, 30, 22, U.Blue);
        controller.recordDetails = U.Label("RecordDetails", properties, font, "", 225, 72, 730, 68, 18);
        Bind(U.Button("BranchMapButton", explorer, font, "분기 지도(B)", 282, 755, 225, 42), controller.OpenMap);
        controller.loadButton = U.Button("LoadButton", explorer, font, "불러오기(L)", 524, 755, 195, 42);
        Bind(controller.loadButton, controller.LoadSelected);
        Bind(U.Button("ReturnButton", explorer, font, "돌아가기", 736, 755, 170, 42), controller.ReturnToTitle);
        RectTransform statusFrame = U.Frame("StatusBar", explorer, 9, 809, 1280, 22, true);
        controller.status = U.Label("Message", statusFrame, font, "날짜와 분기를 선택하세요.", 8, 0, 1240, 20, 16);

        BuildMap(root.transform, controller);
        BuildTaskbar(root.transform, controller);
        var initial = new PlayerData();
        initial.Init();
        controller.Bind(initial); // Visible editor objects, replaced with real records on Start.
        Canvas.ForceUpdateCanvases();
        PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
        UnityEngine.Object.DestroyImmediate(root);
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, scene);
        EditorSceneManager.SaveScene(scene, ScenePath);
        AssetDatabase.SaveAssets();
        // Tests replace dynamic children in edit mode; keep the saved scene linked to its prefab.
        PrefabUtility.UnpackPrefabInstance(instance, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
        Verify(instance.GetComponent<RetroSaveDesktop>());
        Debug.Log("WIN98_SAVE_BUILD_OK: native desktop prefab and SaveSelect scene saved; verification passed.");
    }

    private static RetroSaveSlot BuildSlot()
    {
        RectTransform rect = U.Rect("RetroSaveSlot", null, 0, 0, 456, 222);
        RetroSaveSlot slot = rect.gameObject.AddComponent<RetroSaveSlot>();
        Image background = rect.gameObject.AddComponent<Image>();
        background.color = Color.white;
        slot.button = rect.gameObject.AddComponent<Button>();
        slot.button.targetGraphic = background;
        slot.button.transition = Selectable.Transition.None;
        RectTransform frame = U.Frame("ThumbnailWindow", rect, 0, 0, 456, 174);
        slot.titleBar = U.Fill("TitleBar", frame, 5, 5, 446, 29, U.Gray);
        slot.title = U.Label("BranchTitle", slot.titleBar.transform, font, "기본 분기", 8, 0, 400, 29, 18, Color.white);
        U.Label("MinimizeMark", slot.titleBar.transform, font, "−", 413, 0, 24, 29, 18, Color.white, TextAnchor.MiddleCenter);
        slot.preview = U.Picture("Preview", frame, office, 5, 39, 446, 130, false);
        slot.fileSelection = U.Fill("FileSelection", rect, 70, 181, 316, 25, U.Blue);
        slot.fileName = U.Label("FileName", slot.fileSelection.transform, font, "day00_00.sav", 0, 0, 316, 25, 18,
            Color.white, TextAnchor.MiddleCenter);
        slot.renown = U.Label("Renown", rect, font, "명성 0", 0, 207, 456, 22, 16, U.Gray, TextAnchor.MiddleCenter);
        GameObject asset = PrefabUtility.SaveAsPrefabAsset(rect.gameObject, SlotPath);
        UnityEngine.Object.DestroyImmediate(rect.gameObject);
        return asset.GetComponent<RetroSaveSlot>();
    }

    private static void BuildMap(Transform root, RetroSaveDesktop controller)
    {
        RectTransform map = Window("BranchMapWindow", root, 650, 390, 1210, 620,
            "분기 지도 - One Day Dream", controller.CloseMap);
        controller.mapWindow = map.gameObject;
        Menu(map);
        controller.routeContent = ScrollArea("RouteFolders", map, 16, 93, 210, 505, 198, 490);
        U.Label("Heading", map, font, "이야기의 갈림길", 258, 92, 720, 39, 28);
        U.Label("Description", map, font, "저장된 날짜를 선택해 이어갈 기록을 확인하세요.", 258, 135, 910, 30, 18);
        Wordmark(map, 1000, 98, "분기 지도");
        controller.graphContent = ScrollArea("StoryGraph", map, 245, 185, 943, 258, 910, 240, true);
        RectTransform info = U.Frame("BranchInformation", map, 245, 461, 943, 102, true);
        U.Label("Caption", info, font, "분기 정보", 13, 4, 220, 26, 18);
        controller.mapPreview = U.Picture("Thumbnail", info, office, 14, 35, 144, 56, false);
        controller.mapTitle = U.Label("SelectedNodeTitle", info, font, "", 177, 28, 740, 27, 20, U.Blue);
        controller.mapDetails = U.Label("SelectedNodeDescription", info, font, "", 177, 58, 740, 40, 16);
        U.Label("Legend", map, font, "■ 선택한 기록    □ 저장된 기록    ┄ 저장되지 않은 날짜", 250, 573, 700, 29, 16,
            new Color(.25f,.25f,.25f));
        controller.selectRecordButton = U.Button("SelectRecordButton", map, font, "이 기록 선택", 988, 574, 195, 32);
        Bind(controller.selectRecordButton, controller.UseMapRecord);
    }

    private static void BuildTaskbar(Transform root, RetroSaveDesktop controller)
    {
        RectTransform taskbar = U.Frame("Taskbar", root, 0, 1034, 1920, 46);
        taskbar.anchorMin = new Vector2(0, 0);
        taskbar.anchorMax = new Vector2(1, 0);
        taskbar.pivot = Vector2.zero;
        taskbar.anchoredPosition = Vector2.zero;
        taskbar.sizeDelta = new Vector2(0, 46);
        Bind(U.Button("StartButton", taskbar, font, "시작", 7, 6, 110, 34), controller.ReturnToTitle);
        U.Picture("StartIcon", taskbar, folder, 14, 14, 23, 18);
        U.Frame("ActiveSaveTask", taskbar, 135, 6, 270, 34, true);
        U.Picture("SaveTaskIcon", taskbar, saveIcon, 146, 12, 23, 23);
        U.Label("SaveTaskLabel", taskbar, font, "저장 기록", 183, 6, 210, 34, 18);
        Button mapTask = U.Button("BranchMapTask", taskbar, font, "분기 지도", 417, 6, 245, 34);
        Bind(mapTask, controller.OpenMap);
        controller.mapTask = mapTask.gameObject;
        U.Label("AutosaveStatus", taskbar, font, "진행 상황은 자동으로 저장됩니다.", 1320, 5, 575, 35, 18, null,
            TextAnchor.MiddleRight);
    }

    private static RectTransform Window(string name, Transform parent, float x, float y, float w, float h,
        string title, UnityAction close)
    {
        RectTransform rect = U.Frame(name, parent, x, y, w, h);
        Image bar = U.Fill("TitleBar", rect, 5, 5, w - 10, 36, U.Blue);
        bar.raycastTarget = true;
        RetroWindowDrag drag = bar.gameObject.AddComponent<RetroWindowDrag>();
        drag.window = rect;
        U.Picture("WindowIcon", bar.transform, saveIcon, 7, 7, 23, 23);
        U.Label("Title", bar.transform, font, title, 41, 0, w - 115, 36, 21, Color.white);
        Bind(U.Button("CloseButton", rect, font, "×", w - 41, 9, 29, 28), close);
        return rect;
    }

    private static void Menu(Transform window)
    {
        U.Label("MenuBar", window, font, "파일(F)    보기(V)    도움말(H)", 19, 44, 800, 34, 18);
        U.Fill("MenuSeparator", window, 8, 80, 1180, 1, U.Gray);
    }

    private static void Wordmark(Transform parent, float x, float y, string subtitle)
    {
        U.Label("Wordmark", parent, font, $"ONE DAY\nDREAM\n{subtitle}", x, y, 170, 75, 18,
            new Color(.35f,.35f,.35f), TextAnchor.MiddleRight);
    }

    private static void Shortcut(Transform parent, Sprite icon, string label, float x, float y)
    {
        U.Picture(label + "Icon", parent, icon, x + 15, y, 47, 45);
        U.Label(label + "Label", parent, font, label, x, y + 53, 90, 28, 18, Color.white, TextAnchor.MiddleCenter);
    }

    private static RectTransform ScrollArea(string name, Transform parent, float x, float y,
        float w, float h, float contentW, float contentH, bool horizontal = false)
    {
        RectTransform frame = U.Frame(name, parent, x, y, w, h, true, Color.white);
        RectTransform viewport = U.Rect("Viewport", frame, 3, 3, w - 23, h - (horizontal ? 23 : 6));
        Image maskImage = viewport.gameObject.AddComponent<Image>();
        maskImage.color = Color.white;
        viewport.gameObject.AddComponent<Mask>().showMaskGraphic = true;
        RectTransform content = U.Rect("Content", viewport, 0, 0, contentW, contentH);
        ScrollRect scroll = frame.gameObject.AddComponent<ScrollRect>();
        scroll.viewport = viewport;
        scroll.content = content;
        scroll.horizontal = horizontal;
        scroll.vertical = true;
        scroll.movementType = ScrollRect.MovementType.Clamped;
        scroll.scrollSensitivity = 35;
        scroll.inertia = false;
        scroll.verticalScrollbar = Scrollbar(frame, w - 19, 3, 16, h - (horizontal ? 23 : 6), true);
        scroll.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
        if (horizontal)
        {
            scroll.horizontalScrollbar = Scrollbar(frame, 3, h - 19, w - 23, 16, false);
            scroll.horizontalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
        }
        return content;
    }

    private static Scrollbar Scrollbar(Transform parent, float x, float y, float w, float h, bool vertical)
    {
        Image track = U.Fill(vertical ? "VerticalScrollbar" : "HorizontalScrollbar", parent, x, y, w, h,
            new Color32(223,223,223,255));
        track.raycastTarget = true;
        Scrollbar bar = track.gameObject.AddComponent<Scrollbar>();
        RectTransform sliding = U.Rect("SlidingArea", track.transform, 0, 0, w, h);
        U.Stretch(sliding);
        RectTransform handle = U.Frame("Handle", sliding, 0, 0, w, h);
        U.Stretch(handle);
        bar.handleRect = handle;
        bar.targetGraphic = handle.GetComponent<Image>();
        bar.direction = vertical ? UnityEngine.UI.Scrollbar.Direction.BottomToTop : UnityEngine.UI.Scrollbar.Direction.LeftToRight;
        bar.size = 1;
        return bar;
    }

    private static void Bind(Button button, UnityAction action) => UnityEventTools.AddPersistentListener(button.onClick, action);

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private static void Verify(RetroSaveDesktop controller)
    {
        Require(controller != null && controller.slotPrefab != null, "Desktop/slot prefab is missing.");
        Require(controller.GetComponentsInChildren<SaveSelector>(true).Length == 0, "Legacy save selector still runs.");
        Require(controller.GetComponent<Canvas>().pixelPerfect, "Canvas must be pixel perfect.");
        Require(!controller.mapWindow.activeSelf, "Map must start closed.");
        Require(controller.SelectedSaveId == -1, "Unstarted prefab controller must not have runtime selection state.");
        foreach (Button button in controller.GetComponentsInChildren<Button>(true))
            if (button.onClick.GetPersistentEventCount() > 0)
                Require(button.onClick.GetPersistentTarget(0) != null, $"Missing action target: {button.name}");

        var data = new PlayerData();
        data.Init();
        foreach (int id in new[] { 100, 200, 300, 301, 302, 501 })
            data.Save(new DaySave { dayID = id, renown = id / 10 });
        controller.Bind(data);
        Require(controller.Records.Count == 7 && controller.SelectedSaveId == 501, "Latest save was not selected.");
        controller.SelectRecord(301);
        Require(controller.SelectedSaveId == 301 && controller.itemCount.text == "3개 항목", "Date/branch selection failed.");
        Require(controller.slotContent.GetComponentsInChildren<RetroSaveSlot>().Length == 3, "Not all branches have slots.");
        controller.slotContent.GetComponentsInChildren<RetroSaveSlot>().First(slot => slot.name == "Save_0302").button.onClick.Invoke();
        Require(controller.SelectedSaveId == 302, "Slot button does not select its exact branch.");
        controller.SelectRecord(301);
        Require(controller.slotContent.parent.parent.GetComponent<ScrollRect>().verticalNormalizedPosition > .95f,
            "Selecting a first-row save must reveal its thumbnail instead of scrolling to the bottom.");
        controller.OpenMap();
        Require(controller.mapWindow.activeSelf && controller.mapTask.activeSelf, "Map button did not open the map.");
        controller.FilterRoute(1);
        Require(controller.graphContent.Find("Node_0501") != null && controller.graphContent.Find("Node_0302") == null,
            "Route filter displays incorrect saves.");
        controller.SelectMapRecord(501);
        controller.UseMapRecord();
        Require(controller.SelectedSaveId == 501 && !controller.mapWindow.activeSelf, "Map selection did not return to explorer.");
        controller.SelectRecord(99999);
        Require(controller.SelectedSaveId == 501, "Unknown save IDs must not be selectable.");
        controller.Bind(new PlayerData());
        Require(controller.SelectedSaveId == -1 && !controller.loadButton.interactable, "Empty save must disable load.");
        Require(!controller.selectRecordButton.interactable, "Empty map must disable selection.");
        controller.Bind(data);
        controller.SelectRecord(301);
        Capture(controller, "output/save-select-unity.png");
        controller.OpenMap();
        Capture(controller, "output/save-select-branch-map-unity.png");
        controller.CloseMap();
        var initial = new PlayerData();
        initial.Init();
        controller.Bind(initial);
        // Verification used in-memory sample records only; never persist those scene overrides.
        Debug.Log("WIN98_SAVE_VERIFY_OK: slots, sparse dates, exact branches, map filters, empty saves, persistent events.");
    }

    private static void Capture(RetroSaveDesktop controller, string path)
    {
        Canvas canvas = controller.GetComponent<Canvas>();
        Camera camera = Camera.main;
        var texture = new RenderTexture(1920, 1080, 24);
        var pixels = new Texture2D(1920, 1080, TextureFormat.RGB24, false);
        RenderTexture previous = RenderTexture.active;
        try
        {
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = camera;
            canvas.planeDistance = 1;
            camera.targetTexture = texture;
            Canvas.ForceUpdateCanvases();
            foreach (MaskableGraphic graphic in controller.GetComponentsInChildren<MaskableGraphic>())
            {
                graphic.RecalculateClipping();
                graphic.RecalculateMasking();
                graphic.SetAllDirty();
            }
            Canvas.ForceUpdateCanvases();
            camera.Render();
            RenderTexture.active = texture;
            pixels.ReadPixels(new Rect(0, 0, 1920, 1080), 0, 0);
            pixels.Apply();
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllBytes(path, pixels.EncodeToPNG());
        }
        finally
        {
            camera.targetTexture = null;
            RenderTexture.active = previous;
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.worldCamera = null;
            UnityEngine.Object.DestroyImmediate(pixels);
            texture.Release();
            UnityEngine.Object.DestroyImmediate(texture);
        }
    }
}
