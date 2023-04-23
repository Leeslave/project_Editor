using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    /**
    * 플레이어 데이터를 저장, 로드
    * 플레이어의 유지 데이터들 저장
        인덱스, 날짜, 시간, 장소, 명성치 
    */

    [SerializeField]
    private string savefilePath = "/Resources/Save/";    // 세이브 파일 경로
    [SerializeField]
    public PlayerData playerData = new PlayerData();     // 플레이어 데이터 필드

    /// 에디터 상 DontDestroy설정
    private static PlayerDataManager _instance;
    public static PlayerDataManager Instance { get { return _instance; } }
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
}
