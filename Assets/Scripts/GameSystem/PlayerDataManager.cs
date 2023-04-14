using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    /**
    * 플레이어 데이터를 저장, 로드
    * initNewPlayerData
    * SavePlayerData
    * LoadPlayerData
    */

    [SerializeField]
    private string savefilePath = "/Resources/Save/";    // 세이브 파일 경로

    // 싱글톤화
    private static PlayerDataManager _instance;
    public static PlayerDataManager Instance { get { return _instance; } }

    [SerializeField]
    public PlayerData playerData = new PlayerData();     // 플레이어 데이터 필드

    /// 에디터 상 DontDestroy설정
    private void Awake() {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else 
        {
            Destroy(gameObject);
            return;
        }
    }
    
    /// <summary>
    /// 플레이어 데이터 JSON 저장
    /// </summary>
    /// <param name="SaveFileName">저장 경로 내 파일명</param>
    public void SavePlayerData(string saveFileName)
    {
        string jsonObjectData = JsonUtility.ToJson(playerData);
        
        FileStream fileStream = new FileStream(Application.dataPath + savefilePath + saveFileName + ".json", FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(jsonObjectData);
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();
    }

    /// <summary>
    /// 플레이어 데이터 JSON에서 로드
    /// </summary>
    /// <param name=SaveFileName>저장 경로 내 파일명</param>
    public void LoadPlayerData(string saveFileName)
    { 
        FileStream fileStream = new FileStream(Application.dataPath + savefilePath + saveFileName + ".json", FileMode.Open);
        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();

        string jsonObjectData = Encoding.UTF8.GetString(data);
        playerData = JsonUtility.FromJson<PlayerData>(jsonObjectData);
    }

    // 데이터 초기 설정
    public void InitNewPlayerData()
    {   
        LoadPlayerData(savefilePath+"default.json");
    }


    ///////////////////////////////////////////////////////////

    public static string worldfilePath = "Prefab/MainWorld/";  // 월드 프리팹 경로
    // TODO: MainWorldSceneManager로 이동
    /// <summary>
    /// 플레이어의 위치와 활성화 월드 동기화
    /// </summary>
    /// <remarks>씬 내 WorldCanvas 객체 삭제 후 새로 생성</remarks>
    public static void asyncWorldCanvas()
    {

        var existWorld = GameObject.FindObjectOfType<WorldCanvas>();
        if (existWorld != null)
        {
            Destroy(existWorld.gameObject);
        }
        // GameObject newWorldCanvas = Instantiate(Resources.Load<GameObject>(worldfilePath + playerData.location + "Canvas"));
    }
    //////////////////////////////////////////////////////////////////
}
