using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum DirectPlayMode { MainWorld, OverrideWorld, MiniGame }

[System.Serializable]
public sealed class DirectPlayLaunchSettings
{
    public DirectPlayMode mode;
    public int dayIndex;
    public int timeIndex;
    public string workCode;
    public int stage;
    public DailyData overrideData;

    public static string EditorKey => "DirectPlayLaunch:" + Application.dataPath;
}

/// <summary>테스트 씬의 저장 상태와 날짜 데이터를 실제 씬 초기화 전에 준비한다.</summary>
[DefaultExecutionOrder(-10000)]
public sealed class DirectPlayBootstrap : MonoBehaviour
{
    [SerializeField] private DirectPlayMode mode;
    [SerializeField, Min(0)] private int dayIndex;
    [SerializeField, Range(0, 3)] private int timeIndex;
    [SerializeField] private string workCode;
    [SerializeField, Min(0)] private int stage;
    [SerializeField] private DailyData overrideData;
    [SerializeField] private DirectPlayWorkService workService;
    [SerializeField] private DirectPlaySaveService saveService;

    public void Configure(DirectPlayMode selectedMode, int day, int time, string miniGameCode,
        int miniGameStage, DailyData customData)
    {
        mode = selectedMode;
        dayIndex = day;
        timeIndex = time;
        workCode = miniGameCode;
        stage = miniGameStage;
        overrideData = customData;
    }

    private void Awake()
    {
#if UNITY_EDITOR
        string key = DirectPlayLaunchSettings.EditorKey;
        if (EditorPrefs.HasKey(key))
        {
            string json = EditorPrefs.GetString(key);
            EditorPrefs.DeleteKey(key);
            DirectPlayLaunchSettings settings = JsonConvert.DeserializeObject<DirectPlayLaunchSettings>(json);
            if (settings != null)
                Configure(settings.mode, settings.dayIndex, settings.timeIndex,
                    settings.workCode, settings.stage, settings.overrideData);
        }
#endif
        if (workService == null || saveService == null)
        {
            Debug.LogError("DirectPlayController 서비스 참조가 없습니다.", this);
            enabled = false;
            return;
        }

        Scene target = FindTargetScene();
        if (!target.IsValid())
        {
            Debug.LogError("테스트할 씬을 DirectPlayController와 함께 열어 주세요.", this);
            enabled = false;
            return;
        }

        if (mode == DirectPlayMode.MiniGame)
            workService.ConfigureDirectWork(workCode, stage);
        SceneManager.SetActiveScene(target);
    }

    private void Start()
    {
        if (!enabled) return;
        DailyData data = mode switch
        {
            DirectPlayMode.MainWorld => null,
            DirectPlayMode.OverrideWorld => overrideData,
            DirectPlayMode.MiniGame => CreateMiniGameData(),
            _ => null
        };

        saveService.Configure(mode == DirectPlayMode.MainWorld ? timeIndex : 0, 0, data);
        saveService.Init();
        saveService.SelectDay(dayIndex, 0);
        if (saveService.CurrentSaveId < 0)
        {
            enabled = false;
            return;
        }

        // 날짜 서비스 초기화는 업무를 다시 읽는다. 미니게임의 Awake에서 필요했던 설정을 복원한다.
        if (mode == DirectPlayMode.MiniGame)
            workService.ConfigureDirectWork(workCode, stage);
    }

    private DailyData CreateMiniGameData()
    {
        var data = new DailyData
        {
            date = new Date(2000, 1, 1),
            startLocation = new WorldVector(World.Street, 0),
            workList = new List<Work> { new Work(workCode, stage) }
        };
        string[] hours = { "09", "10", "17", "20" };
        for (int i = 0; i < 4; i++) data.dayTimes[i].daytime = new DayTime(hours[i], "00");
        return data;
    }

    private Scene FindTargetScene()
    {
        Scene active = SceneManager.GetActiveScene();
        if (active.IsValid() && active != gameObject.scene) return active;
        for (int index = 0; index < SceneManager.sceneCount; index++)
        {
            Scene candidate = SceneManager.GetSceneAt(index);
            if (candidate.IsValid() && candidate.isLoaded && candidate != gameObject.scene)
                return candidate;
        }
        return default;
    }
}
