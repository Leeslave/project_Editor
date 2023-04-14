[System.Serializable]
public class PlayerData
{
    /**
    * 플레이어 데이터 클래스
    *   데이터 저장을 위한 직렬화 
    *   - 게임데이터 인덱스
    *   - 날짜 데이터 (YYYY:DD:MM - Time)
    *   - 위치 데이터
    *   - 현재 명성치
    */
    public int index;
    public Date date;
    public int time;
    public string location;
    public int renown;
}