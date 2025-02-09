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
        // 인스턴스 존재 확인
        if (_instance != null)
        {
            // 인스턴스가 본인인지 확인
            if (_instance != this)
            {
                Destroy(gameObject);    // 다른 인스턴스일시 생성 취소
            }
    
            // 본인이면 종료
            return;
        }
    
        // 첫 인스턴스 생성
        _instance = GetComponent<T>();
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
            if (!_instance)
            {
                _instance = FindObjectOfType<T>();
            }
            return _instance;
        }
    }

    protected void Awake()
    {
        // 인스턴스 존재 확인
        if (_instance != null)
        {
            // 인스턴스가 본인인지 확인
            if (_instance != this)
            {
                Destroy(gameObject);    // 다른 인스턴스일시 생성 취소
            }

            // 본인이면 종료
            return;
        }

        // 첫 인스턴스 생성
        _instance = GetComponent<T>();
        DontDestroyOnLoad(gameObject);
    }
}