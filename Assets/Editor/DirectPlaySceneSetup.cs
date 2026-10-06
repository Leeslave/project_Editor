using System;
using System.Collections.Generic;
using System.Linq;
using GameEditor;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;

public sealed class DirectPlaySceneSetup : EditorWindow
{
    private const string ControllerPath = "Assets/Scenes/DirectPlayController.unity";
    private const string WorldPath = "Assets/Scenes/MainWorld.unity";

    private static readonly (string code, string path)[] MiniGames =
    {
        ("Document", "Assets/Scenes/Minigame/Document.unity"),
        ("SecureDocument", "Assets/Scenes/Minigame/Document.unity"),
        ("Maze", "Assets/Scenes/Minigame/Maze.unity"),
        ("Dodge", "Assets/Scenes/Minigame/Dodge.unity"),
        ("ADFGVX", "Assets/Scenes/Minigame/ADFGVX.unity"),
        ("ADFGVX_DT", "Assets/Scenes/Minigame/ADFGVX_DT.unity"),
        ("ADFGVX_ET", "Assets/Scenes/Minigame/ADFGVX_ET.unity"),
        ("VoightKampff", "Assets/Resources/Voight/VoightKampff.unity")
    };
    private static readonly string[] NpcTypes =
    {
        "none", "Rex", "Clover", "Henderson", "Kennedy", "King", "Klayton",
        "Price", "Walter", "Mechanic", "Monk", "Reporter", "Nametag"
    };

    [SerializeField] private DirectPlayMode mode;
    [SerializeField] private int dayIndex;
    [SerializeField] private int timeIndex;
    [SerializeField] private int miniGameIndex;
    [SerializeField] private int stage;
    [SerializeField] private int selectedTime;
    [SerializeField] private DailyData customData;
    private Vector2 scroll;

    [MenuItem("Tools/Test Setup/Start Test Scenes", false, 11)]
    public static void Open() => GetWindow<DirectPlaySceneSetup>("Start Test Scenes");

    private void OnEnable()
    {
        if (customData == null) customData = CreateCustomData();
        miniGameIndex = Mathf.Clamp(miniGameIndex, 0, MiniGames.Length - 1);
    }

    private static DailyData CreateCustomData()
    {
        DailyData data = DayEditorDataService.CreateDefault();
        data.date = new Date(2000, 1, 1);
        return data;
    }

    private void OnGUI()
    {
        string[] modes = { "MainWorld 테스트", "MainWorld 오버라이드 테스트", "미니게임 테스트" };
        using (new EditorGUI.DisabledScope(EditorApplication.isPlayingOrWillChangePlaymode))
            mode = (DirectPlayMode)GUILayout.Toolbar((int)mode, modes);
        scroll = EditorGUILayout.BeginScrollView(scroll);
        EditorGUILayout.Space();

        if (mode != DirectPlayMode.MiniGame)
        {
            dayIndex = Mathf.Max(0, EditorGUILayout.IntField("날짜 인덱스", dayIndex));
            if (mode == DirectPlayMode.MainWorld)
                timeIndex = EditorGUILayout.IntSlider("시작 시간대", timeIndex, 0, 3);
            else DrawCustomData();
        }
        else
        {
            miniGameIndex = Mathf.Clamp(EditorGUILayout.Popup("미니게임", miniGameIndex,
                MiniGames.Select(game => game.code).ToArray()), 0, MiniGames.Length - 1);
            stage = Mathf.Max(0, EditorGUILayout.IntField("Stage", stage));
        }

        EditorGUILayout.Space(12);
        using (new EditorGUI.DisabledScope(EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling))
        {
            if (GUILayout.Button("시작", GUILayout.Height(34))) StartTest();
        }
        DrawCompletionControls();
        EditorGUILayout.EndScrollView();
    }

    private void DrawCustomData()
    {
        if (customData == null) customData = CreateCustomData();
        EditorGUILayout.HelpBox("날짜 JSON을 읽지 않습니다. 시작 시간은 09:00이며 표시 날짜는 2000-01-01입니다.", MessageType.Info);
        DrawWorks();

        var serialized = new SerializedObject(this);
        serialized.Update();
        SerializedProperty data = serialized.FindProperty("customData");
        EditorGUILayout.PropertyField(data.FindPropertyRelative("startLocation"), new GUIContent("시작 위치"), true);

        selectedTime = Mathf.Clamp(GUILayout.Toolbar(selectedTime, new[] { "09:00", "10:00", "17:00", "20:00" }), 0, 3);
        SerializedProperty time = data.FindPropertyRelative("dayTimes").GetArrayElementAtIndex(selectedTime);
        DrawNpcList(time.FindPropertyRelative("npc"));
        EditorGUILayout.PropertyField(time.FindPropertyRelative("block"), new GUIContent("Block"), true);
        EditorGUILayout.PropertyField(time.FindPropertyRelative("bgm"), new GUIContent("BGM"), true);
        serialized.ApplyModifiedProperties();
    }

    private static void DrawNpcList(SerializedProperty npcs)
    {
        EditorGUILayout.LabelField("NPC 대화", EditorStyles.boldLabel);
        for (int i = 0; i < npcs.arraySize; i++)
        {
            SerializedProperty npc = npcs.GetArrayElementAtIndex(i);
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.PropertyField(npc.FindPropertyRelative("name"), new GUIContent("이름"));
                SerializedProperty type = npc.FindPropertyRelative("objectType");
                int current = Array.IndexOf(NpcTypes, type.stringValue);
                type.stringValue = NpcTypes[EditorGUILayout.Popup("NPC 타입", Mathf.Max(0, current), NpcTypes)];
                EditorGUILayout.PropertyField(npc.FindPropertyRelative("positions"), new GUIContent("위치"), true);
                EditorGUILayout.PropertyField(npc.FindPropertyRelative("anchor"), new GUIContent("Anchor"), true);
                EditorGUILayout.PropertyField(npc.FindPropertyRelative("chat"), new GUIContent("대화 파일"), true);
                EditorGUILayout.PropertyField(npc.FindPropertyRelative("onAwake"), new GUIContent("자동 시작"), true);
                if (GUILayout.Button("NPC 삭제"))
                {
                    npcs.DeleteArrayElementAtIndex(i);
                    break;
                }
            }
        }
        if (GUILayout.Button("+ NPC 추가"))
        {
            int index = npcs.arraySize;
            npcs.InsertArrayElementAtIndex(index);
            SerializedProperty npc = npcs.GetArrayElementAtIndex(index);
            npc.FindPropertyRelative("name").stringValue = "";
            npc.FindPropertyRelative("objectType").stringValue = "none";
            npc.FindPropertyRelative("positions").arraySize = 1;
            SerializedProperty anchors = npc.FindPropertyRelative("anchor");
            anchors.arraySize = 1;
            anchors.GetArrayElementAtIndex(0).FindPropertyRelative("size").floatValue = 1f;
            npc.FindPropertyRelative("chat").arraySize = 0;
            npc.FindPropertyRelative("onAwake").arraySize = 0;
        }
    }

    private void DrawWorks()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("업무", EditorStyles.boldLabel);
        for (int i = 0; i < customData.workList.Count; i++)
        {
            Work work = customData.workList[i];
            using (new EditorGUILayout.HorizontalScope())
            {
                int index = Array.FindIndex(MiniGames, game => game.code == work.code);
                int selected = EditorGUILayout.Popup(Mathf.Max(0, index),
                    MiniGames.Select(game => game.code).ToArray());
                work.code = MiniGames[selected].code;
                work.stage = Mathf.Max(0, EditorGUILayout.IntField(work.stage, GUILayout.Width(70)));
                if (GUILayout.Button("삭제", GUILayout.Width(45)))
                {
                    customData.workList.RemoveAt(i);
                    GUIUtility.ExitGUI();
                }
            }
        }
        if (GUILayout.Button("+ 업무 추가")) customData.workList.Add(new Work(MiniGames[0].code, 0));
    }

    private void DrawCompletionControls()
    {
        if (!EditorApplication.isPlaying) return;
        DirectPlayWorkService service = UnityEngine.Object.FindObjectOfType<DirectPlayWorkService>();
        if (service == null || mode == DirectPlayMode.MiniGame) return;
        EditorGUILayout.Space(15);
        EditorGUILayout.LabelField("업무 완료 처리", EditorStyles.boldLabel);
        for (int i = 0; i < service.EntryCount; i++)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField(service.GetEntryLabel(i));
                if (service.IsEntryComplete(i)) EditorGUILayout.LabelField("완료", GUILayout.Width(42));
                else using (new EditorGUI.DisabledScope(!service.CanCompleteEntry(i)))
                {
                    if (GUILayout.Button("완료", GUILayout.Width(52))) service.CompleteEntry(i);
                }
            }
        }
    }

    private void StartTest()
    {
        if (mode == DirectPlayMode.MainWorld)
        {
            try
            {
                DailyData day = DataLoader.GetDayData(dayIndex);
                if (day?.dayTimes == null || day.dayTimes.Length < 4 || day.dayTimes[timeIndex] == null)
                {
                    EditorUtility.DisplayDialog("날짜 데이터 오류",
                        $"dailyData{dayIndex}.json의 시간대 데이터를 사용할 수 없습니다.", "확인");
                    return;
                }
            }
            catch (Exception exception)
            {
                EditorUtility.DisplayDialog("날짜 데이터 오류", exception.Message, "확인");
                return;
            }
        }
        if (mode == DirectPlayMode.OverrideWorld)
        {
            customData.date = new Date(2000, 1, 1);
            string[] hours = { "09", "10", "17", "20" };
            for (int i = 0; i < 4; i++)
            {
                customData.dayTimes[i].daytime = new DayTime(hours[i], "00");
                customData.dayTimes[i].action.Clear();
            }
            List<DayEditorIssue> errors = DayEditorValidator.Validate(customData,
                MiniGames.Select(game => game.code).ToArray(), null, false)
                .Where(issue => issue.severity == DayEditorIssueSeverity.Error).ToList();
            for (int i = 0; i < customData.dayTimes.Length; i++)
            {
                TimeData time = customData.dayTimes[i];
                if (time?.npc == null) continue;
                for (int n = 0; n < time.npc.Count; n++)
                {
                    ChatObjectData npc = time.npc[n];
                    if (npc != null && (npc.positions == null || npc.positions.Count == 0))
                        errors.Add(new DayEditorIssue(DayEditorIssueSeverity.Error,
                            $"dayTimes[{i}].npc[{n}]", "NPC 위치가 하나 이상 필요합니다."));
                    if (npc != null && !NpcTypes.Contains(npc.objectType))
                        errors.Add(new DayEditorIssue(DayEditorIssueSeverity.Error,
                            $"dayTimes[{i}].npc[{n}]", "유효한 NPC 타입을 입력해 주세요."));
                }
            }
            if (errors.Count > 0)
            {
                EditorUtility.DisplayDialog("오버라이드 설정 오류", string.Join("\n", errors), "확인");
                return;
            }
        }

        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
        string targetPath = mode == DirectPlayMode.MiniGame ? MiniGames[miniGameIndex].path : WorldPath;
        Scene controller = EditorSceneManager.OpenScene(ControllerPath, OpenSceneMode.Single);
        Scene target = EditorSceneManager.OpenScene(targetPath, OpenSceneMode.Additive);
        if (!controller.IsValid() || !target.IsValid() || !EditorSceneManager.SetActiveScene(target))
        {
            Debug.LogError("테스트 씬을 열 수 없습니다.");
            return;
        }

        DirectPlayBootstrap bootstrap = UnityEngine.Object.FindObjectOfType<DirectPlayBootstrap>();
        if (bootstrap == null)
        {
            Debug.LogError("DirectPlayBootstrap이 없습니다.");
            return;
        }
        var settings = new DirectPlayLaunchSettings
        {
            mode = mode,
            dayIndex = dayIndex,
            timeIndex = timeIndex,
            workCode = MiniGames[miniGameIndex].code,
            stage = stage,
            overrideData = mode == DirectPlayMode.OverrideWorld ? customData : null
        };
        EditorPrefs.SetString(DirectPlayLaunchSettings.EditorKey, JsonConvert.SerializeObject(settings));
        EditorApplication.isPlaying = true;
    }

    private void OnInspectorUpdate()
    {
        if (EditorApplication.isPlaying) Repaint();
    }
}
