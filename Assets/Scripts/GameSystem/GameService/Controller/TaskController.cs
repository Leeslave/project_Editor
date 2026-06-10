
using GameService;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility;

public class TaskController : ServiceBase<ITaskService>, ITaskService
{
    private readonly List<(Task task, bool isClear)> _tasks = new();
    
    public void Init(DailyData data)
    {
        _tasks.Clear();
    }

    public event Action<Task> OnTaskAdded;
    public event Action<Task> OnTaskCleared;
    public event Action<Task> OnTaskRemoved;

    /// <summary>
    /// Task 추가
    /// </summary>
    /// <param name="task">추가할 Task 데이터</param>
    public void AddTask(Task task)
    {
        _tasks.Add((task, isClear: false));
        OnTaskAdded?.Invoke(task);
    }

    /// <summary>
    /// 모든 Task 목록 반환
    /// </summary>
    /// <returns>Task-클리어여부 리스트 : Readonly</returns>
    public IReadOnlyList<(Task, bool)> GetAllTasks()
    {
        return _tasks.AsReadOnly();
    }

    /// <summary>
    /// Task 정보 확인
    /// </summary>
    /// <param name="key">확인할 Task Key값</param>
    /// <returns>해당하는 Task 정보</returns>
    public Task GetTask(string key)
    {
        return _tasks.FirstOrDefault(tk => tk.task.key == key).task;
    }

    /// <summary>
    /// Task 클리어
    /// </summary>
    /// <param name="key">클리어할 Key</param>
    /// <remarks>K</remarks>
    public void ClearTask(string key)
    {
        for (int i = 0; i < _tasks.Count; i++)
        {
            if (_tasks[i].task.key != key)
            {
                continue;
            }

            Task tmp = _tasks[i].task;
            _tasks[i] = (tmp, isClear: true);
            OnTaskCleared?.Invoke(tmp);
        }
    }

    public void RemoveTask(string key)
    {
        for (int i = 0; i < _tasks.Count; i++)
        {
            if (_tasks[i].task.key != key)
            {
                continue;
            }
            
            Task tmp = _tasks[i].task;
            _tasks.RemoveAt(i);
            
            OnTaskRemoved?.Invoke(tmp);
        }
    }

    public bool IsTaskClear()
    {
        return _tasks.All(t => t.isClear);
    }
}
