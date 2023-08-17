[System.Serializable]
public class SaveData
{
    /**
    * 플레이어 세이브 데이터 클래스
    *   - 위치 데이터
    *   - 현재 명성치
    */
    public readonly World startLocation;
    public readonly int startPosition;
    public int renown;

    public SaveData()
    {
        startLocation = World.Street;
        startPosition = 0;
        renown = 0;
    }
}

/// 세이브 데이터 Wrapper
[System.Serializable]
public class SaveWrapper
{
    public System.Collections.Generic.List<SaveData> data;
}