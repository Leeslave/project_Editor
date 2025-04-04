using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DayBuilder : Singleton<DayBuilder>
{
    /**
     * 날짜데이터 빌드 시스템
     */
    
    [Header("파일 수정")]
    public string fileName;

    [Header("게임 파일 정보")] 
    public string DataPath;
    public List<string> dataFiles = new();
    public DailyData dailyData;

    [Space(20)] 
    public GameObject buttonPrefab;
    public RectTransform dataScroll;
    public List<GameObject> dataButtons = new();


    /// 데이터파일 리스트 불러오기
    public void OnLoadClick()
    {
        // 데이터 목록 불러오기
        dataFiles.Clear();
        dataFiles = DataLoader.GetFileNames(DataPath);

        // 기존 버튼들 삭제
        foreach (var obj in dataButtons)
        {
            Destroy(obj);
        }
        dataButtons.Clear();
        
        // 새로 버튼 생성
        foreach (var file in dataFiles)
        {
            GameObject button = Instantiate(buttonPrefab, dataScroll);
            dataButtons.Add(button);
            
            TMP_Text text = button.GetComponentInChildren<TMP_Text>();
            if (text is not null) text.text = file;
            
            
            button.GetComponent<Button>().onClick.AddListener(() => EditDayData(file));
            break;
        }
    }


    /// <summary>
    /// DayData 수정 시작
    /// </summary>
    /// <remarks>날짜 파일을 불러온 후 </remarks>
    public void EditDayData(string gameFile)
    {
        Debug.Log($"Start Editing {gameFile}");
    }
    
    /// <summary>
    /// 날짜 파일 불러오기
    /// </summary>
    private void GetDayData(string gameFile)
    {
        dailyData = DataLoader.LoadGameData(gameFile);
    }
}