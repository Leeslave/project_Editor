using System;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using U = RetroScreenUi;

/// <summary>원본 Screen을 보존하고 Screen_TEST에 편집 가능한 리메이크 UI를 구성한다.</summary>
public static class RetroScreenSceneBuilder
{
    public const string OriginalScenePath = "Assets/Scenes/Screen.unity";
    public const string ScenePath = "Assets/Scenes/Screen_TEST.unity";
    private static Font font;
    private static TMP_FontAsset tmp;
    private static Sprite box, pressed, selected, inset, close, folder, mail, messages, news, logo;
    private static Scene scene;
    private static RetroScreenDesktop view;

    [MenuItem("Tools/Screen Desktop/Build or Verify Screen_TEST UI")]
    public static void FromMenu()
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) BuildAndVerify();
    }

    public static void BuildAndVerify()
    {
        if (!File.Exists(ScenePath))
        {
            scene = EditorSceneManager.OpenScene(OriginalScenePath, OpenSceneMode.Single);
            Require(EditorSceneManager.SaveScene(scene, ScenePath), "Could not create Screen_TEST from the original scene.");
        }
        else scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
        LoadAssets();
        view = Find<RetroScreenDesktop>();
        if (view != null) { Polish(); EditorSceneManager.SaveScene(scene); VerifyAndCapture(); return; }
        var canvas = FindObject("DisplayCanvas").GetComponent<Canvas>();
        foreach (var instance in canvas.GetComponentsInChildren<Transform>(true)
            .Select(t => PrefabUtility.GetOutermostPrefabInstanceRoot(t.gameObject)).Where(g => g != null).Distinct().ToArray())
            if (PrefabUtility.IsPartOfPrefabInstance(instance))
                PrefabUtility.UnpackPrefabInstance(instance, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);

        ScreenManager screenManager = Find<ScreenManager>();
        WorkManager workManager = Find<WorkManager>();
        MailManager mailManager = Find<MailManager>();
        MsgManager messageManager = Find<MsgManager>();
        NewspaperManager newspaperManager = Find<NewspaperManager>();
        MenuManager menuManager = Find<MenuManager>();
        GameObject mailWindow = FindObject("MailWindow");
        GameObject newsWindow = Reference(newspaperManager, "newsWindow");
        GameObject menuWindow = Reference(menuManager, "menuWindow");
        GameObject messageWindow = FindObject("MsgWindow");
        var desktop = (RectTransform)screenManager.desktop.transform;
        var tutorial = desktop.Find("Tutorial");
        var oldScreen = desktop.parent.gameObject;
        var oldFrame = FindObject("frame");

        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.worldCamera = null;
        canvas.pixelPerfect = true;
        var scaler = canvas.GetComponent<CanvasScaler>();
        scaler.referenceResolution = new Vector2(1600, 1200);
        scaler.matchWidthOrHeight = .5f;
        var letterbox = U.Panel("TerminalLetterbox", canvas.transform, 0, 0, 1600, 1200, Color.black);
        Stretch((RectTransform)letterbox.transform);
        var root = U.Rect("RetroScreenDesktop", canvas.transform, 0, 0, 1600, 1200);
        root.anchorMin = root.anchorMax = root.pivot = new Vector2(.5f, .5f);
        root.anchoredPosition = Vector2.zero;
        view = root.gameObject.AddComponent<RetroScreenDesktop>();
        view.fittedDesktop = root;
        view.font = font; view.inset = inset;
        view.screenManager = screenManager; view.workManager = workManager; view.mailManager = mailManager;
        view.messageManager = messageManager; view.newsManager = newspaperManager; view.menuManager = menuManager;
        view.mailWindow = mailWindow; view.messageWindow = messageWindow; view.newsWindow = newsWindow; view.menuWindow = menuWindow;
        U.Panel("DesktopSurface", root, 0, 0, 1600, 1200, U.Desk);
        Header(root);

        desktop.SetParent(root, false);
        U.Place(desktop, 0, 0, 1600, 1200);
        if (desktop.GetComponent<Image>() != null) desktop.GetComponent<Image>().enabled = false;
        foreach (Transform child in desktop) child.gameObject.SetActive(false);
        var windows = U.Rect("ApplicationWindows", desktop, 314, 178, 1250, 880);
        var home = Window(null, windows, "TerminalHome", "업무 단말 / WORKSPACE", 1250, 880, false);
        view.homeWindow = home.gameObject;
        BuildHome(home);
        BuildWork(Window(workManager.taskWindow, windows, "TaskWindow", "업무 지원 프로그램 / ASSIGNMENTS", 1250, 880), workManager);
        BuildMail(Window(mailWindow, windows, "MailWindow", "수신함 / INTERNAL MAIL", 1250, 880), mailManager);
        BuildMessages(Window(messageWindow, windows, "MsgWindow", "메신저 / INTERNAL MESSAGES", 1250, 880), messageManager);
        BuildNews(Window(newsWindow, windows, "NewsWindow (1)", "신문 열람 / PRESS ARCHIVE", 1250, 880));
        BuildMenu(Window(menuWindow, windows, "MenuWindow", "프로그램 / PROGRAMS", 900, 740));
        U.Place((RectTransform)menuWindow.transform, 175, 50, 900, 740);
        var nav = BuildNavigation(desktop);
        Footer(root, screenManager);
        BuildBoot(screenManager, root);
        if (tutorial != null) AdaptTutorial(tutorial, desktop, nav, mailManager, workManager, newsWindow);

        // 예전 배경과 장식만 숨긴다. 매니저/애니메이션/튜토리얼 객체는 유지한다.
        oldScreen.SetActive(false);
        oldFrame.SetActive(false);
        screenManager.desktop.SetActive(true);
        screenManager.bootPanel.SetActive(false);
        view.homeWindow.SetActive(true);
        foreach (var window in new[] { workManager.taskWindow, mailWindow, messageWindow, newsWindow, menuWindow }) window.SetActive(false);
        Polish();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        VerifyAndCapture();
        Debug.Log("SCREEN_REMAKE_BUILD_OK: Screen_TEST saved; original Screen scene preserved.");
    }

    private static void LoadAssets()
    {
        font = AssetDatabase.LoadAssetAtPath<Font>("Assets/Fonts/neodgm.ttf");
        tmp = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/Fonts/neodgm SDF.asset");
        box = Sprite("Assets/Plugins/Desktop90_UI/Sprites/Boxes/Box.png");
        pressed = Sprite("Assets/Plugins/Desktop90_UI/Sprites/Boxes/BoxPressed.png");
        selected = Sprite("Assets/Plugins/Desktop90_UI/Sprites/Boxes/BoxSelected.png");
        inset = Sprite("Assets/Plugins/Desktop90_UI/Sprites/Boxes/BoxDepressGrey.png");
        close = Sprite("Assets/Plugins/Desktop90_UI/Sprites/Symbols/CloseSymbol.png");
        folder = Sprite("Assets/Images/Documents/Folder.png");
        mail = Sprite("Assets/Images/UI/MailIcon.png");
        messages = Sprite("Assets/Images/Sprite/ChatIcon.png");
        news = Sprite("Assets/Images/Sprite/news 1.png");
        logo = Sprite("Assets/Images/Sprite/Logo.png");
        Require(font != null && tmp != null && box != null && inset != null, "Screen UI assets are missing.");
    }

    private static void Header(Transform root)
    {
        U.Panel("TopRule", root, 32, 28, 1536, 2, new Color32(105, 131, 109, 255));
        U.Icon("ExistingLogo", root, logo, 40, 48, 65, 78);
        U.Label("Agency", root, font, "아슬라니아 정보국", 126, 43, 780, 55, 37, U.Paper);
        U.Label("TerminalCaption", root, font, "INFORMATION BUREAU  /  INTERNAL WORKSTATION", 128, 102, 1020, 30, 19, U.Green);
        U.Label("Operator", root, font, "담당자  클레이튼", 1160, 48, 385, 37, 25, U.Paper, TextAnchor.MiddleRight);
        view.connectionText = U.Label("PowerState", root, font, "업무 화면", 1190, 94, 355, 30, 20, U.Green, TextAnchor.MiddleRight);
        U.Panel("HeaderRule", root, 32, 149, 1536, 2, new Color32(105, 131, 109, 255));
    }

    private static RectTransform Window(GameObject existing, Transform parent, string name, string title, float w, float h, bool closable = true)
    {
        var rect = existing == null ? U.Rect(name, parent, 0, 0, w, h) : (RectTransform)existing.transform;
        rect.SetParent(parent, false);
        U.Place(rect, 0, 0, w, h);
        foreach (Transform child in rect) child.gameObject.SetActive(false);
        var image = rect.GetComponent<Image>() ?? rect.gameObject.AddComponent<Image>();
        image.sprite = box; image.type = Image.Type.Sliced; image.color = U.Face; image.raycastTarget = true;
        var group = rect.GetComponent<CanvasGroup>();
        if (group != null) { group.alpha = 1; group.interactable = group.blocksRaycasts = true; }
        var titlebar = U.Panel("TerminalTitlebar", rect, 7, 7, w - 14, 52, U.Title, null, true);
        U.Icon("ApplicationIcon", titlebar.transform, folder, 12, 9, 33, 33);
        U.Label("WindowTitle", titlebar.transform, font, title, 58, 2, w - 170, 48, 24, U.Paper);
        if (closable)
        {
            var button = Button("Close", titlebar.transform, "", w - 66, 8, 40, 36);
            U.Icon("CloseSymbol", button.transform, close, 10, 8, 20, 20);
            UnityEventTools.AddBoolPersistentListener(button.onClick, rect.gameObject.SetActive, false);
        }
        U.Label("Location", rect, font, "내부망  /  정보 처리 단말", 24, 72, w - 48, 32, 19, new Color(.33f, .38f, .33f));
        U.Panel("ToolbarDivider", rect, 20, 117, w - 40, 2, new Color(.51f, .55f, .48f));
        U.Panel("WindowStatus", rect, 12, h - 41, w - 24, 28, U.Face, inset);
        U.Label("WindowStatusText", rect, font, "INTERNAL USE ONLY", 28, h - 40, w - 60, 26, 16, new Color(.34f, .4f, .34f));
        return rect;
    }

    private static void BuildHome(RectTransform home)
    {
        U.Label("Welcome", home, font, "오늘의 업무", 35, 146, 850, 64, 36);
        U.Label("Description", home, font, "배정된 업무와 수신 자료를 확인하십시오.", 37, 219, 850, 44, 23);
        view.workSummary = U.Label("WorkCount", home, font, "업무 정보 대기", 38, 284, 835, 42, 22);
        var content = Scroll("AssignedWork", home, 34, 343, 850, 311, U.Paper);
        view.taskRows = content;
        view.taskEmpty = U.Label("NoAssignments", home, font, "배정된 업무는 접속 후 표시됩니다.", 62, 388, 780, 110, 24, new Color(.36f, .41f, .35f));
        Bind(Button("OpenWork", home, "업무 프로그램 열기", 35, 691, 850, 73), view.OpenWork);
        var side = U.Panel("OfficeNote", home, 918, 147, 299, 617, U.Paper, inset);
        U.Label("NoteNumber", side.transform, font, "업무 지침", 23, 22, 250, 46, 27);
        U.Panel("NoteRule", side.transform, 23, 85, 250, 2, U.Title);
        U.Label("NoteText", side.transform, font,
            "01\n배정된 업무를\n확인하십시오.\n\n02\n문서와 통신은\n지시에 따라 처리하십시오.\n\n03\n취급 자료의 외부 반출을\n금합니다.",
            23, 113, 250, 435, 23);
        U.Label("SmallStamp", side.transform, font, "내부 열람용", 25, 550, 245, 39, 23, new Color32(136, 63, 53, 255), TextAnchor.MiddleRight);
    }

    private static void BuildWork(RectTransform root, WorkManager manager)
    {
        U.Label("WorkHeading", root, font, "업무 처리 콘솔", 30, 142, 1165, 49, 31);
        U.Label("WorkHint", root, font, "안내가 끝나면 배정된 업무의 코드 또는 이름을 입력하십시오.", 32, 200, 1165, 43, 22);
        var console = U.Panel("CommandSurface", root, 28, 266, 1194, 365, new Color32(12, 23, 17, 255), inset, true);
        var output = manager.workConsoleAnimation.anims[0].GUITextCtrl;
        output.transform.SetParent(console.transform, false);
        U.Place(output.rectTransform, 28, 24, 1138, 318);
        output.font = font; output.fontSize = 24; output.color = U.Green;
        output.alignment = TextAnchor.UpperLeft; output.gameObject.SetActive(true);
        foreach (var animation in manager.workConsoleAnimation.anims) animation.GUITextCtrl = output;
        U.Label("CommandLabel", root, font, "업무 입력", 30, 659, 250, 39, 23);
        var input = manager.consoleInput;
        input.transform.SetParent(root, false);
        U.Place(input.GetComponent<RectTransform>(), 29, 707, 1190, 70);
        var image = input.GetComponent<Image>() ?? input.gameObject.AddComponent<Image>();
        image.sprite = inset; image.type = Image.Type.Sliced; image.color = U.Paper;
        input.targetGraphic = image;
        var area = input.textViewport != null ? input.textViewport : input.textComponent.rectTransform.parent as RectTransform;
        if (area != null) { Stretch(area); area.offsetMin = new Vector2(18, 8); area.offsetMax = new Vector2(-18, -8); }
        var head = input.transform.Find("Head Text");
        if (head != null) head.gameObject.SetActive(false);
        Stretch(input.textComponent.rectTransform);
        input.textComponent.font = tmp; input.textComponent.fontSize = 25; input.textComponent.color = U.Ink;
        input.textComponent.enableAutoSizing = false;
        input.gameObject.SetActive(true);
        U.Label("EnterHelp", root, font, "ENTER  실행     /     업무 코드 또는 이름을 입력하십시오.", 32, 790, 1170, 31, 19);
    }

    private static void BuildMail(RectTransform root, MailManager manager)
    {
        U.Label("MailboxHeading", root, font, "수신 문서", 30, 142, 600, 48, 31);
        U.Label("MailboxCaption", root, font, "정보국 내부 통신", 30, 199, 1000, 34, 21);
        U.Label("ListCaption", root, font, "수신 목록", 30, 253, 340, 33, 22);
        U.Label("ReaderCaption", root, font, "문서 내용", 429, 253, 750, 33, 22);
        manager.mailList = Scroll("InboxList", root, 29, 299, 368, 510, U.Paper);
        var inboxContent = (RectTransform)manager.mailList;
        inboxContent.anchorMax = new Vector2(1, 1);
        inboxContent.sizeDelta = Vector2.zero;
        var listLayout = manager.mailList.gameObject.AddComponent<VerticalLayoutGroup>();
        listLayout.childControlWidth = listLayout.childControlHeight = true;
        listLayout.childForceExpandHeight = false;
        listLayout.childForceExpandWidth = true;
        listLayout.spacing = 4;
        manager.mailList.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        var content = Scroll("MailReader", root, 425, 299, 795, 510, U.Paper);
        var text = manager.mailPanel;
        text.transform.SetParent(content, false);
        U.Place(text.rectTransform, 22, 20, 733, 450);
        text.font = tmp; text.fontSize = 25; text.color = U.Ink;
        text.enableAutoSizing = false; text.enableWordWrapping = true;
        text.alignment = TextAlignmentOptions.TopLeft;
        text.text = "";
        content.gameObject.AddComponent<VerticalLayoutGroup>().padding = new RectOffset(22, 22, 20, 20);
        content.GetComponent<VerticalLayoutGroup>().childForceExpandHeight = false;
        content.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        text.gameObject.SetActive(false);
        view.mailEmpty = U.Label("SelectMail", content.parent, font, "수신 목록에서 문서를 선택하십시오.", 45, 70, 680, 140, 25,
            new Color(.4f, .45f, .38f)).gameObject;
        manager.mailPrefab = BuildMailRow();
        manager.panelSize = 72;
    }

    private static GameObject BuildMailRow()
    {
        var rect = U.Rect("RetroMailRow", null, 0, 0, 330, 72);
        var image = rect.gameObject.AddComponent<Image>(); image.sprite = box; image.type = Image.Type.Sliced; image.color = U.Face;
        var button = rect.gameObject.AddComponent<Button>(); button.targetGraphic = image;
        button.transition = Selectable.Transition.SpriteSwap;
        button.spriteState = new SpriteState { highlightedSprite = selected, selectedSprite = selected, pressedSprite = pressed };
        var layout = rect.gameObject.AddComponent<LayoutElement>(); layout.preferredHeight = 72;
        var label = U.Rect("MailTitle", rect, 17, 12, 288, 48).gameObject.AddComponent<TextMeshProUGUI>();
        label.font = tmp; label.fontSize = 23; label.color = U.Ink; label.raycastTarget = false;
        label.overflowMode = TextOverflowModes.Ellipsis;
        Stretch(label.rectTransform); label.rectTransform.offsetMin = new Vector2(17, 10); label.rectTransform.offsetMax = new Vector2(-17, -10);
        var panel = rect.gameObject.AddComponent<MailPanel>(); panel.titleUI = label;
        Bind(button, panel.OnClick);
        const string path = "Assets/Prefabs/Screen/RetroMailRow.prefab";
        var prefab = PrefabUtility.SaveAsPrefabAsset(rect.gameObject, path);
        UnityEngine.Object.DestroyImmediate(rect.gameObject);
        return prefab;
    }

    private static void BuildMessages(RectTransform root, MsgManager manager)
    {
        U.Label("MessagesHeading", root, font, "내부 메시지", 30, 142, 1100, 48, 31);
        U.Label("MessagesHint", root, font, "수신된 대화를 선택해 내용을 확인하십시오.", 30, 201, 1100, 39, 22);
        manager.MsgListPanel = Scroll("MessageList", root, 30, 271, 365, 543, U.Paper);
        var list = manager.MsgListPanel.gameObject.AddComponent<VerticalLayoutGroup>();
        list.childControlWidth = list.childControlHeight = true; list.childForceExpandHeight = false;
        manager.MsgListPanel.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        manager.MsgPanel = Scroll("Conversation", root, 422, 271, 798, 449, U.Paper);
        var conversation = manager.MsgPanel.parent.parent;
        // 기존 MessagePanel은 Content -> Viewport -> Conversation 계층의 창을 활성화한다.
        var back = Button("Back", conversation, "대화 닫기", 535, 465, 237, 56);
        UnityEventTools.AddBoolPersistentListener(back.onClick, conversation.gameObject.SetActive, false);
        manager.BackButton = back.gameObject;
        back.gameObject.SetActive(false);
        view.messageEmpty = U.Label("NoMessages", root, font, "수신된 메시지가 없습니다.", 460, 400, 700, 110, 25, new Color(.4f, .45f, .38f)).gameObject;
        conversation.gameObject.SetActive(false);
    }

    private static void BuildNews(RectTransform root)
    {
        U.Label("NewsHeading", root, font, "신문 열람", 32, 145, 1160, 50, 32);
        var paper = U.Panel("PressSurface", root, 29, 233, 1192, 577, U.Paper, inset);
        U.Icon("ExistingNewspaperIcon", paper.transform, news, 534, 115, 120, 130);
        U.Label("NoPressData", paper.transform, font, "현재 표시할 신문 자료가 없습니다.", 110, 290, 970, 70, 27,
            new Color(.38f, .43f, .36f), TextAnchor.MiddleCenter);
    }

    private static void BuildMenu(RectTransform root)
    {
        U.Label("ProgramsHeading", root, font, "업무 프로그램", 34, 145, 820, 58, 32);
        string[] names = { "업무 지원", "수신함", "메신저", "신문 열람", "업무 단말 홈" };
        UnityAction[] actions = { view.OpenWork, view.OpenMail, view.OpenMessages, view.OpenNews, view.OpenHome };
        for (int i = 0; i < names.Length; i++)
            Bind(Button("Program " + i, root, names[i], 34, 228 + i * 84, 832, 65), actions[i]);
    }

    private static RectTransform[] BuildNavigation(Transform desktop)
    {
        U.Label("ProgramsCaption", desktop, font, "업무 프로그램", 34, 182, 250, 45, 24, U.Green);
        string[] names = { "업무 단말", "업무 지원", "수신함", "메신저", "신문 열람", "프로그램" };
        Sprite[] icons = { folder, folder, mail, messages, news, folder };
        UnityAction[] actions = { view.OpenHome, view.OpenWork, view.OpenMail, view.OpenMessages, view.OpenNews, view.OpenMenu };
        var result = new RectTransform[names.Length];
        for (int i = 0; i < names.Length; i++)
        {
            var button = Button("Launcher " + i, desktop, "", 32, 252 + i * 110, 251, 87);
            Bind(button, actions[i]);
            U.Icon("ExistingIcon", button.transform, icons[i], 17, 20, 45, 45);
            U.Label("ProgramName", button.transform, font, names[i], 78, 12, 160, 60, 24);
            result[i] = (RectTransform)button.transform;
        }
        U.Label("AccessNote", desktop, font, "INTERNAL\nUSE ONLY", 39, 980, 240, 70, 20, new Color32(109, 140, 116, 255));
        return result;
    }

    private static void Footer(Transform root, ScreenManager manager)
    {
        U.Panel("BottomRule", root, 32, 1090, 1536, 2, new Color32(105, 131, 109, 255));
        view.dateText = U.Label("DateTime", root, font, "업무용 내부 단말", 35, 1113, 850, 38, 22, U.Paper);
        var power = Button("Power", root, "전원", 1010, 1105, 170, 48); Bind(power, manager.OnPowerClicked);
        var reset = Button("Reset", root, "재부팅", 1190, 1105, 170, 48); Bind(reset, manager.OnResetClicked);
        var back = Button("Return", root, "자리로", 1370, 1105, 195, 48);
        UnityEventTools.AddStringPersistentListener(back.onClick, manager.OnReturnClicked, "MainWorld");
        manager.returnButton = back.gameObject;
        view.footerText = U.Label("Policy", root, font, "접근한 자료와 업무 내용은 외부로 반출할 수 없습니다.",
            35, 1160, 1525, 25, 17, U.Green);
    }

    private static void BuildBoot(ScreenManager manager, Transform root)
    {
        var boot = (RectTransform)manager.bootPanel.transform;
        boot.SetParent(root, false);
        U.Place(boot, 32, 178, 1534, 880);
        var image = boot.GetComponent<Image>() ?? boot.gameObject.AddComponent<Image>();
        image.sprite = inset; image.type = Image.Type.Sliced; image.color = new Color32(9, 18, 13, 255);
        var cli = manager.bootCLI;
        U.Place(cli.rectTransform, 70, 270, 1390, 500);
        cli.font = font; cli.fontSize = 27; cli.color = U.Green; cli.alignment = TextAnchor.UpperLeft;
        var logoRoot = (RectTransform)boot.GetChild(0);
        U.Place(logoRoot, 570, 150, 400, 400);
        var logoImage = logoRoot.GetComponentsInChildren<Image>(true).FirstOrDefault();
        if (logoImage != null) logoImage.preserveAspect = true;
        foreach (var text in logoRoot.GetComponentsInChildren<Text>(true)) { text.font = font; text.color = U.Green; }
        U.Label("OffScreenCaption", boot, font, "정보국 업무 단말기\n\n전원 버튼을 눌러 시작하십시오.",
            70, 80, 1390, 150, 30, U.Green);
        boot.SetAsLastSibling();
        // 부팅 시 GetChild(0)를 로고로 사용하는 기존 계약을 유지한다.
        logoRoot.SetAsFirstSibling();
    }

    private static void AdaptTutorial(Transform tutorial, RectTransform desktop, RectTransform[] nav,
        MailManager mailManager, WorkManager workManager, GameObject newsWindow)
    {
        tutorial.SetParent(desktop, false);
        U.Place((RectTransform)tutorial, 0, 0, 1600, 1200);
        tutorial.gameObject.SetActive(true);
        tutorial.SetAsLastSibling();
        var controller = desktop.GetComponent<R_ScreenTutorial>();
        if (controller == null) return;
        controller.workManager = workManager;
        controller.mailManager = mailManager;
        controller.panels = new[] { view.mailWindow, mailManager.mailPanel.gameObject, newsWindow };
        Rect[] targets =
        {
            new Rect(32, 472, 251, 87), new Rect(314, 178, 1250, 880), new Rect(739, 477, 795, 510),
            new Rect(32, 692, 251, 87), new Rect(314, 178, 1250, 880), new Rect(32, 362, 251, 87),
            new Rect(314, 178, 1250, 880)
        };
        for (int i = 0; i < controller.tutorials.Length; i++)
        {
            var popup = controller.tutorials[i].transform;
            U.Place((RectTransform)popup, 0, 0, 1600, 1200);
            var r = targets[Math.Min(i, targets.Length - 1)];
            U.Place((RectTransform)popup.GetChild(0), r.x, r.y, r.width, r.height);
            U.Place((RectTransform)popup.GetChild(1), r.x, r.y, r.width, r.height);
            U.Place((RectTransform)popup.GetChild(2), 355, 350, 1090, 450);
            U.Place((RectTransform)popup.GetChild(3), 375, 965, 1050, 70);
            popup.gameObject.SetActive(false);
        }
    }

    private static RectTransform Scroll(string name, Transform parent, float x, float y, float w, float h, Color color)
    {
        var outer = U.Panel(name, parent, x, y, w, h, color, inset, true);
        var scroll = outer.gameObject.AddComponent<ScrollRect>();
        var viewport = U.Rect("Viewport", outer.transform, 8, 8, w - 16, h - 16);
        viewport.gameObject.AddComponent<RectMask2D>();
        var content = U.Rect("Content", viewport, 0, 0, w - 32, h - 16);
        scroll.viewport = viewport; scroll.content = content;
        scroll.horizontal = false; scroll.vertical = true;
        scroll.movementType = ScrollRect.MovementType.Clamped;
        scroll.scrollSensitivity = 32;
        return content;
    }

    private static Button Button(string name, Transform parent, string text, float x, float y, float w, float h) =>
        U.Button(name, parent, font, text, x, y, w, h, box, selected, pressed);
    private static void Bind(Button button, UnityAction action) => UnityEventTools.AddPersistentListener(button.onClick, action);
    private static Sprite Sprite(string path) => AssetDatabase.LoadAssetAtPath<Sprite>(path);
    private static void Stretch(RectTransform rect)
    { rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one; rect.offsetMin = rect.offsetMax = Vector2.zero; rect.localScale = Vector3.one; }
    private static T Find<T>() where T : Component => scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<T>(true)).FirstOrDefault();
    private static GameObject FindObject(string name) => scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Transform>(true)).First(t => t.name == name).gameObject;
    private static GameObject Reference(UnityEngine.Object owner, string field) => (GameObject)new SerializedObject(owner).FindProperty(field).objectReferenceValue;
    private static void Require(bool value, string message) { if (!value) throw new InvalidOperationException(message); }

    public static void VerifyAndCapture()
    {
        if (!scene.IsValid()) scene = SceneManager.GetActiveScene();
        Require(scene.path == ScenePath, "Remake verification must target Screen_TEST, never the original Screen.");
        if (view == null) view = Find<RetroScreenDesktop>();
        Require(view != null && view.screenManager != null && view.workManager != null && view.mailManager != null, "Screen managers lost their UI bindings.");
        Require(view.workManager.consoleInput.onSubmit.GetPersistentEventCount() > 0, "Original work submit event is missing.");
        Require(view.mailManager.mailPrefab.GetComponent<MailPanel>() != null, "Mail row lost the original MailPanel behavior.");
        Require(view.screenManager.bootPanel.transform.GetChild(0).name == "Logo", "Boot logo child index changed.");
        Require(view.messageManager.MsgPanel.parent.parent.gameObject.name == "Conversation", "Message conversation hierarchy is invalid.");
        foreach (var button in view.GetComponentsInChildren<Button>(true))
        {
            if (!IsExposedControl(button)) continue;
            for (int i = 0; i < button.onClick.GetPersistentEventCount(); i++)
                Require(button.onClick.GetPersistentTarget(i) != null, "Missing button callback: " + button.name);
        }
        view.screenManager.desktop.SetActive(true);
        view.screenManager.bootPanel.SetActive(false);
        view.RefreshPresentation();
        var data = Utility.DataLoader.GetDayData(6);
        view.PreviewAssignments(data);
        Capture("output/screen-remake/desktop.png");
        view.workManager.taskWindow.SetActive(true);
        view.workManager.taskWindow.transform.SetAsLastSibling();
        Capture("output/screen-remake/work.png");
        view.workManager.taskWindow.SetActive(false);
        view.mailWindow.SetActive(true); view.mailWindow.transform.SetAsLastSibling();
        Capture("output/screen-remake/mail.png");
        view.mailWindow.SetActive(false);
        Debug.Log("SCREEN_REMAKE_VERIFY_OK: original work submit, mail data bindings, message hierarchy, persistent actions, boot structure.");
    }

    private static bool IsExposedControl(Button button)
    {
        var windows = new[] { view.homeWindow, view.workManager.taskWindow, view.mailWindow,
            view.messageWindow, view.newsWindow, view.menuWindow, view.screenManager.desktop };
        for (var at = button.transform; at != null && at != view.transform; at = at.parent)
            if (!at.gameObject.activeSelf && !windows.Contains(at.gameObject)) return false;
        return true;
    }

    private static void Polish()
    {
        view.powerButton = view.transform.Find("Power").GetComponent<Button>();
        view.resetButton = view.transform.Find("Reset").GetComponent<Button>();
        view.bootPrompt = view.screenManager.bootPanel.transform.Find("OffScreenCaption").gameObject;
        var inboxContent = (RectTransform)view.mailManager.mailList;
        inboxContent.anchorMin = new Vector2(0, 1); inboxContent.anchorMax = new Vector2(1, 1);
        inboxContent.pivot = new Vector2(0, 1); inboxContent.anchoredPosition = Vector2.zero;
        inboxContent.sizeDelta = new Vector2(0, inboxContent.sizeDelta.y);
        if (view.inboxEmpty == null)
            view.inboxEmpty = U.Label("EmptyInbox", view.mailWindow.transform, font, "수신된 문서가 없습니다.",
                57, 360, 310, 130, 23, U.Ink).gameObject;
        var textFitter = view.mailManager.mailPanel.GetComponent<ContentSizeFitter>();
        if (textFitter != null) UnityEngine.Object.DestroyImmediate(textFitter);
        foreach (var label in view.GetComponentsInChildren<Text>(true))
        {
            if (label.name == "Location" || label.name == "WindowStatusText" || label.name == "SelectMail" || label.name == "NoMessages")
                label.color = U.Ink;
            if (label.name == "NoteText") label.alignment = TextAnchor.UpperLeft;
            if (label.name == "EnterHelp") label.text = "ENTER  실행     /     업무 코드 또는 이름을 입력하십시오.";
        }
        foreach (var scroll in view.GetComponentsInChildren<ScrollRect>(true))
        {
            if (scroll.name != "AssignedWork" && scroll.name != "InboxList" && scroll.name != "MailReader" &&
                scroll.name != "MessageList" && scroll.name != "Conversation") continue;
            if (scroll.verticalScrollbar != null)
            {
                scroll.verticalScrollbar.handleRect.sizeDelta = Vector2.zero;
                continue;
            }
            var rect = (RectTransform)scroll.transform;
            float w = rect.sizeDelta.x, h = rect.sizeDelta.y;
            var track = U.Panel("RetroScrollTrack", rect, w - 22, 8, 16, h - 16, U.Face, inset, true);
            var bar = track.gameObject.AddComponent<Scrollbar>();
            var handle = U.Panel("Handle", track.transform, 1, 1, 14, 50, U.Face, box, true);
            bar.handleRect = (RectTransform)handle.transform; bar.targetGraphic = handle;
            bar.handleRect.sizeDelta = Vector2.zero;
            bar.direction = Scrollbar.Direction.BottomToTop;
            scroll.verticalScrollbar = bar;
            scroll.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.AutoHide;
            scroll.viewport.sizeDelta = new Vector2(w - 34, h - 16);
        }
        EditorUtility.SetDirty(view);
    }

    public static void CaptureRuntime(string path)
    {
        scene = SceneManager.GetSceneByPath(ScenePath);
        view = Find<RetroScreenDesktop>();
        Capture(path);
    }

    private static void Capture(string path)
    {
        var canvas = view.GetComponentInParent<Canvas>();
        var camera = Camera.main;
        var texture = new RenderTexture(1600, 1200, 24);
        var pixels = new Texture2D(1600, 1200, TextureFormat.RGB24, false);
        var previous = RenderTexture.active;
        try
        {
            canvas.renderMode = RenderMode.ScreenSpaceCamera; canvas.worldCamera = camera; canvas.planeDistance = 1;
            camera.targetTexture = texture;
            Canvas.ForceUpdateCanvases();
            foreach (var graphic in view.GetComponentsInChildren<MaskableGraphic>())
            { graphic.RecalculateClipping(); graphic.RecalculateMasking(); graphic.SetAllDirty(); }
            Canvas.ForceUpdateCanvases(); camera.Render(); RenderTexture.active = texture;
            pixels.ReadPixels(new Rect(0, 0, 1600, 1200), 0, 0); pixels.Apply();
            Directory.CreateDirectory(Path.GetDirectoryName(path)); File.WriteAllBytes(path, pixels.EncodeToPNG());
        }
        finally
        {
            camera.targetTexture = null; RenderTexture.active = previous;
            canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.worldCamera = null;
            UnityEngine.Object.DestroyImmediate(pixels); texture.Release(); UnityEngine.Object.DestroyImmediate(texture);
        }
    }
}
