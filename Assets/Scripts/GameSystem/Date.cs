[System.Serializable]
public class Date
{
    /**
    * 날짜 정보를 저장
    *   - 년도
    *   - 월
    *   - 일
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
