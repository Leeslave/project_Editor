using System;
using System.Collections.Generic;

[Serializable]
public class DailyData
{
    /**
    * 하루 루틴동안의 인게임 데이터
    *   - 날짜 정보
    *   - 업무 데이터   
            업무 코드명
            스테이지 번호
            클리어 여부 
    */

    public Date date;   // 날짜 정보    

    public List<Work> workData;    // 업무 정보

    public List<NPCSchedule>[] npcScheduleList; // NPC 변경점 리스트

    /// DailyData를 DailyWrapper로 Wrapping
    public DailyWrapper WrapDailyData()
    {
        DailyWrapper resultWrapper = new DailyWrapper();

        // 날짜 할당
        resultWrapper.date = date;

        // 업무 키, 데이터 리스트
        foreach (var elem in workData)
        {
            resultWrapper.workList.Add(elem.code);
            resultWrapper.workStageList.Add(elem.stage);
        }

        resultWrapper.t0Schedule = npcScheduleList[0];
        resultWrapper.t1Schedule = npcScheduleList[1];
        resultWrapper.t2Schedule = npcScheduleList[2];
        resultWrapper.t3Schedule = npcScheduleList[3];

        return resultWrapper;  
    }

    /// DailyWrapper를 DailyData로 UnWrapping
    public DailyData(DailyWrapper wrapper)
    {
        // 날짜 할당
        date= wrapper.date;

        // 업무 키, 데이터 리스트로 업무 생성
        workData = new List<Work>();
        for(int i = 0; i < wrapper.workList.Count; i++)
        {
            workData.Add(new Work(wrapper.workList[i], wrapper.workStageList[i]));
        }


        npcScheduleList = new List<NPCSchedule>[4];
        for(int i = 0; i<4; i++)
        {
            npcScheduleList[i] = null;
        }

        npcScheduleList[0] = wrapper.t0Schedule;
        npcScheduleList[1] = wrapper.t1Schedule;
        npcScheduleList[2] = wrapper.t2Schedule;
        npcScheduleList[3] = wrapper.t3Schedule;
    }
    
}

[Serializable]
public class DailyWrapper
{ 
    /** 
    * 하루 데이터 Wrapper
        저장되는 데이터 Wrap
    */
    public Date date;
    public List<string> workList = null;
    public List<int> workStageList = null;
    public List<NPCSchedule> t0Schedule = new();
    public List<NPCSchedule> t1Schedule = new();
    public List<NPCSchedule> t2Schedule = new();
    public List<NPCSchedule> t3Schedule = new();
}

[Serializable]
public class Date
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

public class Work
{
    /**
    * 업무 정보
    */
    public string code;     // 업무 코드명
    public int stage;       // 스테이지 번호
    public bool isClear;    // 클리어 여부
    public Work(string _code, int _stage = 0)
    {
        code = _code;
        stage = _stage;
        isClear = false;
    }
} 

[Serializable]
public class NPCSchedule
{
    public string image = null;
    public string chat = null;
    public ChatTriggerType? chatType = null;
    public World? location = null;
    public int? position = null;
    public float? x = null;
    public float? y = null;

    public enum ScheduleType
    {
        create,     // 새로 생성
        remove,     // 삭제
        change,     // 변경
    }
    public ScheduleType type;
}

[Serializable]
public enum ChatTriggerType {   
    OnClick,    // 버튼 누를 시
    OnStart,    // 활성화시 자동 1회
    EveryStart, // 매번 활성화마다
}