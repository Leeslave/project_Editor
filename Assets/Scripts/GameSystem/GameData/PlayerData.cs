using System.Collections.Generic;
using System.Linq;
using Utility;

[System.Serializable]
public class DaySave
{
    public string thumbnail;
    public int renown;
}


[System.Serializable]
public class PlayerData
{
    /**
    * 플레이어 세이브 데이터 클래스
    *   - 날짜별 세이브 정보
    *   - 최신 날짜까지 적용
    */
    
    // 세이브 목록
    private List<DaySave> saveList = new();
    
    public DaySave this[int index]
    {
        get => saveList[index];
        set => saveList[index] = value;
    }
    
    public int Count => saveList.Count;


    /// <summary>
    /// 세이브 데이터 추가
    /// </summary>
    /// <param name="data">추가할 데이터</param>
    /// <param name="idx">추가할 날짜 인덱스 (default: 마지막 날 추가)</param>
    public void Save(DaySave data, int idx = -1)
    {
        // 마지막날 추가
        if (idx < 0 || idx == saveList.Count)
        {
            saveList.Add(data);
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
            EditorLogger.LogError("Invalid save Index");
        }
    }
}