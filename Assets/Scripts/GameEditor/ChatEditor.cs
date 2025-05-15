using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ChatEditor : MonoBehaviour
{
    public List<Paragraph> dataList;
    private TalkParagraph talkData;
    private ChoiceParagraph choiceData;

    [Header("데이터 수정 프리팹")] public GameObject talkPrefab;
    public GameObject choicePrefab;
    public Transform dataPanel;

    [Header("데이터 UI")] 
    public Transform talkUI;
    public Transform choiceUI;


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

        if (newData?.Count != 0)
        {
            dataList = newData;
        }

        // 데이터에 맞춰 버튼들 생성
        if (dataList?.Count == 0) return;
        foreach (var data in dataList)
        {
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

            TMP_Text[] texts = newObj.transform.GetComponentsInChildren<TMP_Text>();
            texts[0].text = dataPanel.childCount.ToString();
        }
    }


    public void LoadParagraph(TalkParagraph data)
    {
        TMP_InputField[] field = talkUI.GetComponentsInChildren<TMP_InputField>();
        field[0].text = data.talker;
        field[1].text = data.talkerInfo;
        
        talkUI.gameObject.SetActive(true);
        choiceUI.gameObject.SetActive(false);
    }


    public void LoadParagraph(ChoiceParagraph data)
    {
        TMP_InputField[] field = choiceUI.GetComponentsInChildren<TMP_InputField>();
        for (int i = 0; i < field.Length || i < 4; i++)
        {
            field[i].text = data.choiceList[i].text;
        }
        
        
        talkUI.gameObject.SetActive(false);
        choiceUI.gameObject.SetActive(true);
    }
}
