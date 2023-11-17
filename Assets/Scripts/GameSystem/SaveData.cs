[System.Serializable]
public class SaveData
{
    /**
    * 플레이어 세이브 데이터 클래스
    *   - 현재 명성치
    */
    public int renown;

    public SaveData()
    {
        renown = 0;
    }
}

/// 세이브 데이터 Wrapper
[System.Serializable]
public class SaveWrapper
{
    public System.Collections.Generic.List<SaveData> list;

    public SaveWrapper()
    {
        list = new();
    }
}