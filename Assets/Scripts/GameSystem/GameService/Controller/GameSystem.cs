using System;
using GameService;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public sealed class GameSystem : Singleton<GameSystem>, ISaveService
{
    /**
     * 게임 메인 시스템 로직
     * - 데이터 로드 및 관리
     * - 세이브 관리
     * - 메인씬 로드 (게임 진입)
     */

    private GameObject loadUI => transform.GetChild(0).gameObject;
    private Action<IService> loadHandler;
    
    public void Init()
    {
        // 서비스 주입
        ServiceProvider.Register<ISaveService>(this);
        
        // TODO: 세이브 파일 무결성 확인
        
        // NOTE: GameSystem 생성 즉시 메인 월드 진입 (방식 개선 필요)
        // 게임 시작
        StartCoroutine(LoadNextScene("MainWorld"));
    }

    public IEnumerator LoadNextScene(string sceneName)
    {
        // 로딩씬 시작
        bool completeLoad = false;
        if (!loadUI)
        {
            loadUI.SetActive(true);
        }
        
        // 메인 씬 로드 시작
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        
        loadHandler = (service =>
        {
            if (service is IDayService)
            {
                completeLoad = true;
                ServiceProvider.OnServiceRegistered -= loadHandler;
            }
        });
        ServiceProvider.OnServiceRegistered += loadHandler;
        
        // Day 시스템 로드까지 대기
        yield return new WaitUntil(() => completeLoad || ServiceProvider.Get<IDayService>() != null);
        var dayService = ServiceProvider.Get<IDayService>();

        // TODO: Load UI 텍스트 액션 추가
        dayService.Init();
        loadUI.SetActive(false);
    }

    public new void Awake()
    {
        base.Awake();
        Init();
    }
    
    
    #region SaveManage
    //////// Save 관리 ////////
    
    [SerializeField] private SaveData save;
    private DaySave currentSave => save[index]

    public int Renown
    {
        get => save.renown;
        set
        {
            save.renown = value;
            OnRenownChanged?.Invoke(value);
        }
    }

    public event Action<int> OnRenownChanged;

    /// <summary>
    /// 세이브 데이터 로드
    /// </summary>
    /// <param name="index">세이브 번호</param>
    public void LoadSaveData(int index = 0)
    {
        save = new SaveData();

        DataLoader.GetPlayerData(index);
    }

    /// <summary>
    /// 명성치 조건 확인
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    public bool CheckRenown(int condition)
    {
        if (save.renown >= condition)
        {
            return true;
        }

        return false;
    }
    #endregion
}
