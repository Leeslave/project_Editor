[System.Serializable]
public class SaveData
{
    /**
    * 플레이어 세이브 데이터 클래스
    *   데이터 저장을 위한 직렬화 
    *   - 위치 데이터
    *   - 현재 명성치
    */
    public World location;
    public int position;
    public int renown;

    public SaveData()
    {
        location = World.Street;
        position = 0;
        renown = 0;
    }
}

[System.Serializable]
public enum World {
    Cafe,
    Bar,
    Street,
    Office
}

[System.Serializable]
public class SaveWrapper
{
    public System.Collections.Generic.List<SaveData> data;
}