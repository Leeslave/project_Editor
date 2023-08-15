using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
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

    [Header("파일 로드 경로")]
    [SerializeField]
    private readonly string playerSavePath = "/Resources/Save/debugsave_1.json";    // 세이브 파일 경로
    [SerializeField]
    private readonly string dailySavePath = "/Resources/GameData/Main/dailyData.json";   // 게임 데이터 파일 경로

    [Header("게임 내 데이터")]
    private List<SaveData> saveList = new();
    private List<DailyData> daily = new();     // 날짜별 데이터 필드

    [Space(10)]
    public int dateIndex = 0;   // 날짜 인덱스
    public int time = 0;    // 현재 시간
    [SerializeField]
    public SaveData player { get { return saveList[dateIndex]; } }      // 세이브 데이터 필드
    [SerializeField] 
    public DailyData todayData { get { return daily[dateIndex]; } }    // 오늘 날짜 데이터 필드
    

    [Serializable]
    class GameDataWrapper { public List<DailyWrapper> dailyDataList = new(); }     // JsonUtility용 Wrapper
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
            saveList.Add(new SaveData());
            LoadSaveData();     // 디버깅 : 세이브 데이터 로드
            LoadGameData();     // 게임 데이터 로드
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    ///<summary>
    /// 날짜 전환
    ///</summary>
    ///<param name="dateIndex">전환할 날짜 인덱스(없으면 다음 날짜로), 시간은 무조건 아침</param>
    public void ChangeDate(int date = -1)
    {
        if (date < 0)
        {
            // 다음 날짜로 이동
            date = dateIndex + 1;   
        }

        // 해당 날짜 불러오기
        dateIndex = date;
        ChangeTime(0);

        // 메인 월드는 재로드
        if (SceneManager.GetActiveScene().name == "MainWorld")
            SceneManager.LoadScene("MainWorld");
    }

    ///<summary>
    /// 시간 전환
    ///</summary>
    ///<param name="time">전환할 시간(없으면 다음 시간대로, 마지막 시간대면 다음 날짜로)</param>
    public void ChangeTime(int nextTime = -1)
    {
        if (nextTime >= 0 && nextTime <= 4)
        {
            // 특정 시간대로 이동
            time = nextTime;
        }
        else
        {
            time++;
            if (time > 4)
            {
                time = 0;
                ChangeDate();
            }
        }
    }

    /// <summary>
    /// 업무 완료 여부 설정
    /// </summary>
    /// <param name="workCode">설정할 업무의 코드명</param>
    /// <param name="isClear">업무 완료 여부</param>
    public void ClearTask()
    {
        string workCode = SceneManager.GetActiveScene().name;
        if (workCode == null || workCode == "")
            return;
        Work currentWork = null;
        foreach(var work in todayData.workData)
        {
            if (work.code == workCode)
            {
                currentWork = work;
                break;
            }
        }
        if (currentWork == null)
        {
            Debug.Log("Work doesn't Match");
            return;
        }
        currentWork.isClear = true;
    }


    /*****
    * 게임 데이터 저장, 로드 시스템
        - Json 파싱으로 게임 데이터 로드
        - Json 파싱으로 플레이어 데이터 저장, 로드
    */

    [System.Serializable]
    class Wrapper { public List<DailyWrapper> dailyDataList = new List<DailyWrapper>(); }     // JsonUtility용 Wrapper
    
    /// JSON으로부터 게임 데이터를 로드
    private void LoadGameData()
    {
        FileStream fileStream = new FileStream(Application.dataPath + dailySavePath, FileMode.Open);
        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();

        // jsonString 읽어오기
        string jsonObjectData = Encoding.UTF8.GetString(data);

        // daily 초기화
        daily = new List<DailyData>();

        //Wrapper로 파싱
        GameDataWrapper wrapper = JsonUtility.FromJson<GameDataWrapper>(jsonObjectData);

        // Wrapper를 DailyData로 전환
        foreach(DailyWrapper element in wrapper.dailyDataList)
        { 
            daily.Add(new DailyData(element));
        }
    }

    /// 플레이어 데이터 JSON에서 로드
    private void LoadSaveData()
    { 
        FileStream fileStream = new FileStream(Application.dataPath + playerSavePath, FileMode.Open);
        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();

        // SaveData로 파싱
        string jsonObjectData = Encoding.UTF8.GetString(data);
        SaveWrapper wrapper = JsonUtility.FromJson<SaveWrapper>(jsonObjectData);

        saveList = wrapper.data;
    }

    /// 플레이어 데이터 JSON 저장
    public void SavePlayerData()
    {
        // json String으로 파싱
        string jsonObjectData = JsonUtility.ToJson(saveList);
        
        FileStream fileStream = new FileStream(Application.dataPath + playerSavePath + playerSavePath, FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(jsonObjectData);
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();
    }
}
