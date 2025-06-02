using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

[Serializable]
public class DailyData
{
    /**
    * 하루 루틴동안의 인게임 데이터
    *   - 날짜
    *   - 각 시간대
    *   플레이 정보
        - 시작 위치
        - 시작 시간대
        - 업무 데이터   
            업무 코드
            스테이지 번호
    */
    
    // 하루 날짜 정보
    public Date date;   // 날짜 

    // 게임 플레이 정보
    public WorldVector startLocation;

    // 업무 정보
    public List<Work> workList = new();

    // 날짜 데이터
    public TimeData[] dayTimes = new TimeData[4];
}


[Serializable]
public class TimeData
{
    /**
    * 1개 시간대 인게임 데이터
    *   시간대
    *   인게임 정보
        - NPC
        - action
        - block
        - bgm 변경
    */
    
    public DayTime daytime;
    public bool isNight;
    
    public List<ChatObjectData> npc = new();
    public List<ActionObjectData> action = new();
    public List<WorldVector> block = new();
    public List<BGMData> bgm = new();
}

[Serializable]
public struct Date
{
    /**
    * 날짜 정보   
    */
    public int year;
    public int month;
    public int day;

    public Date(int _year, int _month, int _day)
    {
        year = _year;
        month = _month;
        day = _day;
    }
}

[Serializable]
public struct DayTime
{
    /**
    * 시간 정보
    */
    public string hour;
    public string minute;

    public DayTime(string _hour, string _minute)
    {
        hour = _hour;
        minute = _minute;
    }

    public override string ToString()
    {
        return $"{hour}:{minute}";
    }
}

[Serializable]
public class Work
{
    /**
    * 업무 정보
    */
    public string code;     // 업무 코드명
    public int stage;       // 스테이지 번호
    public bool isClear;
    public Work(string _code, int _stage = 0)
    {
        code = _code;
        stage = _stage;
        isClear = false;
    }
} 
