
using System;
using System.Collections.Generic;

namespace GameService
{
    [Serializable]
    public struct Task
    {
        public string key;
        public string description;
    }

    public interface ITaskService : IService
    {
        void AddTask(Task task);
        
        IReadOnlyList<(Task, bool)> GetAllTasks();
        
        Task GetTask(string key);
        
        void ClearTask(string key);
        
        bool IsTaskClear();
    }
}
