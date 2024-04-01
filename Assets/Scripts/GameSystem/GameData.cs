

public class GameData
{
    /// 현재 플레이 시각
    public int date = 0;   // 오늘 날짜 인덱스
    public int time = 0;   // 현재 시간

    ///  현재 플레이 위치
    public World location { get; private set; }  // 현재 지역
    public int position { get; private set; }   // 현재 위치

    // 스크린 활성화 여부
    public bool isScreenOn = false; 


    /// <summary>
    /// 위치 값 설정
    /// </summary>
    /// <param name="newPos">설정할 새 위치</param>
    public void SetPosition(int newPos)
    {
        position = newPos;
    }


    /// <summary>
    /// 위치 값 설정
    /// </summary>
    /// <param name="newPos">설정할 새 위치</param>
    public void SetLocation(World newLocation)
    {
        location = newLocation;
    }
}