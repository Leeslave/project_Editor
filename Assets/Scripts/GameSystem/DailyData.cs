using System;
using System.Collections.Generic;

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
    }
    
}

[System.Serializable]
public class DailyWrapper
{ 
    /** 
    * 하루 데이터 Wrapper
        저장되는 데이터 Wrap
    */
    public Date date;
    public List<string> workList;
    public List<int> workStageList;
}

[System.Serializable]
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

