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

    public List<string> saveList = new List<string>(3);

    // TODO:
    /// 존재하는 세이브 파일들 탐색 및 로딩
    List<string> FindAllSave()
    {
        return null;
    }

    public void SelectSave(string saveFileName)
    {
        if (saveFileName == null)
            return;

        GameSystem.Instance.LoadPlayerData(saveFileName);
        SceneManager.LoadScene("MainWorld");
    }
}
