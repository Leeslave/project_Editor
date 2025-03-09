using System.Collections.Generic;
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
    public SaveData Player => saveList[dateIndex];      // 오늘 세이브 데이터
    public DailyData DayData { get; private set; }    // 오늘 날짜 데이터

    
    /// 게임 플레이 데이터 (ReadWrite)
    public int dateIndex;

    public int timeIndex;
    
    public WorldVector currentLocation;
    
    public bool isScreenOn; 
    
    public bool IsTaskClear   // 모든 업무 완료 플래그
    {
        get { 
            bool workResult = true;
            foreach(var workStatus in DayData.workList)
            {
                workResult &= workStatus.isClear;
            }
            return workResult;
        }
    }


    /// <summary>
    /// 게임 첫 로드 시
    /// </summary>
    /// <remarks>세이브/게임 데이터 로드</remarks>
    new void Awake()
    {
        base.Awake();

        saveList = DataLoader.LoadPlayerData();     // 세이브 데이터 로드
        DayData = DataLoader.LoadGameData(dateIndex);     // 게임 데이터 로드  
        
        // TODO: 날짜 선택 기능 구현 (임시로 0일차부터 로드)
        SetDate(0);
    }
    

    ///<summary>
    /// 날짜 전환 (게임 저장)
    ///</summary>
    ///<param name="date">전환할 날짜 인덱스(없으면 다음 날짜로), 시간은 무조건 아침</param>
    public void SetDate(int date = -1)
    {
        // 다음 날짜로 이동시
        if (date == -1)
        {
            date = dateIndex + 1;
        }
        
        // TODO: 이전 날짜 저장
        // DataLoader.SavePlayerData(saveList);

        // 해당 날짜 월드 로드
        // TODO: 로딩씬 진입
        DayData = DataLoader.LoadGameData(date);
        dateIndex = date;
        
        currentLocation = DayData.startLocation;
        SetTime(0);
        isScreenOn = false;
        
        // TODO: 로딩씬 나오고 월드 활성화
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

        // TODO: 월드 리로드 (개선 필요: 현재 장소, 현재 위치를 유지하고 월드를 재로드)
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
        Player.renown += num;
    }
    

    /// <summary>
    /// 특정 업무의 오늘 스테이지 번호 반환
    /// </summary>
    /// <param name="workCode"></param>
    /// <returns></returns>
    public int GetTask(string workCode)
    {
        foreach(var work in DayData.workList)
        {
            if (work.code == workCode)
            {
                return work.stage;
            }
        }
        throw new KeyNotFoundException("Invalid work code");
    }


    /// <summary>
    /// 업무 완료 여부 설정
    /// </summary>
    /// <param name="workCode">설정할 업무의 코드명</param>
    /// <remarks>해당하는 업무를 완료로 전환</remarks>
    public void ClearTask(string workCode)
    {
        // 코드에 해당하는 업무 불러오기
        Work currentWork = null;
        foreach (var work in DayData.workList)
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
            throw new KeyNotFoundException("Invalid work code");
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
