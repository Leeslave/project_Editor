using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    /**
    * 플레이어 데이터를 저장, 로드
    * initNewPlayerData
    * SavePlayerData
    * LoadPlayerData
    */
    public static string savefilePath = "/Resources/Save/";    // 세이브 파일 경로
    public static string worldfilePath = "Prefab/MainWorld/";  // 월드 프리팹 경로


    private class PlayerData
    {
        /**
        * 플레이어 데이터 클래스
        *   - 날짜 데이터 (YYYY:DD:MM - Time)
        *   - 위치 데이터
        *   - 현재 명성치
        */
        public int year;
        public int month;
        public int day;
        public int time;
        public string location;
        public int renown;
    }
    private static PlayerData playerData = new PlayerData();   // PlayerPrefs와 데이터 연동


    // 데이터 초기 설정
    public static void InitNewPlayerData()
    {   
        LoadPlayerData(savefilePath+"initial.json");
        asyncPlayerPrefs();
    }

    /// <summary>
    /// 플레이어 데이터 JSON 저장
    /// </summary>
    /// <param name="SaveFileName">저장 경로 내 파일명</param>
    public static void SavePlayerData(string saveFileName)
    {
        asyncPlayerData();

        string jsonObjectData = JsonUtility.ToJson(playerData);
        
        FileStream fileStream = new FileStream(Application.dataPath + savefilePath + saveFileName + ".json", FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(jsonObjectData);
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();
    }

    /// <summary>
    /// 플레이어 데이터 JSON 로드
    /// </summary>
    /// <param name=SaveFileName>저장 경로 내 파일명</param>
    public static void LoadPlayerData(string saveFileName)
    { 
        FileStream fileStream = new FileStream(Application.dataPath + savefilePath + saveFileName + ".json", FileMode.Open);
        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();

        string jsonObjectData = Encoding.UTF8.GetString(data);
        playerData = JsonUtility.FromJson<PlayerData>(jsonObjectData);

        asyncPlayerPrefs();
    }
    
    

    /// <summary>
    /// 플레이어의 위치와 활성화 월드 동기화
    /// </summary>
    /// <remarks>씬 내 WorldCanvas 객체 삭제 후 새로 생성</remarks>
    public static void asyncSceneData()
    {
        asyncPlayerData();

        var existWorld = GameObject.FindObjectOfType<WorldCanvas>();
        if (existWorld != null)
        {
            Destroy(existWorld.gameObject);
        }
        GameObject newWorldCanvas = Instantiate(Resources.Load<GameObject>(worldfilePath + playerData.location + "Canvas"));
        Debug.Log("New Worlc Loaded: " + newWorldCanvas.ToString());
    }

    
    /**
    * PlayerData -> PlayerPrefs 데이터 동기화
    */
    private static void asyncPlayerPrefs()
    {
        PlayerPrefs.SetInt("Year", playerData.year);
        PlayerPrefs.SetInt("Month", playerData.month);
        PlayerPrefs.SetInt("Day", playerData.day);
        PlayerPrefs.SetInt("Time", playerData.time);
        PlayerPrefs.SetString("Location", playerData.location);
        PlayerPrefs.SetInt("Renown", playerData.renown);
    }

    /**
    * PlayerPrefs -> PlayerData 데이터 동기화
    */
    private static void asyncPlayerData()
    {
        playerData.year = PlayerPrefs.GetInt("Year");
        playerData.month = PlayerPrefs.GetInt("Month");
        playerData.day = PlayerPrefs.GetInt("Day");
        playerData.time = PlayerPrefs.GetInt("Time");
        playerData.location = PlayerPrefs.GetString("Location");
        playerData.renown = PlayerPrefs.GetInt("Renown");
    }
}
