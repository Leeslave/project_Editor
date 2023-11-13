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

    private int resolutionX = 1200;
    private int resolutionY = 900;

    public int todayIndex { get; private set; }   // 오늘 날짜 인덱스
    public int currentTime { get; private set; }    // 현재 시간

    [Header("현재 게임 정보")]
    public World currentLocation;  // 현재 지역
    public int currentPosition;    // 현재 위치
    public bool isScreenOn = false; // 스크린 활성화 여부

    /// 게임 데이터 
    private List<SaveData> saveList = new();    // 저장 데이터
    private List<DailyData> dailyList = new();     // 날짜별 데이터

    [SerializeField]
    public SaveData player { get { return saveList[todayIndex]; } }      // 세이브 데이터
    [SerializeField]
    public DailyData today { get { return dailyList[todayIndex]; } }    // 오늘 날짜 데이터


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

            Screen.SetResolution(resolutionX, resolutionY, false);  // 해상도 고정

            saveList = GameLoader.LoadSaveData();     // 세이브 데이터 로드
            dailyList = GameLoader.LoadGameData();     // 게임 데이터 로드  

            // 초기 데이터 설정
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
        if (date > dailyList.Count)
        {
            Debug.Log($"Day Out Of Range: {date}");
            return;
        }
        if (date < 0)
        {
            // 다음 날짜로 이동
            date = todayIndex + 1;
        }

        // 해당 날짜 불러오기
        todayIndex = date;
        SetTime(0);

        // 게임 저장 (튜토리얼 날짜 제외)
        if (date > 1)
        {
            GameLoader.SavePlayerData(saveList);
        }

        // 위치 이동
        currentLocation = today.startLocation;
        currentPosition = today.startPosition;
        // 메인 월드 재로드
        if (SceneManager.GetActiveScene().name == "MainWorld")
            SceneManager.LoadScene("MainWorld");
    }

    ///<summary>
    /// 시간 전환
    ///</summary>
    ///<param name="time">전환할 시간(없으면 다음 시간대로, 마지막 시간대면 다음 날짜로)</param>
    public void SetTime(int nextTime = -1)
    {
        if (nextTime >= 0 && nextTime < 4)
        {
            // 특정 시간대로 이동
            currentTime = nextTime;
        }
        else
        {
            // 다음 시간대로 이동
            currentTime++;
            
            if (currentTime > 4)
            {
                // 다음 날짜로 이동
                currentTime = 0;
                SetDate();
            }
        }
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
    static public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// 해당 씬 비동기 로드
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    static public IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        while (!asyncLoad.isDone)
        {
            yield return null;
        }
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