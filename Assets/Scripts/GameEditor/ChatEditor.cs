using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatEditor : MonoBehaviour
{
    public List<Paragraph> dataList = new();

    [Header("데이터 수정 프리팹")] public GameObject talkPrefab;
    public GameObject choicePrefab;
    public Transform dataPanel;

    [Header("데이터 UI")] 
    public TalkInput talkUI;
    public ChoiceInput choiceUI;
    
    public Button saveButton;


    /// <summary>
    /// 대사 리스트 새로고침
    /// </summary>
    /// <remarks>대사 리스트에 있는 버튼들을 데이터 순서에 맞춰서 새로 생성</remarks>
    public void RefreshList(List<Paragraph> newData = null)
    {
        // 기존 버튼들 삭제
        foreach (var obj in dataPanel.GetComponentsInChildren<Transform>()
                                                                .Where(t => t != dataPanel.transform))
        {
            Destroy(obj.gameObject);
        }

        if (newData != null)
        {
            dataList = newData;
        }

        // 데이터에 맞춰 버튼들 생성
        for (int i = 0; i < dataList.Count; i++)
        {
            int index = i;
            Paragraph data = dataList[i];
            GameObject newObj;
            if (data is TalkParagraph)
            {
                newObj = Instantiate(talkPrefab, dataPanel);
            }
            else if (data is ChoiceParagraph)
            {
                newObj = Instantiate(choicePrefab, dataPanel);  
            }
            else
                return;
            
            Button btn = newObj.GetComponent<Button>();
            
            //버튼 순서 정렬
            newObj.GetComponentInChildren<TMP_InputField>().text = i.ToString();
            newObj.GetComponentInChildren<TMP_InputField>().onEndEdit.AddListener((string s) =>
            {
                int newIndex = int.Parse(s);
                if (newIndex < 0 || newIndex >= dataList.Count)
                {
                    newObj.GetComponentInChildren<TMP_InputField>().text = index.ToString();
                    return;
                }
                
                int iter = index;
                while (iter != newIndex)
                {
                    if (iter < newIndex)
                    {
                        (dataList[iter + 1], dataList[iter]) = (dataList[iter], dataList[iter + 1]);
                        iter++;
                    }
                    else
                    {
                        (dataList[iter - 1], dataList[iter]) = (dataList[iter], dataList[iter - 1]);
                        iter--;
                    }
                }
                
                RefreshList();
            });
            
            // 대사 미리보기
            TMP_Text[] text = newObj.GetComponentsInChildren<TMP_Text>();
            
            // 버튼 설명 및 로드 함수 이벤트 연결
            if (data is TalkParagraph talk)
            {
                List<Paragraph> newList = new() { talk };
                text[1].text = talk.text;
                
                btn.onClick.AddListener(() => LoadParagraph(index, talk));
                btn.GetComponent<ChatTrigger>()?.SetChatData(newList);
                btn.onClick.AddListener(() => btn.GetComponent<ChatTrigger>()?.StartChat());
            }
            else if (data is ChoiceParagraph choice)
            {
                List<Paragraph> newList = new() { choice };
                // TODO: 선택지 설명 추가
                text[1].text = "선택지";
                
                btn.onClick.AddListener(() => LoadParagraph(index, choice));
                btn.GetComponent<ChatTrigger>()?.SetChatData(newList);
                btn.onClick.AddListener(() => btn.GetComponent<ChatTrigger>()?.StartChat());
            }
        }
    }


    private void LoadParagraph(int index, TalkParagraph data)
    {
        talkUI.index = index;
        talkUI.LoadInput(data);
        
        talkUI.gameObject.SetActive(true);
        choiceUI.gameObject.SetActive(false);
        
        saveButton.onClick.RemoveAllListeners();
        saveButton.onClick.AddListener(()=> talkUI.SubmitInput());
    }


    private void LoadParagraph(int index, ChoiceParagraph data)
    {
        choiceUI.index = index;
        choiceUI.LoadInput(data);
        
        talkUI.gameObject.SetActive(false);
        choiceUI.gameObject.SetActive(true);
        
        saveButton.onClick.RemoveAllListeners();
        saveButton.onClick.AddListener(()=> choiceUI.SubmitInput());
    }


    public void SaveParagraph(int index, TalkParagraph data)
    {
        dataList[index] = data;
        talkUI.gameObject.SetActive(false);
        RefreshList();
    }
    
    
    public void SaveParagraph(int index, ChoiceParagraph data)
    {
        dataList[index] = data;
        choiceUI.gameObject.SetActive(false);
        RefreshList();
    }


    public void AddTalk()
    {
        dataList.Add(new TalkParagraph("새 대사"));
        RefreshList();
    }


    public void AddChoice()
    {
        dataList.Add(new ChoiceParagraph());
        RefreshList();
    }


    public void RemoveLast()
    {
        dataList.RemoveAt(dataList.Count - 1);
        RefreshList();
    }
}
