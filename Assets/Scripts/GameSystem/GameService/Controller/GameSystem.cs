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
            var scene = SceneManager.GetSceneAt(i);
            if (!scene.name.Contains("Controller"))
            {
                _currentScene = scene;
                break;
            }
        }
        
        #if DEBUG
        // 시작화면 및 세이브 선택 시 시작 무시
        if (!_currentScene.name.Contains("Game"))
        {
            StartGame();
        }
        #endif
    }
    
    public void StartGame()
    {
        // 세이브 선택 후 게임 진입
        // 1. DayController 초기화
        // 2. WorkManager 생성
        
    }
    
    # region ServiceManage
    
    /// Service 목록
    private List<IService> services = new();

    public void RegisterService<T>(T service) where T : class, IService
    {
        if (services.Any(x => x is T))
        {
            EditorLogger.LogWarning($"Service already registered: {service.GetType().Name}");
        }
        services.Add(service);
    }

    public T GetService<T>() where T : class, IService
    {
        return services.FirstOrDefault(x => x is T) as T;
    }

    public void UnRegister<T>(T service) where T : class, IService
    {
        if (!services.Contains(service))
        {
            EditorLogger.LogWarning($"Remove Service not registered: {service.GetType().Name}");
        }
        services.Remove(service);
    }
    
    #endregion
    
    #region SceneManage
    
    private Scene _currentScene;
    private bool _isLoading = false;
    private GameObject loadUI => transform.GetChild(0).gameObject;

    public void EnterScene(string sceneName)
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
