using UnityEngine;

namespace GameEditor
{
    public abstract class EditorUIBase : MonoBehaviour
    {
        protected DailyData CurrentData;

        public virtual void Init(DailyData data)
        {
            CurrentData = data;
        }
    }
}
