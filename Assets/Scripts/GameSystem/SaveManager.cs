using GameService;
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
    IDataService dataService;

    void Start()
    {
        dataService = ServiceProvider.Get<IDataService>();
    }

    public void LoadDaySave(int day)
    {
        dataService.LoadDay(day);
        SceneManager.LoadScene("MainController");
    }
}
