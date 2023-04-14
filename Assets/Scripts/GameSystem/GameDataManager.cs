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

    public static string datafilePath = "/Resources/GameData/Main/dailyData.json";     // 게임 데이터 파일 경로

    [System.Serializable]
    class Wrapper { public List<DailyData> dailyDataList = new List<DailyData>(); }     // JsonUtility용 Wrapper

    public static List<DailyData> dailyData;       // 각 Day들의 정보를 저장하는 리스트

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
}
