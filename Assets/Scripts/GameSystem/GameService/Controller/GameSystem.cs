using System;
using GameService;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;

public sealed class GameSystem : Singleton<GameSystem>
{
    /**
     * 게임 메인 시스템 로직
     * - 게임서비스 관리
     * - 씬 운영
     */

    public void Awake()
    {
        // Entry 씬 로드
        if (SceneManager.sceneCount == 1)
        {
            EnterScene("GameStart");
            return;
        }
        
        // 현재 켜져있는 씬 불러오기
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.name.Contains("Controller"))
            {
                continue;
            }

            _currentScene = scene;
            break;
        }
    }
    
    # region ServiceManage
    
    /// Service 목록
    public static SaveService SaveService;
    private static Dictionary<Type, IService> services = new();

    /// <summary>
    /// 서비스 등록
    /// </summary>
    /// <param name="service">등록할 서비스 입력</param>
    /// <typeparam name="T">인터페이스 타입으로 구분</typeparam>
    /// <remarks>이미 등록된 서비스일 시 등록 안 됨, 기존 서비스 유지</remarks>
    public static void RegisterService<T>(T service) where T : class, IService
    {
        Type type = typeof(T);
        if (!services.TryAdd(type, service))
        {
            EditorLogger.LogWarning($"Service already registered: {type.Name}");
        }
    }

    /// <summary>
    /// 등록된 서비스 로드
    /// </summary>
    /// <typeparam name="T">상세 인터페이스 타입으로 구분</typeparam>
    /// <returns>해당 서비스 반환 (미등록시 null)</returns>
    public static T GetService<T>() where T : class, IService
    {
        Type type = typeof(T);
        if (services.TryGetValue(type, out IService service))
        {
            return service as T;
        }
        return null;
    }

    /// <summary>
    /// 전체 서비스 목록 출력 (Save제외)
    /// </summary>
    /// <returns>리스트 형식 반환</returns>
    public static List<IService> GetAllServices()
    {
        return services.Select(kv => kv.Value).ToList();
    }

    /// <summary>
    /// 서비스 등록 해제
    /// </summary>
    /// <typeparam name="T">상세 인터페이스 타입으로 구분</typeparam>
    public static void UnRegister<T>() where T : class, IService
    {
        services.Remove(typeof(T));
    }
    
    #endregion
    
    #region SceneManage
    
    // 현재 활성씬
    private Scene _currentScene;
    [SerializeField] private bool _isLoading = false;
    
    public FadeCurtain loadUI;

    /// <summary>
    /// 씬 입장
    /// </summary>
    /// <param name="sceneName">로드할 씬 명칭</param>
    /// <param name="callback">씬 로드 후 호출 콜백 함수</param>
    public void EnterScene(string sceneName, Action callback = null)
    {
        if (_isLoading)
        {
            EditorLogger.LogWarning($"Scene already on Loading...");
            return;
        }
        
        StartCoroutine(LoadScene(sceneName, callback));
    }
    
    private IEnumerator LoadScene(string sceneName, Action callback)
    {
        // 로딩 시작
        _isLoading = true;
        
        // 로딩씬 화면 페이드아웃
        if (loadUI)
        {
            loadUI.gameObject.SetActive(true);
            loadUI.Fade(FadeMode.Out);
        }
        yield return new WaitUntil(() => !loadUI.isLoading);
        
        // 기존 씬 언로드
        if (_currentScene.isLoaded)
        {
            yield return SceneManager.UnloadSceneAsync(_currentScene);
        }
        
        // 메인 씬 로드 시작
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        
        // 씬 로드 완료 및 정리
        _currentScene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(_currentScene);
        
        // 로딩 종료
        loadUI.gameObject.SetActive(false);
        _isLoading = false;
        
        // 콜백 실행
        callback?.Invoke();
    }
    
    #endregion
}
