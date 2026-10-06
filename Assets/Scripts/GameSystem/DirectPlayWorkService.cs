using System;
using System.Collections.Generic;
using System.Linq;
using GameAction;
using GameService;
using UnityEngine;

/// <summary>업무의 각 스테이지를 메모리에서 독립적으로 관리하는 테스트 서비스.</summary>
[DefaultExecutionOrder(-12000)]
public sealed class DirectPlayWorkService : ServiceBase<IWorkService>, IWorkService
{
    private readonly List<Work> works = new List<Work>();
    private readonly List<bool> completed = new List<bool>();
    private int activeIndex = -1;
    private bool directMiniGame;

    public bool isScreenOn { get; set; }
    public string CurrentWorkCode { get; private set; }
    public event Action OnWorkClear;

    public void Init(DailyData data)
    {
        works.Clear();
        completed.Clear();
        CurrentWorkCode = null;
        activeIndex = -1;
        directMiniGame = false;
        if (data?.workList == null) return;
        foreach (Work work in data.workList)
        {
            if (work == null || string.IsNullOrWhiteSpace(work.code)) continue;
            works.Add(work);
            completed.Add(false);
        }
    }

    public void ConfigureDirectWork(string workCode, int stage)
    {
        works.Clear();
        completed.Clear();
        works.Add(new Work(workCode, stage));
        completed.Add(false);
        activeIndex = 0;
        CurrentWorkCode = workCode;
        directMiniGame = true;
    }

    public List<(string code, string name)> GetList()
    {
        return works.GroupBy(work => work.code, StringComparer.Ordinal)
            .Select(group => (group.Key,
                string.IsNullOrWhiteSpace(group.First().name) ? group.Key : group.First().name.Trim()))
            .ToList();
    }

    public bool TryStartWork(string workCode, out string sceneName)
    {
        sceneName = null;
        int index = FindNext(workCode);
        if (index < 0) return false;
        string target = workCode == "SecureDocument" ? "Document" : workCode;
        if (!Application.CanStreamedLevelBeLoaded(target)) return false;
        activeIndex = index;
        CurrentWorkCode = workCode;
        sceneName = target;
        return true;
    }

    public bool IsWorkClear(string workCode)
    {
        if (CurrentWorkCode == "SecureDocument" && (workCode == "Dodge" || workCode == "Maze"))
            return true;
        return works.Where(work => work.code == workCode).Any() && FindNext(workCode) < 0;
    }

    public int GetStage(string workCode)
    {
        // 구형 미니게임은 다른 업무 코드를 조회하므로 실행 중인 스테이지를 우선한다.
        if (activeIndex >= 0) return works[activeIndex].stage;
        int index = FindNext(workCode);
        return index < 0 ? -1 : works[index].stage;
    }

    public bool ClearWork(string workCode)
    {
        if (activeIndex < 0) return IsWorkClear();
        if (works[activeIndex].code != workCode && !directMiniGame) return IsWorkClear();
        return CompleteEntry(activeIndex);
    }

    public bool CompleteEntry(int index)
    {
        if (!CanCompleteEntry(index)) return IsWorkClear();
        completed[index] = true;
        if (activeIndex == index)
        {
            activeIndex = -1;
            CurrentWorkCode = null;
        }
        if (IsWorkClear())
        {
            new NextTimeAction(2).Invoke();
            OnWorkClear?.Invoke();
        }
        return IsWorkClear();
    }

    public bool CanCompleteEntry(int index)
    {
        return index >= 0 && index < works.Count && !completed[index] &&
               FindNext(works[index].code) == index;
    }

    public int EntryCount => works.Count;
    public string GetEntryLabel(int index) => $"{works[index].code} / Stage {works[index].stage}";
    public bool IsEntryComplete(int index) => completed[index];
    public bool IsWorkClear() => completed.All(value => value);

    private int FindNext(string workCode)
    {
        if (string.IsNullOrWhiteSpace(workCode)) return -1;
        for (int i = 0; i < works.Count; i++)
            if (works[i].code == workCode && !completed[i]) return i;
        return -1;
    }
}
