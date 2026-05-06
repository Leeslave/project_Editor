using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEngine;
using Utility;

namespace Utility
{
    public static class DataLoader
    {
        /*****
        * 게임 데이터 저장, 로드 시스템
            - PlayerSave 로드 및 저장 : json
            - DailyData 로드 : json
            - ChatData 로드 : json
            - 설정파일 로드 및 저장 : json
            - 미니게임 Data 로드 : SO
        */
        
        [Header("File Path")]
        private static readonly string CHATPATH = Path.Join(Application.streamingAssetsPath, "ChatData");
        private static readonly string GAMEDATAPATH = Path.Join(Application.streamingAssetsPath, "DayData"); // 게임 데이터 파일 경로
        private static readonly string GAMEFILE = "dailyData";
        private static readonly string Savepath = Path.Join(Application.persistentDataPath, "Save/savedata.json");    // 세이브 파일 경로

        /// 파일 목록 불러오기
        public static List<string> GetFileNames(string path, string type = "*.json")
        {
            // 폴더 상의 게임 데이터 로드
            if (!Directory.Exists(path))
            {
                EditorLogger.Log($"폴더 경로 오류 : {path}");
            }

            List<string> fileNames = new();
            string[] files = Directory.GetFiles(path, type, SearchOption.AllDirectories); // .json 파일만 검색
            foreach (string file in files)
            {
                string relativePath = Path.GetRelativePath(path, file);
                fileNames.Add(relativePath);
            }

            fileNames.Sort();
            return fileNames;
        }
        
        /// <summary>
        /// 게임 데이터파일 저장
        /// </summary>
        /// <param name="path">저장 경로</param>
        /// <param name="data">데이터</param>
        public static void SaveData<T>(string path, T data)
        {
            // json String으로 파싱
            string jsonText = JsonConvert.SerializeObject(data,
                new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All });

            FileStream fileStream = new(path, FileMode.Create, FileAccess.Write);
            byte[] bytes = Encoding.UTF8.GetBytes(jsonText);
            fileStream.Write(bytes, 0, bytes.Length);
            fileStream.Close();
        }
        
        // Dynamic
        public static void SavePlayerData(PlayerData data) => SaveData(Savepath, data);
        
        public static DailyData GetDayData(int index) =>
            GetData<DailyData>(Path.Join(GAMEDATAPATH, $"{GAMEFILE}{index}.json"));
        public static DailyData GetDayData(string name) =>
            GetData<DailyData>(Path.Join(GAMEDATAPATH, name));
        
        public static Dialogue GetChatData(string name) =>
            GetData<Dialogue>(Path.Join(CHATPATH, name));
        
        public static PlayerData GetPlayerData() =>
            GetData<PlayerData>(Savepath);

        /// <summary>
        /// Load Json Data
        /// </summary>
        /// <param name="path">해당하는 데이터 파일명</param>
        /// <returns>해당 데이터</returns>
        private static T GetData<T>(string path)
        {
            // 파일 읽어오기
            if (!File.Exists(path))
            {
                EditorLogger.LogWarning($"No File Found : {path}");
                return default;
            }

            FileStream fileStream = new(path, FileMode.Open);
            byte[] data = new byte[fileStream.Length];
            fileStream.Read(data, 0, data.Length);
            fileStream.Close();

            // jsonString 읽어오기
            string jsonText = Encoding.UTF8.GetString(data);

            //Wrapper로 파싱
            return JsonConvert.DeserializeObject<T>(jsonText
                , new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All, // 타입 구분
                });
        }
    }
}
