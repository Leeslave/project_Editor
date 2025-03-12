using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;


public static class DataLoader
{
    /*****
    * 게임 데이터 저장, 로드 시스템
        - Json 파싱으로 게임 데이터 로드
        - Json 파싱으로 플레이어 데이터 저장, 로드
    */
    private static string GAMEDATAPATH = Application.dataPath + "/Resources/GameData/Main/";   // 게임 데이터 파일 경로
    private static string GAMEFILE = "dailyData";
    private static string SAVEPATH = Application.dataPath + "/Resources/Save/savedata.json";    // 세이브 파일 경로
    private static string CHATPATH = Application.dataPath + "/Resources/Chat/Text/";     // 대화 파일 경로
    

    /// <summary>
    /// 경로에서 대화 데이터 불러오기
    /// </summary>
    /// <param name="fileName">파일명</param>
    /// <returns>대화 데이터 반환</returns>
    public static List<Paragraph> GetChatData(string fileName)
    {
        FileStream fs = new FileStream(CHATPATH + fileName, FileMode.Open);
        byte[] buffer = new byte[fs.Length];
        fs.Read(buffer, 0, (int)fs.Length);
        fs.Close();
        string jsonText = Encoding.UTF8.GetString(buffer);
        
        Dialogue wrapper = JsonConvert.DeserializeObject<Dialogue>(jsonText, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
        return wrapper.chatList;
    }


    /// JSON으로부터 게임 데이터를 로드
    public static DailyData LoadGameData(int index)
    {
        string gameFile = $"{GAMEDATAPATH}/{GAMEFILE}{index}.json";

        // 파일 읽어오기
        if (!File.Exists(gameFile))
        {
            throw new ArgumentException($"GAME DATA CANNOT FOUND : ${GAMEFILE}{index}");
            // 치명적 오류, 게임 종료시키기
        }
        FileStream fileStream = new FileStream(gameFile, FileMode.Open);
        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();

        // jsonString 읽어오기
        string jsonText = Encoding.UTF8.GetString(data);

        //Wrapper로 파싱
        return JsonConvert.DeserializeObject<DailyData>(jsonText, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });
    }

    
    /// 플레이어 데이터 JSON에서 로드
    public static List<SaveData> LoadPlayerData()
    {
        // 파일 읽어오기
        if (!File.Exists(SAVEPATH))
        {
            throw new ArgumentException($"SAVE DATA CANNOT FOUND : ${SAVEPATH}");
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