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

    [SerializeField]
    private string playerSavePath = "/Resources/Save/";    // 세이브 파일 경로
    [SerializeField]
    private string dailySavePath = "/Resources/GameData/Main/dailyData.json";   // 게임 데이터 파일 경로

    [SerializeField] 
    public SaveData player;      // 세이브 데이터 필드
    private List<DailyData> daily;     // 날짜별 데이터 필드
    [SerializeField] 
    public DailyData todayData { get { return daily[player.dateIndex]; } }    // 오늘 날짜 데이터 필드

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
            player = new SaveData();    // 플레이어 데이터 초기화
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
            player.dateIndex++;   
        }
        else 
        {
            // 특정 날짜로 이동
            player.dateIndex = date;
        }
        ChangeTime(0);
    }

    ///<summary>
    /// 시간 전환
    ///</summary>
    ///<param name="time">전환할 시간(없으면 다음 시간대로, 마지막 시간대면 다음 날짜로)</param>
    public void ChangeTime(int time = -1)
    {
        if (time >= 0 && time <= 4)
        {
            // 특정 시간대로 이동
            player.time = time;
        }
        else
        {
            player.time++;
            if (player.time > 4)
            {
                player.time = 0;
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
        Wrapper wrapper = JsonUtility.FromJson<Wrapper>(jsonObjectData);

        // Wrapper를 DailyData로 전환
        foreach(DailyWrapper element in wrapper.dailyDataList)
        { 
            daily.Add(new DailyData(element));
        }
    }

    /// <summary>
    /// 플레이어 데이터 JSON 저장
    /// </summary>
    /// <param name="SaveFileName">저장 경로 내 파일명</param>
    public void SavePlayerData(string saveFileName)
    {
        // json String으로 파싱
        string jsonObjectData = JsonUtility.ToJson(player);
        
        FileStream fileStream = new FileStream(Application.dataPath + playerSavePath + saveFileName + ".json", FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(jsonObjectData);
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();
    }

    /// <summary>
    /// 플레이어 데이터 JSON에서 로드
    /// </summary>
    /// <param name=SaveFileName>저장 경로 내 파일명</param>
    public void LoadPlayerData(string saveFileName)
    { 
        FileStream fileStream = new FileStream(Application.dataPath + playerSavePath + saveFileName + ".json", FileMode.Open);
        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();

        // SaveData로 파싱
        string jsonObjectData = Encoding.UTF8.GetString(data);
        player = JsonUtility.FromJson<SaveData>(jsonObjectData);
    }
}
