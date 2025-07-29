using UnityEngine;

public class Singleton<T>: MonoBehaviour 
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