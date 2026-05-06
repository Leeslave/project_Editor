using UnityEngine;

namespace GameService
{
    public interface IService
    {
        public void Init(DailyData data);
    }

    public abstract class ServiceBase<TInterface> : MonoBehaviour where TInterface : class, IService
    {
        protected virtual void Awake()
        {
            if (this is TInterface service)
            {
                GameSystem.RegisterService<TInterface>(service);
            }

            OnAwake();
        }

        protected virtual void OnDestroy()
        {
            GameSystem.UnRegister<TInterface>();
            GetDestroy();
        }
        
        public virtual void OnAwake() { }
        public virtual void GetDestroy() { }
    }
}