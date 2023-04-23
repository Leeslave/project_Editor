using System.Collections.Generic;

public class DailyData
{
    /**
    * 하루 루틴동안의 게임 데이터
    * json 로드를 위한 직렬화
    *   - 날짜 정보
    *   - 업무 - 스테이지번호 데이터    
    *   
    * 
    */

    public Date date;   // 날짜 정보     
    public Dictionary<string, int> workData;    // 업무 정보 <업무명, 스테이지>
    
}

[System.Serializable]
class DailyWrapper
{ 
    /** 하루 데이터 Wrapper
    */
    public Date date;
    public List<string> workDataKey;
    public List<int> workDataValue;
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

