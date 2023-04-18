using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class GameDataManager : MonoBehaviour
{
    /**
    * 게임 내 데이터를 로드
    * 하루동안 필요한 데이터 로드 및 관리
    * 날짜 넘어갈때마다 데이터 갱신
    */

    // 싱글톤화
    private static GameDataManager _instance;
    public static GameDataManager Instance { get { return _instance; } }
    /// 에디터 상 DontDestroy설정
    private void Awake() {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            LoadGameData();     // 첫 로드시 게임 데이터 자동 로드
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    [SerializeField]
    private string datafilePath = "/Resources/GameData/Main/dailyData.json";     // 게임 데이터 파일 경로

    [System.Serializable]
    class Wrapper { public List<DailyData> dailyDataList = new List<DailyData>(); }     // JsonUtility용 Wrapper

    private List<DailyData> dailyData ;       // 각 Day들의 정보를 저장하는 리스트

    /// 오늘 날짜의 데이터
    [SerializeField]
    public DailyData todayData
    { 
        get
        { 
            return dailyData[PlayerDataManager.Instance.playerData.index];
        }
    }     

    /// JSON으로부터 게임 데이터를 로드
    private void LoadGameData()
    {
        FileStream fileStream = new FileStream(Application.dataPath + datafilePath, FileMode.Open);
        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();

        string jsonObjectData = Encoding.UTF8.GetString(data);
        Wrapper wrapper = null;
        wrapper = JsonUtility.FromJson<Wrapper>(jsonObjectData);
        dailyData = wrapper.dailyDataList;
    }

    //////////////////////////////////////////////
    /// 게임 데이터 에셋 생성용 빌드 함수
    /////////////////////////////////////////////
    public void CreateGameData()
    {
        Wrapper newGameDataWrapper = new Wrapper();

        List<DailyData> newGameDataList = new List<DailyData>();

        for(int i = 0; i < 10; i++)
        {
            DailyData newData = new DailyData();
            newData.date = new Date(23, 12, 23);

            {
                newData.workData = new Dictionary<string, int>();
                newData.workData.Add("Dodge", 1);
                newData.workData.Add("Encode", 1);
                newData.workData.Add("Decode", 1);
            }


            newData.moveWorldData = new Dictionary<string, Dictionary<int, string>>();
            Dictionary<int, string> newWorldData = new Dictionary<int, string>();
            for (int j = 0; j<4; j++)
            {
                newWorldData.Add(j, "MoveWorldName");
            }
            newData.moveWorldData.Add("DoorName", newWorldData);

            newGameDataList.Add(newData);
        }

        newGameDataWrapper.dailyDataList = newGameDataList;
         
        string jsonString = JsonUtility.ToJson(newGameDataWrapper);

        FileStream fileStream = new FileStream(Application.dataPath + datafilePath + "dailyDataAsset" + ".json", FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(jsonString);
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();
    }
}
