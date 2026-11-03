using System;
using System.IO;
using System.Linq;
using GameService;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>라디오 종료를 기다린 뒤 실제 포커싱 마스크의 클릭 이벤트로 출근한다.</summary>
public static class FirstDayCommuteSmoke
{
    private const string Key = "FirstDayCommuteSmoke";

    [InitializeOnLoadMethod]
    private static void Resume()
    {
        if (SessionState.GetBool(Key, false)) EditorApplication.update += Poll;
    }

    [MenuItem("Tools/Tutorial/Verify First Day Commute %#F9")]
    public static void Run() => RunFromDay(1);

    [MenuItem("Tools/Tutorial/Verify Day Zero To First Commute %#F10")]
    public static void RunFromDayZero() => RunFromDay(0);

    private static void RunFromDay(int startDay)
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) throw new InvalidOperationException("Exit Play mode first");
        SessionState.SetString("DayEditorSceneBuilder.Version", "DayEditorScene.v2:" +
            AssetDatabase.AssetPathToGUID("Assets/Editor/DayEditor.unity"));
        SessionState.SetString(Key + ".restore", SceneManager.GetActiveScene().path);
        EditorSceneManager.OpenScene("Assets/Scenes/DirectPlayController.unity", OpenSceneMode.Single);
        var world = EditorSceneManager.OpenScene(FirstDayCommuteBuilder.ScenePath, OpenSceneMode.Additive);
        SceneManager.SetActiveScene(world);
        UnityEngine.Object.FindObjectOfType<DirectPlayBootstrap>().Configure(DirectPlayMode.MainWorld, startDay, 0,
            null, 0, null);
        SessionState.SetBool(Key, true);
        SessionState.SetInt(Key + ".stage", startDay == 0 ? -2 : 0);
        SessionState.SetBool(Key + ".fromDayZero", startDay == 0);
        SessionState.SetFloat(Key + ".next", 0);
        SessionState.SetFloat(Key + ".deadline", (float)EditorApplication.timeSinceStartup + 180);
        SessionState.SetString(Key + ".saveHash", SaveHash());
        Directory.CreateDirectory("output/commute");
        File.WriteAllText("output/commute/result.txt", "RUNNING");
        EditorApplication.update -= Poll; EditorApplication.update += Poll;
        EditorApplication.isPlaying = true;
    }

    private static void Poll()
    {
        int stage = SessionState.GetInt(Key + ".stage", 0);
        if (stage == 99)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            SessionState.SetBool(Key, false); EditorApplication.update -= Poll;
            string restore = SessionState.GetString(Key + ".restore", "");
            if (!string.IsNullOrEmpty(restore)) EditorSceneManager.OpenScene(restore, OpenSceneMode.Single);
            return;
        }
        if (!EditorApplication.isPlaying || EditorApplication.isCompiling ||
            EditorApplication.timeSinceStartup < SessionState.GetFloat(Key + ".next", 0)) return;
        try
        {
            Require(EditorApplication.timeSinceStartup < SessionState.GetFloat(Key + ".deadline", 0), "Commute timeout: " + stage);
            var tutorial = UnityEngine.Object.FindObjectOfType<FirstDayCommuteTutorial>();
            Require(stage == 7 || tutorial != null, "Missing commute tutorial");
            var location = GameSystem.GetService<ILocationService>();
            switch (stage)
            {
                case -2:
                    if (Chat.Instance == null || !Chat.Instance.IsOpen) return;
                    Require(GameSystem.SaveService.DayIndex == 0 && !tutorial.HasStarted,
                        "Tutorial interrupted day zero");
                    if (Chat.Instance.text.text.StartsWith("시작하지."))
                    {
                        var backdrop = new SerializedObject(Chat.Instance).FindProperty("background")
                            .objectReferenceValue as UnityEngine.UI.Image;
                        Require(backdrop != null && backdrop.gameObject.activeInHierarchy &&
                            backdrop.sprite != null && backdrop.sprite.name == "Walter_!",
                            "Final Walter line exposed the street");
                        ScreenCapture.CaptureScreenshot("output/commute/day-zero-walter.png");
                        Next(-1);
                    }
                    else
                    {
                        Chat.Instance.NextChat();
                        Next(-2);
                    }
                    break;
                case -1:
                    if (GameSystem.SaveService.DayIndex == 1) { Next(0); break; }
                    Require(!tutorial.HasStarted, "Tutorial started before day one");
                    Chat.Instance.NextChat();
                    Next(-1); break;
                case 0:
                    if (Chat.Instance == null || !Chat.Instance.IsOpen) return;
                    Require(!tutorial.HasStarted && !tutorial.spotlight.gameObject.activeSelf, "Tutorial interrupted radio");
                    // 최소 한 프레임은 튜토리얼이 아침 대화를 관찰하도록 기다린다.
                    Next(1); break;
                case 1:
                    Chat.Instance.SkipChat(); Next(2); break;
                case 2:
                    if (!tutorial.HasStarted || tutorial.spotlight.CurrentBorder == null) return;
                    if (!tutorial.spotlight.CurrentBorder.gameObject.activeInHierarchy ||
                        !tutorial.spotlight.CurrentBorder.GetComponent<EventTrigger>().triggers
                            .Any(t => t.eventID == EventTriggerType.PointerClick)) return;
                    Require(tutorial.spotlight.Steps[0].TargetObject == tutorial.movementToggle.gameObject,
                        "First step must be movement button");
                    Click(tutorial, Vector2.zero); // 강조 밖 클릭으로는 진행하지 않는다.
                    Require(tutorial.spotlight.Steps.Count == 4, "Wrong click advanced tutorial");
                    ScreenCapture.CaptureScreenshot("output/commute/movement-button.png"); Next(20); break;
                case 20:
                    ClickTarget(tutorial); Next(3); break;
                case 3:
                    Require(tutorial.toCompanyStreet.gameObject.activeInHierarchy, "Movement arrows not shown");
                    ScreenCapture.CaptureScreenshot("output/commute/movement.png"); Next(30); break;
                case 30:
                    ClickTarget(tutorial); Next(4); break;
                case 4:
                    Require(location.CurrentPosition.location == World.Street && location.CurrentPosition.position == 1,
                        "Did not reach company street");
                    Require(tutorial.spotlight.Steps[0].TargetObject == tutorial.companyBuilding, "Company explanation missing");
                    Require(tutorial.spotlight.GetComponentsInChildren<TMPro.TMP_Text>().Any(t => t.text.Contains("아슬라니아 정보국")),
                        "Company explanation invisible");
                    ScreenCapture.CaptureScreenshot("output/commute/company.png"); Next(40); break;
                case 40:
                    Click(tutorial, new Vector2(Screen.width * .5f, Screen.height * .5f)); Next(5); break;
                case 5:
                    Require(tutorial.spotlight.Steps[0].TargetObject == tutorial.companyEntrance.gameObject, "Entrance not focused");
                    ScreenCapture.CaptureScreenshot("output/commute/entrance.png"); Next(50); break;
                case 50:
                    ClickTarget(tutorial); Next(6); break;
                case 6:
                    Require(location.CurrentPosition.location == World.Hallway && location.CurrentPosition.position == 0,
                        "Did not enter hallway");
                    Require(tutorial.IsComplete && !tutorial.spotlight.gameObject.activeSelf, "Tutorial did not release input");
                    Require(SaveHash() == SessionState.GetString(Key + ".saveHash", ""), "Player save changed");
                    Require(Application.CanStreamedLevelBeLoaded(ScreenSceneRoute.Name), "Remake screen unavailable");
                    // 실제 단말기 진입 버튼으로 리메이크 씬을 로드한다. 날짜/진행은 메모리에서만 설정한다.
                    if (Chat.Instance != null && Chat.Instance.IsOpen) Chat.Instance.SkipChat();
                    GameSystem.GetService<IDayService>().Time = 1;
                    location.MoveLocation(new WorldVector(World.Office, 3));
                    UnityEngine.Object.FindObjectOfType<ScreenButton>().OnPointerClick(new PointerEventData(EventSystem.current));
                    Next(7); break;
                case 7:
                    if (!SceneManager.GetSceneByName(ScreenSceneRoute.Name).isLoaded || GameSystem.Instance.IsLoading) return;
                    Require(UnityEngine.Object.FindObjectOfType<RetroScreenDesktop>() != null, "Remake desktop missing");
                    Require(!SceneManager.GetSceneByName("Screen").isLoaded, "Original screen loaded");
                    Require(SaveHash() == SessionState.GetString(Key + ".saveHash", ""), "Player save changed after screen entry");
                    File.WriteAllText("output/commute/result.txt",
                        (SessionState.GetBool(Key + ".fromDayZero", false)
                            ? "DAY_ZERO_OK: final Walter backdrop retained; actual NEXTDAY transition to day one; " : "") +
                        "COMMUTE_PLAY_OK: radio wait, movement focus, wrong-click protection, company explanation, actual street/entrance movement, input released, real ScreenButton loads Screen_TEST; player save unchanged.");
                    Debug.Log("COMMUTE_PLAY_OK");
                    SessionState.SetInt(Key + ".stage", 99); EditorApplication.isPlaying = false; break;
            }
        }
        catch (Exception exception)
        {
            Debug.LogException(exception); File.WriteAllText("output/commute/result.txt", exception.ToString());
            SessionState.SetInt(Key + ".stage", 99); EditorApplication.isPlaying = false;
        }
    }

    private static void ClickTarget(FirstDayCommuteTutorial tutorial)
    {
        Transform target = tutorial.spotlight.Steps[0].TargetObject.transform;
        var canvas = target.GetComponentInParent<Canvas>();
        Vector2 center = target is RectTransform
            ? RectTransformUtility.WorldToScreenPoint(canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main, target.position)
            : TutorialController.WorldToScreenInCameraRect(Camera.main, target.position);
        Click(tutorial, center);
    }

    private static void Click(FirstDayCommuteTutorial tutorial, Vector2 position)
    {
        var pointer = new PointerEventData(EventSystem.current) { position = position, clickCount = 1 };
        var trigger = tutorial.spotlight.CurrentBorder.GetComponent<EventTrigger>();
        Require(trigger.triggers.Any(t => t.eventID == EventTriggerType.PointerClick), "Focus input not ready");
        ExecuteEvents.Execute(trigger.gameObject, pointer, ExecuteEvents.pointerClickHandler);
    }

    private static void Next(int stage)
    {
        SessionState.SetInt(Key + ".stage", stage);
        SessionState.SetFloat(Key + ".next", (float)EditorApplication.timeSinceStartup + 1.2f);
    }
    private static string SaveHash()
    {
        string path = Path.Combine(Application.persistentDataPath, "Player/savedata.json");
        if (!File.Exists(path)) return "missing";
        using var sha = System.Security.Cryptography.SHA256.Create();
        return Convert.ToBase64String(sha.ComputeHash(File.ReadAllBytes(path)));
    }
    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
