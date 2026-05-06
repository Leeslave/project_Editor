using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;

public class ChatBuilder : Singleton<ChatBuilder>
{
    /**
     * 대사 데이터 빌드 시스템
     */
    
    [Header("파일 수정")]
    public string fileName;
    public List<Paragraph> chat;
    public bool onEdit;

    [Space(20)] 
    [Header("게임 파일 정보")] 
    public string dataPath;
    public List<string> dataFiles = new();
    
    [Space(20)] 
    [Header("에디터 오브젝트")] 
    public GameObject buttonPrefab;
    public RectTransform dataScroll;
    private List<GameObject> dataButtons = new();
    public ChatEditor chatEditPanel;


    /// 데이터파일 리스트 불러오기
    public void OnLoadClick()
    {
        // 데이터 목록 불러오기
        dataFiles.Clear();
        dataFiles = DataLoader.GetFileNames(dataPath);

        // 기존 버튼들 삭제
        foreach (var obj in dataButtons)
        {
            Destroy(obj);
        }
        dataButtons.Clear();
        
        // 새로 버튼 생성
        Debug.Log($"Create new Buttons {dataFiles.Count}");
        foreach (var file in dataFiles)
        {
            GameObject button = Instantiate(buttonPrefab, dataScroll);
            dataButtons.Add(button);
            
            TMP_Text text = button.GetComponentInChildren<TMP_Text>();
            if (text is not null) text.text = file;
            
            
            button.GetComponent<Button>().onClick.AddListener(() => EditChat(file));
        }
        chatEditPanel.gameObject.SetActive(false);
    }


    /// <summary>
    /// DayData 수정 시작
    /// </summary>
    /// <remarks>날짜 파일을 불러온 후 </remarks>
    public void EditChat(string gameFile)
    {
        chat = DataLoader.GetChatData(gameFile).chatList;
        if (chat is null)
        {
            Debug.Log("Cannot Find chat to Edit");
            return;
        }
        
        fileName = gameFile;
        Debug.Log($"Start Editing {gameFile}");
        onEdit = true;
        dataScroll.gameObject.SetActive(false);
        chatEditPanel.gameObject.SetActive(true);
        chatEditPanel.RefreshList(chat);
    }
    
    
    /// <summary>
    /// DayData 수정 시작
    /// </summary>
    /// <remarks>날짜 파일을 불러온 후 </remarks>
    public void SaveChatData()
    {
        if (!onEdit) return;
        
        var path = Path.Combine(dataPath, fileName);
        Dialogue newChat = new() { chatList = chat };
        DataLoader.SaveData(path, newChat);
        fileName = "";
        chat = null;
        onEdit = false;
        chatEditPanel.gameObject.SetActive(false);
        dataScroll.gameObject.SetActive(true);
    }
}
