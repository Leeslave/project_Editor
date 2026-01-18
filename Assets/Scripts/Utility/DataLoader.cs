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
    private static readonly string CHATPATH = Path.Combine(Application.streamingAssetsPath, "ChatData");
    private static readonly string GAMEDATAPATH = Path.Combine(Application.streamingAssetsPath, "DayData");   // 게임 데이터 파일 경로
    private static readonly string GAMEFILE = "dailyData";

    private static List<SaveData> saveData;
    
    // 세이브 파일 구분
    #if RELEASE
    private static string SAVEPATH = Application.persistentDataPath + "/Save/savedata.json";    // 세이브 파일 경로
    #endif
    #if DEBUG
    private static string SAVEPATH = Path.Combine(Application.streamingAssetsPath, "Save", "SaveData.json");    // 세이브 파일 경로
    #endif
    
    
    #region FileManage
    
    /// 파일 목록 불러오기
    public static List<string> GetFileNames(string path, string type = "*.json")
    {
        // 폴더 상의 게임 데이터 로드
        if (!Directory.Exists(path))
        {
            Debug.Log($"폴더 경로 오류 : {path}");
        }
        
        List<string> fileNames = new();
        string[] files;

        files = Directory.GetFiles(path, type, SearchOption.AllDirectories); // .json 파일만 검색
        foreach (string file in files)
        {
            string relativePath = file.Substring(path.Length); // 파일에서 path 부분을 제거
            fileNames.Add(relativePath);
        }
        
        fileNames.Sort();
        return fileNames;
    }
    
    #endregion
    
    #region DailyData
    
    #if DEBUG
    /// 게임 데이터파일 저장하기 : DEBUG
    public static void SaveGameData(string path, DailyData data)
    {
        // json String으로 파싱
        string jsonText = JsonConvert.SerializeObject(data, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

        FileStream fileStream = new(path, FileMode.Create, FileAccess.Write);
        byte[] bytes = Encoding.UTF8.GetBytes(jsonText);
        fileStream.Write(bytes, 0, bytes.Length);
        fileStream.Close();
    }
    #endif
    
    /// <summary>
    /// 날짜 데이터 파일 로드
    /// </summary>
    /// <param name="index">해당하는 데이터 인덱스</param>
    /// <returns>날짜 데이터</returns>
    /// <exception cref="ArgumentException">해당하는 파일 없을 시 예외 발생</exception>
    public static DailyData GetDayData(int index)
    {
        var gameFile = Path.Combine(GAMEDATAPATH, $"{GAMEFILE}{index}.json");

        return GetDayData(gameFile);
    }
    
    /// <summary>
    /// 날짜 데이터 파일 로드
    /// </summary>
    /// <param name="gameFile">해당하는 데이터 파일명</param>
    /// <returns>날짜 데이터</returns>
    /// <exception cref="ArgumentException">해당하는 파일 없을 시 예외 발생</exception>
    public static DailyData GetDayData(string gameFile)
    {
        // 파일 읽어오기
        if (!File.Exists(gameFile))
        {
            throw new ArgumentException($"GAME DATA CANNOT FOUND : ${gameFile}");
            // TODO: 치명적 오류, 게임 종료시키기 (게임데이터 검사 추가)
        }
        FileStream fileStream = new(gameFile, FileMode.Open);
        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();

        // jsonString 읽어오기
        string jsonText = Encoding.UTF8.GetString(data);

        //Wrapper로 파싱
        return JsonConvert.DeserializeObject<DailyData>(jsonText
            ,new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,            // 타입 구분
            });
    }
    
    #endregion
    
    #region ChatData
    
    #if DEBUG
    /// 대사 파일 저장하기
    public static void SaveChatData(string path, List<Paragraph> data)
    {
        Dialogue dialogue = new(){ chatList = data };
        // json String으로 파싱
        string jsonText = JsonConvert.SerializeObject(dialogue, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

        FileStream fileStream = new(path, FileMode.Create, FileAccess.Write);
        byte[] bytes = Encoding.UTF8.GetBytes(jsonText);
        fileStream.Write(bytes, 0, bytes.Length);
        fileStream.Close();
    }
    #endif
    
    /// 파일명으로 대화 데이터를 로드
    public static List<Paragraph> GetChatData(string fileName)
    {
        var path = Path.Combine(CHATPATH, fileName);
        FileStream fs = new(path, FileMode.Open);
        byte[] buffer = new byte[fs.Length];
        fs.Read(buffer, 0, (int)fs.Length);
        fs.Close();
        string jsonText = Encoding.UTF8.GetString(buffer);
        
        Dialogue wrapper = JsonConvert.DeserializeObject<Dialogue>(jsonText,
            new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,            // 타입 구분
            });
        return wrapper.chatList;
    }
    
    #endregion

    #region PlayerData  
    
    public static SaveData GetPlayerData(int index)
    {
        if (index < 0 || index >= saveData.Count)
        {
            return null;
        }

        return saveData[index];
    }
    
    /// 플레이어 세이브 데이터를 로드
    public static List<SaveData> InitData()
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
        SaveWrapper wrapper = JsonConvert.DeserializeObject<SaveWrapper>(jsonText
            ,new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,            // 타입 구분
            });
        return wrapper.list;
    }

    public static void PushPlayerData(SaveData save, uint index)
    {
        
    }
    
    /// 플레이어 데이터 JSON 저장
    public static void SaveData()
    {
        SaveWrapper wrapper = new();
        foreach (var iter in saveData)
        {
            wrapper.list.Add(iter);
        }

        // json String으로 파싱
        string jsonText = JsonConvert.SerializeObject(wrapper,   new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

        FileStream fileStream = new(SAVEPATH, FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(jsonText);
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();
    }
    
    #endregion
}