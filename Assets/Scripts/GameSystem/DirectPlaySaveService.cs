using System;
using GameService;
using UnityEngine;
using Utility;

/// <summary>디스크에 기록하지 않고 날짜와 평판을 제공하는 테스트용 저장 서비스.</summary>
[DefaultExecutionOrder(-12000)]
public sealed class DirectPlaySaveService : SaveService
{
    private PlayerData playerData;
    private int initialTimeIndex;
    private int initialRenown;
    private DailyData suppliedData;

    public void Configure(int timeIndex, int renown, DailyData data)
    {
        initialTimeIndex = timeIndex;
        initialRenown = renown;
        suppliedData = data;
    }

    public override void Init()
    {
        playerData = new PlayerData();
        playerData.Init();
        Save = null;
    }

    public override PlayerData GetPlayerData() => playerData;

    public override void SelectDay(int day, int branch)
    {
        if (day < 0 || branch < 0 || branch > 99)
        {
            EditorLogger.LogError("DirectPlay 날짜 또는 분기 설정이 올바르지 않습니다.");
            return;
        }

        DailyData dailyData = suppliedData ?? DataLoader.GetDayData(day);
        if (dailyData?.dayTimes == null || initialTimeIndex < 0 || dailyData.dayTimes.Length < 4 ||
            dailyData.dayTimes.Length <= initialTimeIndex ||
            dailyData.dayTimes[initialTimeIndex] == null)
        {
            EditorLogger.LogError($"DirectPlay 날짜 데이터를 사용할 수 없습니다: {day}");
            return;
        }

        int renown = Save?.renown ?? initialRenown;
        Save = new DaySave { dayID = day * 100 + branch, renown = renown };
        playerData?.Save(Save.Clone());
        InitializeServices(dailyData);
    }

    private void InitializeServices(DailyData dailyData)
    {
        IDayService dayService = GameSystem.GetService<IDayService>();
        IWorkService workService = GameSystem.GetService<IWorkService>();
        ITaskService taskService = GameSystem.GetService<ITaskService>();
        ILocationService locationService = GameSystem.GetService<ILocationService>();

        dayService?.Init(dailyData);
        if (dayService != null) dayService.Time = initialTimeIndex;
        workService?.Init(dailyData);
        taskService?.Init(dailyData);
        locationService?.Init(dailyData);

        foreach (IService service in GameSystem.GetAllServices())
        {
            if (ReferenceEquals(service, dayService) || ReferenceEquals(service, workService) ||
                ReferenceEquals(service, taskService) || ReferenceEquals(service, locationService)) continue;
            service.Init(dailyData);
        }
    }

    public override void SwitchDay()
    {
        if (Save == null) return;
        initialTimeIndex = 0;
        SelectDay(DayIndex + 1, Save.dayID % 100);
    }

    public override void SwitchBranch(int branch)
    {
        if (Save == null || branch < 0 || branch > 99) return;
        Save = Save.Clone(DayIndex * 100 + branch);
        playerData?.Save(Save.Clone());
    }

    public override bool CheckRenown(int condition)
    {
        return Save != null && Renown >= condition;
    }
}
