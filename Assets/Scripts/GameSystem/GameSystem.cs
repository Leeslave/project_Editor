using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameSystem : MonoBehaviour
{
    /**
    * 게임 내 데이터 관리 시스템
        플레이어 데이터를 저장, 로드
        게임 데이터를 로드, 관리
        날짜, 시간대, 진행상황 적용
    */ 

    /// 플레이 데이터 
    private List<SaveData> saveList = new();    // 저장 데이터
    private List<DailyData> dailyList = new();     // 날짜별 데이터

    public SaveData player { get { return saveList[gameData.date]; } }      // 오늘 세이브 데이터
    public DailyData today { get { return dailyList[gameData.date]; } }    // 오늘 날짜 데이터

    /// 오늘 날짜 데이터
    public GameData gameData = new();

    // 스크린 활성화 여부
    public bool isScreenOn = false; 
    
    // 업무 클리어 여부
    public bool isTaskClear   // 모든 업무 완료 플래그
    {
        get { 
            bool workResult = true;
            foreach(var workStatus in Instance.today.workList.Values)
            {
                workResult = workResult & workStatus;
            }
            return workResult;
        }
    }

    // 싱글턴
    private static GameSystem _instance;
    public static GameSystem Instance
    {
        get { return _instance; }
    }


    private void Awake()
    {
        if (!_instance)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);

            saveList = DataLoader.LoadSaveData();     // 세이브 데이터 로드
            dailyList = DataLoader.LoadGameData();     // 게임 데이터 로드  

            SetDate(0);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    ///<summary>
    /// 날짜 전환 (게임 저장)
    ///</summary>
    ///<param name="dateIndex">전환할 날짜 인덱스(없으면 다음 날짜로), 시간은 무조건 아침</param>
    public void SetDate(int date = -1)
    {
        // 다음 날짜로 이동시
        if (date == -1)
        {
            date = gameData.date + 1;
        }

        // 날짜 오류
        if (date > dailyList.Count || date < 0)
        {
            Debug.Log($"Day Out Of Range: {date}");
            return;
        }

        // 해당 날짜 불러오기
        gameData.date = date;
        gameData.time = 0;

        // 게임 저장 (튜토리얼 날짜 제외)
        if (date > 1)
        {
            DataLoader.SavePlayerData(saveList);
        }

        ObjectDatabase.Instance.Read();
        
        if (SceneManager.GetActiveScene().name == "MainWorld")
        {
            SceneManager.LoadScene("DayLoading");
        }
    }

    ///<summary>
    /// 다음 시간대로 전환
    ///</summary>
    ///<param name="time">전환할 시간(마지막 시간대면 다음 날짜로)</param>
    public void SetTime(int _time)
    {
        if (_time < 0 || _time >= 4)
            return;
        gameData.time = _time;

        if (SceneManager.GetActiveScene().name == "MainWorld")
        {
            WorldSceneManager.Instance.ReloadWorld();
        }
    }
    

    /// <summary>
    /// 특정 업무의 오늘 스테이지 번호 반환
    /// </summary>
    /// <param name="workCode"></param>
    /// <returns></returns>
    public int GetTask(string workCode)
    {
        foreach(var work in today.workList)
        {
            if (work.Key.code == workCode)
            {
                return work.Key.stage;
            }
        }
        return -1;
    }


    /// <summary>
    /// 업무 완료 여부 설정
    /// </summary>
    /// <param name="workCode">설정할 업무의 코드명</param>
    /// <param name="isClear">업무 완료 여부</param>
    public void ClearTask(string workCode)
    {
        // 코드에 해당하는 업무 불러오기
        Work currentWork = null;
        foreach (var work in today.workList)
        {
            if (work.Key.code == workCode)
            {
                currentWork = work.Key;
                break;
            }
        }

        // 업무 불일치 오류
        if (currentWork == null)
        {
            Debug.Log("Work doesn't Match");
            return;
        }

        // 업무 완료로 전환
        today.workList[currentWork] = true;
    }

    /// <summary>
    /// 해당 씬 로드
    /// </summary>
    /// <param name="sceneName"></param>
    public static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
