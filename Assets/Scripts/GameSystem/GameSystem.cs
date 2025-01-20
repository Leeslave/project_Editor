using System.Collections.Generic;
using System.Data;
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

    /// 게임 데이터 (ReadOnly)
    private List<SaveData> saveList = new();    // 저장 데이터
    public SaveData player => saveList[dateIndex];      // 오늘 세이브 데이터
    public DailyData dayData { get; private set; }    // 오늘 날짜 데이터

    
    /// 게임 플레이 데이터 (ReadWrite)
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
        
        // TODO: 날짜 선택 기능 구현 (임시로 0일차부터 로드)
        dayData = DataLoader.LoadGameData(dateIndex);     // 게임 데이터 로드  
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

        // TODO: 해당 날짜 불러오기
        /*
         * 로딩씬 진입,
         * 날짜 데이터 로드, 
         * 시간대 로드 SetTime(0)
         * 날짜 시작 위치 설정
         */
        dayData = DataLoader.LoadGameData(date);

        isScreenOn = false;

        // 로딩씬 끝
    }

    
    ///<summary>
    /// 다음 시간대로 전환
    ///</summary>
    ///<param name="time">전환할 시간</param>
    public void SetTime(int time)
    {
        // 시간대 오류
        if (time is < 0 or >= 4)
            return;
        
        /* 시간대 로드
        - NPC 생성
            - 지역 락
            - BGM 변경
        */
        timeIndex = time;

        // TODO: 월드 리로드 (개선 필요)
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
    public void AddRenown(int num)
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
        throw new DataException("Invalid work code");
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
        if (currentWork is null)
        {
            throw new DataException("Invalid work code");
        }

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
}
