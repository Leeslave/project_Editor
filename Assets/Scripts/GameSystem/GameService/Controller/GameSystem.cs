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

    public static void RegisterService<T>(T service) where T : class, IService
    {
        var type = typeof(T);
        if (!services.TryAdd(type, service))
        {
            EditorLogger.LogWarning($"Service already registered: {type.Name}");
        }
    }

    public static T GetService<T>() where T : class, IService
    {
        var type = typeof(T);
        if (services.TryGetValue(type, out IService service))
        {
            return service as T;
        }
        return null;
    }

    public static List<IService> GetAllServices()
    {
        List<IService> result = new();
        result = services.Select(kv => kv.Value).ToList();
        
        return result;
    }

    public static void UnRegister<T>() where T : class, IService
    {
        services.Remove(typeof(T));
    }
    
    #endregion
    
    #region SceneManage
    
    private Scene _currentScene;
    private bool _isLoading = false;
    private GameObject loadUI => transform.GetChild(0).gameObject;

    public void EnterScene(string sceneName = "GameStart")
    {
        if (_isLoading)
        {
            EditorLogger.LogWarning($"Scene already on Loading...");
            return;
        }
        
        StartCoroutine(LoadScene(sceneName));
    }
    
    public IEnumerator LoadScene(string sceneName)
    {
        // 로딩 시작
        _isLoading = true;
        if (!loadUI)
        {
            loadUI.SetActive(true);
            // TODO: 로딩 애니메이션 시작
        }
        
        // 기존 씬 언로드
        if (_currentScene.isLoaded)
        {
            yield return SceneManager.UnloadSceneAsync(_currentScene);
        }
        
        // 메인 씬 로드 시작
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        
        _currentScene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(_currentScene);
        
        loadUI.SetActive(false);
        _isLoading = false;
    }
    
    #endregion
}
