using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    // 싱글턴 인스턴스를 저장하는 필드
    private static T _instance;

    // 애플리케이션이 종료 중인지 확인하는 플래그 (종료 시 접근하여 Null 참조 오류가 나는 것을 방지)
    private static bool _applicationIsQuitting = false;

    public virtual void Awake()
    {
        
    }

    /// <summary>
    /// 싱글턴 인스턴스를 가져옵니다.
    /// </summary>
    public static T Instance
    {
        get
        {
            // 1. 애플리케이션 종료 중에는 새로운 인스턴스를 만들지 않습니다.
            if (_applicationIsQuitting)
            {
                Debug.LogWarning("[Singleton] Instance '" + typeof(T).Name +
                                 "' already destroyed on application quit. Returning null.");
                return null;
            }

            // 2. 인스턴스가 없으면 찾거나 새로 생성합니다.
            if (!_instance)
            {
                // 씬에서 인스턴스를 찾습니다.
                _instance = FindObjectOfType<T>();
            }
            return _instance;
        }
    }
}

public class SingletonObject<T>: MonoBehaviour 
                            where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance is null)
            {
                _instance = FindObjectOfType<T>();
            }
            return _instance;
        }
    }

    protected void Awake()
    {
        if (_instance is null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
}