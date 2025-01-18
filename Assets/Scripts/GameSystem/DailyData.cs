using System;
using System.Collections.Generic;

[Serializable]
public class DailyWrapper
{ 
    public Date date;
    public DayTime[] dateTimes = new[]
    {
        new DayTime(6, 30),
        new DayTime(9, 0),
        new DayTime(17, 0),
        new DayTime(19, 30)
    };

    public string startLocation = "Street";
    public int startPosition = 0;


    public List<Work> workList = new();
    public List<NPCData> objList = new();
}

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

    public DayTime[] dateTimes = new DayTime[4];      // 각 시간대


    /// 게임 플레이 정보
    public World startLocation;     // 시작 장소

    public int startPosition;   //시작 위치


    /// 업무 정보
    public Dictionary<Work, bool> workList = new();

    
    /// Wrapper에서 생성자
    public DailyData(DailyWrapper wrapper)
    {
        // 날짜, 시간대
        date = wrapper.date;
        dateTimes = wrapper.dateTimes;

        // 시작 위치
        try
        {
            startPosition = wrapper.startPosition;
            startLocation = Enum.Parse<World>(wrapper.startLocation);   // string을 World로 할당
        }
        catch(ArgumentException)
        {
            // 시작 위치값 오류 시 예외처리
            startLocation = World.Street;
        }

        // 업무
        foreach(var work in wrapper.workList)
        {
            workList.Add(work, false);
        }
    }
    
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
    
    public List<NPCData> npcList = new();
    public List<BlockData> blockList = new();
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
    public Work(string _code, int _stage = 0)
    {
        code = _code;
        stage = _stage;
    }
} 