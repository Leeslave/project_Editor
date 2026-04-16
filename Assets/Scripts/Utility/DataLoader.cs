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
        private static readonly string CHATPATH = Path.Combine(Application.streamingAssetsPath, "ChatData");
        private static readonly string GAMEDATAPATH = Path.Combine(Application.streamingAssetsPath, "DayData"); // 게임 데이터 파일 경로
        private static readonly string GAMEFILE = "dailyData";
        public static readonly string Savepath = Path.Combine(Application.persistentDataPath, "/Save/savedata.json");    // 세이브 파일 경로

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
                string relativePath = file.Substring(path.Length); // 파일에서 path 부분을 제거
                fileNames.Add(relativePath);
            }

            fileNames.Sort();
            return fileNames;
        }
        
        /// 게임 데이터파일 저장하기
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

        /// <summary>
        /// 날짜 데이터 파일 로드
        /// </summary>
        /// <param name="index">해당하는 데이터 인덱스</param>
        /// <returns>날짜 데이터</returns>
        /// <exception cref="ArgumentException">해당하는 파일 없을 시 예외 발생</exception>
        public static DailyData GetDayData(int index)
        {
            var filePath = Path.Combine(GAMEDATAPATH, $"{GAMEFILE}{index}.json");

            return GetData<DailyData>(filePath);
        }

        /// <summary>
        /// Load Json Data
        /// </summary>
        /// <param name="path">해당하는 데이터 파일명</param>
        /// <returns>해당 데이터</returns>
        public static T GetData<T>(string path)
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
