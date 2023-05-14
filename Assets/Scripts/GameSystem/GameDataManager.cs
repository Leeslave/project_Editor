using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameDataManager : MonoBehaviour
{
    /**
    * 게임 내 데이터를 로드
    * 하루동안 필요한 데이터 로드 및 관리
    * 날짜 넘어갈때마다 데이터 갱신
    */

    // 데이터 리스트
    [SerializeField]
    private string dataFile = "/Resources/GameData/Main/dailyData.json";     // 게임 데이터 파일 경로
    private List<DailyData> dailyData = new List<DailyData>();       // 각 Day들의 정보를 저장하는 리스트

    [SerializeField]
    public DailyData todayData      ///오늘 날짜의 데이터
    { 
        get
        { 
            return dailyData[PlayerDataManager.Instance.saveData.dateIndex];
        }
    }  

    /////////////// 에디터 상 DontDestroy설정
    private static GameDataManager _instance;
    public static GameDataManager Instance { get { return _instance; } }
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

    ///<summary>
    /// 게임 내 시간 전환
    ///</summary>
    ///<param name="dateIndex">전환
    public void progressDate(int dateIndex = -1)
    {
        if (dateIndex < 0)
            PlayerDataManager.Instance.saveData.dateIndex++;
        else
            PlayerDataManager.Instance.saveData.dateIndex = dateIndex;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// JSON으로부터 게임 데이터를 로드
    private void LoadGameData()
    {
        FileStream fileStream = new FileStream(Application.dataPath + dataFile, FileMode.Open);
        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();

        // jsonString 읽어오기
        string jsonObjectData = Encoding.UTF8.GetString(data);

        //Wrapper로 파싱
        Wrapper wrapper = JsonUtility.FromJson<Wrapper>(jsonObjectData);

        // Wrapper를 DailyData로 전환
        foreach(DailyWrapper element in wrapper.dailyDataList)
        {
            dailyData.Add(WrapDailyData(element));
        }
    }


    ////////////// 데이터 저장/로드를 위한 직렬 클래스 //////////////////
    [System.Serializable]
    class Wrapper { public List<DailyWrapper> dailyDataList = new List<DailyWrapper>(); }     // JsonUtility용 Wrapper

    /// DailyData를 DailyWrapper로 Wrapping
    private DailyWrapper WrapDailyData(DailyData data)
    {
        DailyWrapper resultWrapper = new DailyWrapper();

        // 날짜 할당
        resultWrapper.date = data.date;

        // 업무 키, 데이터 리스트
        resultWrapper.workDataKey = data.workData.Keys.ToList();
        resultWrapper.workDataValue = data.workData.Values.ToList();

        return resultWrapper;  
    }

    /// DailyWrapper를 DailyData로 UnWrapping
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

            // 날짜별 업무 생성
            {
                newDailyData.workData = new Dictionary<string, int>();
                newDailyData.workData.Add("Dodge", 1);
                newDailyData.workData.Add("Encode", 1);
                newDailyData.workData.Add("Decode", 1);
            }

            newWrapperList.Add(WrapDailyData(newDailyData));
        }

        newGameDataWrapper.dailyDataList = newWrapperList;        

        string jsonString = JsonUtility.ToJson(newGameDataWrapper);
        FileStream fileStream = new FileStream(Application.dataPath + dataFile + "dailyDataAsset" + ".json", FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(jsonString);
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();
    }
}
