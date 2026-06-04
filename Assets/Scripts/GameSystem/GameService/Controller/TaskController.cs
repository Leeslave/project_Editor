
using GameService;
using System.Collections.Generic;
using System.Linq;

public class TaskController : ServiceBase<ITaskService>, ITaskService
{
    private List<(Task task, bool isClear)> _tasks = new();
    
    public void Init(DailyData data)
    {
        _tasks.Clear();
    }

    public void AddTask(Task task)
    {
        throw new System.NotImplementedException();
    }

    public List<(Task, bool)> GetAllTasks()
    {
        throw new System.NotImplementedException();
    }

    public Task GetTask(string key)
    {
        throw new System.NotImplementedException();
    }

    public void ClearTask(string key)
    {
        var task = _tasks.FirstOrDefault(t => t.task.key == key);
        
    }

    public bool IsTaskClear()
    {
        return _tasks.All(t => t.isClear);
    }
}
