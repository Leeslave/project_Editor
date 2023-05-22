using System.Collections;
using System.Collections.Generic;
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

    // 저장 데이터 리스트
    public List<string> saveList = new List<string>(3);

    // TODO:
    /// 존재하는 세이브 파일들 탐색 및 로딩
    List<string> FindAllSave()
    {
        return null;
    }

    /// <summary>
    /// 저장 데이터 선택 및 실행
    /// </summary>
    ///<param name="SaveFileName">저장 데이터의 파일 이름</param>
    ///<remarks>저장 데이터를 불러오고 씬 로딩</remarks>
    public void SelectSave(string saveFileName)
    {
        if (saveFileName == null)
            return;

        GameSystem.Instance.LoadPlayerData(saveFileName);
        SceneManager.LoadScene("MainWorld");
    }
}
