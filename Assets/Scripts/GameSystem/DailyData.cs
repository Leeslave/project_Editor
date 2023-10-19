using System;
using System.Collections.Generic;

public class DailyData
{
    /**
    * 하루 루틴동안의 인게임 데이터
    *   - 날짜 정보
    *   - 업무 - 스테이지번호 데이터    
    */

    public Date date;   // 날짜 정보     
    public Dictionary<Tuple<string, int>, bool> workData;    // 업무 정보 <업무명, 스테이지>

    /// DailyData를 DailyWrapper로 Wrapping
    public DailyWrapper WrapDailyData()
    {
        DailyWrapper resultWrapper = new DailyWrapper();

        // 날짜 할당
        resultWrapper.date = date;

        // 업무 키, 데이터 리스트
        foreach (var elem in workData)
        {
            resultWrapper.workList.Add(elem.Key.Item1);
            resultWrapper.workStageList.Add(elem.Key.Item2);
        }

        return resultWrapper;  
    }

    /// DailyWrapper를 DailyData로 UnWrapping
    public DailyData(DailyWrapper wrapper)
    {
        // 날짜 할당
        date= wrapper.date;

        // 업무 키, 데이터 리스트로 딕셔너리 생성
        workData = new Dictionary<Tuple<string, int>, bool>();
        for(int i = 0; i < wrapper.workList.Count; i++)
        {
            workData.Add(new Tuple<string, int>(wrapper.workList[i], wrapper.workStageList[i]), false);
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
    * 날짜 정보를 저장  
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

