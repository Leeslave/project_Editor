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

    [Header("게임 내 데이터")]
    public int dateIndex = 0;   // 날짜 인덱스
    private List<SaveData> saveList = new();    // 날짜별 저장 데이터
    private List<DailyData> dailyList = new();     // 날짜별 데이터 필드

    [Space(10)]
    public int time = 0;    // 현재 시간
    public World location;  // 현재 지역
    public int position;    // 현재 위치
    public bool isScreenOn = false; // 스크린 활성화 여부
    
    [SerializeField]
    public SaveData player { get { return saveList[dateIndex]; } }      // 세이브 데이터 필드
    [SerializeField] 
    public DailyData today { get { return dailyList[dateIndex]; } }    // 오늘 날짜 데이터 필드
    
    
    
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

            saveList.Add(new SaveData());
            saveList = GameLoader.LoadSaveData();     // 디버깅 : 세이브 데이터 로드
            SetDate(0);
            dailyList = GameLoader.LoadGameData();     // 게임 데이터 로드   

            for(int i = 0; i< 3; i++)
            {
                Debug.Log($"{i}시간대 변경점 로그");
                foreach(var iter in today.npcScheduleList[i])
                {
                    if (iter == null)
                    {
                        Debug.Log("비어있음");
                        continue;
                    }
                    Debug.Log($"{iter.chat} : {iter.type}");
                } 
            }
                    
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
    public void SetDate(int date = -1)
    {
        if (date < 0 || date > 3)
        {
            // 다음 날짜로 이동
            date = dateIndex + 1;   
        }

        // 해당 날짜 불러오기
        dateIndex = date;
        SetTime(0);
        location = player.startLocation;
        position = player.startPosition;
        

        // 메인 월드는 재로드
        if (SceneManager.GetActiveScene().name == "MainWorld")
            SceneManager.LoadScene("MainWorld");
    }

    ///<summary>
    /// 시간 전환
    ///</summary>
    ///<param name="time">전환할 시간(없으면 다음 시간대로, 마지막 시간대면 다음 날짜로)</param>
    public void SetTime(int nextTime = -1)
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
                SetDate();
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
        // 현재 씬 이름으로 코드 불러오기
        string workCode = SceneManager.GetActiveScene().name;
        if (workCode == null || workCode == "")
            return;

        // 코드에 해당하는 업무 불러오기
        Work currentWork = null;
        foreach(var work in today.workData)
        {
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

        // 업무 완료로 전환
        currentWork.isClear = true;
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
        private static readonly string playerSavePath = "/Resources/Save/debugsave_1.json";    // 세이브 파일 경로
        [SerializeField]
        private static readonly string dailySavePath = "/Resources/GameData/Main/dailyData.json";   // 게임 데이터 파일 경로

        [Serializable]
        class GameDataWrapper { public List<DailyWrapper> dailyDataList = new(); }     // JsonUtility용 DailyData들 Wrapper
    
        /// JSON으로부터 게임 데이터를 로드
        public static List<DailyData> LoadGameData()
        {
            // daily 초기화
            List<DailyData> result = new();

            // 파일 읽어오기
            FileStream fileStream = new FileStream(Application.dataPath + dailySavePath, FileMode.Open);
            byte[] data = new byte[fileStream.Length];
            fileStream.Read(data, 0, data.Length);
            fileStream.Close();

            // jsonString 읽어오기
            string jsonObjectData = Encoding.UTF8.GetString(data);            

            //Wrapper로 파싱
            GameDataWrapper wrapper = JsonUtility.FromJson<GameDataWrapper>(jsonObjectData);

            // Wrapper를 DailyData로 전환
            foreach(DailyWrapper element in wrapper.dailyDataList)
            { 
                result.Add(new DailyData(element));
            }

            return result;
        }

        /// 플레이어 데이터 JSON에서 로드
        public static List<SaveData> LoadSaveData()
        { 
            // 파일 읽어오기
            FileStream fileStream = new FileStream(Application.dataPath + playerSavePath, FileMode.Open);
            byte[] data = new byte[fileStream.Length];
            fileStream.Read(data, 0, data.Length);
            fileStream.Close();

            // jsonString 읽어오기
            string jsonString = Encoding.UTF8.GetString(data);

            // Wrapper로 파싱
            SaveWrapper wrapper = JsonUtility.FromJson<SaveWrapper>(jsonString);

            return wrapper.data;
        }

        /// 플레이어 데이터 JSON 저장
        public static void SavePlayerData(List<SaveData> saveList)
        {
            // json String으로 파싱
            string jsonObjectData = JsonUtility.ToJson(saveList);
            
            FileStream fileStream = new FileStream(Application.dataPath + playerSavePath + playerSavePath, FileMode.Create);
            byte[] data = Encoding.UTF8.GetBytes(jsonObjectData);
            fileStream.Write(data, 0, data.Length);
            fileStream.Close();
        }
    }