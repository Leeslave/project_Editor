[System.Serializable]
public class SaveData
{
    /**
    * 플레이어 세이브 데이터 클래스
    *   데이터 저장을 위한 직렬화 
    *   - 날짜 인덱스
    *   - 위치 데이터
    *   - 현재 명성치
    */
    public int dateIndex;
    public int time;
    public string location;
    public int renown;

    public SaveData()
    {
        dateIndex = 0;
        time = 0;
        location = "Office";
        renown = 0;
    }
}