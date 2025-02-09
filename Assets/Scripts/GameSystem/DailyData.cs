using System;
using System.Collections.Generic;

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
    
    /// 하루 날짜 정보
    public Date date;   // 날짜 

    /// 게임 플레이 정보
    public WorldVector startLocation;

    /// 업무 정보
    public List<Work> workList = new();

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
        - block
        - bgm 변경
    */
    
    public DayTime daytime;

    public bool isNight;
    
    public List<NPCData> npcList = new();
    public List<WorldVector> blockList = new();
    public List<BGMData> bgmList = new();
}


[Serializable]
public class Date
{
    /**
    * 날짜 정보  
    */
    public uint year;
    public uint month;
    public uint day;

    public Date(uint _year, uint _month, uint _day)
    {
        year = _year;
        month = _month;
        day = _day;
    }
}

[Serializable]
public class DayTime
{
    /**
    * 시간 정보
    */
    public uint hour;
    public uint minute;

    public DayTime(uint _hour = 0, uint _minute = 0)
    {
        hour = _hour;
        minute = _minute;
    }

    public override string ToString()
    {
        return $"{hour:D2}:{minute:D2}";
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
    }
} 