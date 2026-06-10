using GameService;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Utility;

public class TaskPanel : MonoBehaviour
{
    public GameObject TaskPrefab;
    public Transform TaskParent;

    private const string taskPrefix = "[ ]";
    private const string clearPrefix = "[X]";
    
    private readonly List<(Task task, bool isClear, TMP_Text obj)>  _tasks = new();
    
    private void Start()
    {
        if (GameSystem.GetService<ITaskService>() == null)
        {
            EditorLogger.LogWarning("No TaskService found.");
            return;
        }

        Init();
    }

    /// <summary>
    /// Panel 초기 세팅
    /// </summary>
    private void Init()
    {
        // Inject
        ITaskService taskService = GameSystem.GetService<ITaskService>();
        taskService.OnTaskAdded += task => AddTask(task);
        taskService.OnTaskCleared += ClearTask;
        taskService.OnTaskRemoved += RemoveTask;
        
        // Task List Refresh
        Clear();
        
        foreach (var iter in taskService.GetAllTasks())
        {
            AddTask(iter.task, iter.isClear);
        }
    }

    /// <summary>
    /// 패널 초기화
    /// </summary>
    private void Clear()
    {
        if (_tasks.Count == 0) return;
        
        foreach (var obj in _tasks.Select(t => t.obj))
        {
            Destroy(obj);
        }
        
        _tasks.Clear();
    }

    /// <summary>
    /// Task 추가
    /// </summary>
    /// <param name="task">추가할 task 데이터</param>
    /// <param name="isClear">클리어 여부</param>
    private void AddTask(Task task, bool isClear = false)
    {
        // Set Text
        string prefix = isClear ? clearPrefix : taskPrefix;
            
        TMP_Text text = Instantiate(TaskPrefab, TaskParent).GetComponent<TMP_Text>();
        text.text = $"{prefix} {task.description}";

        if (isClear)
        {
            text.fontStyle |= FontStyles.Strikethrough;
        }
        
        text.gameObject.SetActive(true);
            
        // Add to List
        _tasks.Add((task, isClear, text));
    }

    private void ClearTask(Task task)
    {
        var taskSet = _tasks.FirstOrDefault(set => set.task.Equals(task));

        if (taskSet.obj == null) return;
        
        // Set Text
        taskSet.obj.text = $"{clearPrefix} {task.description}";
        taskSet.obj.fontStyle |= FontStyles.Strikethrough;
        
        taskSet.isClear = true;
    }

    /// <summary>
    /// Task 제거
    /// </summary>
    /// <param name="task">제거할 task 데이터</param>
    private void RemoveTask(Task task)
    {
        var taskSet = _tasks.FirstOrDefault(set => set.task.Equals(task));

        if (taskSet.obj == null)
        {
            return;
        }

        Destroy(taskSet.obj);
        _tasks.Remove(taskSet);
    }
}
