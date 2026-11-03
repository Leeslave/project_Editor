using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using GameService;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>메모리 전용 DirectPlay 서비스로 실제 부팅·창·메일·재부팅 동작을 검증한다.</summary>
public static class RetroScreenPlaySmoke
{
    private const string Key = "RetroScreenPlaySmoke";

    [InitializeOnLoadMethod]
    private static void Resume()
    {
        if (SessionState.GetBool(Key, false)) EditorApplication.update += Poll;
    }

    public static void Run()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/DirectPlayController.unity", OpenSceneMode.Single);
        var screen = EditorSceneManager.OpenScene(RetroScreenSceneBuilder.ScenePath, OpenSceneMode.Additive);
        SceneManager.SetActiveScene(screen);
        UnityEngine.Object.FindObjectOfType<DirectPlayBootstrap>().Configure(DirectPlayMode.OverrideWorld, 6, 0, null, 0,
            Utility.DataLoader.GetDayData(6));
        var view = screen.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<RetroScreenDesktop>(true)).First();
        // 부팅 전 씬 상태로 진입한다. 저장된 씬이나 플레이어 기록은 변경하지 않는다.
        ScreenManager.IsScreenOn = false;
        view.screenManager.desktop.SetActive(false);
        view.screenManager.bootPanel.SetActive(true);
        SessionState.SetBool(Key, true);
        SessionState.SetInt(Key + ".stage", 0);
        SessionState.SetInt(Key + ".frames", 0);
        SessionState.SetInt(Key + ".result", 0);
        SessionState.SetString(Key + ".save", SaveHash());
        SessionState.SetString(Key + ".originalScene", FileHash(RetroScreenSceneBuilder.OriginalScenePath));
        SessionState.SetFloat(Key + ".deadline", (float)EditorApplication.timeSinceStartup + 90);
        EditorApplication.update -= Poll; EditorApplication.update += Poll;
        EditorApplication.isPlaying = true;
    }

    private static void Poll()
    {
        int stage = SessionState.GetInt(Key + ".stage", 0);
        if (stage == 99)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            SessionState.SetBool(Key, false);
            EditorApplication.update -= Poll;
            EditorApplication.Exit(SessionState.GetInt(Key + ".result", 1));
            return;
        }
        if (!EditorApplication.isPlaying || EditorApplication.isCompiling) return;
        try
        {
            Require(EditorApplication.timeSinceStartup < SessionState.GetFloat(Key + ".deadline", 0), "Screen smoke test timed out.");
            int frames = SessionState.GetInt(Key + ".frames", 0) + 1;
            SessionState.SetInt(Key + ".frames", frames);
            if (frames < 25) return;
            var view = UnityEngine.Object.FindObjectOfType<RetroScreenDesktop>();
            Require(view != null && GameSystem.SaveService is DirectPlaySaveService, "Screen did not use isolated test services.");
            switch (stage)
            {
                case 0:
                    Require(view.screenManager.CurrentMode == ScreenManager.ScreenMode.Off, "Initial power state changed.");
                    Click(view, "Power");
                    SessionState.SetInt(Key + ".stage", 1);
                    break;
                case 1:
                    if (!view.screenManager.desktop.activeSelf) return;
                    view.RefreshPresentation();
                    Require(view.taskRows.childCount == GameSystem.GetService<IWorkService>().GetList().Count, "Displayed assignments differ from daily data.");
                    RetroScreenSceneBuilder.CaptureRuntime("output/screen-remake/desktop-runtime.png");
                    Click(view, "Launcher 1");
                    Require(view.workManager.taskWindow.activeSelf, "Work launcher did not open the original program.");
                    SessionState.SetInt(Key + ".stage", 2);
                    break;
                case 2:
                    if (!view.workManager.consoleInput.gameObject.activeInHierarchy) return;
                    Require(view.workManager.workConsoleAnimation.anims[0].GUITextCtrl.text.Length > 0, "Original work console animation did not render.");
                    RetroScreenSceneBuilder.CaptureRuntime("output/screen-remake/work-runtime.png");
                    Click(view, "Launcher 2");
                    Require(view.mailWindow.activeSelf, "Mail launcher failed.");
                    var panel = view.mailManager.mailList.GetComponentsInChildren<MailPanel>().FirstOrDefault();
                    Require(panel != null, "Day 6 mail was not loaded into the redesigned inbox.");
                    if (panel != null)
                    {
                        Canvas.ForceUpdateCanvases();
                        var row = panel.GetComponent<RectTransform>();
                        Require(row.rect.width > 200 && row.rect.height >= 60 && panel.titleUI.text.Length > 0,
                            "Mail row has no readable/clickable area.");
                        var inbox = view.mailManager.mailList.GetComponentInParent<ScrollRect>();
                        Require(RectTransformUtility.RectangleContainsScreenPoint(inbox.viewport, row.TransformPoint(row.rect.center)),
                            "Mail row is outside its visible viewport.");
                        panel.GetComponent<Button>().onClick.Invoke();
                        Require(view.mailManager.mailPanel.gameObject.activeSelf && view.mailManager.mailPanel.text.Length > 0, "Existing mail contents failed to open.");
                    }
                    view.RefreshPresentation();
                    foreach (var scroll in view.GetComponentsInChildren<ScrollRect>())
                        if (scroll.verticalScrollbar != null && scroll.verticalScrollbar.gameObject.activeInHierarchy)
                            Require(scroll.verticalScrollbar.handleRect.rect.width <= scroll.verticalScrollbar.GetComponent<RectTransform>().rect.width + 1,
                                "Scrollbar handle overflows its track.");
                    RetroScreenSceneBuilder.CaptureRuntime("output/screen-remake/mail-runtime.png");
                    Click(view, "Launcher 3"); Require(view.messageWindow.activeSelf, "Message launcher failed.");
                    Click(view, "Launcher 4"); Require(view.newsWindow.activeSelf, "News launcher failed.");
                    Click(view, "Launcher 5"); Require(view.menuWindow.activeSelf, "Program menu failed.");
                    Click(view, "Launcher 5"); Require(!view.menuWindow.activeSelf, "Program menu toggle failed.");
                    Require(GameSystem.GetService<IWorkService>().GetList().All(w => !GameSystem.GetService<IWorkService>().IsWorkClear(w.code)), "Opening a program completed work.");
                    Click(view, "Reset");
                    Require(view.screenManager.CurrentMode == ScreenManager.ScreenMode.OnBoot, "Reboot failed.");
                    SessionState.SetInt(Key + ".stage", 3);
                    break;
                case 3:
                    if (!view.screenManager.desktop.activeSelf) return;
                    Click(view, "Power"); Click(view, "Power");
                    SessionState.SetInt(Key + ".stage", 4);
                    break;
                case 4:
                    if (ScreenManager.IsScreenOn) return;
                    Require(!view.screenManager.desktop.activeSelf && view.screenManager.returnButton.activeSelf, "Power-off/return state failed.");
                    Require(SaveHash() == SessionState.GetString(Key + ".save", ""), "Player save file changed.");
                    Require(FileHash(RetroScreenSceneBuilder.OriginalScenePath) == SessionState.GetString(Key + ".originalScene", ""), "Original Screen scene changed.");
                    Debug.Log("SCREEN_REMAKE_PLAY_OK: Screen_TEST boot, daily work rows, work console, existing mail, messages/news/menu, reboot, power-off; original Screen and player save unchanged.");
                    Finish(0);
                    break;
            }
        }
        catch (Exception exception) { Debug.LogException(exception); Finish(1); }
    }

    private static void Click(RetroScreenDesktop view, string name)
    {
        var button = view.GetComponentsInChildren<Button>(true).First(b => b.name == name);
        button.onClick.Invoke();
    }

    private static string SaveHash()
    {
        string path = Path.Combine(Application.persistentDataPath, "Player/savedata.json");
        return FileHash(path);
    }
    private static string FileHash(string path)
    {
        if (!File.Exists(path)) return "missing";
        using (var sha = SHA256.Create()) return Convert.ToBase64String(sha.ComputeHash(File.ReadAllBytes(path)));
    }
    private static void Require(bool value, string message) { if (!value) throw new InvalidOperationException(message); }
    private static void Finish(int result)
    {
        SessionState.SetInt(Key + ".result", result); SessionState.SetInt(Key + ".stage", 99);
        EditorApplication.isPlaying = false;
    }
}
