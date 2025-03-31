using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.UI;

public class DataBuilder : MonoBehaviour
{
    /**
     * 게임 데이터 빌드 시스템
     * : 별도의 Editor로 조작 예정
     * - 폴더를 통해 현재 저장되어있는 데이터 확인 가능
     * - 특정 파일을 선택하여 수정/생성
     *      1. DailyData
     *      2. ChatData : List<Paragraph>
     */
    
    [Header("파일 수정")]
    public string fileName;

    [Header("게임 파일 정보")] 
    public string currentFileType;
    public string DailyDataPath;
    public string ChatPath;
    public List<string> dataFiles = new();
    private DailyData dailyData;
    private List<Paragraph> chatData = new();

    [Space(20)] 
    public GameObject buttonPrefab;
    public RectTransform scrollContent;
    public List<GameObject> dataButtons = new();


    /// 데이터 불러오기
    public void OnLoadClick(string fileType)
    {
        // 데이터 목록 불러오기
        dataFiles.Clear();
        currentFileType = fileType;
        string path;
        switch (fileType)
        {
            case "DailyData":
                path = DailyDataPath;
                break;
            case "ChatData":
                path = ChatPath;
                break;
            default:
                Debug.Log("FileType Error");
                return;
        }
        dataFiles = GetFileNames(path);

        // 기존 버튼들 삭제
        foreach (var obj in dataButtons)
        {
            Destroy(obj);
        }
        dataButtons.Clear();
        
        // 새로 버튼 생성
        foreach (var file in dataFiles)
        {
            GameObject button = Instantiate(buttonPrefab, scrollContent);
            dataButtons.Add(button);
            
            TMP_Text text = button.GetComponentInChildren<TMP_Text>();
            if (text is not null) text.text = file;
            
            
            switch (fileType)
            {
                case "DailyData":
                    button.GetComponent<Button>().onClick.AddListener(() => EditDayData(file));
                    break;
                case "ChatData":
                    button.GetComponent<Button>().onClick.AddListener(() => EditChatData(file));
                    break;
            }
        }
    }


    public void EditDayData(string gameFile)
    {
        Debug.Log($"Start Editing {gameFile}");
    }
    
    public void EditChatData(string gameFile)
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


    /// <summary>
    /// 대사 파일 불러오기
    /// </summary>
    private void GetChatData(string gameFile)
    {
        chatData = DataLoader.GetChatData(gameFile);
    }
    
    
    /// <summary>
    /// 파일 목록 불러오기
    /// </summary>
    private List<string> GetFileNames(string path)
    {
        // 폴더 상의 게임 데이터 로드
        if (!Directory.Exists(path))
        {
            Debug.Log($"폴더 경로 오류 : {path}");
        }
        
        List<string> fileNames = new();
        string[] files;

        files = Directory.GetFiles(path, "*.json", SearchOption.AllDirectories); // .json 파일만 검색
        foreach (string file in files)
        {
            string relativePath = file.Substring(path.Length); // 파일에서 path 부분을 제거
            fileNames.Add(relativePath);
        }
        
        fileNames.Sort();
        return fileNames;
    }
}