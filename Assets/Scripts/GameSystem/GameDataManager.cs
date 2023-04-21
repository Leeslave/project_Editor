using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System.Linq;

public class GameDataManager : MonoBehaviour
{
    /**
    * 게임 내 데이터를 로드
    * 하루동안 필요한 데이터 로드 및 관리
    * 날짜 넘어갈때마다 데이터 갱신
    */

    // 데이터 리스트
    [SerializeField]
    private string datafilePath = "/Resources/GameData/Main/dailyData.json";     // 게임 데이터 파일 경로
    private static GameDataManager _instance;
    public static GameDataManager Instance { get { return _instance; } }
    private List<DailyData> dailyData ;       // 각 Day들의 정보를 저장하는 리스트

    [SerializeField]
    public DailyData todayData      ///오늘 날짜의 데이터
    { 
        get
        { 
            return dailyData[PlayerDataManager.Instance.playerData.index];
        }
    }  

    /////////////// 에디터 상 DontDestroy설정
    private void Awake() {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            //TODO: LoadGameData();     // 첫 로드시 게임 데이터 자동 로드
        }
        else
        {
            Destroy(gameObject);
            return;
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
        
        foreach(DailyWrapper element in wrapper.dailyDataList)
        {
            dailyData.Add(WrapDailyData(element));
        }
    }


    ////////////// 데이터 저장/로드를 위한 직렬 클래스
    [System.Serializable]
    class DailyWrapper  // 하루 데이터 Wrapper
    { 
        public Date date;
        public List<string> workDataKey;
        public List<int> workDataValue;
        public List<string> doorDataKey;
        public List<List<string>> doorDataValue;
    }

    [System.Serializable]
    class Wrapper { public List<DailyWrapper> dailyDataList = new List<DailyWrapper>(); }     // JsonUtility용 Wrapper

    private DailyWrapper WrapDailyData(DailyData data)
    {
        DailyWrapper resultWrapper = new DailyWrapper();

        // 날짜 할당
        resultWrapper.date = data.date;

        // 업무 키, 데이터 리스트
        resultWrapper.workDataKey = data.workData.Keys.ToList();
        resultWrapper.workDataValue = data.workData.Values.ToList();

        // 월드 이동 키, 데이터 리스트 작성
        resultWrapper.doorDataKey = data.moveWorldData.Keys.ToList();
        resultWrapper.doorDataValue = data.moveWorldData.Values.ToList();

        return resultWrapper;  
    }

    private DailyData WrapDailyData(DailyWrapper wrapper)
    {
        DailyData resultData = new DailyData();

        // 날짜 할당
        resultData.date= wrapper.date;

        // 업무 키, 데이터 리스트로 딕셔너리 생성
        resultData.workData = new Dictionary<string, int>();
        for(int i = 0; i < wrapper.workDataKey.Count; i++)
        {
            resultData.workData.Add(wrapper.workDataKey[i], wrapper.workDataValue[i]);
        }

        // 월드 이동 키, 데이터 리스트로 딕셔너리 생성
        resultData.moveWorldData = new Dictionary<string, List<string>>();
        for(int i = 0; i < wrapper.doorDataKey.Count; i++)
        {
            resultData.moveWorldData.Add(wrapper.doorDataKey[i], wrapper.doorDataValue[i]);
        }

        return resultData;
    }


    //////////////////////////////////////////////
    /// 게임 데이터 에셋 생성용 빌드 함수
    /////////////////////////////////////////////

    public void CreateGameData()
    {
        Wrapper newGameDataWrapper = new Wrapper();

        List<DailyWrapper> newWrapperList = new List<DailyWrapper>();

        for(int i = 0; i < 2; i++)
        {
            DailyData newDailyData = new DailyData();
            // 날짜 생성
            newDailyData.date = new Date(23, 12, 23);

            // 미니게임 생성
            {
                newDailyData.workData = new Dictionary<string, int>();
                newDailyData.workData.Add("Dodge", 1);
                newDailyData.workData.Add("Encode", 1);
                newDailyData.workData.Add("Decode", 1);
            }

            // 월드 이동 데이터
            newDailyData.moveWorldData = new Dictionary<string, List<string>>();
            List<string> newMoveWorldValue = new List<string>();
            for (int j = 0; j<4; j++)
            {
                newMoveWorldValue.Add("MoveWorldName" + j.ToString());
            }
            newDailyData.moveWorldData.Add("DoorName1", newMoveWorldValue);
            newDailyData.moveWorldData.Add("DoorName2", newMoveWorldValue);

            newWrapperList.Add(WrapDailyData(newDailyData));
        }

        newGameDataWrapper.dailyDataList = newWrapperList;        

        string jsonString = JsonUtility.ToJson(newGameDataWrapper);
        FileStream fileStream = new FileStream(Application.dataPath + datafilePath + "dailyDataAsset" + ".json", FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(jsonString);
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();
    }
}
