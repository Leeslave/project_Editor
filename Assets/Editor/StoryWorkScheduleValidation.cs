using System;
using System.Linq;
using EditorGame.Documents;
using GameAction;
using GameService;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Utility;

/// <summary>실제 배정 데이터와 업무 서비스를 사용하되 플레이어 저장 파일에는 기록하지 않는다.</summary>
public static class StoryWorkScheduleValidation
{
    private const string Key = "StoryWorkScheduleValidation";

    [InitializeOnLoadMethod]
    private static void Resume() => EditorApplication.playModeStateChanged += OnPlayModeChanged;

    public static void Run()
    {
        // 자동 편집기 씬 재생성이 이 검증의 Play 전환에 끼어들지 않도록 한다.
        SessionState.SetString("DayEditorSceneBuilder.Version", "DayEditorScene.v2:" +
            AssetDatabase.AssetPathToGUID("Assets/Editor/DayEditor.unity"));
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        SessionState.SetBool(Key, true);
        SessionState.SetInt(Key + ".result", 1);
        EditorApplication.isPlaying = true;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (!SessionState.GetBool(Key, false)) return;
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            try { Validate(); SessionState.SetInt(Key + ".result", 0); }
            catch (Exception exception) { Debug.LogException(exception); }
            EditorApplication.isPlaying = false;
        }
        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            SessionState.SetBool(Key, false);
            EditorApplication.Exit(SessionState.GetInt(Key + ".result", 1));
        }
    }

    private static void Validate()
    {
        var root = new GameObject("Story schedule validation (memory only)");
        var day = root.AddComponent<DayController>();
        var work = root.AddComponent<WorkController>();
        var save = root.AddComponent<DirectPlaySaveService>();
        GameSystem.UnRegister<IDayService>();
        GameSystem.UnRegister<IWorkService>();
        GameSystem.RegisterService<IDayService>(day);
        GameSystem.RegisterService<IWorkService>(work);
        GameSystem.SaveService = save;
        save.Init();
        int completions = 0;
        work.OnWorkClear += () => completions++;
        try
        {
            var cipher = JsonConvert.DeserializeObject<ADFGVXStageData>(
                Resources.Load<TextAsset>("GameData/Encrypt/ADFGVXStageData").text);
            for (int index = 0; index <= 13; index++)
            {
                DailyData data = DataLoader.GetDayData(index);
                Require(data != null && data.dayTimes.Length >= 4, "Missing day " + index);
                Require(data.workList.Select(w => w.code).Distinct().Count() == data.workList.Count,
                    "Duplicate work code on day " + index);
                save.Configure(1, 0, data);
                save.SelectDay(index, 0);
                int eventBefore = completions;
                if (index == 13)
                {
                    Require(data.workList.Count == 0, "Ending must have no required office work");
                    Require(data.dayTimes[0].npc.Any(n => n.chat.Any(p => p.Contains("13-H"))),
                        "Ending dialogue missing");
                    continue;
                }
                foreach (Work item in data.workList.Where(w => w.availableFromTime == 0))
                {
                    if (item.code == "ADFGVX")
                        Require(cipher.Encrypt.ContainsKey(item.stage.ToString()) ||
                            cipher.Decrypt.ContainsKey(item.stage.ToString()), "Missing cipher stage");
                    if (item.code == "Document" || item.code == "SecureDocument")
                        DocumentStageRepository.Load(item.code, item.stage).ValidateForExecution();
                    Require(work.GetList().Any(w => w.code == item.code), "Unavailable " + index + "/" + item.code);
                    Require(work.TryStartWork(item.code, out string scene), "Cannot start " + item.code);
                    Require(!string.IsNullOrEmpty(scene), "Missing scene");
                    if (index == 12)
                    {
                        Require(!work.GetList().Any(w => w.code == "VoightKampff"), "Exam exposed in morning");
                        Require(!work.TryStartWork("VoightKampff", out _), "Exam starts early");
                        Require(!work.ClearWork("VoightKampff"), "Exam completes early");
                    }
                    work.ClearWork(item.code);
                    Require(work.IsWorkClear(item.code), "Completion not recorded");
                }
                Require(day.Time == 2 && work.IsWorkClear(), "Morning work does not reach afternoon: " + index);
                Require(completions == eventBefore + 1, "Completion event count: " + index);
                Work last = data.workList.Last(w => w.availableFromTime == 0);
                work.ClearWork(last.code);
                Require(completions == eventBefore + 1, "Duplicate completion event");
                if (index == 12)
                {
                    day.Time = 3;
                    Require(!work.IsWorkClear(), "Evening exam must be pending");
                    Require(work.TryStartWork("VoightKampff", out string scene) && scene == "VoightKampff",
                        "Evening exam unavailable");
                    Require(ActionHandler.Create("EXAM", "") is StoryExamAction, "EXAM not registered");
                    work.ClearWork("VoightKampff");
                    Require(day.Time == 3 && work.IsWorkClear(), "Exam completion changed story phase");
                }
                Debug.Log("STORY_WORK_DAY_OK: " + index);
            }
            GameSystem.UnRegister<IWorkService>();
            var direct = root.AddComponent<DirectPlayWorkService>();
            GameSystem.RegisterService<IWorkService>(direct);
            var finalDay = DataLoader.GetDayData(12);
            day.Init(finalDay); day.Time = 1; direct.Init(finalDay);
            Require(direct.GetList().Count == 1 && direct.TryStartWork("ADFGVX", out _), "DirectPlay morning");
            direct.ClearWork("ADFGVX");
            Require(day.Time == 2 && direct.IsWorkClear(), "DirectPlay afternoon blocked");
            day.Time = 3;
            Require(!direct.IsWorkClear() && direct.TryStartWork("VoightKampff", out _), "DirectPlay evening");
            Debug.Log("STORY_WORK_SCHEDULE_OK: Day 0-13; actual work services, scenes, stages, unlocks, afternoon transition, delayed exam and ending.");
        }
        finally
        {
            GameSystem.UnRegister<IDayService>();
            GameSystem.UnRegister<IWorkService>();
            GameSystem.SaveService = null;
            UnityEngine.Object.DestroyImmediate(root);
        }
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
