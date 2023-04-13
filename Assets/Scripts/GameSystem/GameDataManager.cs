using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class GameDataManager : MonoBehaviour
{
    /**
    * 게임 내 데이터를 로드
    * load GameData 
    * async WorldScene
    * 
    */

    // 싱글톤화
    private static GameDataManager _instance;
    public static GameDataManager Instance { get { return _instance; } }
    private void Awake() {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);

            LoadGameData();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public static string datafilePath = "/Resources/GameData/Main/dailyData.json";     // 게임 데이터 파일 경로

    [System.Serializable]
    private class DailyData
    {
        /**
        * 하루 루틴동안의 게임 데이터
        *   - 날짜 정보
        *   - 업무 - 스테이지번호 데이터
        *   - 시간대별 이동가능 월드 (문마다 스크립트 문자열과 비교)
        *     (특수이벤트도 컷신 데이터 포함된 이동 월드로 구현)
        */

        public Date date;
        public Dictionary<string, int> workData;
        public Dictionary<string, Dictionary<int, string>> moveWorldData;
    }

    [System.Serializable]
    class Wrapper { public List<DailyData> dailyDataList = new List<DailyData>(); }     // JsonUtility용 Wrapper

    private static List<DailyData> dailyData;       // 각 Day들의 정보를 저장하는 리스트

    public static void LoadGameData()
    {
        FileStream fileStream = new FileStream(Application.dataPath + datafilePath, FileMode.Open);
        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();


        string jsonObjectData = Encoding.UTF8.GetString(data);
        Debug.Log(jsonObjectData);
        Wrapper wrapper = null;
        wrapper = JsonUtility.FromJson<Wrapper>(jsonObjectData);
        Debug.Log(wrapper.dailyDataList.Count);
        dailyData = wrapper.dailyDataList;
    }

    ////////////////
    public static void printTodayData(int i) {
        Debug.Log(dailyData[i].ToString());
    }

    private void Start() {
        printTodayData(0);
    }

    //////////test Area
}
