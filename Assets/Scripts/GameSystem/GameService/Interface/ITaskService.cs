
using System;
using System.Collections.Generic;

namespace GameService
{
    [Serializable]
    public struct Task : IEquatable<Task>
    {
        public string key;
        public string description;

        public bool Equals(Task other)
        {
            return key == other.key && description == other.description;
        }

        public override bool Equals(object obj)
        {
            return obj is Task other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(key, description);
        }
    }

    public interface ITaskService : IService
    {
        event Action<Task> OnTaskAdded;
        event Action<Task> OnTaskCleared;
        event Action<Task> OnTaskRemoved;
        
        void AddTask(Task task);
        
        IReadOnlyList<(Task task, bool isClear)> GetAllTasks();
        
        Task GetTask(string key);
        
        void ClearTask(string key);
        
        void RemoveTask(string key);
        
        bool IsTaskClear();
    }
}
