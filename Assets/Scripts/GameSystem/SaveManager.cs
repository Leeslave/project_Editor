using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    /**
    * 세이브 파일들 표시 및 선택
        - 세이브 목록 불러와서 표시
        - 특정 세이브 선택 및 로딩
        - 세이브 로드 후 씬 전환
    */

    [SerializeField]
    private string saveFile;
    private List<SaveData> saveList = new List<SaveData>();  // 저장 데이터 리스트

    void Awake()
    {
        LoadPlayerData();
    }

    /// <summary>
    /// 저장 데이터 선택 및 실행
    /// </summary>
    ///<param name="SaveFileName">저장 데이터의 파일 이름</param>
    ///<remarks>저장 데이터를 불러오고 씬 로딩</remarks>
    public void SelectSave(int index)
    {
        if (index < 0)
            return;

        GameSystem.Instance.player = saveList[index];
        SceneManager.LoadScene("MainWorld");
    }
    
    /// <summary>
    /// 플레이어 데이터 JSON에서 로드
    /// </summary>
    /// <param name=SaveFileName>저장 경로 내 파일명</param>
    private void LoadPlayerData()
    { 
        FileStream fileStream = new FileStream(Application.dataPath + saveFile + ".json", FileMode.Open);
        byte[] data = new byte[fileStream.Length];
        fileStream.Read(data, 0, data.Length);
        fileStream.Close();

        // SaveData로 파싱
        string jsonObjectData = Encoding.UTF8.GetString(data);
        SaveWrapper wrapper = JsonUtility.FromJson<SaveWrapper>(jsonObjectData);

        saveList = wrapper.data;
    }
}
