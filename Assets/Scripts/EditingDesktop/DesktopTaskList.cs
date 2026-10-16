using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DesktopTaskList : MonoBehaviour
{
    private const int NewsTaskType = 0;
    private const int ProfileTaskType = 2;
    [SerializeField] private TMP_Text[] _labels;
    private readonly Dictionary<int, List<TaskEntry>> _entriesByType = new Dictionary<int, List<TaskEntry>>
    {
        { NewsTaskType, new List<TaskEntry>() },
        { ProfileTaskType, new List<TaskEntry>() }
    };
    private readonly string[] _profileFields = { "국적", "직업", "부서", "소속", "얼굴" };
    private readonly string[] _newsActions = { "추가", "삭제", "변경" };
    private int _entryCount;

    private sealed class TaskEntry
    {
        public int LabelIndex;
        public int NewsLine;
        public string Title;
        public string Detail;
    }

    // 현재 스테이지의 인물 및 뉴스 지시를 화면 항목으로 만든다.
    private void Start()
    {
        EditingDesktopController controller = EditingDesktopController.Instance;
        if (controller == null || controller.Instructions == null) return;
        if (controller.Instructions.InfoInst != null)
            foreach (ProfileEditDirective instruction in controller.Instructions.InfoInst)
            {
                string detail = instruction.ToDo == 4
                    ? $"Face{instruction.Before} > Face{instruction.After}"
                    : $"{controller.ProfileOptions[instruction.ToDo][instruction.Before].Trim('\r')} > " +
                      $"{controller.ProfileOptions[instruction.ToDo][instruction.After].Trim('\r')}";
                controller.PersonDataManager.PeopleCorrect.Add(
                    new Tuple<string, int, int, int>(instruction.Target, instruction.ToDo, instruction.After, _entryCount));
                AddEntry(ProfileTaskType, $"{instruction.Target} {_profileFields[instruction.ToDo]} 변경", detail, 0);
            }
        if (controller.Instructions.NewsInst != null)
            foreach (NewsEditDirective instruction in controller.Instructions.NewsInst)
            {
                string detail = instruction.ToDo == 0 ? instruction.Goal :
                    instruction.ToDo == 1 ? "" : $"{instruction.Normal} > {instruction.Revise}";
                controller.NewsManager.NewsAnsLine[instruction.Line] = true;
                controller.NewsManager.NewsChange[instruction.Line] = instruction.Goal;
                AddEntry(NewsTaskType, $"{instruction.Line + 1}번째 줄 {_newsActions[instruction.ToDo]}", detail, instruction.Line);
            }
        gameObject.SetActive(false);
    }

    // 지시의 설명과 화면 인덱스를 함께 저장한다.
    private void AddEntry(int type, string title, string detail, int newsLine)
    {
        var entry = new TaskEntry { LabelIndex = _entryCount++, Title = title, Detail = detail, NewsLine = newsLine };
        _entriesByType[type].Add(entry);
        RefreshEntry(type, entry, false);
        _labels[entry.LabelIndex].gameObject.SetActive(true);
    }

    public void CheckList(int type, int line, bool clear, int taskIndex = -1)
    {
        if (!_entriesByType.TryGetValue(type, out List<TaskEntry> entries)) return;
        TaskEntry entry = taskIndex < 0 ? entries.FindLast(item => item.NewsLine == line) :
            taskIndex < entries.Count ? entries[taskIndex] : null;
        if (entry != null) RefreshEntry(type, entry, clear);
    }

    // 완료 상태에 맞춰 지시 문구의 색과 취소선을 갱신한다.
    private void RefreshEntry(int type, TaskEntry entry, bool completed)
    {
        string category = type == NewsTaskType ? "News" : "Info";
        string headingColor = completed ? "#800000" : "#FF0000";
        string bodyColor = completed ? "#808080" : "#FFFFFF";
        string text = $"<color={headingColor}>{category}</color><color={bodyColor}> {entry.Title}" +
            $"<size=20>\n\n {entry.Detail}</size></color>";
        _labels[entry.LabelIndex].text = completed ? "<s>" + text + "</s>" : text;
    }
}
