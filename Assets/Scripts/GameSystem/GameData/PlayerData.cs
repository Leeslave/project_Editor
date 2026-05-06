using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Utility;

[System.Serializable]
[JsonObject]
public class DaySave
{
    public int dayID;       // Day * 100 + branch
    public string thumbnail;
    public int renown;

    [JsonIgnore]
    public int Day => dayID / 100;
    [JsonIgnore]
    public int Branch => dayID % 100;
    
    public DaySave Clone(int newID = -1)
    {
        if (newID == -1) newID = dayID;
        
        return new DaySave { dayID = newID, thumbnail = thumbnail, renown = renown };
    }
}


[System.Serializable]
[JsonObject]
public class PlayerData : IEnumerable<DaySave>
{
    /**
    * 플레이어 세이브 데이터 클래스
    *   - 날짜별 세이브 정보
    *   - 최신 날짜까지 적용
    */
    
    // 세이브 목록
    [JsonProperty]
    private List<DaySave> _saveList = new();
    public DaySave this[int dayID] => _saveList.FirstOrDefault(s => s.dayID == dayID);
    
    [JsonIgnore]
    public int Count => _saveList.Count;
    [JsonIgnore]
    public int DayCount => _saveList[^1].dayID / 100;       // 저장된 마지막 날짜 (정렬)

    /// <summary>
    /// 첫 날짜 생성
    /// </summary>
    public void Init()
    {
        _saveList.Add(new DaySave());
    }

    /// <summary> 
    /// 세이브 데이터 추가
    /// </summary>
    /// <param name="data">추가할 데이터</param>
    public void Save(DaySave data)
    {
        // 1. 동일 ID 존재시 대체
        // 2. 마지막날 기준 +1일 이상 차이(100 이상) 날 시 경고, 세이브 안함
        // 3. 동일 날짜+해당 분기 없을 시 분기 순으로 삽입
        var oldSave = _saveList.FirstOrDefault(x => x.dayID == data.dayID);
        
        // Already Exist Save
        if (oldSave != null)
        {
            oldSave.renown =  data.renown;
            return;
        }
        
        // New Save
        int lastDay = _saveList.Last().dayID % 100;
        if (data.dayID % 100 > lastDay + 1)
        {
            // Day Error
            EditorLogger.LogWarning($"Invalid Day Index {data.dayID}, Last Save : {lastDay}");
        }
        
        _saveList.Add(data);
        // Sort
        _saveList = _saveList.OrderBy(x => x.dayID).ToList();
    }

    public IEnumerator<DaySave> GetEnumerator()
    {
        return _saveList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
