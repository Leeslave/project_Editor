using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class GameSystem : MonoBehaviour
{
    /**
    * 게임 내 데이터 관리 시스템
        플레이어 데이터를 저장, 로드
        게임 데이터를 로드, 관리
        날짜, 시간대, 진행상황 적용
    */ 

    /// 현재 플레이 시각
    public int date { get; private set; } = 0;   // 오늘 날짜 인덱스
    public int time { get; private set; }  = 0;   // 현재 시간

    ///  현재 플레이 위치
    public World location { get; private set; }  // 현재 지역
    public int position { get; private set; }    // 현재 위치

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

    /// 플레이 데이터 
    private List<SaveData> saveList = new();    // 저장 데이터
    private List<DailyData> dailyList = new();     // 날짜별 데이터

    public SaveData player { get { return saveList[date]; } }      // 오늘 세이브 데이터
    public DailyData today { get { return dailyList[date]; } }    // 오늘 날짜 데이터


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

            saveList = GameLoader.LoadSaveData();     // 세이브 데이터 로드
            dailyList = GameLoader.LoadGameData();     // 게임 데이터 로드  

            // 초기 데이터 설정 (로딩 씬 설정 후 삭제)
            SetDate(0);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    
    /// <summary>
    /// 지역 값 설정
    /// </summary>
    /// <param name="newLocation">설정할 새 지역</param>
    public void SetLocation(World newLocation)
    {
        location = newLocation;
    }


    /// <summary>
    /// 위치 값 설정
    /// </summary>
    /// <param name="newPos">설정할 새 위치</param>
    public void SetPosition(int newPos)
    {
        position = newPos;
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
            date = this.date + 1;
        }

        // 날짜 오류
        if (date > dailyList.Count || date < 0)
        {
            Debug.Log($"Day Out Of Range: {date}");
            return;
        }

        // 해당 날짜 불러오기
        this.date = date;
        SetTime(0);

        // 게임 저장 (튜토리얼 날짜 제외)
        if (date > 1)
        {
            GameLoader.SavePlayerData(saveList);
        }


        // TODO: 메인 월드 재로드, 로딩 씬으로 대체
        if (SceneManager.GetActiveScene().name == "MainWorld")
            SceneManager.LoadScene("DayLoading");
    }

    ///<summary>
    /// 다음 시간대로 전환
    ///</summary>
    ///<param name="time">전환할 시간(마지막 시간대면 다음 날짜로)</param>
    public void SetTime(int _time)
    {
        if (_time < 0 || _time >= 4)
            return;
        time = _time;
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

public static class GameLoader
{
    /*****
    * 게임 데이터 저장, 로드 시스템
        - Json 파싱으로 게임 데이터 로드
        - Json 파싱으로 플레이어 데이터 저장, 로드
    */
    [SerializeField]
    private static readonly string GAMEDATAPATH = Application.dataPath + "/Resources/GameData/Main/dailyData.json";   // 게임 데이터 파일 경로
    [SerializeField]
    private static readonly string SAVEPATH = Application.dataPath + "/Resources/Save/savedata.json";    // 세이브 파일 경로

    [Serializable]
    class GameDataWrapper { public List<DailyWrapper> days = new(); }     // JsonUtility용 DailyData들 Wrapper
    


    /// JSON으로부터 게임 데이터를 로드
    public static List<DailyData> LoadGameData()
    {
        // daily 초기화
        List<DailyData> result = new();

        // 파일 읽어오기
        if (!File.Exists(GAMEDATAPATH))
        {
            throw new Exception($"GAME DATA CANNOT FOUND : ${GAMEDATAPATH}");
            // 치명적 오류, 게임 종료시키기
        }
        FileStream fileStream = new FileStream(GAMEDATAPATH, FileMode.Open);
        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();

        // jsonString 읽어오기
        string jsonText = Encoding.UTF8.GetString(data);

        //Wrapper로 파싱
        GameDataWrapper wrapper = JsonConvert.DeserializeObject<GameDataWrapper>(jsonText, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

        // Wrapper를 DailyData로 전환
        foreach (DailyWrapper element in wrapper.days)
        {
            result.Add(new DailyData(element));
        }

        return result;
    }

    /// 플레이어 데이터 JSON에서 로드
    public static List<SaveData> LoadSaveData()
    {
        // 파일 읽어오기
        if (!File.Exists(SAVEPATH))
        {
            throw new Exception($"SAVE DATA CANNOT FOUND : ${SAVEPATH}");
            // 치명적 오류, 게임 종료시키기
        }
        FileStream fileStream = new FileStream(SAVEPATH, FileMode.Open);
        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();

        // jsonString 읽어오기
        string jsonText = Encoding.UTF8.GetString(data);

        // Wrapper로 파싱
        SaveWrapper wrapper = JsonConvert.DeserializeObject<SaveWrapper>(jsonText,  new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

        return wrapper.list;
    }

    /// 플레이어 데이터 JSON 저장
    public static void SavePlayerData(List<SaveData> saveList)
    {
        SaveWrapper wrapper = new();
        foreach (var iter in saveList)
        {
            wrapper.list.Add(iter);
        }

        // json String으로 파싱
        string jsonText = JsonConvert.SerializeObject(wrapper,   new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

        FileStream fileStream = new FileStream(SAVEPATH, FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(jsonText);
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();
    }
}