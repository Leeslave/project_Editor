using System.Collections.Generic;
using System.Linq;
using Utility;

[System.Serializable]
public struct DaySave
{
    public int renown;
}


[System.Serializable]
public class SaveData
{
    /**
    * 플레이어 세이브 데이터 클래스
    *   - 날짜별 세이브 정보
    *   - 최신 날짜까지 적용
    */
    public List<DaySave> saveList = new();

    /// <summary>
    /// 세이브 데이터 추가
    /// </summary>
    /// <param name="idx">추가할 날짜 인덱스</param>
    /// <param name="data">추가할 데이터</param>
    /// <remarks>해당 날짜 전까지 누락 시 마지막 데이터 기준으로 일괄 추가</remarks>
    public void Save(int idx, DaySave data)
    {
        if (idx < 0)
        {
            EditorLogger.LogWarning("Save Index Error");
            return;
        }
        
        // 특정 날짜 저장
        if (idx < saveList.Count)
        {
            saveList[idx] = data;
        }
        // 최신 날짜까지 저장
        else
        {
            var lastSave = saveList.LastOrDefault();
            while (saveList.Count <= idx)
            {
                saveList.Add(lastSave);
            }
            saveList.Add(data);
        }
    }
}