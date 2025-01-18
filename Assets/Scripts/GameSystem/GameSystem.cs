using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameSystem : SingletonObject<GameSystem>
{
    /**
    * 게임 내 데이터 관리 시스템
        플레이어 데이터를 저장, 로드
        게임 데이터를 로드, 관리
        날짜, 시간대, 진행상황 적용
    */ 

    /// 게임 데이터 
    private List<SaveData> saveList = new();    // 저장 데이터
    private SaveData player => saveList[dateIndex];      // 오늘 세이브 데이터
    public DailyData dayData { get; private set; }    // 오늘 날짜 데이터

    /// 게임 플레이 데이터 
    public int dateIndex = 0;

    public int timeIndex = 0;
    
    public WorldVector currentLocation;
    
    public bool isScreenOn = false; 
    
    public bool isTaskClear   // 모든 업무 완료 플래그
    {
        get { 
            bool workResult = true;
            foreach(var workStatus in dayData.workList)
            {
                workResult = workResult & workStatus.isClear;
            }
            return workResult;
        }
    }


    new void Awake()
    {
        base.Awake();

        saveList = DataLoader.LoadSaveData();     // 세이브 데이터 로드
        dayData = DataLoader.LoadGameData(dateIndex);     // 게임 데이터 로드  

        // TODO: 날짜 선택 기능 구현 (임시로 0일차부터 로드)
        SetDate(0);
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
            date = dateIndex + 1;
        }
        
        // TODO: 이전 날짜 저장
        // DataLoader.SavePlayerData(saveList);

        // TODO: 날짜 로드, 로딩씬
        dayData = DataLoader.LoadGameData(date);

        // TODO: 해당 날짜 불러오기

        isScreenOn = false;

        ObjectDatabase.Instance.Read();
    }

    ///<summary>
    /// 다음 시간대로 전환
    ///</summary>
    ///<param name="time">전환할 시간(마지막 시간대면 다음 날짜로)</param>
    public void SetTime(int _time)
    {
        // 시간대 오류
        if (_time < 0 || _time >= 4)
            return;
        
        // 시간대 적용
        // gameData.time = _time;

        // 월드 리로드
        if (SceneManager.GetActiveScene().name == "MainWorld")
        {
            WorldSceneManager.Instance.ReloadWorld();
        }
    }


    /// <summary>
    /// 명성치 적용
    /// </summary>
    /// <param name="num">변경할 만큼의 명성치</param>
    /// <returns></returns>
    public void SetRenown(int num)
    {
        player.renown += num;
    }
    

    /// <summary>
    /// 특정 업무의 오늘 스테이지 번호 반환
    /// </summary>
    /// <param name="workCode"></param>
    /// <returns></returns>
    public int GetTask(string workCode)
    {
        foreach(var work in dayData.workList)
        {
            if (work.code == workCode)
            {
                return work.stage;
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
        foreach (var work in dayData.workList)
        {
            if (work.isClear == true)
                continue;
            if (work.code == workCode)
            {
                currentWork = work;
                break;
            }
        }

        // 업무 불일치 오류
        if (currentWork == null)
        {
            Debug.Log("Work doesn't Match");
            return;
        }

        // TODO: 업무 완료로 전환
        currentWork.isClear = true;
    }

    
    /// <summary>
    /// 해당 씬 로드
    /// </summary>
    /// <param name="sceneName"></param>
    public static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    
    
#if DEBUG
    public DailyData GetDailyData(int num)
    {
        return dayData;
    }
#endif
}
