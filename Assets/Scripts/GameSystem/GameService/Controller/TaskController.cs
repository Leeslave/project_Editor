
using GameService;
using System;
using System.Collections.Generic;
using System.Linq;

public class TaskController : ServiceBase<ITaskService>, ITaskService
{
    private readonly List<(Task task, bool isClear)> _tasks = new();
    
    public void Init(DailyData data)
    {
        _tasks.Clear();
    }

    public void AddTask(Task task)
    {
        _tasks.Add((task, isClear: false));
    }

    public IReadOnlyList<(Task, bool)> GetAllTasks()
    {
        return _tasks.AsReadOnly();
    }

    public Task GetTask(string key)
    {
        return _tasks.FirstOrDefault(tk => tk.task.key == key).task;
    }

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
        }
    }

    public bool IsTaskClear()
    {
        return _tasks.All(t => t.isClear);
    }
}
